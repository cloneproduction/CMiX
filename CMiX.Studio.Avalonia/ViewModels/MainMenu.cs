// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using CMiX.Core;
using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

namespace CMiX.Studio.Avalonia.ViewModels
{
    public partial class MainMenu : ObservableRecipient, IControl, IRecipient<IMessage>
    {
        public MainMenu(Project project, ControlFactory controlFactory, UndoManager undoManager, IDialogService dialogService)
        {
            _dialogService = dialogService;
            Project = project;
            IsActive = true;
            _undoManager = undoManager;

            NewProjectCommand = new RelayCommand(NewProject);
            OpenProjectCommand = new AsyncRelayCommand(OpenProject);
            SaveProjectCommand = new AsyncRelayCommand(SaveProject);
            SaveAsProjectCommand = new AsyncRelayCommand(SaveAsProject);
            AddCompositionCommand = new RelayCommand(AddComposition);
            AddLayerCommand = new RelayCommand(AddLayer);
            DeleteCompositionCommand = new RelayCommand(DeleteComposition);
            DuplicateCompositionCommand = new RelayCommand(DuplicateComposition);
            DeleteLayerCommand = new RelayCommand(DeleteLayer);
            DuplicateLayerCommand = new RelayCommand(DuplicateLayer);
            UndoCommand = new RelayCommand(() => undoManager.Undo());
            RedoCommand = new RelayCommand(() => undoManager.Redo());
            CloseWindowCommand = new RelayCommand(CloseMainWindow);

            ControlFactory = controlFactory;
        }

        private readonly UndoManager _undoManager;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private string _folderPath;

        public Guid ID { get; set; } = Guid.NewGuid();
        public ControlFactory ControlFactory { get; set; }
        public Project Project { get; set; }

        public ICommand NewProjectCommand { get; }
        public ICommand SaveProjectCommand { get; }
        public ICommand SaveAsProjectCommand { get; }
        public ICommand OpenProjectCommand { get; }
        public ICommand AddCompositionCommand { get; }
        public ICommand AddLayerCommand { get; }
        public ICommand DeleteCompositionCommand { get; }
        public ICommand DuplicateCompositionCommand { get; }
        public ICommand DeleteLayerCommand { get; }
        public ICommand DuplicateLayerCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand CloseWindowCommand { get; }

        // Handed over by MainViewModel, which owns the seven top level repository managers. They
        // are transient in the container, so resolving them here would build fresh instances
        // instead of the live ones a new project has to empty.
        public IReadOnlyList<PrefabManager> RepositoryManagers { get; set; } = Array.Empty<PrefabManager>();

        private static void CloseMainWindow()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow?.Close();
        }

        public void AddLayer()
        {
            // Mirrors the layer manager view's AddItemCommand, CommandParameter typeof(Layer) button.
            // Posted for the same reason as AddComposition, so the popup teardown finishes before
            // the bound layer name is touched.
            Dispatcher.UIThread.Post(() =>
            {
                var layerManager = (Project.CompositionManager.SelectedItem as Composition)?.LayerManager;
                layerManager?.AddItemCommand.Execute(typeof(Layer));
            });
        }

        public void AddComposition()
        {
            // Mirrors the Composition tab's New Composition button, CommandParameter typeof(Composition).
            // Posted because the MenuItem still has its own flyout Popup open while this runs, and
            // mutating SelectedItem while the Outliner header sits behind that Popup left the name
            // display painted with the pre add frame until a manual refresh such as double clicking
            // into edit mode. Posting waits for the popup teardown to finish before the header is
            // touched, matching the DispatcherPost pattern already used for popup timing elsewhere.
            Dispatcher.UIThread.Post(() => Project.CompositionManager.AddItemCommand.Execute(typeof(Composition)));
        }

        private void DeleteComposition()
        {
            // Posted for the same reason as AddComposition, so the popup teardown finishes before
            // the bound composition list is touched.
            Dispatcher.UIThread.Post(() =>
            {
                if (Project.CompositionManager.SelectedItem is not Composition composition) return;
                Project.CompositionManager.DeleteItem(composition);
            });
        }

        private void DuplicateComposition()
        {
            // Posted for the same reason as AddComposition, so the popup teardown finishes before
            // the bound composition list is touched.
            Dispatcher.UIThread.Post(() =>
            {
                if (Project.CompositionManager.SelectedItem is not Composition composition) return;
                var model = (CompositionModel)composition.ToModel();
                Project.CompositionManager.AddItem(CloneWithNewGuids(model));
            });
        }

        private void DeleteLayer()
        {
            // Posted for the same reason as AddComposition, so the popup teardown finishes before
            // the bound layer list is touched.
            Dispatcher.UIThread.Post(() =>
            {
                if (Project.CompositionManager.SelectedItem is not Composition composition) return;
                if (composition.LayerManager.SelectedItem is not Layer layer) return;
                composition.LayerManager.DeleteItem(layer);
            });
        }

