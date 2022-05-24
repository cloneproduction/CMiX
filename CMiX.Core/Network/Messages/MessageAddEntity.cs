
// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Network.Messages
{
    public class MessageAddEntity : IMessage
    {
        public MessageAddEntity()
        {

        }

        public MessageAddEntity(Guid parentID, IEntity entity)
        {
            EntityModel = (IEntityModel)entity.GetModel();
            ID = parentID;
            EntityType = entity.GetType();
        }

        public Type EntityType { get; set; }
        public IEntityModel EntityModel { get; set; }
        public Guid ID { get; set; }
    }
}
