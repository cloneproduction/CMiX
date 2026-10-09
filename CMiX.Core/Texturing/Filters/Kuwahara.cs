// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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
        }

        public ModulatableValue<float> Radius => Bindables[0];
        public GenericValue<KuwaharaType> Type { get; set; }

        public override IControlModel ToModel()
        {
            var model = new KuwaharaModel
            {
                Type = (GenericValueModel<KuwaharaType>)Type.ToModel(),
                Radius = (ModulatableValueModel<float>)Radius.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (KuwaharaModel)model;
            LoadBaseModel(m);
            Radius.FromModel(m.Radius);
            Type.FromModel(m.Type);
        }
    }
}
