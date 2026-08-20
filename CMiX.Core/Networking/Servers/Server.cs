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

        public Server(PrefabService prefabService,
                      GenericValue<string> ip,
                      GenericValue<int> port,
                      Project project)
        {
            ID = Guid.NewGuid();
            IP = ip;
            Port = port;
            IP.Value = "127.0.0.1";
            Port.Value = 8080;
            PrefabService = prefabService;
            _project = project;
            ClientIsConnected = false;
            ServerIsRunning = false;
            DataSent = false;
            Status = "Disconnected";
            ConnectedClients = new ObservableCollection<ConnectedClient>();
            Statistics = new ServerStatistics();
            StartCommand = new RelayCommand(Start);
            RestartCommand = new RelayCommand(Restart);
            StopCommand = new RelayCommand(Stop);
            ApplySettingsCommand = new AsyncRelayCommand(ApplyAsync);
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

        // Placeholder for the connect-time state-hash comparison with the Engine (not built yet).
        // Defaults true so the sync indicator does not read as a permanent alarm before that
        // check exists; wire this up to the real comparison once it does.
        [ObservableProperty]
        private bool _isInSync = true;

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
            // Captured once so a Stop that runs on the UI thread while this send is in flight
            // cannot null the field out from under the continuation below.
            var server = WatsonTcpServer;
            if (server == null) return;

            try
            {
                foreach (var connectedClient in ConnectedClients.ToList())
                    await server.SendAsync(connectedClient.ID, data);
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

                if (envelope.Payload is MessageStateHash stateHash)
                {
                    Dispatch(() => HandleStateHash(stateHash));
                    return;
                }

                // Same rule as the outgoing side (ControlMessenger.SendMessage): while not in
                // sync, content messages are dropped rather than silently applied, so an edit made
                // on the other side while unresolved cannot leak in either. The sync protocol's own
                // messages are exempt, or a mismatch could never be resolved.
                if (!IsInSync && !SyncProtocolMessages.IsSyncProtocol(envelope.Payload)) return;

                Dispatch(() => WeakReferenceMessenger.Default.Send(envelope.Payload));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        // Split out so the comparison itself is testable without a real transport - feed it a
        // MessageStateHash directly rather than going through MessagePack/WatsonTcp.
        internal void HandleStateHash(MessageStateHash message)
        {
            IsInSync = message.Hash == ProjectStateHash.Compute(_project);
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
                Status = ClientIsConnected ? "Connected" : "Disconnected";
            });

            // Each side sends its own hash exactly once, on its own "connected" trigger - not as a
            // reply to receiving one, which would risk a send/reply loop. The other side (Engine)
            // needs its own equivalent "just connected -> send my hash" trigger for this to be a
            // real two-way check; see HandleStateHash for the comparison this feeds into.
            SendMessage(new MessageStateHash(Guid.NewGuid(), ProjectStateHash.Compute(_project)));
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
                Status = ClientIsConnected ? "Connected" : "Disconnected";
            });
        }

        private bool _starting;

        // The guard covers two paths into Start. The direct property set on ServerIsRunning
        // reenters here through OnServerIsRunningChanged while WatsonTcpServer is still being
        // constructed, and a caller invoking Start while a server is already bound must not
        // construct and bind a second one.
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

            // Watson's own Stop cancels the token its Dispose then waits on internally
            // (DisconnectClientsAsync(...).Wait()), so a still connected client makes that
            // wait observe a TaskCanceledException and Task.Wait rethrows it wrapped in an
            // AggregateException. None of Watson's teardown is ours to fix, so every step
            // below is guarded the same way and the remaining teardown always runs.
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
            Status = "Disconnected";
        }

        // Stopping is the whole teardown, so a server that ever ends up in a collection managed by
        // the generic delete path is released the same way ServerManager releases it.
        public void Dispose() => Stop();

        private async Task ApplyAsync()
        {
            if (ValidateIPv4(IP.Value) && await ValidatePortAsync(IP.Value, Port.Value))
                ErrorMessage = "Settings applied successfully!";
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
