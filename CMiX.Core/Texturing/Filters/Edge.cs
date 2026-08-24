// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Edge : TextureFilterBase
    {
        public Edge(PrefabService prefabService,
                    GenericValue<float> radius,
                    GenericValue<float> brightness,
                    GenericValue<float> control,
                    Blend blend)
            : base(prefabService, control, blend)
        {
            Radius = radius;
            Brightness = brightness;
        }

        public GenericValue<float> Radius { get; set; }
        public GenericValue<float> Brightness { get; set; }

        public override IControlModel ToModel()
        {
            var model = new EdgeModel
            {
                Radius = (GenericValueModel<float>)Radius.ToModel(),
                Brightness = (GenericValueModel<float>)Brightness.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (EdgeModel)model;
            LoadBaseModel(m);
            Radius.FromModel(m.Radius);
            Brightness.FromModel(m.Brightness);
        }
    }
}
