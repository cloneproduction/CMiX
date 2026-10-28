// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Animations;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Modulation.Modulators
{
    // One float output and one multiplier for each Redis address of the project.
    public class RedisAddressModulator : ModulatorBase, INotifyOutputRemoved, IDisposable
    {
        public RedisAddressModulator(PrefabService prefabService, RedisAddressRegistry registry, PrefabManager gainManager)
            : base(prefabService)
        {
            Registry = registry;
            GainManager = gainManager;
        }

        public RedisAddressRegistry Registry { get; }
        public PrefabManager GainManager { get; set; }

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

        public IEnumerable<RedisAddressGain> Gains => GainManager.ManagerData.Items.OfType<RedisAddressGain>();

        // Gives each address one gain and takes away the gains of the addresses that are gone.
        public void EnsureGains()
        {
            var addresses = Registry.Addresses.ToList();

            foreach (var gain in Gains.Where(gain => addresses.All(address => address.ID != gain.AddressID.Value)).ToList())
                GainManager.RemoveItem(gain);

            foreach (var address in addresses.Where(address => Gains.All(gain => gain.AddressID.Value != address.ID)).ToList())
            {
                GainManager.AddItem(typeof(RedisAddressGain));
                ((RedisAddressGain)GainManager.SelectedItem).AddressID.Value = address.ID;
            }

            foreach (var gain in Gains)
                gain.Address = addresses.FirstOrDefault(address => address.ID == gain.AddressID.Value);
        }

        public void Dispose() => GainManager.Dispose();

        public override IControlModel ToModel() => new RedisAddressModulatorModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            GainManager = (PrefabManagerModel)GainManager.ToModel()
        };

        public override void FromModel(IControlModel model)
        {
            var m = (RedisAddressModulatorModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);

            LoadManager(GainManager, m.GainManager);
        }
    }
}
