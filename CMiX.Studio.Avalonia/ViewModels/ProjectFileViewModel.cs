// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CMiX.Studio.Avalonia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

namespace CMiX.Studio.Avalonia.ViewModels
{
    // The project file of the session: its path, the title bar text and the four file commands with their dialogs.
    public sealed partial class ProjectFileViewModel : ObservableObject
    {
        public ProjectFileViewModel(ProjectSession session, UnsavedChanges unsavedChanges, IDialogService dialogService,
                                    IUnsavedChangesPrompt unsavedChangesPrompt, IMessageNotice messageNotice)
        {
            _session = session;
            _unsavedChanges = unsavedChanges;
            _dialogService = dialogService;
            _unsavedChangesPrompt = unsavedChangesPrompt;
            _messageNotice = messageNotice;
        }

        private readonly ProjectSession _session;
        private readonly UnsavedChanges _unsavedChanges;
        private readonly IDialogService _dialogService;
        private readonly IUnsavedChangesPrompt _unsavedChangesPrompt;
        private readonly IMessageNotice _messageNotice;

        public bool HasUnsavedChanges => _unsavedChanges.HasChanges;

        // True: the project may be discarded. Save counts only when the save cleared the changes, so a cancelled Save As keeps the project.
        public async Task<bool> ConfirmDiscardAsync()
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
        private string? projectPath;

        public string TitleBarText =>
            $"CMiX - {(string.IsNullOrWhiteSpace(ProjectPath) ? "(Unsaved)" : Path.GetFileName(ProjectPath))}";

        [RelayCommand]
        private async Task NewProject()
        {
            if (!await ConfirmDiscardAsync()) return;
            _session.NewProject();
            ProjectPath = null;
            _unsavedChanges.MarkSaved();
        }

        [RelayCommand]
        private async Task OpenProject()
        {
            if (!await ConfirmDiscardAsync()) return;
            var file = await _dialogService.ShowOpenFileDialogAsync(this, new OpenFileDialogSettings
            {
                Filters = new List<FileFilter> { new FileFilter("CMiX Project", "cmix") }
            });

            var path = file?.LocalPath;
            if (string.IsNullOrWhiteSpace(path)) return;

            await OpenProjectFromPath(path);
        }

        public async Task OpenProjectFromPath(string path)
        {
            try
            {
                // The parse comes first, so a parse failure leaves the open session untouched.
                var model = await _session.ReadAsync(path);
                ProjectPath = null;
                _session.LoadProject(model);
                // The path is set after the apply, so a failed apply leaves it null.
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

        [RelayCommand]
        private async Task SaveProject()
        {
            if (!string.IsNullOrWhiteSpace(ProjectPath))
            {
                await WriteProjectGuarded(ProjectPath);
                return;
            }
            await SaveAsProject();
        }

        [RelayCommand]
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
