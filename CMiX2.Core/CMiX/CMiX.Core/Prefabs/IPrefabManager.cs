// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;

namespace CMiX.Core.Prefabs
{
    public interface IPrefabManager
    {
        Guid ID { get; set; }
        ICommand AddItemCommand { get; set; }
        ICommand DeleteItemCommand { get; set; }
        ICommand RenameCommand { get; }

        void AddItem();
        void DeleteItem(IPrefab prefabContainer);
        void DeleteItem(Guid id);
        void Rename();
        IPrefab GetPrefab(Guid guid);
        void AddPrefab(IPrefabModel prefabModel);
        void MovePrefab(int oldIndex, int newIndex);
        void SelectPrefab(Guid prefabID);
        void ChangePrefab(Guid containerID, Guid prefabID);
    }
}
