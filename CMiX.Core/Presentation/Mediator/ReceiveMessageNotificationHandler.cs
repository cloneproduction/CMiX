// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Components;
using MediatR;

namespace CMiX.Core.Presentation.Mediator
{
    public class ReceiveMessageNotificationHandler : INotificationHandler<ReceiveMessageNotification>
    {
        public ReceiveMessageNotificationHandler(IMediator mediator)
        {
            //_componentDatabase = componentDatabase;
            _mediator = mediator;
        }

        //private readonly IComponentDatabase _componentDatabase;
        private readonly IMediator _mediator;

        public Task Handle(ReceiveMessageNotification notification, CancellationToken cancellationToken)
        {
            Message message = notification.Message;
            var id = message.IDs.First();
            //var component = _componentDatabase.GetComponent(id);

            //Console.WriteLine("ReceiveMessageNotification by Component with ID : " + component.ID);
            //component.ReceiveMessage(message);
            return Task.CompletedTask;
        }
    }
}
