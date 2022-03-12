// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Network.Messages
{
    public class MessageRemoveTransformModifier : IMessage
    {
        public MessageRemoveTransformModifier()
        {

        }

        public MessageRemoveTransformModifier(Guid ParentID, ITransformModifier transformModifier)
        {
            this.ID = ParentID;
            this.TransformModifierModel = transformModifier.GetModel();
        }

        public Guid ID { get; set; }
        public IModel TransformModifierModel { get; set; }
    }
}
