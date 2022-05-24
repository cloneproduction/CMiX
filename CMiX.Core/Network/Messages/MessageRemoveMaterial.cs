// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Network.Messages
{
    public class MessageRemoveMaterial : IMessage
    {
        public MessageRemoveMaterial()
        {

        }

        public MessageRemoveMaterial(Guid id, Material material)
        {
            ID = id;
            MaterialID = material.ID;
        }

        public Guid MaterialID { get; set; }
        public Guid ID { get; set; }
    }
}
