// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Blur : TextureFilterBase
    {
        public Blur(PrefabService prefabService,
                    ModulatableValue<float> strength,
                    Blend blend,
                    PrefabManager modulatorManager)
            : base(prefabService, blend, modulatorManager)
        {
            Bindables = new List<ModulatableValue<float>> { strength };
        }

        public ModulatableValue<float> Strength => Bindables[0];

        public override IControlModel ToModel()
        {
            var model = new BlurModel { Strength = (ModulatableValueModel<float>)Strength.ToModel() };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (BlurModel)model;
            LoadBaseModel(m);
            Strength.FromModel(m.Strength);
        }
    }
}
