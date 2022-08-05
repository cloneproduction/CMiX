// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;

namespace CMiX.Core.Presentation.ViewModels.Prefab
{
    public class MessageRemoveSlot : IMessage
    {
        public MessageRemoveSlot()
        {

        }

        public MessageRemoveSlot(Guid managerID, PrefabSlot prefabSlot)
        {
            ID = managerID;
            SlotID = prefabSlot.ID;
        }

        public Guid ID { get; set; }
        public Guid SlotID { get; set; }
    }
}
