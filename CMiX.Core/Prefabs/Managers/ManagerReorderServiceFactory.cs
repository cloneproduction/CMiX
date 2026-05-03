// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Prefabs.Managers
{
    public delegate IManagerReorderService ManagerReorderServiceFactory(
        CollectionManager collectionManager, Action<int, int> onMove);
}
