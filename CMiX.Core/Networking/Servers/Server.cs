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

        public ICommand StartCommand { get; }
        public ICommand RestartCommand { get; }
        public ICommand ApplySettingsCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand StopCommand { get; }


        public Guid ID { get; set; }

        public PrefabService PrefabService { get; set; }
        public WatsonTcpServer WatsonTcpServer { get; set; }
        public ServerStatistics Statistics { get; set; }
        public GenericValue<string> IP { get; set; }
        public GenericValue<int> Port { get; set; }

        private string ipPort { get; set; }


        public void SendMessage(IMessage message)
        {
            if (message == null) return;
            Console.WriteLine("SendMessage of type " + message.GetType().Name);
            var data = MessagePackSerialization.Serialize(message);
            _ = SendAsync(data);
        }



        public IMessage SendMessageRequest(IMessage message)
        {
            var data = MessagePackSerialization.Serialize(message);

            IMessage messageResult = null;

            if (WatsonTcpServer != null)
            {
                try
                {
                    var response = WatsonTcpServer.SendAndWaitAsync(5000, clientID, data);
                    byte[] received = response.Result.Data;
                    messageResult = MessagePackSerialization.Deserialize<IMessage>(new ReadOnlyMemory<byte>(received));
                    Console.WriteLine("Client replied : " + messageResult.GetType().Name);
                }
                catch (TimeoutException)
                {
                    Console.WriteLine("Too slow...");
                }
            }

            return messageResult;
        }




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

        private ObservableCollection<ConnectedClient> _connectedClients;
        public ObservableCollection<ConnectedClient> ConnectedClients
        {
            get => _connectedClients;
            set => SetProperty(ref _connectedClients, value);
        }


        partial void OnServerIsRunningChanged(bool value)
        {
            if(value)
            {
                this.Start();
                return;
            }
            Stop();
        }

        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            IMessage message = MessagePackSerialization.Deserialize<IMessage>(new ReadOnlyMemory<byte>(e.Data));
            Console.WriteLine("Message received from vvvv: " + message.GetType().Name);
            Application.Current.Dispatcher.Invoke(() => WeakReferenceMessenger.Default.Send(message));
        }


        private void ClientDisconnected(object sender, DisconnectionEventArgs e)
        {
            Console.WriteLine("Client disconnected: " + IP + ": " + e.Reason.ToString());

            Application.Current.Dispatcher.Invoke(delegate
            {
                for (var i = ConnectedClients.Count - 1; i >= 0; i--)
                {
                    if (ConnectedClients[i].IPPORT == e.Client.IpPort)
                    {
                        ConnectedClients.Remove(ConnectedClients[i]);
                    }
                }
            });

            ClientIsConnected = ConnectedClients.Count > 0;

            if (!ClientIsConnected)
                Status = "Disconnected";
        }



        Guid clientID;

        private void ClientConnected(object sender, ConnectionEventArgs e)
        {
            Console.WriteLine("Client connected: " + e.Client.IpPort);
            var connectedClient = new ConnectedClient(e.Client.IpPort);
            connectedClient.Name = e.Client.Name;
            connectedClient.ID = e.Client.Guid;
            ipPort = e.Client.IpPort;
            
            Application.Current.Dispatcher.Invoke(delegate
            {
                ConnectedClients.Add(connectedClient);
            });

            ClientIsConnected = ConnectedClients.Count > 0;

            if (ClientIsConnected)
                Status = "Connected";

            clientID = e.Client.Guid;
        }


        private SyncResponse SyncRequestReceived(SyncRequest arg)
        {
            return new SyncResponse(arg, "Hello back at you from Server!");
        }



        private async Task SendAsync(byte[] data)
        {
            if (WatsonTcpServer == null) return;
            foreach (var connectedClient in ConnectedClients.ToList())
            {
                await WatsonTcpServer.SendAsync(connectedClient.ID, data);
            }
            Statistics.Update(WatsonTcpServer);
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

        public void Pause()
        {

        }


        public void Apply()
        {
            if (ValidateIPv4(IP.Value) && ValidatePort(IP.Value, Port.Value))
            {
                ErrorMessage = "Settings applied succefully !";
                //CanApply = false;
                //OkIsFocused = true;
            }
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
                if (ex.ErrorCode == 10061) // connection refused = port is free
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

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
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
