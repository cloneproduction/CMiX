//// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
//// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

//using NetMQ;
//using NetMQ.Sockets;
//using System;

//namespace CMiX.Core.Services
//{
//    public class ClientShimHandler : IShimHandler
//    {
//        private PairSocket shim;
//        private NetMQPoller poller;
//        private SubscriberSocket subscriber;
//        private string Address;
//        private string Topic;


//        public ClientShimHandler(string address, string topic)
//        {
//            this.Address = address;
//            this.Topic = topic;
//        }

//        public void Initialise(object state)
//        {
//        }

//        const int MegaByte = 1024;
//        public void Run(PairSocket shim)
//        {
//            using (subscriber = new SubscriberSocket())
//            using (poller = new NetMQPoller() { subscriber })
//            {
//                subscriber.Connect(Address);
//                subscriber.Subscribe(Topic);
//                subscriber.Options.ReceiveHighWatermark = 1000;
//                subscriber.Options.MulticastRate = 1000 * MegaByte;
//                subscriber.ReceiveReady += OnSubscriberReady;

//                this.shim = shim;
//                this.shim.ReceiveReady += Shim_ReceiveReady;
//                this.shim.SignalOK();

//                poller.Run();
//            }
//        }

//        private void Shim_ReceiveReady(object sender, NetMQSocketEventArgs e)
//        {
//            NetMQMessage msg = e.Socket.ReceiveMultipartMessage();
//        }

//        private void OnSubscriberReady(object sender, NetMQSocketEventArgs e)
//        {
            
//            NetMQMessage msg = new NetMQMessage();
//            msg = e.Socket.ReceiveMultipartMessage();

//            //this.shim.SendFrame("pouet");
//            OnReceiveChange(msg);

//        }

//        public event EventHandler<NetMQMessageEventArgs> ReceiveChangeEvent;
//        public void OnReceiveChange(NetMQMessage netMQMessage)
//        {
//            ReceiveChangeEvent?.Invoke(this, new NetMQMessageEventArgs(netMQMessage));
//        }

//        //private void OnShimReady(object sender, NetMQSocketEventArgs e)
//        //{
//        //    string command = e.Socket.ReceiveFrameString();
//        //    if (command == NetMQActor.EndShimMessage)
//        //        poller.Stop();
//        //}
//    }
//}
