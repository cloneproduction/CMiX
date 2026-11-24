// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Pulse : TextureFilterBase
    {
        public Pulse(PrefabService prefabService,
                     ModulatableValue<float> center,
                     ModulatableValue<float> slopeWidth,
                     ModulatableValue<float> centerWidth,
                     GenericValue<bool> invert,
                     GenericValue<PulseChannel> channel,
                     Blend blend,
                     ModulatorHost modulators)
            : base(prefabService, blend, modulators)
        {
            Invert = invert;
            Channel = channel;

            Bindables = new List<ModulatableValue<float>> { center, slopeWidth, centerWidth };
        }

        public ModulatableValue<float> Center => Bindables[0];
        public ModulatableValue<float> SlopeWidth => Bindables[1];
        public ModulatableValue<float> CenterWidth => Bindables[2];
        public GenericValue<bool> Invert { get; set; }
        public GenericValue<PulseChannel> Channel { get; set; }

        public override IControlModel ToModel()
        {
            var model = new PulseModel
            {
                Center = (ModulatableValueModel<float>)Center.ToModel(),
                SlopeWidth = (ModulatableValueModel<float>)SlopeWidth.ToModel(),
                CenterWidth = (ModulatableValueModel<float>)CenterWidth.ToModel(),
                Invert = (GenericValueModel<bool>)Invert.ToModel(),
                Channel = (GenericValueModel<PulseChannel>)Channel.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (PulseModel)model;
            LoadBaseModel(m);
            Center.FromModel(m.Center);
            SlopeWidth.FromModel(m.SlopeWidth);
            CenterWidth.FromModel(m.CenterWidth);
            Invert.FromModel(m.Invert);
            Channel.FromModel(m.Channel);
        }
    }
}
