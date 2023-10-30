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
        public ServerManager(ServerFactory serverFactory, CerasSerializer cerasSerializer)
        {
            ServerFactory = serverFactory;

            Settings settings = new Settings("127.0.0.1", 8080);
            var server = new Server(settings, cerasSerializer);
            Servers = new ObservableCollection<Server>();
            Servers.Add(server);

            AddItemCommand = new RelayCommand(AddServer);
            DeleteItemCommand = new RelayCommand(DeleteServer);
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

        public void AddServer()
        {
            //var messenger = ServerFactory.CreateServer();
            //MessageService.Servers.Add(messenger);
        }

        private void DeleteServer()
        {
            if (SelectedServer != null)
            {
                SelectedServer.Stop();
                Servers.Remove(SelectedServer);

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
