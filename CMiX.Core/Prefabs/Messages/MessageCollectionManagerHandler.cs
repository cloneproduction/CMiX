// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Prefabs.Messages
{
    public class MessageCollectionManagerHandler
    {
        public MessageCollectionManagerHandler()
        { 

        }

        public bool Handle(CollectionManager collection, IMessage message)
        {
            switch (message)
            {
                case null:
                    throw new ArgumentNullException(nameof(message));

                case MessageSelectedItemChanged messageSelectedItemChanged:
                    collection.ManagerData.Select(collection.ControlRepository.GetControl(messageSelectedItemChanged.ControlID));
                    return true;

                case MessageAddItem messageAddPrefab:
                    // A late peer can see an add for an item its snapshot already holds.
                    if (collection.ManagerData.Items.Any(item => item.ID == messageAddPrefab.Model.ID))
                    {
                        collection.ManagerData.SelectAt(messageAddPrefab.SelectedIndex);
                        return true;
                    }
                    collection.InsertItem(messageAddPrefab.Model, messageAddPrefab.SelectedIndex);
                    return true;

                case MessageRemoveItem messageRemovePrefab:
                    collection.DeleteItem(messageRemovePrefab.ModelID);
                    collection.ManagerData.SelectAt(messageRemovePrefab.SelectedIndex);
                    return true;

                case MessageMoveItem messageMovePrefab:
                    collection.ManagerData.Move(messageMovePrefab.OldIndex, messageMovePrefab.NewIndex);
                    return true;

                case MessageRemoveSelectedItem:
                    collection.ManagerData.ClearSelection();
                    return true;

                default:
                    return false;
            }
        }
    }
}
