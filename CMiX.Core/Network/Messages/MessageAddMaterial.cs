// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Network.Messages
{
    public class MessageAddMaterial : IMessage
    { 
        public MessageAddMaterial()
        {

        }

        public MessageAddMaterial(Guid parentID, Material material)
        {
            MaterialModel = (TextureModel)material.GetModel();
            ID = parentID;
        }

        public TextureModel MaterialModel { get; set; }
        public Guid ID { get; set; }
    }
}
