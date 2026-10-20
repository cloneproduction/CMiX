// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Animations;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Modulation.Modulators
{
    // A list of Redis addresses. Each address is one float output.
    public class RedisAddressModulator : ModulatorBase, IDisposable
    {
        public RedisAddressModulator(PrefabService prefabService, PrefabManager addressManager)
            : base(prefabService)
        {
            AddressManager = addressManager;
        }

        public PrefabManager AddressManager { get; set; }

        // The output name is the ID of the address, so editing the address keeps the assignments.
        public override IReadOnlyList<IModulatorOutput> Outputs =>
            AddressManager.ManagerData.Items
                .OfType<RedisAddress>()
                .Select(address => new AddressedOutput(address.ID.ToString(), address.Value))
                .ToList();

        public void Dispose() => AddressManager.Dispose();

        public override IControlModel ToModel() => new RedisAddressModulatorModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            AddressManager = (PrefabManagerModel)AddressManager.ToModel()
        };

        public override void FromModel(IControlModel model)
        {
            var m = (RedisAddressModulatorModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);

            LoadManager(AddressManager, m.AddressManager);
        }
    }
}
