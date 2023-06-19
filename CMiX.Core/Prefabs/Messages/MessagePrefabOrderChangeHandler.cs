// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefab.Managers;

namespace CMiX.Core.Prefab.Messages
{
    public class MessagePrefabOrderChangeHandler : IMessageHandler
    {
        public MessagePrefabOrderChangeHandler()
        {

        }

        public bool Handle(IControl control, IMessage message)
        {
            if (control is IPrefabManagerDraggable prefabManagerDraggable)
            {
                if (message is MessagePrefabOrderChange msg)
                {
                    prefabManagerDraggable.UpdateComponentOrder(msg.IDs);
                    return true;
                }
            }

            return false;
        }
    }
}
