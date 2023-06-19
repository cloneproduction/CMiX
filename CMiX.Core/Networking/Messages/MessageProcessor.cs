// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Materials;
using CMiX.Core.Modifiers.Message;
using CMiX.Core.Prefab.Messages;
using CMiX.Core.Services;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Networking.Messages
{
    public class MessageProcessor
    {
        public MessageProcessor(CompositionService compositionService)
        {
            IMapper Mapper = compositionService.Mapper;
            MessageHandlers = new Dictionary<Type, IMessageHandler>();
            MessageHandlers.Add(typeof(MessageUpdateViewModel), new MessageUpdateViewModelHandler(Mapper));
            MessageHandlers.Add(typeof(MessageAddPrefab), new MessagePrefabManagerHandler());
            MessageHandlers.Add(typeof(MessageRemovePrefab), new MessagePrefabManagerHandler());
            MessageHandlers.Add(typeof(MessageMovePrefab), new MessagePrefabManagerHandler());
            MessageHandlers.Add(typeof(MessagePrefabOrderChange), new MessagePrefabOrderChangeHandler());
            MessageHandlers.Add(typeof(MessageAddModifier), new MessageModifierManagerHandler());
            MessageHandlers.Add(typeof(MessageRemoveModifier), new MessageModifierManagerHandler());
            MessageHandlers.Add(typeof(MessageMoveModifier), new MessageModifierManagerHandler());

            MessageHandlers.Add(typeof(MessageSelectedPrefabChanged), new MessageSelectorHandler<Material>());
            MessageHandlers.Add(typeof(MessageSelectorAddPrefab), new MessageSelectorHandler<Material>());
        }

        private Dictionary<Type, IMessageHandler> MessageHandlers { get; set;}

        public void ProcessMessage(IMessage message)
        {
            var msg = WeakReferenceMessenger.Default.Send(new MessageRequestControl(message));

            ControlMessenger.CanSend = false;

            if (msg.HasReceivedResponse && MessageHandlers[message.GetType()].Handle((IControl)msg.Response, message))
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
