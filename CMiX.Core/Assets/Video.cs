// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
