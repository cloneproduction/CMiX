// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Modulation.Modulators
{
    // One Redis address of the project as a float output, times a gain.
    public class RedisAddressModulator : ModulatorBase, INotifyOutputRemoved
    {
        public RedisAddressModulator(PrefabService prefabService, RedisAddressRegistry registry,
                                     GenericValue<Guid?> selectedAddressID, GenericValue<float> gain)
            : base(prefabService)
        {
            Registry = registry;
            SelectedAddressID = selectedAddressID;
            Gain = gain;

            SelectedAddressID.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(GenericValue<Guid?>.Value))
                    OnPropertyChanged(nameof(SelectedAddress));
            };
        }

        public RedisAddressRegistry Registry { get; }
        public GenericValue<Guid?> SelectedAddressID { get; set; }
        public GenericValue<float> Gain { get; set; }

        public RedisAddress SelectedAddress
        {
            get => Registry.Addresses.FirstOrDefault(address => address.ID == SelectedAddressID.Value);
            set => SelectedAddressID.Value = value?.ID;
        }

        public event Action<string> OutputRemoved;

        // The output name stays the same for every selection, so a change of the selection keeps the assignments.
        public override IReadOnlyList<IModulatorOutput> Outputs =>
            new[] { new AddressedOutput("Value", SelectedAddress?.Value ?? "Value") };

        public void OnAddressRemoved(Guid id)
        {
            if (SelectedAddressID.Value != id) return;

            // The bound values go back to normal before the selection clears.
            OutputRemoved?.Invoke("Value");
            SelectedAddressID.Value = null;
        }

        public override IControlModel ToModel() => new RedisAddressModulatorModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            SelectedAddressID = (GenericValueModel<Guid?>)SelectedAddressID.ToModel(),
            Gain = (GenericValueModel<float>)Gain.ToModel()
        };

        public override void FromModel(IControlModel model)
        {
            var m = (RedisAddressModulatorModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            SelectedAddressID.FromModel(m.SelectedAddressID);
            Gain.FromModel(m.Gain);
        }
    }
}
