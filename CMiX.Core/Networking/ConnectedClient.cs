// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Networking
{
    public class ConnectedClient : ObservableObject
    {
        public ConnectedClient(string ipPort)
        {
            IPPORT = ipPort;
            IP = ipPort.Split(':')[0];
            Port = ipPort.Split(':')[1];
        }

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
