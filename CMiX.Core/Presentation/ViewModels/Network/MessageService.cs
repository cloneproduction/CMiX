// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using Ceras;
using CMiX.Core.Network.Communicators;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class MessageService : ObservableObject, IMessageService
    {
        public MessageService()
        {
            Servers = new ObservableCollection<Server>();
            Serializer = new CerasSerializer();
        }


        private CerasSerializer Serializer { get; set; }
        public Client Client { get; set; }
        public ObservableCollection<Server> Servers { get; set; }


        public void SetCommunicator(Communicator communicator)
        {
            communicator.MessageSent += Communicator_MessageSent;
        }

        public void UnSetCommunicator(Communicator communicator)
        {
            communicator.MessageSent -= Communicator_MessageSent;
        }


        private void Communicator_MessageSent(object sender, MessageEventArgs e)
        {
            this.SendMessage(e.Message);
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
