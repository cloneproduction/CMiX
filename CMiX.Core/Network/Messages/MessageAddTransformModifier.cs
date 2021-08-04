// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Network.Messages
{
    public class MessageAddTransformModifier : IMessage
    {
        public MessageAddTransformModifier()
        {

        }

        public MessageAddTransformModifier(Guid parentID, ITransformModifier transformModifier)
        {
            ID = parentID;
            TransformModifierModel = transformModifier.GetModel() as ITransformModifierModel;
        }


        public ITransformModifierModel TransformModifierModel { get; set; }
        public Guid ID { get; set; }
    }
}
