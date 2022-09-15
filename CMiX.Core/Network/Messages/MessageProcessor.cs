// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Network.Messages
{
    public class MessageProcessor
    {
        public MessageProcessor(CompositionService compositionService)
        {
            MessageHandlers = new Dictionary<Type, IMessageHandler>();

            MessageHandlers.Add(typeof(MessageUpdateViewModel), new MessageUpdateViewModelHandler());
            MessageHandlers.Add(typeof(MessageChangePrefab), new MessageChangePrefabHandler(compositionService));

            MessageHandlers.Add(typeof(MessageAddPrefab), new MessagePrefabManagerHandler());
            MessageHandlers.Add(typeof(MessageRemovePrefab), new MessagePrefabManagerHandler());
            MessageHandlers.Add(typeof(MessageMovePrefab), new MessagePrefabManagerHandler());
            MessageHandlers.Add(typeof(MessagePrefabOrderChange), new MessagePrefabOrderChangeHandler());
            MessageHandlers.Add(typeof(MessageSelectPrefab), new MessagePrefabManagerHandler());

            MessageHandlers.Add(typeof(MessagePrefabContainerChanged), new MessagePrefabContainerChangedHandler(compositionService));

            MessageHandlers.Add(typeof(MessageAddModifier), new MessageModifierManagerHandler());
            MessageHandlers.Add(typeof(MessageRemoveModifier), new MessageModifierManagerHandler());
            MessageHandlers.Add(typeof(MessageMoveModifier), new MessageModifierManagerHandler());

            MessageHandlers.Add(typeof(MessageAsset), new MessageAssetHandler());
        }

        private Dictionary<Type, IMessageHandler> MessageHandlers { get; set;}

        public void ProcessMessage(IMessage message)
        {
            var msg = WeakReferenceMessenger.Default.Send(new MessageRequestControl(message));

            if (msg.HasReceivedResponse && MessageHandlers[message.GetType()].Handle(msg.Response, message))
            {
                Console.WriteLine("Message " + message.GetType().Name + " handled");
                return;
            }

            Console.WriteLine("WARNING ! Message " + message.GetType().Name + " wasn't handled");
        }
    }
}
