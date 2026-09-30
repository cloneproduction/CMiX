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
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

namespace CMiX.Studio.Avalonia.ViewModels
{
    public partial class MainMenu : ObservableRecipient, IControl
    {
        public MainMenu(Project project, ControlFactory controlFactory, UndoManager undoManager, IDialogService dialogService)
        {
            _dialogService = dialogService;
            Project = project;
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
        private string? _folderPath;

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

        private void NewProject() => ResetSession();

        // Empties the running session. The seven repository managers and the composition manager
        // between them reference every control the repository holds, so letting all eight go
        // releases the whole graph, and clearing the undo stack afterwards disposes the removed
        // instances its commands were still holding on to.
        private void ResetSession()
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

            await OpenProjectFromPath(path);
        }

        // The open sequence minus the file dialog, so a caller that already has a path, the tests
        // among them, drives the same steps the menu command does.
        public async Task OpenProjectFromPath(string path)
        {
            try
            {
                var loaded = await Task.Run(() =>
                {
                    var projectModel = ProjectSerializer.Load(path);

                    // Zero compositions is a valid, saveable project state (an empty document, the
                    // same as any comparable creative tool), not an error - the loop below just adds
                    // nothing when this list is empty.
                    var compositions = projectModel.CompositionManager.ManagerData.Items
                        .Cast<CompositionModel>()
                        .Select(CloneWithNewGuids)
                        .ToList();

                    return (Project: projectModel, Compositions: compositions);
                });

                // Opening replaces the session rather than merging the file into it, so what was
                // open is emptied here the way File > New empties it. The sweep only runs once the
                // file has been parsed, so a parse failure above leaves the open session untouched
                // and reports over a window the user has not had emptied behind the dialog.
                ResetSession();

                // The load builds the graph through the very managers a user action goes through,
                // so every add and every selection it makes would record a step on the stack the
                // reset just emptied and a Ctrl+Z on a freshly opened file would step backwards
                // into the load. Recording is off for the apply so those steps are never created
                // at all, which is also why the reset stays outside: its own clear is what disposes
                // the instances the replaced session's commands were still holding.
                //
                // Suppression counts, so the master beat restore below, which suppresses and
                // resumes around each of its own writes, nests inside this without reopening
                // recording halfway through.
                _undoManager.SuppressUndo();
                try
                {
                    foreach (var composition in loaded.Compositions)
                        Project.CompositionManager.AddItem(composition);

                    // Adding always selects the item just added, so the loop above always leaves the
                    // last composition selected. Restore whichever one was actually selected when the
                    // file was saved instead, by position in the file's own list.
                    var savedIndex = loaded.Project.CompositionManager.ManagerData.SelectedIndex;
                    if (savedIndex >= 0 && savedIndex < loaded.Compositions.Count)
                        Project.CompositionManager.Collection.SelectedItemChanged(savedIndex);

                    // The master beat keeps the ids it was saved with, so it is restored from the
                    // file as loaded rather than from the guid replaced clone each composition goes
                    // through. Project files written before the master beat was serialized carry
                    // none. PrefabService and Model (the project's 3D backdrop) carry their own
                    // well known ids the same way and are restored the same way.
                    if (loaded.Project.MasterBeat != null)
                        Project.MasterBeat.FromModel(loaded.Project.MasterBeat);

                    if (loaded.Project.PrefabService != null)
                        Project.PrefabService.FromModel(loaded.Project.PrefabService);

                    if (loaded.Project.Model != null)
                        Project.Model.FromModel(loaded.Project.Model);
                }
                finally
                {
                    _undoManager.ResumeUndo();
                }

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
            return JsonSerializer.Deserialize<T>(json, ProjectSerializer.Options)
                ?? throw new InvalidOperationException($"Failed to clone {typeof(T).Name}: deserialization returned null.");
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
            var projectModel = ProjectModelBuilder.Build(Project);
            await Task.Run(() => ProjectSerializer.Save(projectModel, path));
        }

        public IControlModel ToModel() => throw new NotImplementedException();
        public void FromModel(IControlModel model) => throw new NotImplementedException();
    }
}
