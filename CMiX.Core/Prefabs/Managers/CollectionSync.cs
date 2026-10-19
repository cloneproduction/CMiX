// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs.Messages;

namespace CMiX.Core.Prefabs.Managers
{
    public class CollectionSync
    {
        private readonly ControlMessenger _messenger;
        private readonly MessageFactory _messageFactory;

        public CollectionSync(CollectionManager collection, ControlMessenger messenger, MessageFactory messageFactory)
        {
            Collection = collection;
            _messenger = messenger;
            _messageFactory = messageFactory;
        }

        public CollectionManager Collection { get; }

        public void SendAdd(IControl item, int index) =>
            _messenger.SendMessage(_messageFactory.CreateMessage<MessageAddItem>(Collection.ManagerData.ID, item, index));

        public void SendRemove(IControl item, int selectedIndex) =>
            _messenger.SendMessage(_messageFactory.CreateMessage<MessageRemoveItem>(Collection.ManagerData.ID, item, selectedIndex));

        public void SendMove(int oldIndex, int newIndex) =>
            _messenger.SendMessage(_messageFactory.CreateMessage<MessageMoveItem>(Collection.ManagerData.ID, oldIndex, newIndex));

        public void SendSelect(IControl item, int index) =>
            _messenger.SendMessage(_messageFactory.CreateMessage<MessageSelectedItemChanged>(Collection.ManagerData.ID, item, index));

        public void SendRemoveSelected() =>
            _messenger.SendMessage(_messageFactory.CreateMessage<MessageRemoveSelectedItem>(Collection.ManagerData.ID));
    }
}
