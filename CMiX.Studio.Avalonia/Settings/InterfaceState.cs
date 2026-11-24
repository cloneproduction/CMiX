// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Studio.Avalonia.Settings
{
    // The interface settings of the Studio (StudioSettings.Interface), live, for the views and the Settings tab. A change is saved at once.
    // The save keeps the other parts of the file. With no file, nothing is read or saved.
    public sealed partial class InterfaceState : ObservableObject
    {
        private bool loading;

        private readonly StudioSettingsFile? file;

        public InterfaceState(StudioSettingsFile? file = null) => this.file = file;

        // The hint strip of the keys under the tree. On by default.
        [ObservableProperty]
        private bool showKeyHints = true;

        // A missing or broken file gives the defaults.
        public void Load()
        {
            if (file == null)
                return;

            var settings = file.Load().Interface ?? new InterfaceSettings();
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
            if (loading || file == null)
                return;

            var settings = file.Load();
            settings.Interface = new InterfaceSettings { ShowKeyHints = value };
            try
            {
                file.Save(settings);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // The value stays in use until the Studio closes.
            }
        }
    }
}
