// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Network.Messages;

namespace CMiX.Core.Presentation.ViewModels.Modifiers
{
    public class MessageAddModifier : IMessage
    {
        public MessageAddModifier()
        {

        }

        public MessageAddModifier(Guid parentID, IModifier textureFilter)
        {
            ID = parentID;
            ModifierModel = (IModifierModel)textureFilter.GetModel();
        }


        public IModifierModel ModifierModel { get; set; }
        public Guid ID { get; set; }
    }
}
