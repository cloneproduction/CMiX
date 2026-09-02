// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using VL.Serialization.MessagePack;
using WatsonTcp;

namespace CMiX.Core.Networking
{
    public partial class Client : ObservableRecipient, IMessageSender
    {
        private readonly SyncCoordinator _sync;

        public Client(Project project, ControlMessenger controlMessenger)
        {
            _sync = new SyncCoordinator(project, this, controlMessenger);
            _sync.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SyncCoordinator.IsInSync))
                    OnPropertyChanged(nameof(IsInSync));
            };
        }

        public bool IsInSync => _sync.IsInSync;
        public ICommand PushCommand => _sync.PushCommand;
        public ICommand PullCommand => _sync.PullCommand;

        public WatsonTcpClient WatsonTcpClient { get; set; }

        [ObservableProperty]
        private bool _serverIsConnected = false;

        public string DeconnectionReason { get; set; }
        public int Port { get; set; }
        public string IP { get; set; }

        private CancellationTokenSource _cts;
        private Task _connectTask;

        public void SendMessageToControls(IMessage message)
        {
            WeakReferenceMessenger.Default.Send(message);
        }

        public void SendMessage(IMessage message)
        {
            if (WatsonTcpClient == null || !ServerIsConnected) return;
            var envelope = new MessageEnvelope
            {
                SenderID = MessageSender.VVVV,
                MessageID = Guid.NewGuid(),
                Payload = message
            };
            var data = MessagePackSerialization.Serialize(envelope);
            _ = WatsonTcpClient.SendAsync(data);
        }

        public void Start(string ip, int port)
        {
            IP = ip;
            Port = port;

            _cts?.Cancel();
            _connectTask?.Wait();

            WatsonTcpClient?.Dispose();

            WatsonTcpClient = new WatsonTcpClient(IP, Port);
            WatsonTcpClient.Events.ServerConnected += ServerConnected;
            WatsonTcpClient.Events.ServerDisconnected += ServerDisconnected;
            WatsonTcpClient.Events.MessageReceived += MessageReceived;
            WatsonTcpClient.Settings.ConnectTimeoutSeconds = 5;

            _cts = new CancellationTokenSource();
            _connectTask = TryToConnect(_cts.Token);
        }

        public void Stop()
        {
            _cts?.Cancel();
            WatsonTcpClient?.Disconnect();
        }



        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var envelope = MessagePackSerialization.Deserialize<MessageEnvelope>(new ReadOnlyMemory<byte>(e.Data));
            if (envelope.SenderID == MessageSender.VVVV) return;

            if (SyncProtocolMessages.IsSyncProtocol(envelope.Payload))
            {
                _sync.TryHandle(envelope.Payload);
                return;
            }

            // Drop content messages while unsynced, mirroring the outgoing block.
            if (_sync.ShouldBlockIncoming(envelope.Payload)) return;

            WeakReferenceMessenger.Default.Send(envelope.Payload);
        }

        private void ServerConnected(object sender, ConnectionEventArgs e)
        {
            ServerIsConnected = true;
            Console.WriteLine("Server Connected");
            _cts.Cancel();
            _sync.SendOwnHash();
        }

        private void ServerDisconnected(object sender, DisconnectionEventArgs e)
        {
            DeconnectionReason = e.Reason.ToString();
            ServerIsConnected = false;
            Console.WriteLine("Server Disconnected");
            Start(IP, Port);
        }

        private async Task TryToConnect(CancellationToken cancellationToken)
        {
            int delaySeconds = 1;

            while (!WatsonTcpClient.Connected && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    WatsonTcpClient.Connect();
                }
                catch (Exception)
                {
                    Console.WriteLine($"Retrying connection to {IP}:{Port} in {delaySeconds}s...");
                }

                if (cancellationToken.IsCancellationRequested)
                    break;

                await Task.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken);
                delaySeconds = Math.Min(delaySeconds * 2, 30);
            }
        }
    }
}
