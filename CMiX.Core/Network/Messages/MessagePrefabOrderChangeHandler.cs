// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Network.Messages
{
    public class MessagePrefabOrderChangeHandler : IMessageHandler
    {
        public MessagePrefabOrderChangeHandler()
        {

        }
        public bool Handle(IControl control, IMessage message)
        {
            var msg = message as MessagePrefabOrderChange;

            if(control is IPrefabManagerDraggable prefabManagerDraggable)
            {
                prefabManagerDraggable.UpdateComponentOrder(msg.IDs);
                return true;
            }

            return false;
        }
    }
}
