// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Models.Component;
using CMiX.Core.Presentation.ViewModels.Components;

namespace CMiX.Core.Network.Messages
{
    public class MessageCreateLayerMask : IMessage
    {
        public MessageCreateLayerMask()
        {

        }

        public MessageCreateLayerMask(Guid parentID, Layer layer)
        {
            ID = parentID;
            LayerID = layer.ID;
            LayerMaskModel = (LayerMaskModel)layer.LayerMask.GetModel();
        }

        public Guid ID { get; set; }
        public Guid LayerID { get; set; }
        public LayerMaskModel LayerMaskModel { get; set;}
    }
}
