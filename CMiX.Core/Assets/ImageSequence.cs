// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
