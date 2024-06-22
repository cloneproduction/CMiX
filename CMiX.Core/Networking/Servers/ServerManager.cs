// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Networking.Messenger;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Networking.Servers
{
    public partial class ServerManager : ObservableObject, IControl
    {
        public ServerManager(ManagerData managerData,
                             ControlRepository controlRepository,
                             ControlFactory controlFactory)
        {
            ControlFactory = controlFactory;
            ManagerData = managerData;
            ControlRepository = controlRepository;

            AddItemCommand = new RelayCommand<Type>(AddItem);
            DeleteItemCommand = new RelayCommand<IControl>(DeleteItem);
            ReplaceSelectedItemCommand = new RelayCommand<IControl>(ReplaceItem);
            //RemoveSelectedItemCommand = new RelayCommand(RemoveSelectedItem);
        }

        public Guid ID { get; set; }
        public ICommand AddItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }
        //public ICommand RemoveSelectedItemCommand { get; set; }
        public ICommand ReplaceSelectedItemCommand { get; set; }
        public ControlFactory ControlFactory { get; set; }
        public ControlRepository ControlRepository { get; set; }
        public ManagerData ManagerData { get; set; }

        private IControl _selectedItem;
        public IControl SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }
        

        private void AddItem(Type type)
        {
            var prefab = ControlFactory.Create(type);
            ControlRepository.AddControl(prefab);

            var items = ManagerData.Items;

            if (SelectedItem is EmptyPrefab emptyPrefab && prefab is IPrefab pre)
            {
                items[items.IndexOf(emptyPrefab)] = prefab;
            }
            else
            {
                items.Add(prefab);
            }

            SelectedItem = prefab;
            ManagerData.SelectedIndex = items.IndexOf(prefab);
        }

        public void DeleteItem(IControl control)
        {
            var index = ManagerData.Items.IndexOf(control);

            if (control == null)
                return;

            ManagerData.Items.RemoveAt(index);

            if (ManagerData.Items.Count == 0)
            {
                SelectedItem = null;
                ManagerData.SelectedIndex = -1;
                return;
            }

            if (index == 0)
            {
                SelectedItem = ManagerData.Items[0];
                ManagerData.SelectedIndex = 0;
                return;
            }

            if (index > 0)
            {
                SelectedItem = ManagerData.Items[index - 1];
                ManagerData.SelectedIndex = index - 1;
                return;
            }
        }

        public void ReplaceItem(IControl control)
        {
            var prefab = (IPrefab)control;
            var items = ManagerData.Items;
            var index = -1;

            if (ManagerData.Items.Count == 0)
            {
                index = 0;
                items.Add(prefab);
            }
            else
            {
                index = ManagerData.SelectedIndex;
                items[index] = prefab;
            }

            SelectedItem = prefab;
            ManagerData.SelectedIndex = index;
        }
    }
}
