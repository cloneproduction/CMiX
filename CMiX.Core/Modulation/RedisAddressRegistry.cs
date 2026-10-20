// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Specialized;
using System.Windows.Input;
using CMiX.Core.Animations;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Modulation
{
    // The Redis addresses of the project. Every RedisAddressModulator gives them as outputs.
    public class RedisAddressRegistry
    {
        private readonly UndoManager _undoManager;

        public RedisAddressRegistry(PrefabManager addressManager, UndoManager undoManager)
        {
            addressManager.ManagerData.ID = ManagerIDs.RedisAddressManager;
            addressManager.UndoManager = undoManager;
            _undoManager = undoManager;
            AddressManager = addressManager;
            addressManager.Activate();

            addressManager.ManagerData.Items.CollectionChanged += OnItemsChanged;
            DeleteAddressCommand = new RelayCommand<IControl>(DeleteAddress);
        }

        public PrefabManager AddressManager { get; }

        public ICommand DeleteAddressCommand { get; }

        public IEnumerable<RedisAddress> Addresses =>
            AddressManager.ManagerData.Items.OfType<RedisAddress>();

        // The name of an address output is the ID of the address.
        public event Action<string> AddressRemoved;

        // The values bound to the address go back to normal in the same undo step as the delete.
        public void DeleteAddress(IControl address)
        {
            _undoManager?.BeginCapture();
            try
            {
                AddressManager.DeleteItem(address);
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
                AddressRemoved?.Invoke(address.ID.ToString());
        }
    }
}
