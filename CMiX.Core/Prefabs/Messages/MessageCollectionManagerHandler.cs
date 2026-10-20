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
                    collection.Select(messageSelectedItemChanged.ControlID);
                    return true;

                case MessageAddItem messageAddItem:
                    // A late peer can see an add for an item its snapshot already holds.
                    if (collection.ManagerData.Items.Any(item => item.ID == messageAddItem.Model.ID))
                    {
                        collection.ManagerData.SelectAt(messageAddItem.SelectedIndex);
                        return true;
                    }
                    collection.InsertItem(messageAddItem.Model, messageAddItem.SelectedIndex);
                    return true;

                case MessageRemoveItem messageRemoveItem:
                    collection.RemoveItem(messageRemoveItem.ModelID);
                    collection.ManagerData.SelectAt(messageRemoveItem.SelectedIndex);
                    return true;

                case MessageMoveItem messageMoveItem:
                    collection.ManagerData.Move(messageMoveItem.OldIndex, messageMoveItem.NewIndex);
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
