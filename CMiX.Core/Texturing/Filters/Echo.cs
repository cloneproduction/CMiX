// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Echo : TextureFilterBase
    {
        public Echo(PrefabService prefabService,
                    ModulatableValue<float> factor,
                    Blend blend,
                    PrefabManager modulatorManager)
            : base(prefabService, blend, modulatorManager)
        {
            Bindables = new List<ModulatableValue<float>> { factor };
        }

        public ModulatableValue<float> Factor => Bindables[0];

        public override IControlModel ToModel()
        {
            var model = new EchoModel { Factor = (ModulatableValueModel<float>)Factor.ToModel() };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (EchoModel)model;
            LoadBaseModel(m);
            Factor.FromModel(m.Factor);
        }
    }
}
