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

        CancellationTokenSource cts;

        public int Port { get; set; }
        public string IP { get; set; }

        public void Start(string IP, int Port)
        {
            this.IP = IP;
            this.Port = Port;

            if(WatsonTcpClient != null)
                WatsonTcpClient.Dispose();

            WatsonTcpClient = new WatsonTcpClient(IP, Port);

            //WatsonTcpClient.Keepalive.EnableTcpKeepAlives = true;
            //WatsonTcpClient.Keepalive.TcpKeepAliveInterval = 5;      // seconds to wait before sending subsequent keepalive
            //WatsonTcpClient.Keepalive.TcpKeepAliveTime = 5;          // seconds to wait before sending a keepalive
            //WatsonTcpClient.Keepalive.TcpKeepAliveRetryCount = 5;

            WatsonTcpClient.Events.ServerConnected += ServerConnected;
            WatsonTcpClient.Events.ServerDisconnected += ServerDisconnected;
            WatsonTcpClient.Events.MessageReceived += MessageReceived;

            WatsonTcpClient.Settings.ConnectTimeoutSeconds = 5;

            cts = new CancellationTokenSource();
            _ = TryToConnect(cts.Token);
        }

        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            IMessage message = MessagePackSerialization.Deserialize<IMessage>(new ReadOnlyMemory<byte>(e.Data));
            Console.WriteLine("Message Received of type : " + message.GetType().Name);
            WeakReferenceMessenger.Default.Send(message);
        }

        private void ServerDisconnected(object sender, DisconnectionEventArgs e)
        {
            DeconnectionReason = e.Reason.ToString();
            Console.WriteLine(DeconnectionReason);
            ServerIsConnected = false;

            cts = new CancellationTokenSource();
            _ = TryToConnect(cts.Token);
            Console.WriteLine("Server Disconnected and Disposed");
        }

        private void ServerConnected(object sender, ConnectionEventArgs e)
        {
            ServerIsConnected = true;
            Console.WriteLine("Server Connected");
            cts.Cancel();
        }

        private async Task TryToConnect(CancellationToken cancellationToken)
        {
            if(cts == null)
            {

            }

            while (!WatsonTcpClient.Connected)
            {
                _ = Task.Run(() =>
                {
                    try
                    {
                        WatsonTcpClient.Connect();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Trying to connect to server " + "IP " +  IP.ToString() + "PORT " + Port.ToString());
                    }
                }, cancellationToken);
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }

        public void Stop()
        {
            WatsonTcpClient.Disconnect();
        }


        public SyncResponse SyncRequestReceived(SyncRequest arg)
        {
            Console.WriteLine("Data size is " + arg.Data.Length);
            return new SyncResponse(arg, "Client receive the request, send the ProjectModel back to Server");
        }
    }
}
