// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using Ceras;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels
{
    public class MessageService : ObservableRecipient, IMessageService
    {
        public MessageService()
        {
            Servers = new ObservableCollection<Server>();
            Serializer = new CerasSerializer();
            Client = new Client();
            Client.DataReceived += Client_DataReceived;
            WeakReferenceMessenger.Default.Register<MessageService, IMessage, string>(this, "OUT", (r, m) => r.Receive(m));
        }

        private void Client_DataReceived(object sender, DataEventArgs e)
        {
            IMessage message = Serializer.Deserialize<IMessage>(e.Data);
            Messenger.Send(message, "IN");
        }

        private void Receive(IMessage m)
        {
            this.SendMessage(m as Message);
        }


        private CerasSerializer Serializer { get; set; }
        public Client Client { get; set; }
        public ObservableCollection<Server> Servers { get; set; }

        public void StartClient(Settings settings)
        {
            Client.IP = settings.IP;
            Client.Port = settings.Port;
            Client.Start();
        }

        public void SendMessage(Message message)
        {
            var data = Serializer.Serialize(message);
            foreach (var server in Servers)
            {
                server.Send(data);
                Console.WriteLine("DataSender SendMessage");
            }
        }
    }
}
