// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Modulation.Modulators
{
    // A modulator that selects one Redis address of the project and gives it as one output.
    public abstract class RedisModulatorBase : ModulatorBase, INotifyOutputRemoved
    {
        protected const string OutputName = "Value";

        protected RedisModulatorBase(PrefabService prefabService, RedisAddressRegistry registry,
                                     GenericValue<Guid?> selectedAddressID)
            : base(prefabService)
        {
            Registry = registry;
            SelectedAddressID = selectedAddressID;

            SelectedAddressID.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(GenericValue<Guid?>.Value))
                    OnPropertyChanged(nameof(SelectedAddress));
            };
        }

        public RedisAddressRegistry Registry { get; }
        public GenericValue<Guid?> SelectedAddressID { get; set; }

        public RedisAddress SelectedAddress
        {
            get => Registry.Addresses.FirstOrDefault(address => address.ID == SelectedAddressID.Value);
            set => SelectedAddressID.Value = value?.ID;
        }

        public event Action<string> OutputRemoved;

        // The output name stays the same for every selection, so a change of the selection keeps the assignments.
        protected abstract IModulatorOutput Output { get; }

        public override IReadOnlyList<IModulatorOutput> Outputs => new[] { Output };

        public void OnAddressRemoved(Guid id)
        {
            if (SelectedAddressID.Value != id) return;

            // The bound values go back to normal before the selection clears.
            OutputRemoved?.Invoke(OutputName);
            SelectedAddressID.Value = null;
        }
    }
}
