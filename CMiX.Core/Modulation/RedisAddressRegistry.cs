// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Specialized;
using System.Windows.Input;
using CMiX.Core.Animations;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Modulation
{
    // The Redis addresses of the project. A Redis modulator selects one of them.
    public class RedisAddressRegistry
    {
        private readonly UndoManager _undoManager;
        private readonly ControlRepository _controlRepository;

        public RedisAddressRegistry(PrefabManager addressManager, UndoManager undoManager, ControlRepository controlRepository)
        {
            addressManager.ManagerData.ID = ManagerIDs.RedisAddressManager;
            _undoManager = undoManager;
            _controlRepository = controlRepository;
            AddressManager = addressManager;
            addressManager.Activate();

            addressManager.ManagerData.Items.CollectionChanged += OnItemsChanged;
            AddAddressCommand = new RelayCommand(AddAddress);
            RemoveAddressCommand = new RelayCommand<IControl>(RemoveAddress);
        }

        public PrefabManager AddressManager { get; }

        public ICommand AddAddressCommand { get; }
        public ICommand RemoveAddressCommand { get; }

        public IEnumerable<RedisAddress> Addresses =>
            AddressManager.ManagerData.Items.OfType<RedisAddress>();

        // A new address has a name, "Address.001" and so on, so that the list row has a text to edit.
        public void AddAddress() => InOneUndoStep(() =>
            AddressManager.AddItem(typeof(RedisAddress), item => ((RedisAddress)item).Value = NextName()));

        // The first free name of the form "Address.001", as for the other items.
        private string NextName()
        {
            var used = Addresses.Select(address => address.Value).ToHashSet();
            for (var number = 1; ; number++)
            {
                var name = $"{DefaultName}.{number:000}";
                if (!used.Contains(name))
                    return name;
            }
        }

        private const string DefaultName = "Address";

        // The modulators that select the address clear their selection in the same undo step as the removal.
        public void RemoveAddress(IControl address) => InOneUndoStep(() => AddressManager.RemoveItem(address));

        private void InOneUndoStep(Action change)
        {
            _undoManager?.BeginCapture();
            try
            {
                change();
            }
            finally
            {
                _undoManager?.EndCapture();
            }
        }

        private void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Remove || e.OldItems == null) return;

            foreach (var address in e.OldItems.OfType<RedisAddress>())
                foreach (var modulator in _controlRepository.Controls.OfType<RedisModulatorBase>().ToList())
                    modulator.OnAddressRemoved(address.ID);
        }
    }
}
