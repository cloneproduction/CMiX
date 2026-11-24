// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.Settings
{
    // The settings file of the Studio at one path. A test gives a temp path.
    public sealed class StudioSettingsFile
    {
        public StudioSettingsFile(string filePath) => FilePath = filePath;

        public string FilePath { get; }

        public StudioSettings Load() => StudioSettings.Load(FilePath);

        public void Save(StudioSettings settings) => settings.Save(FilePath);
    }
}
