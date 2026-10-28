// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Modulation.Modulators
{
    // One Redis address of the project as a bool output.
    public class RedisBoolModulator : RedisModulatorBase
    {
        public RedisBoolModulator(PrefabService prefabService, RedisAddressRegistry registry,
                                  GenericValue<Guid?> selectedAddressID)
            : base(prefabService, registry, selectedAddressID)
        {
        }

        protected override IModulatorOutput Output =>
            new AddressedOutput<bool>(OutputName, SelectedAddress?.Value ?? OutputName);

        public override IControlModel ToModel() => new RedisBoolModulatorModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            SelectedAddressID = (GenericValueModel<Guid?>)SelectedAddressID.ToModel()
        };

        public override void FromModel(IControlModel model)
        {
            var m = (RedisBoolModulatorModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            SelectedAddressID.FromModel(m.SelectedAddressID);
        }
    }
}
