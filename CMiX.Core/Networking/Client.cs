// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking.Messages;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using WatsonTcp;

namespace CMiX.Core.Services
{
    public class Client : ObservableRecipient
    {
        public Client(MessageProcessor messageProcessor)
        {
            ServerIsConnected = false;
            MessageProcessor = messageProcessor;
        }

        private MessageProcessor MessageProcessor { get; set; }
        public WatsonTcpClient WatsonTcpClient { get; set; }

        public string IP { get; set; }
        public int Port { get; set; }
        public bool IsRunning { get; private set; }
        public bool ServerIsConnected { get; set; }
        public string DeconnectionReason { get; set; }


        public void Start(Settings settings)
        {
            if(WatsonTcpClient != null)
                WatsonTcpClient.Dispose();

                WatsonTcpClient = new WatsonTcpClient(settings.IP, settings.Port);
                WatsonTcpClient.Events.ServerConnected += ServerConnected;
                WatsonTcpClient.Events.ServerDisconnected += ServerDisconnected;
                WatsonTcpClient.Events.MessageReceived += MessageReceived;
                WatsonTcpClient.Settings.ConnectTimeoutSeconds = 5;

                _ = TryToConnect(WatsonTcpClient);
        }

        //private SyncResponse SyncRequestReceived(SyncRequest arg)
        //{
        //    var projectModel = Serializer.Deserialize<ProjectModel>(arg.Data);
        //    Console.WriteLine("Data size is " + arg.Data.Length);
        //    Console.WriteLine("Client received the request of type :  " + projectModel.GetType());
        //    return new SyncResponse(arg, "Client receive the request, send the ProjectModel back to Server");
        //}

        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            MessageProcessor.ProcessMessage(e.Data);
            Console.WriteLine("Message Data Received by Clients");
        }

        private void ServerDisconnected(object sender, DisconnectionEventArgs e)
        {
            Console.WriteLine("Server " + e.Client.IpPort + " disconnected");
            DeconnectionReason = e.Reason.ToString();
            ServerIsConnected = false;
            _ = TryToConnect(this.WatsonTcpClient);
        }

        private void ServerConnected(object sender, ConnectionEventArgs e)
        {
            if (e.Client == null)
                return;

            ServerIsConnected = true;
            Console.WriteLine("Server " + e.Client.IpPort + " connected");
        }


        private async Task TryToConnect(WatsonTcpClient watsonTcpClient)
        {
            while (!watsonTcpClient.Connected)
            {
                _ = Task.Run(() =>
                  {
                      try
                      {
                          watsonTcpClient.Connect();
                      }
                      catch (Exception)
                      {
                          Console.WriteLine("Trying to connect to server");
                      }
                  });
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }

        public void Stop()
        {
            WatsonTcpClient.Disconnect();
        }
    }
}
