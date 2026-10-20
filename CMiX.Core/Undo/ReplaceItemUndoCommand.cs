// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core.Undo
{
    // Backs the reset item path, which swaps a fresh instance in for one already in a collection.
    // Both instances stay alive here while the command sits on a stack, following the same
    // ownership rule as RemoveItemUndoCommand: whichever of the two is left unreferenced once the
    // command is dropped is disposed then, and never earlier, so an undo can always put the
    // original back with its nested contents intact.
    //
    // The swap travels over the wire as the remove and add pair the engine already handles, since
    // there is no replace message in the protocol. That mirrors what undoing a delete sends, so
    // the receiving side reorders the item to the end of its collection exactly as it already
    // does there.
    public class ReplaceItemUndoCommand : IUndoCommand, IDisposable
    {
        private readonly CollectionSync _sync;
        private readonly IControl _previousItem;
        private readonly IControl _newItem;
        private readonly int _index;

        public ReplaceItemUndoCommand(CollectionSync sync,
                                  IControl previousItem,
                                  IControl newItem,
                                  int index)
        {
            _sync = sync;
            _previousItem = previousItem;
            _newItem = newItem;
            _index = index;
        }

        public void Execute() => Replace(_previousItem, _newItem);

        public void Undo() => Replace(_newItem, _previousItem);

        private void Replace(IControl outgoing, IControl incoming)
        {
            var collection = _sync.Collection;
            var index = collection.ManagerData.Items.IndexOf(outgoing);
            if (index < 0) index = _index;

            collection.RemoveItem(outgoing);
            _sync.SendRemove(outgoing, collection.ManagerData.SelectedIndex);

            if (index > collection.ManagerData.Items.Count)
                index = collection.ManagerData.Items.Count;

            collection.InsertItem(incoming, index);
            _sync.SendAdd(incoming, index);
        }

        public void Dispose()
        {
            OwnedControl.DisposeIfOrphaned(_sync.Collection.ControlRepository, _previousItem);
            OwnedControl.DisposeIfOrphaned(_sync.Collection.ControlRepository, _newItem);
        }
    }
}
