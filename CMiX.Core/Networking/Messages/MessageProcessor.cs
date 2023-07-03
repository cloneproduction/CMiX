// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Materials;
using CMiX.Core.Prefab.Messages;
using CMiX.Core.Services;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Networking.Messages
{
    public class MessageProcessor
    {
        public MessageProcessor(CompositionService compositionService)
        {
            MessageHandlers = new Dictionary<Type, IMessageHandler>();

            MessageHandlers.Add(typeof(MessageValueChange), new MessageValueChangeHandler(compositionService.Mapper));

            MessageHandlers.Add(typeof(MessageAddItem), new MessageCollectionManagerHandler());
            MessageHandlers.Add(typeof(MessageRemoveItem), new MessageCollectionManagerHandler());
            MessageHandlers.Add(typeof(MessageMoveItem), new MessageCollectionManagerHandler());
            MessageHandlers.Add(typeof(MessageReplaceItem), new MessageCollectionManagerHandler());

            MessageHandlers.Add(typeof(MessageItemOrderChange), new MessagePrefabOrderChangeHandler());
            MessageHandlers.Add(typeof(MessageSelectedPrefabChanged), new MessageSelectorHandler<Material>());
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
