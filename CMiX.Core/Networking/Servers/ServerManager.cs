// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Windows.Input;
using Ceras;
using CMiX.Core.Networking.Messenger;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Network
{
    public partial class ServerManager : ObservableObject
    {
        public ServerManager(ServerFactory serverFactory)
        {
            ServerFactory = serverFactory;

            Servers = new ObservableCollection<Server>();

            AddItemCommand = new RelayCommand<Settings>(AddNewServer);
            DeleteItemCommand = new RelayCommand<Server>(DeleteServer);
            RenameServerCommand = new RelayCommand<Server>(RenameServer);
            EditMessengerSettingsCommand = new RelayCommand<Server>(EditMessengerSettings);
        }

        public ICommand EditMessengerSettingsCommand { get; }
        public ICommand AddItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }
        public ICommand RenameServerCommand { get; set; }

        private ServerFactory ServerFactory { get; set; }
        public ObservableCollection<Server> Servers { get; set; }


        [ObservableProperty]
        private Server _selectedServer;

        public void EditMessengerSettings(Server server)
        {
            //Settings settings = server.GetSettings();
            //bool? success = DialogService.ShowDialog<MessengerSettingsWindow>(this, settings);
            //if (success == true)
            //    server.SetSettings(settings);
        }

        public void AddNewServer(Settings settings)
        {
            var server = ServerFactory.CreateServer(settings);
            Servers.Add(server);
        }

        private void DeleteServer(Server server)
        {
            if (server != null)
            {
                server.Stop();
                Servers.Remove(server);

                if (Servers.Count > 0)
                {
                    SelectedServer = Servers[0];
                    return;
                }

                SelectedServer = null;
            }
        }

        private void RenameServer(Server obj)
        {
            if (obj != null)
                obj.IsRenaming = true;
        }
    }
}
