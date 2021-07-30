// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace CMiX.Core.Network.Messages
{
    public class MessageUpdateViewModel : IMessage//, IMessage
    {
        public MessageUpdateViewModel()
        {

        }

        public MessageUpdateViewModel(IControl control)
        {
            Model = control.GetModel();
            ID = control.ID;
        }

        public Guid ID { get; set; }
        public IModel Model { get; set; }
    }
}
