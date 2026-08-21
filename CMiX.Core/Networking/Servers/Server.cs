// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Windows.Input;
using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using VL.Serialization.MessagePack;
using WatsonTcp;

namespace CMiX.Core.Networking.Servers
{
    public partial class Server : ObservableRecipient, IPrefab, IMessageSender, IDisposable
    {
        private readonly Project _project;
        private readonly SyncCoordinator _sync;

        public Server(PrefabService prefabService,
                      GenericValue<string> ip,
                      GenericValue<int> port,
                      Project project,
                      ControlMessenger controlMessenger)
        {
            ID = Guid.NewGuid();
            IP = ip;
            Port = port;
            IP.Value = "127.0.0.1";
            Port.Value = 8080;
            PrefabService = prefabService;
            _project = project;
            _sync = new SyncCoordinator(project, this, controlMessenger);
            _sync.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SyncCoordinator.IsInSync))
                    OnPropertyChanged(nameof(IsInSync));
            };
            ClientIsConnected = false;
            ServerIsRunning = false;
            DataSent = false;
            ConnectedClients = new ObservableCollection<ConnectedClient>();
            Statistics = new ServerStatistics();
            StartCommand = new RelayCommand(Start);
            RestartCommand = new RelayCommand(Restart);
            StopCommand = new RelayCommand(Stop);
            IsActive = true;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public WatsonTcpServer WatsonTcpServer { get; set; }
        public ServerStatistics Statistics { get; set; }
        public GenericValue<string> IP { get; set; }
        public GenericValue<int> Port { get; set; }

        public bool IsInSync => _sync.IsInSync;
        public ICommand PushCommand => _sync.PushCommand;
        public ICommand PullCommand => _sync.PullCommand;

        public ICommand StartCommand { get; }
        public ICommand RestartCommand { get; }
        public ICommand StopCommand { get; }

        private Guid _clientID;
        private ObservableCollection<ConnectedClient> _connectedClients;

        // Distinguishes three states rather than just two, so a listener that's up but has no
        // Engine attached yet doesn't look identical to one that failed to bind at all.
        public string Status => !ServerIsRunning ? "Not listening" : ClientIsConnected ? "Connected" : "Listening";

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
            OnPropertyChanged(nameof(Status));
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
            // Captured so a concurrent Stop can't null the field mid-send.
            var server = WatsonTcpServer;
            if (server == null) return;

            // Caught per client so one stale client can't abort the send to the rest.
            foreach (var connectedClient in ConnectedClients.ToList())
            {
                try
                {
                    await server.SendAsync(connectedClient.ID, data);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            try
            {
                Dispatch(() => Statistics.Update(server));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private Action<Action> _dispatcherAction;

        public void SetDispatcher(Action<Action> dispatcherAction)
        {
            _dispatcherAction = dispatcherAction;
        }

        private void Dispatch(Action action)
        {
            if (_dispatcherAction != null)
                _dispatcherAction(action);
            else
                action();
        }

        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                var envelope = MessagePackSerialization.Deserialize<MessageEnvelope>(new ReadOnlyMemory<byte>(e.Data));
                if (envelope.SenderID == MessageSender.WPF) return;

                if (SyncProtocolMessages.IsSyncProtocol(envelope.Payload))
                {
                    Dispatch(() => _sync.TryHandle(envelope.Payload));
                    return;
                }

                // Drop content messages while unsynced, mirroring the outgoing block.
                if (_sync.ShouldBlockIncoming(envelope.Payload)) return;

                Dispatch(() => WeakReferenceMessenger.Default.Send(envelope.Payload));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void ClientConnected(object sender, ConnectionEventArgs e)
        {
            var connectedClient = new ConnectedClient(e.Client.IpPort)
            {
                Name = e.Client.Name,
                ID = e.Client.Guid
            };

            _clientID = e.Client.Guid;
            Dispatch(() =>
            {
                ConnectedClients.Add(connectedClient);
                ClientIsConnected = ConnectedClients.Count > 0;
                OnPropertyChanged(nameof(Status));
            });

            _sync.SendOwnHash();
        }

        private void ClientDisconnected(object sender, DisconnectionEventArgs e)
        {
            Dispatch(() =>
            {
                for (var i = ConnectedClients.Count - 1; i >= 0; i--)
                {
                    if (ConnectedClients[i].IPPORT == e.Client.IpPort)
                        ConnectedClients.Remove(ConnectedClients[i]);
                }

                ClientIsConnected = ConnectedClients.Count > 0;
                OnPropertyChanged(nameof(Status));
            });
        }

        private bool _starting;

        // Guards against a second Start while one is already running or mid-construction.
        public void Start()
        {
            if (_starting || WatsonTcpServer != null) return;

            _starting = true;
            try
            {
                var server = new WatsonTcpServer(IP.Value, Port.Value);
                server.Events.ClientConnected += ClientConnected;
                server.Events.ClientDisconnected += ClientDisconnected;
                server.Events.MessageReceived += MessageReceived;
                server.Start();
                WatsonTcpServer = server;
                ServerIsRunning = true;
            }
            catch (Exception ex)
            {
                WatsonTcpServer = null;
                ErrorMessage = ex.Message;
                ServerIsRunning = false;
            }
            finally
            {
                _starting = false;
            }
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        public void Stop()
        {
            if (WatsonTcpServer == null) return;
            var server = WatsonTcpServer;
            server.Events.ClientConnected -= ClientConnected;
            server.Events.ClientDisconnected -= ClientDisconnected;
            server.Events.MessageReceived -= MessageReceived;

            // Watson can throw during its own teardown, so each step is guarded to make sure
            // the rest still runs.
            foreach (var client in ConnectedClients.ToList())
            {
                try
                {
                    server.DisconnectClientAsync(client.ID);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            try
            {
                server.Stop();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            try
            {
                server.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            WatsonTcpServer = null;
            ServerIsRunning = false;
        }

        public void Dispose() => Stop();

        public async Task<bool> ApplyAsync()
        {
            var ip = IP.Value;
            var port = Port.Value;

            // Stopped first so the port-availability check below can't see this server's own
            // listener and mistake it for something else already bound to the address.
            Stop();
            var valid = ValidateIPv4(ip) && await ValidatePortAsync(ip, port);

            // Re-applied so Start() binds exactly what was just validated, not whatever the
            // two-way bound IP/Port fields hold by now if the user kept editing them while the
            // check above was still running.
            IP.Value = ip;
            Port.Value = port;
            Start();

            // Reflects whether the server actually ended up listening, not just whether the
            // settings passed validation - Start() can still fail after a successful check.
            return valid && WatsonTcpServer != null;
        }

        private async Task<bool> ValidatePortAsync(string host, int port)
        {
            if (port == 0)
            {
                ErrorMessage = "Port cannot be 0";
                return false;
            }

            IPAddress ipa;
            if (!IPAddress.TryParse(host, out ipa))
            {
                try
                {
                    var addresses = await Dns.GetHostAddressesAsync(host);
                    if (addresses.Length == 0)
                    {
                        ErrorMessage = "IP Address is not valid";
                        return false;
                    }
                    ipa = addresses[0];
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    return false;
                }
            }

            try
            {
                using (var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3)))
                {
                    await sock.ConnectAsync(ipa, port, cts.Token);
                }

                ErrorMessage = "Port already in use";
                return false;
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    ErrorMessage = string.Empty;
                    return true;
                }
                ErrorMessage = ex.Message;
                return false;
            }
            catch (OperationCanceledException)
            {
                ErrorMessage = "Connection timed out";
                return false;
            }
            catch (Exception ex)
            {
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
