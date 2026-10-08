// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Edge : TextureFilterBase
    {
        public Edge(PrefabService prefabService,
                    ModulatableValue<float> radius,
                    ModulatableValue<float> brightness,
                    Blend blend,
                    PrefabManager modulatorManager)
            : base(prefabService, blend, modulatorManager)
        {
            Bindables = new List<ModulatableValue<float>> { radius, brightness };
        }

        public ModulatableValue<float> Radius => Bindables[0];
        public ModulatableValue<float> Brightness => Bindables[1];

        public override IControlModel ToModel()
        {
            var model = new EdgeModel
            {
                Radius = (ModulatableValueModel<float>)Radius.ToModel(),
                Brightness = (ModulatableValueModel<float>)Brightness.ToModel()
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
