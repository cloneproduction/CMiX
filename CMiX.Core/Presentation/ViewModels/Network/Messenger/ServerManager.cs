// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Windows.Input;
using CMiX.Core.Presentation.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmDialogs;

namespace CMiX.Core.Presentation.ViewModels.Network
{
    public class ServerManager : ObservableObject
    {
        public ServerManager(IMessageService messageService, IDialogService dialogService)
        {
            MessageService = messageService;
            DialogService = dialogService;

            ServerFactory = new ServerFactory();

            AddItemCommand = new RelayCommand(AddServer);
            DeleteItemCommand = new RelayCommand(DeleteServer);
            RenameServerCommand = new RelayCommand<Server>(RenameServer);
            EditMessengerSettingsCommand = new RelayCommand<Server>(EditMessengerSettings);
        }


        public ICommand EditMessengerSettingsCommand { get; }
        public ICommand AddItemCommand { get; set; }
        public ICommand DeleteItemCommand { get; set; }
        public ICommand RenameServerCommand { get; set; }
        private IDialogService DialogService { get; set; }


        private ServerFactory ServerFactory { get; set; }
        public IMessageService MessageService { get; set; }


        public ObservableCollection<Server> Servers
        {
            get => MessageService.Servers;
        }


        private Server _selectedServer;
        public Server SelectedServer
        {
            get => _selectedServer;
            set => SetProperty(ref _selectedServer, value);
        }


        public void EditMessengerSettings(Server server)
        {
            Settings settings = server.GetSettings();
            bool? success = DialogService.ShowDialog<MessengerSettingsWindow>(this, settings);
            if (success == true)
                server.SetSettings(settings);
        }

        public void AddServer()
        {
            var messenger = ServerFactory.CreateServer(this.DialogService);
            MessageService.Servers.Add(messenger);
        }

        private void DeleteServer()
        {
            if (SelectedServer != null)
            {
                SelectedServer.Stop();
                MessageService.Servers.Remove(SelectedServer);

                if (MessageService.Servers.Count > 0)
                {
                    SelectedServer = MessageService.Servers[0];
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
