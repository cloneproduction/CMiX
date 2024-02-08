// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Prefabs.Managers
{
    public partial class PrefabManagerBase : ObservableRecipient, IControl, IRecipient<IMessage>
    {
        public PrefabManagerBase(ManagerData managerData, ControlFactory controlFactory, ManagerMessenger managerMessenger)
        {
            ManagerMessenger = managerMessenger;
            ControlFactory = controlFactory;
            ManagerData = managerData;
            ManagerData.PropertyChanged += ManagerData_PropertyChanged;
            AddItemCommand = new RelayCommand<Type>(AddItem);
            DeleteItemCommand = new RelayCommand<IControl>(DeleteItem);

            IsActive = true;
        }

        private void ManagerData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ManagerMessenger.SendSelectedItemChanged(ManagerData.ID, ManagerData.SelectedItem, ManagerData.SelectedIndex);
        }

        public Guid ID { get; set; }
        public ICommand AddItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }

        ManagerMessenger ManagerMessenger { get; set; }
        ControlFactory ControlFactory { get; set; }
        public ManagerData ManagerData { get; set; }

        public void AddItem(Type type)
        {
            var prefab = ControlFactory.Create(type);
            var items = ManagerData.Items;

            if (ManagerData.SelectedItem is EmptyPrefab emptyPrefab && prefab is IPrefab pre)
            {
                items[items.IndexOf(ManagerData.SelectedItem)] = prefab;
                ManagerMessenger.SendReplaceEmptyPrefab(ManagerData.ID, pre, emptyPrefab);
            }
            else
            {
                items.Add(prefab);
                ManagerMessenger.SendAddItem(ManagerData.ID, prefab);
            }

            ManagerData.SelectedItem = prefab;
        }

        public void AddItem(IControlModel controlModel)
        {
            var selectedItem = ManagerData.SelectedItem;
            var items = ManagerData.Items;

            var prefab = ControlFactory.Create(controlModel);

            if (selectedItem is EmptyPrefab)
                items[items.IndexOf(selectedItem)] = prefab;
            else
                items.Add(prefab);

            ManagerData.SelectedItem = prefab;
        }

        public void DeleteItem(Guid id)
        {
            var prefab = ManagerData.Items.FirstOrDefault(x => x.ID == id);

            if (prefab == null)
                return;

            ManagerData.Items.Remove(prefab);
        }

        public void DeleteItem(IControl control)
        {
            var index = ManagerData.SelectedIndex;

            if (control == null)
                return;

            ManagerData.Items.RemoveAt(index);
            ManagerMessenger.SendMessageRemoveItem(ManagerData.ID, control);

            if (ManagerData.Items.Count == 0)
            {
                ManagerData.SelectedItem = null;
                ManagerData.SelectedIndex = -1;
                return;
            }

            if (index == 0)
            {
                ManagerData.SelectedItem = ManagerData.Items[0];
                return;
            }

            if (index > 0)
            {
                ManagerData.SelectedIndex = index - 1;
                return;
            }
        }

        public void MoveItem(int oldIndex, int newIndex)
        {
            var items = ManagerData.Items;

            if (items.Count == 0)
                return;

            items.Move(oldIndex, newIndex);
        }

        public void SelectedItemIsRenaming()
        {
            var prefab = ManagerData.SelectedItem as IPrefab;
            prefab.IsRenaming.Value = true;
        }

        public void OnSelectedItemChanged(IControl oldValue, IControl newValue)
        {
            if (newValue == null)
                return;

            if (ManagerData.SelectedItem == null)
                return;

            if (ManagerData.Items.Count == 0)
                return;

            if (ManagerData.SelectedIndex < 0)
                return;

            if (ManagerData.SelectedIndex >= ManagerData.Items.Count)
                return;

            var index = ManagerData.SelectedIndex;

            ManagerData.SelectedIndex = index;
            ManagerData.SelectedItem = newValue;

            ManagerMessenger.SendSelectedItemChanged(ManagerData.ID, newValue, index);
        }

        public void SelectedItemChanged(Guid selectedItemID, int index)
        {
            var prefab = ControlFactory.GetPrefab(selectedItemID);

            if (prefab == null)
                return;

            if (index < 0)
                return;

            if (ManagerData.Items.Count == 0)
                return;

            if (index >= ManagerData.Items.Count)
                return;

            ManagerData.SelectedIndex = index;
            ManagerData.SelectedItem = prefab;
        }


        public void Rename()
        {
            if (ManagerData.SelectedItem is IPrefab prefab)
            {
                if (prefab.GetType() != typeof(EmptyPrefab))
                    prefab.IsRenaming.Value = true;
            }
        }

        public void Receive(IMessage message)
        {
            ManagerMessenger.Receive(this, message);
        }
    }
}
