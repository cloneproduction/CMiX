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
    // The Redis addresses of the project. Every RedisAddressModulator gives them as outputs.
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
            DeleteAddressCommand = new RelayCommand<IControl>(DeleteAddress);
        }

        public PrefabManager AddressManager { get; }

        public ICommand AddAddressCommand { get; }
        public ICommand DeleteAddressCommand { get; }

        public IEnumerable<RedisAddress> Addresses =>
            AddressManager.ManagerData.Items.OfType<RedisAddress>();

        // The name of an address output is the ID of the address.
        public event Action<string> AddressRemoved;

        public void AddAddress() => InOneUndoStep(() => AddressManager.AddItem(typeof(RedisAddress)));

        // The values bound to the address go back to normal in the same undo step as the delete.
        public void DeleteAddress(IControl address) => InOneUndoStep(() => AddressManager.RemoveItem(address));

        // For a modulator that is new or from an older project. It leaves no undo step.
        public void EnsureGainsWithoutUndo(RedisAddressModulator modulator)
        {
            _undoManager?.SuppressUndo();
            try
            {
                modulator.EnsureGains();
            }
            finally
            {
                _undoManager?.ResumeUndo();
            }
        }

        // Every modulator of the project gets or loses the gain of the address in the same undo step.
        private void InOneUndoStep(Action change)
        {
            _undoManager?.BeginCapture();
            try
            {
                change();

                foreach (var modulator in _controlRepository.Controls.OfType<RedisAddressModulator>().ToList())
                    modulator.EnsureGains();
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
