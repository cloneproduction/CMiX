// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Mapping;

namespace CMiX.Core.Prefabs.Managers
{
    public class ManagerPairProfile : ControlPairProfile
    {
        public ManagerPairProfile()
        {
            CreatePair<ReorderablePrefabManager, PrefabManagerModel>();
            CreatePair<PrefabManager, PrefabManagerModel>();
            CreatePair<PrefabManagerBase, PrefabManagerModel>();
            CreatePair<ManagerData, ManagerDataModel>();
        }
    }
}
