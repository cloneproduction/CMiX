// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Network
{
    public class ConnectedClient : ObservableObject
    {
        public ConnectedClient(string ipPort)
        {
            IPPORT = ipPort;
            IP = ipPort.Split(':')[0];
            Port = ipPort.Split(':')[1];

            SendResyncRequestCommand = new RelayCommand(SendResyncRequest);
        }

        public void SendResyncRequest()
        {
            Console.WriteLine();
        }

        public ICommand SendResyncRequestCommand { get; }

        private Guid _iD;
        public Guid ID
        {
            get => _iD;
            set => SetProperty(ref _iD, value);
        }

        private string _ipPort;
        public string IPPORT
        {
            get => _ipPort;
            set => SetProperty(ref _ipPort, value);
        }

        private bool _unSync;
        public bool UnSync
        {
            get => _unSync;
            set => SetProperty(ref _unSync, value);
        }

        private string _port;
        public string Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        private string _ip;
        public string IP
        {
            get => _ip;
            set => SetProperty(ref _ip, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
    }
}
