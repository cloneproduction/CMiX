// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs.Messages;

namespace CMiX.Core.Prefabs.Managers
{
    public class PrefabManager : PrefabManagerBase
    {
        public PrefabManager()
        {
            
        }
        public PrefabManager(ManagerData managerData, ControlFactory controlFactory, ManagerMessenger managerMessenger) : base (managerData, controlFactory, managerMessenger)
        {

        }
    }
}
