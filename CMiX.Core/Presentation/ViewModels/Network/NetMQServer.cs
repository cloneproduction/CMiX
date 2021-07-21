//// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
//// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

//using System;
//using NetMQ;
//using NetMQ.Sockets;

//namespace CMiX.Core.Services
//{
//    public class NetMQServer
//    {
//        public class ShimHandler : IShimHandler
//        {
//            private PairSocket shim;
//            private NetMQPoller poller;
//            private PublisherSocket publisher;
//            private string Address;

//            public ShimHandler(string address)
//            {
//                Address = address;
//            }

//            public void Initialise(object state)
//            {

//            }

//            const int MegaByte = 1024;

//            public void Run(PairSocket shim)
//            {
//                using (publisher = new PublisherSocket())
//                {
//                    publisher.Bind(Address);
//                    publisher.Options.SendHighWatermark = 1000;
//                    publisher.Options.MulticastRate = 1000 * MegaByte;
//                    this.shim = shim;
//                    shim.ReceiveReady += OnShimReady;
//                    shim.SignalOK();
//                    poller = new NetMQPoller { shim, publisher };
//                    poller.Run();
//                }
//            }

//            private void OnShimReady(object sender, NetMQSocketEventArgs e)
//            {
//                NetMQMessage msg = e.Socket.ReceiveMultipartMessage();
//                if (msg[0].ConvertToString() == NetMQActor.EndShimMessage)
//                {
//                    poller.Stop();
//                    return;
//                }
//                publisher.SendMultipartMessage(msg);
//            }

//            private void UpdateString(string stringmessage, string propertyToUpdate)
//            {
//                propertyToUpdate = stringmessage;
//            }
//        }

//        public NetMQServer(string address)
//        {
//            Address = address;
//        }

//        private NetMQActor actor;

//        public string Address { get; set; }

//        public void Start()
//        {
//            if (actor != null)
//                return;
//            actor = NetMQActor.Create(new ShimHandler(Address));
//            Console.WriteLine($"NetMQClient Started with Address : {Address}");
//        }

//        public void Stop()
//        {
//            if (actor != null)
//            {
//                actor.Dispose();
//                actor = null;
//            }
//        }

//        public void ReStart()
//        {
//            if (actor == null)
//                return;
//            Stop();
//            Start();
//        }

//        public void SendObject(string topic, byte[] message)
//        {
//            if (actor == null)
//                return;

//            var msg = new NetMQMessage(3);
//            msg.Append(topic);
//            //msg.Append(messageAddress);
//            msg.Append(message);
//            actor.SendMultipartMessage(msg);
//            Console.WriteLine("SendObject with Topic " + topic);
//        }
//    }
//}
