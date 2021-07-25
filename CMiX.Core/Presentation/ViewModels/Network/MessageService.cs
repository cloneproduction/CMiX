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
    public class MessageService : ObservableRecipient, 
        IRecipient<MessageUpdateViewModel>,
        IRecipient<MessageAddComponent>,
        IRecipient<MessageRemoveComponent>,
        IMessageService
    {
        public MessageService(CerasSerializer serializer)
        {
            Servers = new ObservableCollection<Server>();
            Serializer = serializer;
            Client = new Client();
            Client.DataReceived += Client_DataReceived;
            Messenger.RegisterAll(this, "OUT");
        }


        private CerasSerializer Serializer { get; set; }
        public Client Client { get; set; }
        public ObservableCollection<Server> Servers { get; set; }


        private void Client_DataReceived(object sender, DataEventArgs e)
        {
            IMessage message = Serializer.Deserialize<IMessage>(e.Data);

            if(message is MessageAddComponent)
                WeakReferenceMessenger.Default.Send((MessageAddComponent)message, "IN");

            if (message is MessageRemoveComponent)
                WeakReferenceMessenger.Default.Send((MessageRemoveComponent)message, "IN");

            if (message is MessageUpdateViewModel)
                WeakReferenceMessenger.Default.Send((MessageUpdateViewModel)message, "IN");
        }




        public void StartClient(Settings settings)
        {
            Client.IP = settings.IP;
            Client.Port = settings.Port;
            Client.Start();
        }

        public void SendMessage(IMessage message)
        {
            var data = Serializer.Serialize(message);
            foreach (var server in Servers)
            {
                server.Send(data);
                Console.WriteLine("DataSender SendMessage");
            }
        }

        public void Receive(MessageUpdateViewModel message)
        {
            this.SendMessage(message);
        }

        public void Receive(MessageAddComponent message)
        {
            this.SendMessage(message);
        }

        public void Receive(MessageRemoveComponent message)
        {
            this.SendMessage(message);
        }
    }
}
