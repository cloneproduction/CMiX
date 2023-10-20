// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Prefabs
{
    public interface IPrefabFactory
    {
        IPrefab CreatePrefab(PrefabService prefabService);
        IPrefab CreatePrefab(PrefabService prefabService, IPrefabModel prefabModel);
        bool AppliesTo(Type type);
    }
}
