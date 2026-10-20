// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs;

namespace CMiX.Core.Modulation.Modulators
{
    // One float output for each Redis address of the project.
    public class RedisAddressModulator : ModulatorBase, INotifyOutputRemoved
    {
        public RedisAddressModulator(PrefabService prefabService, RedisAddressRegistry registry)
            : base(prefabService)
        {
            Registry = registry;
        }

        public RedisAddressRegistry Registry { get; }

        public event Action<string> OutputRemoved
        {
            add => Registry.AddressRemoved += value;
            remove => Registry.AddressRemoved -= value;
        }

        // The output name is the ID of the address, so editing the address keeps the assignments.
        public override IReadOnlyList<IModulatorOutput> Outputs =>
            Registry.Addresses
                .Select(address => new AddressedOutput(address.ID.ToString(), address.Value))
                .ToList();

        public override IControlModel ToModel() => new RedisAddressModulatorModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel()
        };

        public override void FromModel(IControlModel model)
        {
            var m = (RedisAddressModulatorModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
        }
    }
}
