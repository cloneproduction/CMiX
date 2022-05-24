// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CMiX.Core.Presentation.ViewModels.Prefab;

namespace CMiX.Core.Presentation.ViewModels
{
    public interface IPrefabManager
    {
        Guid ID { get; set; }
        ICommand AddItemCommand { get; set; }
        ICommand DeleteItemCommand { get; set; }
        ICommand RenameCommand { get; }

        ObservableCollection<IPrefab> Prefabs { get; set; }

        void AddItem();
        void AddItem(IPrefabModel prefabModel);
        void DeleteItem();
        void DeleteItem(Guid id);
        void Rename();
        IPrefab SelectedItem { get; set; }
    }
}
