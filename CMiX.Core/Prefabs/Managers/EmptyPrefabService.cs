// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs.Messages;

namespace CMiX.Core.Prefabs.Managers
{
    public class EmptyPrefabService
    {
        public EmptyPrefabService(ManagerData managerData, ControlFactory prefabFactory, ManagerMessenger managerMessenger)
        {
            ManagerData = managerData;
            PrefabFactory = prefabFactory;
            ManagerMessenger = managerMessenger;
        }

        ManagerMessenger ManagerMessenger { get; set; }
        ControlFactory PrefabFactory { get; set; }
        ManagerData ManagerData { get; set; }

        public void ReplaceEmptyPrefab(Guid emptyPrefabID, IControlModel controlModel)
        {
            var emptyPrefab = ManagerData.Items.FirstOrDefault(x => x.ID == emptyPrefabID);
            int index = ManagerData.Items.IndexOf(emptyPrefab);
            IControl prefab = ManagerData.Items.ToList().Find(x => x.ID == emptyPrefabID);

            if (prefab is not EmptyPrefab)
                return;

            prefab = PrefabFactory.Create(controlModel);
            ManagerData.Items[index] = prefab;
        }
    }
}
