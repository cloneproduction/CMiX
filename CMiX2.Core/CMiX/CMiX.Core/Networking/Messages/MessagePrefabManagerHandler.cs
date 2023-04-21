// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs.Message;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Networking.Messages
{
    internal class MessagePrefabManagerHandler : IMessageHandler
    {
        public MessagePrefabManagerHandler()
        {

        }

        public bool Handle(IControl control, IMessage message)
        {
            if(control is IPrefabManager prefabManager)
            {

                if (message is MessageAddPrefab messageAddPrefab)
                {
                    prefabManager.AddPrefab(messageAddPrefab.Model);
                    return true;
                }

                if (message is MessageRemovePrefab messageRemovePrefab)
                {
                    prefabManager.DeleteItem(messageRemovePrefab.PrefabID);
                    return true;
                }

                if (message is MessageMovePrefab messageMovePrefab)
                {
                    prefabManager.MovePrefab(messageMovePrefab.OldIndex, messageMovePrefab.NewIndex);
                    return true;
                }

                //if (message is MessageSelectPrefab messageSelectPrefab)
                //{
                //    prefabManager.SelectPrefab(messageSelectPrefab.PrefabID);
                //    return true;
                //}

            }

            return false;
        }
    }
}
