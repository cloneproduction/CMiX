// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.IO;
using System.Text.Json;

namespace CMiX.Studio.Avalonia.Settings
{
    // Reads and writes a settings file as indented JSON. It holds no state.
    public static class JsonSettingsFile
    {
        private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

        // A missing, empty, broken or locked file gives new T(). The Studio must start in every case.
        public static T Load<T>(string path) where T : new()
        {
            try
            {
                if (!File.Exists(path)) return new T();

                var json = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(json)) return new T();

                return JsonSerializer.Deserialize<T>(json, Options) ?? new T();
            }
            catch (Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException)
            {
                return new T();
            }
        }

        // Creates the folder when it is missing.
        public static void Save<T>(string path, T value)
        {
            var folder = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(folder))
                Directory.CreateDirectory(folder);

            File.WriteAllText(path, JsonSerializer.Serialize(value, Options));
        }
    }
}
