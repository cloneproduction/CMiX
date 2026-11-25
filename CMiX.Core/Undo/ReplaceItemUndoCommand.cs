// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Undo
{
    // Swaps a fresh instance in for an item of the list, and owns both while the step is on a stack.
    // The swap travels as a remove and an add, because the protocol has no replace message.
    public class ReplaceItemUndoCommand : ItemListUndoCommand, IDisposable
    {
        private readonly IControl _previousItem;
        private readonly IControl _newItem;
        private readonly int _index;

        public ReplaceItemUndoCommand(IItemList list,
                                  IControl previousItem,
                                  IControl newItem,
                                  int index) : base(list)
        {
            _previousItem = previousItem;
            _newItem = newItem;
            _index = index;
        }

        protected override void Change() => Replace(_previousItem, _newItem);

        protected override void ChangeBack() => Replace(_newItem, _previousItem);

        private void Replace(IControl outgoing, IControl incoming)
        {
            var index = List.IndexOf(outgoing);
            if (index < 0) index = _index;

            List.Remove(outgoing);

            if (index > List.Count)
                index = List.Count;

            List.Insert(incoming, index);
        }

        public void Dispose()
        {
            List.Release(_previousItem);
            List.Release(_newItem);
        }
    }
}
