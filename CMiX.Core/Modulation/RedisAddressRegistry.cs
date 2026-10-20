// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Animations;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;

namespace CMiX.Core.Modulation
{
    // The Redis addresses of the project. Every RedisAddressModulator gives them as outputs.
    public class RedisAddressRegistry
    {
        public RedisAddressRegistry(PrefabManager addressManager, UndoManager undoManager)
        {
            addressManager.ManagerData.ID = ManagerIDs.RedisAddressManager;
            addressManager.UndoManager = undoManager;
            AddressManager = addressManager;
            addressManager.Activate();
        }

        public PrefabManager AddressManager { get; }

        public IEnumerable<RedisAddress> Addresses =>
            AddressManager.ManagerData.Items.OfType<RedisAddress>();
    }
}
