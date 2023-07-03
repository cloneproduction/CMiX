// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Collections;

namespace CMiX.Core.Prefab.Managers
{
    public interface IPrefabManager : ICollectionManager
    {
        Guid ID { get; set; }
        ICommand AddItemCommand { get; set; }
        ICommand DeleteItemCommand { get; set; }

        void AddItem(Type type);
        void DeleteItem(IPrefab prefabContainer);
        void Rename();
        IPrefab GetPrefab(Guid guid);
    }
}
