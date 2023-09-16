// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Materials;
using CMiX.Core.Prefab.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Networking.Messages
{
    public class MessageProcessor
    {
        public MessageProcessor()
        {
            MessageHandlers = new Dictionary<Type, IMessageHandler>();

            MessageHandlers.Add(typeof(MessageValueChange), new MessageValueChangeHandler());
            MessageHandlers.Add(typeof(MessageAddItem), new MessageCollectionManagerHandler());
            MessageHandlers.Add(typeof(MessageRemoveItem), new MessageCollectionManagerHandler());
            MessageHandlers.Add(typeof(MessageMoveItem), new MessageCollectionManagerHandler());
            MessageHandlers.Add(typeof(MessageReplaceEmptyPrefab), new MessageCollectionManagerHandler());
            MessageHandlers.Add(typeof(MessageSelectedItemChanged), new MessageCollectionManagerHandler());
        }

        private Dictionary<Type, IMessageHandler> MessageHandlers { get; set; }

        public void ProcessMessage(IMessage message)
        {
            var msg = WeakReferenceMessenger.Default.Send(new MessageRequestControl(message));

            ControlMessenger.CanSend = false;

            if (msg.HasReceivedResponse && MessageHandlers[message.GetType()].Handle(msg.Response, message))
            {
                Console.WriteLine("Message " + message.GetType().Name + " handled");
                ControlMessenger.CanSend = true;
                return;
            }

            ControlMessenger.CanSend = true;

            Console.WriteLine("WARNING ! Message " + message.GetType().Name + " wasn't handled");
        }
    }
}
