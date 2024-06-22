// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Ceras;
using CMiX.Core.BaseControls;
using CMiX.Core.Network;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WatsonTcp;

namespace CMiX.Core.Networking.Messenger
{
    public class Server : ObservableRecipient, IControl, IPrefab
    {
        public Server(PrefabService prefabService, CerasSerializer cerasSerializer)
        {
            ID = Guid.NewGuid();

            Serializer = cerasSerializer;
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

            IsActive= true;
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
        public CerasSerializer Serializer { get; set; }


        private string ipPort { get; set; }

        public void SendMessage(IMessage message)
        {
            Console.WriteLine("MessageService SendMessage of type " + message.GetType().Name);
            var data = Serializer.Serialize(message);
            this.Send(data);
        }

        public GenericValue<string> IP { get; set; }
        public GenericValue<int> Port { get; set; }



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

        private bool _serverIsRunning;
        public bool ServerIsRunning
        {
            get => _serverIsRunning;
            set => SetProperty(ref _serverIsRunning, value);
        }

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


        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine("Message from " + e.Client.IpPort + ": " + Encoding.UTF8.GetString(e.Data));
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

        //public void SendRequestProjectSync(byte[] data)
        //{
        //    if (WatsonTcpServer != null)
        //    {
        //        try
        //        {
        //            SyncResponse resp = WatsonTcpServer.SendAndWait(5000, this.ipPort, data);
        //            //SyncResponse resp = WatsonTcpServer.SendAndWait(5000, this.ipPort, "Project model requested from Server");
        //            Console.WriteLine("Client replied : " + Encoding.UTF8.GetString(resp.Data));
        //        }
        //        catch (TimeoutException)
        //        {
        //            Console.WriteLine("Too slow...");
        //        }
        //    }
        //}

        async void Send(byte[] data)
        {
            if (WatsonTcpServer != null)
            {
                //var clients = WatsonTcpServer.ListClients();
                foreach (var connectedClient in ConnectedClients)
                {
                    await WatsonTcpServer.SendAsync(clientID, data);
                    //var success = WatsonTcpServer.Send(connectedClient.IPPORT, data);
                    //if (success)
                    //    Debug.WriteLine("WatsonTcpServer SendObject with  Topic : " + this.Topic + " Data Size = " + data.Length + "to address : " + $"{IP}:{Port}");
                }
                Statistics.Update(WatsonTcpServer);
            }
        }


        public void Start()
        {
            //if (WatsonTcpServer == null)
            //    return;

            //if (WatsonTcpServer.IsListening == true)


            ipPort = $"{IP}:{Port}";
            WatsonTcpServer = new WatsonTcpServer(IP.Value, Port.Value);

            WatsonTcpServer.Events.ClientConnected += ClientConnected;
            WatsonTcpServer.Events.ClientDisconnected += ClientDisconnected;
            WatsonTcpServer.Events.MessageReceived += MessageReceived;
            //WatsonTcpServer.Callbacks.SyncRequestReceived = SyncRequestReceived;
            WatsonTcpServer.Start();
            ServerIsRunning = true;
            Console.WriteLine();
        }


        public void Restart()
        {
            Stop();
            Start();
        }

        public void Stop()
        {
            if (WatsonTcpServer == null)
                return;

            foreach (var client in ConnectedClients)
                WatsonTcpServer.DisconnectClientAsync(client.ID);

            WatsonTcpServer.Stop();
            WatsonTcpServer.Dispose();
            WatsonTcpServer = null;
            ServerIsRunning = false;
        }

        //WatsonTcpServer.Events.ClientConnected -= ClientConnected;
        //WatsonTcpServer.Events.ClientDisconnected -= ClientDisconnected;
        //WatsonTcpServer.Events.MessageReceived -= MessageReceived;
        //WatsonTcpServer.Stop();
        //ClientIsConnected = false;
        //ServerIsRunning = false;
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
            var ipa = Dns.GetHostAddresses(host)[0];
            try
            {
                var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(ipa, port);
                if (sock.Connected == true)  // Port is in use and connection is successful
                {
                    ErrorMessage = "Port already in use";
                    return false;
                }
                sock.Close();

            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10061)  // Port is unused and could not establish connection 
                {
                    ErrorMessage = string.Empty;
                    return true;
                }
                else
                    ErrorMessage = ex.Message;
            }
            if (port == 0)
                return false;

            return false;
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
    }
}
