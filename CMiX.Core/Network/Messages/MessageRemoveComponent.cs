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

        public MessageRemoveComponent(IComponent component)
        {
            ComponentID = component.ID;
        }

        public Guid ComponentID { get; set; }

        public void Process<T>(T receiver)
        {
            //ComponentManager componentManager = receiver as ComponentManager;
            //componentManager?.DeleteComponent(ComponentID);
        }
    }
}
