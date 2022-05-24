// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Network.Messages
{
    public class MessageRemoveEntity : IMessage
    {
        public MessageRemoveEntity()
        {

        }

        public MessageRemoveEntity(Guid id, IEntity entity)
        {
            ID = id;
            EntityID = entity.ID;
        }

        public Guid EntityID { get; set; }
        public Guid ID { get; set; }
    }
}
