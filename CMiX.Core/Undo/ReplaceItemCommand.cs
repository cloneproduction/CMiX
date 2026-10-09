// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Prefabs.Messages;

namespace CMiX.Core.Undo
{
    // Backs the reset item path, which swaps a fresh instance in for one already in a collection.
    // Both instances stay alive here while the command sits on a stack, following the same
    // ownership rule as RemoveItemCommand: whichever of the two is left unreferenced once the
    // command is dropped is disposed then, and never earlier, so an undo can always put the
    // original back with its nested contents intact.
    //
    // The swap travels over the wire as the remove and add pair the engine already handles, since
    // there is no replace message in the protocol. That mirrors what undoing a delete sends, so
    // the receiving side reorders the item to the end of its collection exactly as it already
    // does there.
    public class ReplaceItemCommand : IUndoCommand, IDisposable
    {
        private readonly CollectionManager _collection;
        private readonly ControlMessenger _messenger;
        private readonly MessageFactory _messageFactory;
        private readonly IControl _previousItem;
        private readonly IControl _newItem;
        private readonly int _index;

        public ReplaceItemCommand(CollectionManager collection,
                                  ControlMessenger messenger,
                                  MessageFactory messageFactory,
                                  IControl previousItem,
                                  IControl newItem,
                                  int index)
        {
            _collection = collection;
            _messenger = messenger;
            _messageFactory = messageFactory;
            _previousItem = previousItem;
            _newItem = newItem;
            _index = index;
        }

        public void Execute() => Replace(_previousItem, _newItem);

        public void Undo() => Replace(_newItem, _previousItem);

        private void Replace(IControl outgoing, IControl incoming)
        {
            var index = _collection.ManagerData.Items.IndexOf(outgoing);
            if (index < 0) index = _index;

            _collection.DeleteItem(outgoing);
            _messenger.SendMessage(_messageFactory.CreateMessage<MessageRemoveItem>(
                _collection.ManagerData.ID, outgoing, _collection.ManagerData.SelectedIndex));

            if (index > _collection.ManagerData.Items.Count)
                index = _collection.ManagerData.Items.Count;

            _collection.InsertItem(incoming, index);
            _messenger.SendMessage(_messageFactory.CreateMessage<MessageAddItem>(
                _collection.ManagerData.ID, incoming, index));
        }

        public void Dispose()
        {
            OwnedControl.DisposeIfOrphaned(_collection.ControlRepository, _previousItem);
            OwnedControl.DisposeIfOrphaned(_collection.ControlRepository, _newItem);
        }
    }
}
