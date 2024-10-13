// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.ViewModels.Assets;

namespace CMiX.Core.BaseControls
{
    public class AssetSelectorModel : IControlModel
    {
        public AssetSelectorModel()
        {
            ID = Guid.NewGuid();
            Asset = new GenericValueModel<IAsset>();
        }

        public Guid ID { get; set; }
        public GenericValueModel<IAsset> Asset { get; set; }
    }
}
