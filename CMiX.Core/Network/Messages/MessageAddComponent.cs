// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Components;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace CMiX.Core.Network.Messages
{
    public class MessageAddComponent : ValueChangedMessage<Component>
    {
        //public MessageAddComponent()
        //{

        //}

        public MessageAddComponent(Component component) : base(component)
        {
            ComponentModel = component.GetModel() as IComponentModel;
            //IDs = new List<Guid>();
            //IDs.Add(id);
        }

        public IComponentModel ComponentModel { get; set; }

        //public async void ReceiveWithMediator(IMediator mediator)
        //{
        //    await mediator.Publish(new ReceiveAddNewComponentNotification(IDs.First(), ComponentModel));
        //}

        //public override void Process<T>(T receiver)
        //{
        //    var component = receiver as Component;
        //    Component newComponent = component.ComponentFactory.CreateComponent(ComponentModel);
        //    component.AddComponent(newComponent);
        //    newComponent.SetCommunicator(component.Communicator);
        //    Console.WriteLine("ReceiveMessageAddComponent Count is " + component.Components.Count);
        //}
    }
}
