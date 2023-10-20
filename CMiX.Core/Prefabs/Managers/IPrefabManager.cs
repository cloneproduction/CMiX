// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Collections;

namespace CMiX.Core.Prefabs.Managers
{
    public interface IPrefabManager : ICollectionManager
    {
        ICommand AddItemCommand { get; set; }
        //ICommand DeleteItemCommand { get; set; }

        void AddItem(Type type);
        void ReplaceEmptyPrefab(Guid emptyPrefabID, IControlModel controlModel);
        void SelectedItemChanged(Guid id, int index);

        void Rename();
    }
}
