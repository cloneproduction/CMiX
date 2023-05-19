// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Assets
{
    public class AssetImageModel : IAssetModel
    {
        public AssetImageModel()
        {
            ID = Guid.NewGuid();
        }

        public Guid ID { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public string FilePath { get; set; }
        public bool FileExist { get; set; }
    }
}
