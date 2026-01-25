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
                case null:
                    throw new ArgumentNullException(nameof(message));

                case MessageSelectedItemChanged messageSelectedItemChanged:
                    prefabManagerBase.SelectedItemChanged(messageSelectedItemChanged.Control, messageSelectedItemChanged.Index);
                    return true;

                case MessageAddItem messageAddPrefab:
                    prefabManagerBase.AddItem(messageAddPrefab.Model);
                    return true;

                case MessageRemoveItem messageRemovePrefab:
                    prefabManagerBase.DeleteItem(messageRemovePrefab.ModelID);
                    return true;

                case MessageMoveItem messageMovePrefab:
                    prefabManagerBase.MoveItem(messageMovePrefab.OldIndex, messageMovePrefab.NewIndex);
                    return true;

                case MessageReplaceItem messageReplaceItem:
                    prefabManagerBase.ReplaceItem(messageReplaceItem.ControlModel, messageReplaceItem.Index);
                    return true;

                case MessageRemoveSelectedItem messageRemoveSelectedItem:
                    prefabManagerBase.RemoveSelectedItem();
                    return true;

                default:
                    return false;
            }
        }
    }
}
