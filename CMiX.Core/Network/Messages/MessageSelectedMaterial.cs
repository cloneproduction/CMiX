// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Network.Messages
{
    public class MessageSelectedMaterial : IMessage
    {
        public MessageSelectedMaterial()
        {

        }

        public MessageSelectedMaterial(Guid id, Guid itemID)
        {
            ID = id;
            ItemId = itemID;
        }

        public Guid ItemId { get; set; }
        public Guid ID { get; set; }
    }
}
