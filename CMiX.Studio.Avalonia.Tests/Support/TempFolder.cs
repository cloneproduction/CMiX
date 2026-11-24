// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.Tests.Support
{
    // A folder with a new name in the temp folder of the system. Dispose removes it and all that it holds.
    internal sealed class TempFolder : IDisposable
    {
        public TempFolder(string prefix, bool create = true)
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"cmix-{prefix}-{Guid.NewGuid():N}");
            if (create)
                Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public string PathOf(string name) => System.IO.Path.Combine(Path, name);

        // Writes the text to a new file in the folder and gives its path.
        public string File(string name, string text = "x")
        {
            var path = PathOf(name);
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path)!);
            System.IO.File.WriteAllText(path, text);
            return path;
        }

        // Makes a folder in the folder with the named files in it and gives its path.
        public string Folder(string name, params string[] files)
        {
            var path = PathOf(name);
            Directory.CreateDirectory(path);
            foreach (var file in files)
                System.IO.File.WriteAllText(System.IO.Path.Combine(path, file), "x");
            return path;
        }

        // A test can put a file in the place of the folder, so both are removed.
        public void Dispose()
        {
            if (Directory.Exists(Path))
                Directory.Delete(Path, true);
            else if (System.IO.File.Exists(Path))
                System.IO.File.Delete(Path);
        }
    }
}
