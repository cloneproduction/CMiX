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

        CancellationTokenSource _cts;
        private Task _connectTask;

        public int Port { get; set; }
        public string IP { get; set; }

        public void Start(string IP, int Port)
        {
            this.IP = IP;
            this.Port = Port;

            // cancel any existing loop and wait for it to finish
            _cts?.Cancel();
            _connectTask?.Wait();

            if (WatsonTcpClient != null)
                WatsonTcpClient.Dispose();

            WatsonTcpClient = new WatsonTcpClient(IP, Port);
            WatsonTcpClient.Events.ServerConnected += ServerConnected;
            WatsonTcpClient.Events.ServerDisconnected += ServerDisconnected;
            WatsonTcpClient.Events.MessageReceived += MessageReceived;
            WatsonTcpClient.Settings.ConnectTimeoutSeconds = 5;

            _cts = new CancellationTokenSource();
            _connectTask = TryToConnect(_cts.Token);
        }

        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            IMessage message = MessagePackSerialization.Deserialize<IMessage>(new ReadOnlyMemory<byte>(e.Data));
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} Message Received of type : {message.GetType().Name}");
            WeakReferenceMessenger.Default.Send(message);
        }

        private void ServerDisconnected(object sender, DisconnectionEventArgs e)
        {
            DeconnectionReason = e.Reason.ToString();
            Console.WriteLine(DeconnectionReason);
            ServerIsConnected = false;
            Console.WriteLine("Server Disconnected");
            Start(IP, Port); // ← reuse Start which handles cleanup and restarts the loop
        }

        private void ServerConnected(object sender, ConnectionEventArgs e)
        {
            ServerIsConnected = true;
            Console.WriteLine("Server Connected");
            _cts.Cancel();
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
                delaySeconds = Math.Min(delaySeconds * 2, 30); // 1s, 2s, 4s, 8s, 16s, 30s max
            }
        }

        public void Stop()
        {
            _cts?.Cancel();
            //_connectTask?.Wait();
            WatsonTcpClient?.Disconnect();
        }


        public SyncResponse SyncRequestReceived(SyncRequest arg)
        {
            Console.WriteLine("Data size is " + arg.Data.Length);
            return new SyncResponse(arg, "Client receive the request, send the ProjectModel back to Server");
        }
    }
}
