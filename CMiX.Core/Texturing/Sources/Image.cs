// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Sources
{
    public partial class Image : TextureSourceBase, IAssetTextureSource
    {
        public Image(PrefabService prefabService,
                     PrefabManager textureModifierManager,
                     Integer2 resolution,
                     AssetSelector assetSelector)
            : base(prefabService, textureModifierManager)
        {
            Resolution = resolution;
            AssetSelector = assetSelector;
        }

        public Integer2 Resolution { get; set; }
        public AssetSelector AssetSelector { get; set; }

        public override IControlModel ToModel()
        {
            var model = new ImageModel
            {
                Resolution = (Integer2Model)Resolution.ToModel(),
                AssetSelector = (AssetSelectorModel)AssetSelector.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (ImageModel)model;
            LoadBaseModel(m);
            Resolution.FromModel(m.Resolution);
            AssetSelector.FromModel(m.AssetSelector);
        }
    }
}
