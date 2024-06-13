// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Windows.Input;
using CMiX.Core.Networking.Messenger;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Network
{
    public partial class ServerManager : ObservableObject
    {
        public ServerManager(ControlFactory controlFactory, ManagerData managerData)
        {
            ControlFactory = controlFactory;
            ManagerData = managerData;

            Servers = new ObservableCollection<Server>();

            AddItemCommand = new RelayCommand<Type>(AddNewServer);
            DeleteItemCommand = new RelayCommand<Server>(DeleteServer);
            RenameServerCommand = new RelayCommand(RenameServer);
        }


        public ICommand AddItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }
        public ICommand RenameServerCommand { get; set; }

        public ControlFactory ControlFactory { get; set; }
        public ObservableCollection<Server> Servers { get; set; }
        public ManagerData ManagerData { get; set; }


        [ObservableProperty]
        private Server _selectedServer;

        [ObservableProperty]
        private int _selectedIndex;


        public void AddNewServer(Type type)
        {
            var server = (Server)ControlFactory.Create(type);
            ManagerData.Items.Add(server);
        }

        private void DeleteServer(Server server)
        {
            var index = ManagerData.Items.IndexOf(server);

            if (server == null)
                return;

            server.Stop();
            ManagerData.Items.Remove(server);

            if (Servers.Count == 0)
            {
                SelectedServer = null;
                SelectedIndex = -1;
                return;
            }

            if (index == 0)
            {
                SelectedServer = Servers[0];
                SelectedIndex = 0;
                return;
            }

            if (index > 0)
            {
                SelectedServer = Servers[index - 1];
                SelectedIndex = index - 1;
                return;
            }
        }

        private void RenameServer()
        {
            if (SelectedServer is Server server)
                server.PrefabService.IsRenaming.Value = true;
        }
    }
}
