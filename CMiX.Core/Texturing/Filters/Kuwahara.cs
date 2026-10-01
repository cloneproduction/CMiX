// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Kuwahara : TextureFilterBase
    {
        public Kuwahara(PrefabService prefabService,
                        ModulatableValue<float> radius,
                        GenericValue<KuwaharaType> type,
                        Blend blend,
                        PrefabManager modulatorManager)
            : base(prefabService, blend, modulatorManager)
        {
            Type = type;
            Bindables = new List<ModulatableValue<float>> { radius };

            radius.Label = "Radius";
            radius.SetDefault(1.0f);
        }

        public ModulatableValue<float> Radius => Bindables[0];
        public GenericValue<KuwaharaType> Type { get; set; }

        public override IControlModel ToModel()
        {
            var model = new KuwaharaModel
            {
                Type = (GenericValueModel<KuwaharaType>)Type.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (KuwaharaModel)model;
            LoadBaseModel(m);
            Type.FromModel(m.Type);
        }
    }
}
