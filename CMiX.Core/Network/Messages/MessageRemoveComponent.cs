// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels.Components;

namespace CMiX.Core.Network.Messages
{
    public class MessageRemoveComponent : IComponentMessage
    {
        public MessageRemoveComponent()
        {

        }

        public MessageRemoveComponent(Guid parentID, IComponent component)
        {
            ComponentID = component.ID;
            ID = parentID;
        }

        public Guid ComponentID { get; set; }
        public Guid ID { get; set; }
    }
}
