// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Assets
{
    public record AssetDirectoryModel : IAssetModel
    {
        public string FilePath { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public Guid ID { get; set; } = Guid.NewGuid();
        public bool FileExist { get; set; }
        public bool IsRenaming { get; set; }
    }
}
