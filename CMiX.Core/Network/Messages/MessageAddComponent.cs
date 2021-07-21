// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Components;

namespace CMiX.Core.Network.Messages
{
    public class MessageAddComponent : Message, IComponentMessage
    {
        public MessageAddComponent()
        {

        }

        public MessageAddComponent(Guid recipientID, Component component)
        {
            ComponentModel = component.GetModel() as IComponentModel;
            ID = recipientID;
        }


        public IComponentModel ComponentModel { get; set; }
        public Guid ID { get; set; }


        public override void Process<T>(T receiver)
        {
            var component = receiver as Component;
            if (component != null && component.ID == this.ID)
            {
                Component newComponent = component.ComponentFactory.CreateComponent(ComponentModel);
                component.AddComponent(newComponent);
                Console.WriteLine("ReceiveMessageAddComponent Count is " + component.Components.Count);
            }
        }
    }
}
