// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.ComponentModel;
using CMiX.Studio.Avalonia.Services;
using HanumanInstitute.MvvmDialogs.Avalonia;
using HanumanInstitute.MvvmDialogs.FileSystem;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

namespace CMiX.Studio.Avalonia.Tests
{
    // Answers the save file dialog with a fixed path, without a window.
    public sealed class FakeSaveFileDialogManager : MainWindowDialogManager
    {
        private readonly string _path;

        public FakeSaveFileDialogManager(string path)
            : base(new EmptyViewLocator(), new DialogFactory())
        {
            _path = path;
        }

        public override Task<object?> ShowFrameworkDialogAsync<TSettings>(INotifyPropertyChanged? ownerViewModel, TSettings settings, Func<object, string>? resultToString = null)
        {
            if (settings is SaveFileDialogSettings)
                return Task.FromResult<object?>(new DesktopDialogStorageFile(_path));
            return base.ShowFrameworkDialogAsync(ownerViewModel, settings, resultToString);
        }
    }
}
