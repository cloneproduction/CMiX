// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Input;
using CMiX.Core.BaseControls;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using VL.Serialization.MessagePack;
using WatsonTcp;

namespace CMiX.Core.Networking.Servers
{
    public partial class Server : ObservableRecipient, IPrefab
    {
        public Server(PrefabService prefabService,
                      GenericValue<string> ip,
                      GenericValue<int> port)
        {
            ID = Guid.NewGuid();
            IP = ip;
            Port = port;
            PrefabService = prefabService;
            ClientIsConnected = false;
            ServerIsRunning = false;
            DataSent = false;
            Status = "Disconnected";
            ConnectedClients = new ObservableCollection<ConnectedClient>();
            Statistics = new ServerStatistics();
            StartCommand = new RelayCommand(Start);
            PauseCommand = new RelayCommand(Pause);
            RestartCommand = new RelayCommand(Restart);
            StopCommand = new RelayCommand(Stop);
            ApplySettingsCommand = new RelayCommand(Apply);
            IsActive = true;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public WatsonTcpServer WatsonTcpServer { get; set; }
        public ServerStatistics Statistics { get; set; }
        public GenericValue<string> IP { get; set; }
        public GenericValue<int> Port { get; set; }

        public ICommand StartCommand { get; }
        public ICommand RestartCommand { get; }
        public ICommand ApplySettingsCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand StopCommand { get; }

        private Guid _clientID;
        private ObservableCollection<ConnectedClient> _connectedClients;

        private string _status;
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private bool _clientIsConnected;
        public bool ClientIsConnected
        {
            get => _clientIsConnected;
            set => SetProperty(ref _clientIsConnected, value);
        }

        [ObservableProperty]
        private bool _serverIsRunning;

        private bool _dataSent;
        public bool DataSent
        {
            get => _dataSent;
            set => SetProperty(ref _dataSent, value);
        }

        public ObservableCollection<ConnectedClient> ConnectedClients
        {
            get => _connectedClients;
            set => SetProperty(ref _connectedClients, value);
        }

        partial void OnServerIsRunningChanged(bool value)
        {
            if (value)
            {
                Start();
                return;
            }
            Stop();
        }

        public void SendMessage(IMessage message)
        {
            if (message == null) return;
            var envelope = new MessageEnvelope
            {
                SenderID = MessageSender.WPF,
                MessageID = Guid.NewGuid(),
                Payload = message
            };
            var data = MessagePackSerialization.Serialize(envelope);
            _ = SendAsync(data);
        }

        private async Task SendAsync(byte[] data)
        {
            if (WatsonTcpServer == null) return;
            foreach (var connectedClient in ConnectedClients.ToList())
                await WatsonTcpServer.SendAsync(connectedClient.ID, data);
            Statistics.Update(WatsonTcpServer);
        }

        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var envelope = MessagePackSerialization.Deserialize<MessageEnvelope>(new ReadOnlyMemory<byte>(e.Data));
            if (envelope.SenderID == MessageSender.WPF) return;
            Application.Current.Dispatcher.Invoke(() => WeakReferenceMessenger.Default.Send(envelope.Payload));
        }

        private void ClientConnected(object sender, ConnectionEventArgs e)
        {
            var connectedClient = new ConnectedClient(e.Client.IpPort)
            {
                Name = e.Client.Name,
                ID = e.Client.Guid
            };

            Application.Current.Dispatcher.Invoke(() => ConnectedClients.Add(connectedClient));

            _clientID = e.Client.Guid;
            ClientIsConnected = ConnectedClients.Count > 0;
            Status = ClientIsConnected ? "Connected" : "Disconnected";
        }

        private void ClientDisconnected(object sender, DisconnectionEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                for (var i = ConnectedClients.Count - 1; i >= 0; i--)
                {
                    if (ConnectedClients[i].IPPORT == e.Client.IpPort)
                        ConnectedClients.Remove(ConnectedClients[i]);
                }
            });

            ClientIsConnected = ConnectedClients.Count > 0;
            Status = ClientIsConnected ? "Connected" : "Disconnected";
        }

        public void Start()
        {
            WatsonTcpServer = new WatsonTcpServer(IP.Value, Port.Value);
            WatsonTcpServer.Events.ClientConnected += ClientConnected;
            WatsonTcpServer.Events.ClientDisconnected += ClientDisconnected;
            WatsonTcpServer.Events.MessageReceived += MessageReceived;
            WatsonTcpServer.Start();
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        public void Stop()
        {
            if (WatsonTcpServer == null) return;
            WatsonTcpServer.Events.ClientConnected -= ClientConnected;
            WatsonTcpServer.Events.ClientDisconnected -= ClientDisconnected;
            WatsonTcpServer.Events.MessageReceived -= MessageReceived;
            foreach (var client in ConnectedClients)
                WatsonTcpServer.DisconnectClientAsync(client.ID);
            WatsonTcpServer.Stop();
            WatsonTcpServer.Dispose();
            WatsonTcpServer = null;
            ServerIsRunning = false;
        }

        public void Pause() { }

        public void Apply()
        {
            if (ValidateIPv4(IP.Value) && ValidatePort(IP.Value, Port.Value))
                ErrorMessage = "Settings applied successfully!";
        }

        public bool ValidatePort(string host, int port)
        {
            if (port == 0)
            {
                ErrorMessage = "Port cannot be 0";
                return false;
            }

            try
            {
                var ipa = Dns.GetHostAddresses(host)[0];
                var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(ipa, port);
                sock.Close();
                ErrorMessage = "Port already in use";
                return false;
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10061)
                {
                    ErrorMessage = string.Empty;
                    return true;
                }
                ErrorMessage = ex.Message;
                return false;
            }
        }

        public bool ValidateIPv4(string ipString)
        {
            ErrorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(ipString))
            {
                ErrorMessage = "IP Address is not valid";
                return false;
            }

            var splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                ErrorMessage = "IP Address is not valid";
                return false;
            }

            return splitValues.All(r => byte.TryParse(r, out _));
        }

        public IControlModel ToModel() => new ServerModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            IP = (GenericValueModel<string>)IP.ToModel(),
            Port = (GenericValueModel<int>)Port.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (ServerModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            IP.FromModel(m.IP);
            Port.FromModel(m.Port);
        }
    }
}
