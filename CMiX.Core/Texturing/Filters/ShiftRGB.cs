// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class ShiftRGB : TextureFilterBase
    {
        public ShiftRGB(PrefabService prefabService,
                        ModulatableValue<float> direction,
                        ModulatableValue<float> shift,
                        ModulatableValue<float> hue,
                        Blend blend,
                        PrefabManager modulatorManager)
            : base(prefabService, blend, modulatorManager)
        {
            Bindables = new List<ModulatableValue<float>> { direction, shift, hue };
        }

        public ModulatableValue<float> Direction => Bindables[0];
        public ModulatableValue<float> Shift => Bindables[1];
        public ModulatableValue<float> Hue => Bindables[2];

        public override IControlModel ToModel()
        {
            var model = new ShiftRGBModel
            {
                Direction = (ModulatableValueModel<float>)Direction.ToModel(),
                Shift = (ModulatableValueModel<float>)Shift.ToModel(),
                Hue = (ModulatableValueModel<float>)Hue.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (ShiftRGBModel)model;
            LoadBaseModel(m);
            Direction.FromModel(m.Direction);
            Shift.FromModel(m.Shift);
            Hue.FromModel(m.Hue);
        }
    }
}
