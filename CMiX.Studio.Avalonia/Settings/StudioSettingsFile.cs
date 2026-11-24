// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.Settings
{
    // The settings file of the Studio at one path. A test gives a temp path.
    public sealed class StudioSettingsFile
    {
        public StudioSettingsFile(string filePath) => FilePath = filePath;

        public string FilePath { get; }

        // A missing, empty, broken or locked file falls back to defaults. The Studio must start in every case.
        public StudioSettings Load() => JsonSettingsFile.Load<StudioSettings>(FilePath);

        public void Save(StudioSettings settings) => JsonSettingsFile.Save(FilePath, settings);
    }
}
