// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Prefabs.Messages
{
    public class MessageCollectionManagerHandler
    {
        public MessageCollectionManagerHandler()
        {

        }

        public bool Handle(PrefabManager prefabManagerBase, IMessage message)
        {
            switch (message)
            {
                case MessageAddItem messageAddPrefab:
                    prefabManagerBase.AddItem(messageAddPrefab.Model);
                    return true;
                case MessageRemoveItem messageRemovePrefab:
                    prefabManagerBase.DeleteItem(messageRemovePrefab.Control);
                    return true;
                case MessageMoveItem messageMovePrefab:
                    prefabManagerBase.MoveItem(messageMovePrefab.OldIndex, messageMovePrefab.NewIndex);
                    return true;
                case MessageReplaceItem messageReplaceItem:
                    prefabManagerBase.ReplaceItem(messageReplaceItem.ControlModel, messageReplaceItem.Index);
                    return true;
                case MessageSelectedItemChanged messageSelectedItemChanged:
                    prefabManagerBase.SelectedItemChanged(messageSelectedItemChanged.SelectedPrefabID, messageSelectedItemChanged.Index);
                    return true;
                default:
                    return false;
                case null:
                    throw new ArgumentNullException(nameof(message));
            }
        }
    }
}
