// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Pixelate : TextureFilterBase
    {
        public Pixelate(PrefabService prefabService,
                        GenericValue<float> control,
                        Blend blend,
                        Vector2 factor)
            : base(prefabService, control, blend)
        {
            Factor = factor;
        }

        public Vector2 Factor { get; set; }

        public override IControlModel ToModel()
        {
            var model = new PixelateModel
            {
                Factor = (Vector2Model)Factor.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (PixelateModel)model;
            LoadBaseModel(m);
            Factor.FromModel(m.Factor);
        }
    }
}
