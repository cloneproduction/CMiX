// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Assets
{
    public class ImageSequence : IAsset
    {
        public ImageSequence()
        {

        }
        public ImageSequence(string path)
        {
            FilePath = path;
        }

        public bool FileExist
        {
            get => Directory.Exists(FilePath);
        }

        public string Name
        {
            get => Path.GetFileName(Path.TrimEndingDirectorySeparator(FilePath));
        }

        public bool IsSelected { get; set; }

        // The folder path.
        public string FilePath { get; set; }
    }
}
