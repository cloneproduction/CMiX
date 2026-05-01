// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using VL.Serialization.MessagePack;
using WatsonTcp;

namespace CMiX.Core.Services
{
    public class Client : ObservableRecipient
    {
        public Client()
        {
            ServerIsConnected = false;
        }

        public WatsonTcpClient WatsonTcpClient { get; set; }
        public bool ServerIsConnected { get; set; }
        public string DeconnectionReason { get; set; }
        public int Port { get; set; }
        public string IP { get; set; }

        private CancellationTokenSource _cts;
        private Task _connectTask;

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
            WeakReferenceMessenger.Default.Send(envelope.Payload);
        }

        private void ServerConnected(object sender, ConnectionEventArgs e)
        {
            ServerIsConnected = true;
            Console.WriteLine("Server Connected");
            _cts.Cancel();
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
