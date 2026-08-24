// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Sources
{
    public partial class VideoIn : TextureSourceBase, ITextureSource
    {
        public VideoIn(PrefabService prefabService,
                       PrefabManager textureModifierManager,
                       GenericValue<int> sizeX,
                       GenericValue<int> sizeZ)
            : base(prefabService, textureModifierManager)
        {
            SizeX = sizeX;
            SizeY = sizeZ;
            Resolution = new Integer2(sizeX, sizeZ);
        }

        public GenericValue<int> SizeX { get; set; }
        public GenericValue<int> SizeY { get; set; }
        public Integer2 Resolution { get; set; }

        public override IControlModel ToModel()
        {
            var model = new VideoInModel
            {
                SizeX = (GenericValueModel<int>)SizeX.ToModel(),
                SizeY = (GenericValueModel<int>)SizeY.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (VideoInModel)model;
            LoadBaseModel(m);
            SizeX.FromModel(m.SizeX);
            SizeY.FromModel(m.SizeY);
        }
    }
}
