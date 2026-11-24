// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Studio.Avalonia.Settings
{
    // The interface settings of the Studio (StudioSettings.Interface), for the views and the Settings tab. A change is saved at once.
    // The save keeps the other parts of the file. With no file path, nothing is read or saved.
    public sealed partial class InterfaceOptions : ObservableObject
    {
        private bool loading;

        public InterfaceOptions(string? filePath = null) => FilePath = filePath;

        public string? FilePath { get; }

        // The hint strip of the keys under the tree. On by default.
        [ObservableProperty]
        private bool showKeyHints = true;

        // A missing or broken file gives the defaults.
        public void Load()
        {
            if (FilePath == null)
                return;

            var settings = StudioSettings.Load(FilePath).Interface ?? new InterfaceSettings();
            loading = true;
            try
            {
                ShowKeyHints = settings.ShowKeyHints;
            }
            finally
            {
                loading = false;
            }
        }

        partial void OnShowKeyHintsChanged(bool value)
        {
            if (loading || FilePath == null)
                return;

            var settings = StudioSettings.Load(FilePath);
            settings.Interface = new InterfaceSettings { ShowKeyHints = value };
            try
            {
                settings.Save(FilePath);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // The value stays in use until the Studio closes.
            }
        }
    }
}
