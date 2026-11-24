// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CMiX.Core.Compositing;
using CMiX.Studio.Avalonia.Notices;
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
        private static readonly List<FileFilter> ProjectFilters = new() { new FileFilter("CMiX Project", "cmix") };

        // One fingerprint of the whole project. A caller reads it once and then asks with AskToDiscardAsync.
        public bool HasUnsavedChanges => _unsavedChanges.HasChanges;

        // True: the project may be discarded.
        public async Task<bool> ConfirmDiscardAsync() => !HasUnsavedChanges || await AskToDiscardAsync();

        // Asks without a fingerprint check. Save counts only when the file was written, so a cancelled Save As keeps the project.
        public async Task<bool> AskToDiscardAsync() =>
            await _unsavedChangesPrompt.AskAsync() switch
            {
                UnsavedChangesChoice.Save => await WriteOrAskForPath(),
                UnsavedChangesChoice.DontSave => true,
                _ => false
            };

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TitleBarText))]
        private string? projectPath;

        public string TitleBarText =>
            $"CMiX - {(string.IsNullOrWhiteSpace(ProjectPath) ? "(Unsaved)" : Path.GetFileName(ProjectPath))}";

        [RelayCommand]
        private async Task NewProject()
        {
            if (!await ConfirmDiscardAsync()) return;
            _session.ClearAll();
            ProjectPath = null;
            _unsavedChanges.MarkSaved();
        }

        [RelayCommand]
        private async Task OpenProject()
        {
            if (!await ConfirmDiscardAsync()) return;
            var file = await _dialogService.ShowOpenFileDialogAsync(this, new OpenFileDialogSettings { Filters = ProjectFilters });

            var path = file?.LocalPath;
            if (string.IsNullOrWhiteSpace(path)) return;

            await OpenProjectFromPath(path);
        }

        public async Task OpenProjectFromPath(string path)
        {
            ProjectModel model;
            try
            {
                model = await _session.ReadAsync(path);
            }
            catch (Exception ex)
            {
                await ShowOpenFailure(path, ex);
                return;
            }

            // A failed apply leaves an empty session, which is the saved state with no path.
            ProjectPath = null;
            try
            {
                _session.LoadProject(model);
                ProjectPath = path;
            }
            catch (Exception ex)
            {
                await ShowOpenFailure(path, ex);
                return;
            }
            finally
            {
                _unsavedChanges.MarkSaved();
            }

            // The push is outside the catch: the file is open, and a push failure shows in the sync status.
            await _session.PushToPeersAsync();
        }

        private Task ShowOpenFailure(string path, Exception ex) =>
            _messageNotice.ShowErrorAsync($"{Path.GetFileName(path)}: {ex.Message}", "Open Project");

        [RelayCommand]
        private Task SaveProject() => WriteOrAskForPath();

        [RelayCommand]
        private Task SaveAsProject() => AskForPathAndWrite();

        private Task<bool> WriteOrAskForPath() =>
            string.IsNullOrWhiteSpace(ProjectPath) ? AskForPathAndWrite() : WriteProjectGuarded(ProjectPath);

        // True when the file was written. A cancelled dialog or a failed write returns false and keeps the path.
        private async Task<bool> AskForPathAndWrite()
        {
            var file = await _dialogService.ShowSaveFileDialogAsync(this, new SaveFileDialogSettings
            {
                Filters = ProjectFilters,
                DefaultExtension = "cmix"
            });

            var path = file?.LocalPath;
            if (string.IsNullOrWhiteSpace(path)) return false;

            if (!await WriteProjectGuarded(path)) return false;
            ProjectPath = path;
            return true;
        }

        // True when the file was written. A failure shows the notice and returns false.
        private async Task<bool> WriteProjectGuarded(string path)
        {
            try
            {
                var model = _session.CaptureProject();
                await _session.WriteAsync(model, path);
                _unsavedChanges.MarkSaved(model);
                return true;
            }
            catch (Exception ex)
            {
                await _messageNotice.ShowErrorAsync($"{Path.GetFileName(path)}: {ex.Message}", "Save Project");
                return false;
            }
        }
    }
}
