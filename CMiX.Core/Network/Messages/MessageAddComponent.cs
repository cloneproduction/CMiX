// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Components;

namespace CMiX.Core.Network.Messages
{
    public class MessageAddComponent : IComponentMessage
    {
        public MessageAddComponent()
        {

        }

        public MessageAddComponent(Guid parentID, Component component)
        {
            ComponentModel = component.GetModel();
            ParentID = parentID;
        }


        public IComponentModel ComponentModel { get; set; }
        public Guid ParentID { get; set; }
    }
}
