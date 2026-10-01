// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

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

            radius.Label = "Radius";
            brightness.Label = "Brightness";
            radius.SetDefault(1.0f);
            brightness.SetDefault(1.0f);
        }

        public ModulatableValue<float> Radius => Bindables[0];
        public ModulatableValue<float> Brightness => Bindables[1];

        public override IControlModel ToModel()
        {
            var model = new EdgeModel();
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (EdgeModel)model;
            LoadBaseModel(m);
        }
    }
}
