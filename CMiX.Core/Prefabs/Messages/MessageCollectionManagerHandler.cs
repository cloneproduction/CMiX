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

        public bool Handle(PrefabManagerBase prefabManagerBase, IMessage message)
        {
            if (message is MessageAddItem messageAddPrefab)
            {
                prefabManagerBase.AddItem(messageAddPrefab.Model);
                return true;
            }

            if (message is MessageRemoveItem messageRemovePrefab)
            {
                prefabManagerBase.DeleteItem(messageRemovePrefab.Control);
                return true;
            }

            if (message is MessageMoveItem messageMovePrefab)
            {
                prefabManagerBase.MoveItem(messageMovePrefab.OldIndex, messageMovePrefab.NewIndex);
                return true;
            }

            if (prefabManagerBase is IPrefabManager prefabManager)
            {
                if (message is MessageReplacePrefab messageSelectedPrefabChanged)
                {
                    if(prefabManager is ReorderablePrefabManager manager)
                        manager.EmptyPrefabService.ReplaceEmptyPrefab(messageSelectedPrefabChanged.emptyPrefabID, messageSelectedPrefabChanged.ControlModel);
                    return true;
                }

                if (message is MessageSelectedItemChanged messageSelectedItemChanged)
                {
                    prefabManagerBase.SelectedItemChanged(messageSelectedItemChanged.SelectedPrefabID, messageSelectedItemChanged.Index);
                    return true;
                }
            }

            return false;
        }
    }
}
