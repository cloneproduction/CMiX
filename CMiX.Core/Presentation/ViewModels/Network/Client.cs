// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using Ceras;
using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using WatsonTcp;

namespace CMiX.Core.Services
{
    public class Client : ObservableRecipient
    {
        public Client()
        {
            Serializer = new CerasSerializer();
        }


        public event EventHandler<DataEventArgs> DataReceived;
        private void OnDataReceived(object sender, DataEventArgs e)
        {
            DataReceived?.Invoke(sender, e);
        }


        private CerasSerializer Serializer { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public bool IsRunning { get; private set; }


        public string Address
        {
            get { return String.Format("tcp://{0}:{1}", IP, Port); }
        }


        public WatsonTcpClient WatsonTcpClient { get; set; }
        public void Start()
        {
            WatsonTcpClient = new WatsonTcpClient(IP, Port);
            WatsonTcpClient.Events.ServerConnected += ServerConnected;
            WatsonTcpClient.Events.ServerDisconnected += ServerDisconnected;
            WatsonTcpClient.Events.MessageReceived += MessageReceived;
            WatsonTcpClient.Callbacks.SyncRequestReceived = SyncRequestReceived;
            WatsonTcpClient.Settings.ConnectTimeoutSeconds = 5;

            TryToConnect(WatsonTcpClient);

            Console.WriteLine($"WatsonTcp Started with Address " + Address);
        }

        private SyncResponse SyncRequestReceived(SyncRequest arg)
        {
            var projectModel = Serializer.Deserialize<ProjectModel>(arg.Data);
            Console.WriteLine("Data size is " + arg.Data.Length);
            Console.WriteLine("ProjectModel had " + projectModel.ComponentModels.Count + "Components");
            Console.WriteLine("Client received the request of type :  " + projectModel.GetType());
            return new SyncResponse(arg, "Client receive the request, send the ProjectModel back to Server");
        }

        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine("Client MessageReceived");
            OnDataReceived(sender, new DataEventArgs(e.Data));
        }

        private void ServerDisconnected(object sender, DisconnectionEventArgs e)
        {
            Console.WriteLine("Server " + e.IpPort + " disconnected");
            TryToConnect(WatsonTcpClient);
        }

        private void ServerConnected(object sender, ConnectionEventArgs e)
        {
            Console.WriteLine("Server " + e.IpPort + " connected");
        }


        private void TryToConnect(WatsonTcpClient watsonTcpClient)
        {
            while (!watsonTcpClient.Connected)
            {
                try
                {
                    Console.WriteLine("Connecting...");
                    watsonTcpClient.Connect();
                }
                catch (Exception)
                {
                    Console.WriteLine("Can't Connect");
                }
            }
        }

        public void Stop()
        {
            WatsonTcpClient.Disconnect();
        }


        //public void Stop()
        //{
        //    //if (Poller != null)
        //    //    Poller.StopAsync();
        //}

        //public SubscriberSocket SubscriberSocket { get; set; }
        //public NetMQPoller Poller { get; set; }

        //private void ClientSub_ReceiveReady(object sender, NetMQSocketEventArgs e)
        //{
        //    string topic = e.Socket.ReceiveFrameString();
        //    byte[] data = e.Socket.ReceiveFrameBytes();
        //    OnDataReceived(sender, new DataEventArgs(data));
        //}
    }
}
