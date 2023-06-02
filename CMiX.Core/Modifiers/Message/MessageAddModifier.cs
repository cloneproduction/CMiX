// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Networking.Messages;

namespace CMiX.Core.Modifiers.Message
{
    public class MessageAddModifier : IMessage
    {
        public MessageAddModifier()
        {

        }

        public MessageAddModifier(Guid parentID, IControlModel model)
        {
            ID = parentID;
            Model = model;
        }

        public Guid ID { get; set; }
        public IControlModel Model { get; set; }
    }
}
