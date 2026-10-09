// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Prefabs.Managers
{
    public delegate IManagerReorderService ManagerReorderServiceFactory(
        CollectionManager collectionManager, Action<int, int> onMove);
}
