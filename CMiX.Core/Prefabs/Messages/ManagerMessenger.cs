// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.ObjectModel;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Networking.Messenger;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Prefabs.Messages
{
    public class ManagerMessenger
    {
        public ManagerMessenger(MessageFactory messageFactory, ObservableCollection<Server> servers)
        {
            Servers = servers;
            MessageFactory = messageFactory;
            CanSend = true;
            CollectionHandler = new MessageCollectionManagerHandler();
        }

        MessageFactory MessageFactory { get; }
        ObservableCollection<Server> Servers {  get; }
        public MessageCollectionManagerHandler CollectionHandler { get; set; }

        private bool CanSend;


        public void Receive(PrefabManager prefabManagerBase, IMessage messagePrefab)
        {
            if (messagePrefab is not IMessageManager messageManager)
                return;

            if (prefabManagerBase.ManagerData.ID != messagePrefab.ID)
                return;

            CanSend = false;
            CollectionHandler.Handle(prefabManagerBase, messagePrefab);
            CanSend = true;

            Console.WriteLine("Message " + messagePrefab.GetType().Name + " handled by ManagerMessenger");
        }

        void Send(IMessage message)
        {
            foreach (Server server in Servers)
                server.SendMessage(message);
        }

        public void SendAddItem(Guid id, IControl control)
        {
            var message = MessageFactory.CreateMessage(typeof(MessageAddItem), id, control);
            Send(message);
        }

        public void SendSelectedItemChanged(Guid id, IControl control, int index)
        {
            var message = MessageFactory.CreateMessage(typeof(MessageSelectedItemChanged), id, control, index);
            Send(message);
        }

        public void SendReplaceItem(Guid id, IControl control, int index)
        {
            var message = MessageFactory.CreateMessage(typeof(MessageReplaceItem), id, control, index);
            Send(message);
        }

        public void SendMessageRemoveItem(Guid id, IControl control)
        {
            var message = MessageFactory.CreateMessage(typeof(MessageRemoveItem), id, control);
            Send(message);
        }

        internal void SendMessageMoveItem(Guid id, int sourceIndex, int targetIndex)
        {
            var message = MessageFactory.CreateMessage(typeof(MessageMoveItem), id, sourceIndex, targetIndex);
            Send(message);
        }

        internal void SendRemoveSelectedItem(Guid id)
        {
            var message = MessageFactory.CreateMessage(typeof(MessageRemoveSelectedItem), id);
            Send(message);
        }
    }
}
