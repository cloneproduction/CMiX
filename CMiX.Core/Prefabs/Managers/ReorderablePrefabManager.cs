// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs.Messages;

namespace CMiX.Core.Prefabs.Managers
{
    public class ReorderablePrefabManager : PrefabManagerBase
    {
        public ReorderablePrefabManager(ManagerData managerData, ControlFactory controlFactory, ManagerMessenger managerMessenger) : base (managerData, controlFactory, managerMessenger)
        {
            ManagerData = managerData;
            ManagerReorderService = new ManagerReorderService(managerData, managerMessenger);
            EmptyPrefabService = new EmptyPrefabService(managerData, controlFactory, managerMessenger);
        }

        public EmptyPrefabService EmptyPrefabService { get; set; }
        public ManagerReorderService ManagerReorderService { get; set; }
    }
}
