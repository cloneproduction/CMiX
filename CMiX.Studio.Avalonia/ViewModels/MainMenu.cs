// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using CMiX.Core.Compositing;
using CMiX.Studio.Avalonia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

namespace CMiX.Studio.Avalonia.ViewModels
{
    public partial class MainMenu : ObservableObject
    {
        public MainMenu(Project project, ProjectSession session, IDialogService dialogService,
                        UnsavedChanges unsavedChanges, IUnsavedChangesPrompt unsavedChangesPrompt, IMessageNotice messageNotice)
        {
            _session = session;
            _dialogService = dialogService;
            _unsavedChanges = unsavedChanges;
            _unsavedChangesPrompt = unsavedChangesPrompt;
            _messageNotice = messageNotice;
            Project = project;

            NewProjectCommand = new RelayCommand(NewProject);
            OpenProjectCommand = new AsyncRelayCommand(OpenProject);
            SaveProjectCommand = new AsyncRelayCommand(SaveProject);
            SaveAsProjectCommand = new AsyncRelayCommand(SaveAsProject);
            AddCompositionCommand = new RelayCommand(AddComposition);
            AddLayerCommand = new RelayCommand(AddLayer);
            RemoveCompositionCommand = new RelayCommand(RemoveComposition);
            DuplicateCompositionCommand = new RelayCommand(DuplicateComposition);
            RemoveLayerCommand = new RelayCommand(RemoveLayer);
            DuplicateLayerCommand = new RelayCommand(DuplicateLayer);
        }

        private readonly ProjectSession _session;
        private readonly IDialogService _dialogService;
        private readonly UnsavedChanges _unsavedChanges;
        private readonly IUnsavedChangesPrompt _unsavedChangesPrompt;
        private readonly IMessageNotice _messageNotice;

        public bool HasUnsavedChanges => _unsavedChanges.HasChanges;

        // Asks what to do with the unsaved changes before the window closes. True: the window may close.
        // Save counts only when the project was saved, so a Save As that is cancelled keeps the window open.
        public async Task<bool> ConfirmCloseAsync()
        {
            if (!_unsavedChanges.HasChanges)
                return true;

            switch (await _unsavedChangesPrompt.AskAsync())
            {
                case UnsavedChangesChoice.Save:
                    await SaveProject();
                    return !_unsavedChanges.HasChanges;
                case UnsavedChangesChoice.DontSave:
                    return true;
                default:
                    return false;
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TitleBarText))]
        private string? _projectPath;

        // Text of the title bar: the file name of the open project, or "(Unsaved)" when there is none.
        // The full path is the tooltip of the title bar.
        public string TitleBarText =>
            $"CMiX - {(string.IsNullOrWhiteSpace(ProjectPath) ? "(Unsaved)" : Path.GetFileName(ProjectPath))}";

        public Project Project { get; set; }

        public ICommand NewProjectCommand { get; }
        public ICommand SaveProjectCommand { get; }
        public ICommand SaveAsProjectCommand { get; }
        public ICommand OpenProjectCommand { get; }
        public ICommand AddCompositionCommand { get; }
        public ICommand AddLayerCommand { get; }
        public ICommand RemoveCompositionCommand { get; }
        public ICommand DuplicateCompositionCommand { get; }
        public ICommand RemoveLayerCommand { get; }
        public ICommand DuplicateLayerCommand { get; }

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

        private void RemoveComposition()
        {
            // Posted for the same reason as AddComposition, so the popup teardown finishes before
            // the bound composition list is touched.
            Dispatcher.UIThread.Post(() =>
            {
                if (Project.CompositionManager.SelectedItem is not Composition composition) return;
                Project.CompositionManager.RemoveItem(composition);
            });
        }

        private void DuplicateComposition()
        {
            // Posted for the same reason as AddComposition, so the popup teardown finishes before
            // the bound composition list is touched.
            Dispatcher.UIThread.Post(() =>
            {
                if (Project.CompositionManager.SelectedItem is not Composition composition) return;
                Project.CompositionManager.DuplicateItem(composition);
            });
        }

        private void RemoveLayer()
        {
            // Posted for the same reason as AddComposition, so the popup teardown finishes before
            // the bound layer list is touched.
            Dispatcher.UIThread.Post(() =>
            {
                if (Project.CompositionManager.SelectedItem is not Composition composition) return;
                if (composition.LayerManager.SelectedItem is not Layer layer) return;
                composition.LayerManager.RemoveItem(layer);
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
                composition.LayerManager.DuplicateItem(layer);
            });
        }

        private void NewProject()
        {
            _session.NewProject();
            ProjectPath = null;
            _unsavedChanges.MarkSaved();
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
                // The parse comes first, so a parse failure leaves the open session untouched.
                var model = await _session.ReadAsync(path);
                ProjectPath = null;
                _session.LoadProject(model);
                ProjectPath = path;
                _unsavedChanges.MarkSaved();
            }
            catch (Exception ex)
            {
                await _messageNotice.ShowErrorAsync($"{Path.GetFileName(path)}: {ex.Message}", "Open Project");
                return;
            }

            // The push is outside the catch: the file is open, and a push failure shows in the sync status.
            await _session.PushToPeersAsync();
        }

        private async Task SaveProject()
        {
            if (!string.IsNullOrWhiteSpace(ProjectPath))
            {
                await WriteProjectGuarded(ProjectPath);
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

            ProjectPath = path;
            await WriteProjectGuarded(ProjectPath);
        }

        private async Task WriteProjectGuarded(string path)
        {
            try
            {
                await WriteProject(path);
                _unsavedChanges.MarkSaved();
            }
            catch (Exception ex)
            {
                await _messageNotice.ShowErrorAsync($"{Path.GetFileName(path)}: {ex.Message}", "Save Project");
            }
        }

        private Task WriteProject(string path) => _session.WriteAsync(_session.CaptureProject(), path);
    }
}
