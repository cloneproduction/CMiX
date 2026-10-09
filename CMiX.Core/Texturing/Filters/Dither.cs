// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Dither : TextureFilterBase
    {
        public Dither(PrefabService prefabService,
                      ModulatableValue<float> threshold,
                      Blend blend,
                      PrefabManager modulatorManager)
            : base(prefabService, blend, modulatorManager)
        {
            Bindables = new List<ModulatableValue<float>> { threshold };
        }

        public ModulatableValue<float> Threshold => Bindables[0];

        public override IControlModel ToModel()
        {
            var model = new DitherModel { Threshold = (ModulatableValueModel<float>)Threshold.ToModel() };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (DitherModel)model;
            LoadBaseModel(m);
            Threshold.FromModel(m.Threshold);
        }
    }
}
