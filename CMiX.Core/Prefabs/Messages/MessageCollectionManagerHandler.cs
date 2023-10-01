// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Collections;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefab.Managers;

namespace CMiX.Core.Prefab.Messages
{
    internal class MessageCollectionManagerHandler : IMessageHandler
    {
        public MessageCollectionManagerHandler()
        {

        }

        public bool Handle(IControl control, IMessage message)
        {
            if (control is ICollectionManager collectionManager)
            {

                if (message is MessageAddItem messageAddPrefab)
                {
                    collectionManager.AddItem(messageAddPrefab.Model);
                    return true;
                }

                if (message is MessageRemoveItem messageRemovePrefab)
                {
                    collectionManager.DeleteItem(messageRemovePrefab.Control);
                    return true;
                }

                if (message is MessageMoveItem messageMovePrefab)
                {
                    collectionManager.MoveItem(messageMovePrefab.OldIndex, messageMovePrefab.NewIndex);
                    return true;
                }
            }

            if(control is IPrefabManager prefabManager)
            {
                if (message is MessageReplaceEmptyPrefab messageSelectedPrefabChanged)
                {
                    prefabManager.ReplaceEmptyPrefab(messageSelectedPrefabChanged.emptyPrefabID, messageSelectedPrefabChanged.ControlModel);
                    return true;
                }

                if (message is MessageSelectedItemChanged messageSelectedItemChanged)
                {
                    prefabManager.SelectedItemChanged(messageSelectedItemChanged.SelectedPrefabID, messageSelectedItemChanged.Index);
                    return true;
                }
            }

            return false;
        }
    }
}
