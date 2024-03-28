// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.ViewModels.Assets;

namespace CMiX.Core.Texturing.Sources
{
    public class ImageModel : IControlModel, IPrefabModel
    {
        public ImageModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            Resolution = new Integer2Model(0, 0);
            Asset = new GenericValueModel<Asset>(null);
        }

        public PrefabServiceModel PrefabService { get; set; }
        public Integer2Model Resolution { get; set; }
        public Guid ID { get; set; }
        public GenericValueModel<Asset> Asset { get; set; }
    }
}
