// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Texturing.Sources;
using CMiX.Core.Assets;

namespace CMiX.Core.Assets
{
    public class Video : IAsset
    {
        public Video()
        {
            
        }
        public Video(string path)
        {
            FilePath = path;
        }

        public bool FileExist
        {
            get => File.Exists(FilePath);
        }

        public string Name
        {
            get => Path.GetFileNameWithoutExtension(FilePath);
        }

        public bool IsSelected { get; set; }
        public string FilePath { get; set; }
    }
}
