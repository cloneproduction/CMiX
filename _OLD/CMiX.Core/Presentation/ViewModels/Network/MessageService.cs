// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using Ceras;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.ViewModels.Service;
using CMiX.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels
{
    public class MessageService : ObservableRecipient, IMessageService
    {
        public MessageService(CompositionService compositionService, CerasSerializer serializer)
        {
            Servers = new ObservableCollection<Server>();
            Serializer = serializer;

            Client = new Client();
            Client.DataReceived += Client_DataReceived;
            IsActive = true;

            MessageProcessor = new MessageProcessor(compositionService);
        }


        public MessageProcessor MessageProcessor { get; set; }

        private void Client_DataReceived(object sender, DataEventArgs e)
        {
            IMessage message = Serializer.Deserialize<IMessage>(e.Data);
            MessageProcessor.ProcessMessage(message);
            WeakReferenceMessenger.Default.Send(message, MessageType.In);
        }


        protected override void OnActivated()
        {
            Messenger.Register<IMessage, int>(this, MessageType.Out, (r, m) => SendMessage(m));
        }


        private CerasSerializer Serializer { get; set; }
        public Client Client { get; set; }
        public ObservableCollection<Server> Servers { get; set; }


        public void StartClient(Settings settings)
        {
            Client.Start(settings);
        }

        public void SendMessage(IMessage message)
        {
            Console.WriteLine("MessageService SendMessage of type " + message.GetType().Name);
            var data = Serializer.Serialize(message);
            foreach (var server in Servers)
            {
                server.Send(data);
            }
        }
    }
}
