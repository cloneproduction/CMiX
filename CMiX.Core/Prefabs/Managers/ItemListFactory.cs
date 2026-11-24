// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Undo;

namespace CMiX.Core.Prefabs.Managers
{
    // Gives the synced list of a collection. The DI binds it to the messenger.
    public delegate IItemList ItemListFactory(CollectionManager collection);
}