        private void DuplicateLayer()
        {
            // Posted for the same reason as AddComposition, so the popup teardown finishes before
            // the bound layer list is touched.
            Dispatcher.UIThread.Post(() =>
            {
                if (Project.CompositionManager.SelectedItem is not Composition composition) return;
                if (composition.LayerManager.SelectedItem is not Layer layer) return;
                var model = (LayerModel)layer.ToModel();
                composition.LayerManager.AddItem(CloneWithNewGuids(model));
            });
        }

        private void NewProject()
        {
            Project.CompositionManager.ClearAll();
            foreach (var manager in RepositoryManagers)
                manager.ClearAll();
            _undoManager.Clear();
            FolderPath = null;
        }

        private async Task OpenProject()
        {
            var file = await _dialogService.ShowOpenFileDialogAsync(this, new OpenFileDialogSettings
            {
                Filters = new List<FileFilter> { new FileFilter("CMiX Project", "cmix") }
            });

            var path = file?.LocalPath;
            if (string.IsNullOrWhiteSpace(path)) return;

            try
            {
                var loaded = await Task.Run<(ProjectModel Project, CompositionModel Composition)>(() =>
                {
                    var projectModel = ProjectSerializer.Load(path);
                    if (projectModel?.CompositionManager?.ManagerData?.Items?.FirstOrDefault() is not CompositionModel compositionModel)
                        return (null, null);

                    return (projectModel, CloneWithNewGuids(compositionModel));
                });

                if (loaded.Composition == null)
                {
                    await _dialogService.ShowMessageBoxAsync(this, "File does not contain a composition.", "Open Project");
                    return;
                }

                Project.CompositionManager.AddItem(loaded.Composition);

                // The master beat keeps the ids it was saved with, so it is restored from the file
                // as loaded rather than from the guid replaced clone the composition goes through.
                // Project files written before the master beat was serialized carry none.
                if (loaded.Project.MasterBeat != null)
                    Project.MasterBeat.FromModel(loaded.Project.MasterBeat);

                FolderPath = path;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageBoxAsync(this, $"{Path.GetFileName(path)}: {ex.Message}", "Open Project");
            }
        }

        // Shared by DuplicateComposition, DuplicateLayer and OpenProject, which all need a deep
        // copy of a model with every id replaced so the clone does not collide with the original.
        private static T CloneWithNewGuids<T>(T model) where T : IControlModel
        {
            var json = ReplaceAllGuids(JsonSerializer.Serialize(model, ProjectSerializer.Options));
            return JsonSerializer.Deserialize<T>(json, ProjectSerializer.Options);
        }

        private static string ReplaceAllGuids(string json)
        {
            var guidPattern = @"[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}";
            var guidMap = new Dictionary<string, string>();
            return Regex.Replace(json, guidPattern, match =>
            {
                var original = match.Value;
                if (!guidMap.TryGetValue(original, out var newGuid))
                {
                    newGuid = Guid.NewGuid().ToString();
                    guidMap[original] = newGuid;
                }
                return newGuid;
            });
        }

        private async Task SaveProject()
        {
            if (!string.IsNullOrWhiteSpace(FolderPath))
            {
                await WriteProjectGuarded(FolderPath);
                return;
            }
            await SaveAsProject();
        }

        private async Task SaveAsProject()
        {
            var file = await _dialogService.ShowSaveFileDialogAsync(this, new SaveFileDialogSettings
            {
                Filters = new List<FileFilter> { new FileFilter("CMiX Project", "cmix") },
                DefaultExtension = "cmix"
            });

            var path = file?.LocalPath;
            if (string.IsNullOrWhiteSpace(path)) return;

            FolderPath = path;
            await WriteProjectGuarded(FolderPath);
        }

        private async Task WriteProjectGuarded(string path)
        {
            try
            {
                await WriteProject(path);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageBoxAsync(this, $"{Path.GetFileName(path)}: {ex.Message}", "Save Project");
            }
        }

        private async Task WriteProject(string path)
        {
            var selectedComposition = Project.CompositionManager.SelectedItem as Composition;
            if (selectedComposition == null) return;

            var projectModel = ProjectModelBuilder.Build(selectedComposition, Project.MasterBeat);
            await Task.Run(() => ProjectSerializer.Save(projectModel, path));
        }

        public void Receive(IMessage message)
        {
            //MainMenuMessenger.Receive(this, message);
        }

        public IControlModel ToModel() => throw new NotImplementedException();
        public void FromModel(IControlModel model) => throw new NotImplementedException();
    }
}
