// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Ceras;
using CMiX.Core.Network;
using CMiX.Core.Networking.Messages;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using WatsonTcp;

namespace CMiX.Core.Networking.Messenger
{
    public class Server : ObservableRecipient
    {
        public Server(Settings settings, CerasSerializer cerasSerializer)
        {
            Serializer = cerasSerializer;
            ClientIsConnected = false;
            ServerIsRunning = false;
            DataSent = false;
            SetSettings(settings);

            Name = $"Server (0)";
            Status = "Disconnected";

            ConnectedClients = new ObservableCollection<ConnectedClient>();
            Statistics = new ServerStatistics();

            PauseCommand = new RelayCommand(Pause);
            EditSettingsCommand = new RelayCommand(EditSettings);
            ApplySettingsCommand = new RelayCommand(Apply);

            IsActive= true;
        }

        public CerasSerializer Serializer { get; set; }

        protected override void OnActivated()
        {
            this.Messenger.Register<IMessage, int>(this, MessageType.Out, (r, m) => SendMessage(m));
        }

        public void SendMessage(IMessage message)
        {
            Console.WriteLine("MessageService SendMessage of type " + message.GetType().Name);
            var data = Serializer.Serialize(message);
            this.Send(data);
        }

        public ICommand ApplySettingsCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand EditSettingsCommand { get; }


        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _status;
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private string _ip;
        public string IP
        {
            get => _ip;
            set => SetProperty(ref _ip, value);
        }

        private string _topic;
        public string Topic
        {
            get => _topic;
            set => SetProperty(ref _topic, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private int _port;
        public int Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        private bool _isRenaming;
        public bool IsRenaming
        {
            get => _isRenaming;
            set => SetProperty(ref _isRenaming, value);
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


        private string ipPort { get; set; }
        public WatsonTcpServer WatsonTcpServer { get; set; }
        public ServerStatistics Statistics { get; set; }


        public Settings GetSettings()
        {
            return new Settings(IP, Port);
        }

        public void SetSettings(Settings settings)
        {
            IP = settings.IP;
            Port = settings.Port;
            Start();
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

        private void ClientConnected(object sender, ConnectionEventArgs e)
        {
            Console.WriteLine("Client connected: " + e.Client.IpPort);
            var connectedClient = new ConnectedClient(e.Client.IpPort);
            ipPort = e.Client.IpPort;
            Application.Current.Dispatcher.Invoke(delegate
            {
                ConnectedClients.Add(connectedClient);
            });

            ClientIsConnected = ConnectedClients.Count > 0;

            if (ClientIsConnected)
                Status = "Connected";
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


        public async void Send(byte[] data)
        {
            if (WatsonTcpServer != null)
            {
                foreach (var connectedClient in ConnectedClients)
                {
                    await WatsonTcpServer.SendAsync(connectedClient.IPPORT, data);
                    //    var success = WatsonTcpServer.Send(connectedClient.IPPORT, data);
                    //    if (success)
                    //        Console.WriteLine("WatsonTcpServer SendObject with  Topic : " + this.Topic + " Data Size = " + data.Length + "to address : " + $"{IP}:{Port}");
                }
                Statistics.Update(WatsonTcpServer);
            }
        }

        public void EditSettings()
        {
            //Settings settings = this.GetSettings();
            //bool? success = DialogService.ShowDialog<MessengerSettingsWindow>(this, settings);
            //if (success == true)
            //    this.SetSettings(settings);
        }

        public void Start()
        {
            ipPort = $"{IP}:{Port}";
            WatsonTcpServer = new WatsonTcpServer(IP, Port);

            WatsonTcpServer.Events.ClientConnected += ClientConnected;
            WatsonTcpServer.Events.ClientDisconnected += ClientDisconnected;
            WatsonTcpServer.Events.MessageReceived += MessageReceived;
            WatsonTcpServer.Callbacks.SyncRequestReceived = SyncRequestReceived;
            WatsonTcpServer.Start();
            ServerIsRunning = true;
        }

        public void Stop()
        {
            if (WatsonTcpServer != null)
            {
                WatsonTcpServer.Stop();
                WatsonTcpServer.DisconnectClients();
                WatsonTcpServer.Events.ClientConnected -= ClientConnected;
                WatsonTcpServer.Events.ClientDisconnected -= ClientDisconnected;
                WatsonTcpServer.Events.MessageReceived -= MessageReceived;
                ClientIsConnected = false;
                ServerIsRunning = false;
                WatsonTcpServer.Dispose();
            }
        }

        public void Pause()
        {

        }


        public void Apply()
        {
            if (ValidateIPv4(IP) && ValidatePort(IP, Port))
            {
                ErrorMessage = "Settings applied succefully !";
                //CanApply = false;
                //DialogResult = true;
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
