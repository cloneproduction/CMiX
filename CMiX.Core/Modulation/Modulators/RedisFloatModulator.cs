// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Modulation.Modulators
{
    // One Redis address of the project as a float output, times a gain.
    public class RedisFloatModulator : RedisModulatorBase
    {
        public RedisFloatModulator(PrefabService prefabService, RedisAddressRegistry registry,
                                   GenericValue<Guid?> selectedAddressID, GenericValue<float> gain)
            : base(prefabService, registry, selectedAddressID)
        {
            Gain = gain;
        }

        public GenericValue<float> Gain { get; set; }

        protected override IModulatorOutput Output =>
            new AddressedOutput<float>(OutputName, SelectedAddress?.Value ?? OutputName);

        public override IControlModel ToModel() => new RedisFloatModulatorModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            SelectedAddressID = (GenericValueModel<Guid?>)SelectedAddressID.ToModel(),
            Gain = (GenericValueModel<float>)Gain.ToModel()
        };

        public override void FromModel(IControlModel model)
        {
            var m = (RedisFloatModulatorModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            SelectedAddressID.FromModel(m.SelectedAddressID);
            Gain.FromModel(m.Gain);
        }
    }
}
