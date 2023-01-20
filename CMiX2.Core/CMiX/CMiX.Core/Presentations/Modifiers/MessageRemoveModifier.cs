// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Networking.Messages;

namespace CMiX.Core.Presentation.ViewModels.Modifiers
{
    public class MessageRemoveModifier : IMessage
    {
        public MessageRemoveModifier()
        {

        }

        public MessageRemoveModifier(Guid ParentID, IModifier modifier)
        {
            this.ID = ParentID;
            this.ModifierID = modifier.ID;
        }

        public Guid ID { get; set; }
        public Guid ModifierID { get; set; }
    }
}
