// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                var json = ReplaceAllGuids(JsonSerializer.Serialize(model, ProjectSerializer.Options));
                var cloned = JsonSerializer.Deserialize<CompositionModel>(json, ProjectSerializer.Options);
                Project.CompositionManager.AddItem(cloned);
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
                var json = ReplaceAllGuids(JsonSerializer.Serialize(model, ProjectSerializer.Options));
                var cloned = JsonSerializer.Deserialize<LayerModel>(json, ProjectSerializer.Options);
                composition.LayerManager.AddItem(cloned);
            });
        }

        private void NewProject()
        {
            Project.CompositionManager.ClearAll();
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
                var cloned = await Task.Run(() =>
                {
                    var projectModel = ProjectSerializer.Load(path);
                    if (projectModel?.CompositionManager?.ManagerData?.Items?.FirstOrDefault() is not CompositionModel compositionModel)
                        return null;

                    var json = JsonSerializer.Serialize(compositionModel, ProjectSerializer.Options);
                    json = ReplaceAllGuids(json);
                    return JsonSerializer.Deserialize<CompositionModel>(json, ProjectSerializer.Options);
                });

                if (cloned == null)
                {
                    await _dialogService.ShowMessageBoxAsync(this, "File does not contain a composition.", "Open Project");
                    return;
                }

                Project.CompositionManager.AddItem(cloned);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageBoxAsync(this, $"{Path.GetFileName(path)}: {ex.Message}", "Open Project");
            }
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

            var compositionModel = (CompositionModel)selectedComposition.ToModel();

            var projectModel = new ProjectModel
            {
                MasterBeat = (MasterBeatModel)Project.MasterBeat.ToModel(),
                CompositionManager = new PrefabManagerModel
                {
                    ManagerData = new ManagerDataModel
                    {
                        Items = new Collection<IControlModel> { compositionModel },
                        SelectedIndex = 0
                    }
                }
            };
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
