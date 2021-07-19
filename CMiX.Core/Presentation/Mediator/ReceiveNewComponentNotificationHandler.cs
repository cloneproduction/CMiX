// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using CMiX.Core.Presentation.ViewModels.Components;
using MediatR;

namespace CMiX.Core.Presentation.Mediator
{
    public class ReceiveAddNewComponentNotificationHandler //: INotificationHandler<ReceiveAddNewComponentNotification>
    {
        public ReceiveAddNewComponentNotificationHandler()
        {

        }
        //public ReceiveAddNewComponentNotificationHandler(IComponentDatabase componentDatabase)
        //{
        //    _componentDatabase = componentDatabase;
        //}

        //private readonly IComponentDatabase _componentDatabase;

        //public Task Handle(ReceiveAddNewComponentNotification notification, CancellationToken cancellationToken)
        //{
        //    //var component = _componentDatabase.GetComponent(notification.ID);
        //    //var newComponent = component.ComponentFactory.CreateComponent(notification.ComponentModel);
        //    //component.AddComponent(newComponent);
        //    //_componentDatabase.AddComponent(newComponent);
        //    //Console.WriteLine("ReceiveMessageAddComponent Count is " + component.Components.Count);
        //    //return Task.CompletedTask;
        //}
    }
}
