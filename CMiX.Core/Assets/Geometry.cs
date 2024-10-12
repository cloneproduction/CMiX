// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.ViewModels.Assets;

namespace CMiX.Core.Assets
{
    public class Geometry : IAsset
    {
        public Geometry(string path)
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
