// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;

namespace CMiX.Core.Presentation.ViewModels
{
    public class MessageAddModifier : IMessage
    {
        public MessageAddModifier()
        {

        }

        public MessageAddModifier(Guid parentID, IModel model)
        {
            ID = parentID;
            Model = model;
        }

        public Guid ID { get; set; }
        public IModel Model { get; set; }
    }
}
