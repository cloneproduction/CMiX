// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking.Messages;
using CMiX.Core.ViewModels;

namespace CMiX.Core.Modifiers.Message
{
    public class MessageModifierManagerHandler : IMessageHandler
    {
        public MessageModifierManagerHandler()
        {

        }

        public bool Handle(IControl control, IMessage message)
        {
            if (control is ModifierManager modifierManager)
            {
                if (message is MessageAddModifier messageAddModifier)
                {
                    modifierManager.Create(messageAddModifier.Model as IModifierModel);
                    return true;
                }

                if (message is MessageRemoveModifier messageRemoveModifier)
                {
                    modifierManager.Remove(messageRemoveModifier.ModifierID);
                    return true;
                }

                if (message is MessageMoveModifier messageModifierMove)
                {
                    modifierManager.Move(messageModifierMove.OldIndex, messageModifierMove.NewIndex);
                    return true;
                }
            }

            return false;
        }
    }
}
