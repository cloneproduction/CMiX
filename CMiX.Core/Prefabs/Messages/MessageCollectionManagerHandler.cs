// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Collections;
using CMiX.Core.Networking.Messages;

namespace CMiX.Core.Prefab.Messages
{
    internal class MessageCollectionManagerHandler : IMessageHandler
    {
        public MessageCollectionManagerHandler()
        {

        }

        public bool Handle(IControl control, IMessage message)
        {
            if (control is ICollectionManager prefabManager)
            {

                if (message is MessageAddItem messageAddPrefab)
                {
                    prefabManager.AddItem(messageAddPrefab.Model);
                    return true;
                }

                if (message is MessageRemoveItem messageRemovePrefab)
                {
                    prefabManager.DeleteItem(messageRemovePrefab.Control);
                    return true;
                }

                if (message is MessageMoveItem messageMovePrefab)
                {
                    prefabManager.MoveItem(messageMovePrefab.OldIndex, messageMovePrefab.NewIndex);
                    return true;
                }

                if (message is MessageReplaceItem messageSelectedPrefabChanged)
                {
                    prefabManager.ReplaceItem(messageSelectedPrefabChanged.OldPrefabID, messageSelectedPrefabChanged.NewPrefabID);
                    return true;
                }

                if (message is MessageSelectedItemChanged messageSelectedItemChanged)
                {
                    prefabManager.SelectedItemChanged(messageSelectedItemChanged.SelectedPrefabID);
                    return true;
                }
            }

            return false;
        }
    }
}
