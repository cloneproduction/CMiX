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
    // The Redis addresses of the project. A RedisAddressModulator selects one of them.
    public class RedisAddressRegistry
    {
        private readonly UndoManager _undoManager;
        private readonly ControlRepository _controlRepository;

        public RedisAddressRegistry(PrefabManager addressManager, UndoManager undoManager, ControlRepository controlRepository)
        {
            addressManager.ManagerData.ID = ManagerIDs.RedisAddressManager;
            addressManager.UndoManager = undoManager;
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

        public void AddAddress() => InOneUndoStep(() => AddressManager.AddItem(typeof(RedisAddress)));

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
                foreach (var modulator in _controlRepository.Controls.OfType<RedisAddressModulator>().ToList())
                    modulator.OnAddressRemoved(address.ID);
        }
    }
}
