// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Presentations.ViewModels;

namespace CMiX.Core.Presentations.Modifiers.Message
{
    public class MessageRemoveModifier : IMessage
    {
        public MessageRemoveModifier()
        {

        }

        public MessageRemoveModifier(Guid ParentID, IModifier modifier)
        {
            ID = ParentID;
            ModifierID = modifier.ID;
        }

        public Guid ID { get; set; }
        public Guid ModifierID { get; set; }
    }
}
