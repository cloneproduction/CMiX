// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Undo
{
    // A list of controls that sends every change to the engine.
    public interface IItemList
    {
        int Count { get; }

        IControl SelectedItem { get; }

        int SelectedIndex { get; }

        int IndexOf(IControl item);

        // Inserts the item, then sends MessageAddItem with the real index of the item.
        void Insert(IControl item, int index);

        // Removes the item, then sends MessageRemoveItem with the selected index. Returns the old index, or -1.
        int Remove(IControl item);

        // Moves the item, then sends MessageMoveItem.
        void Move(int oldIndex, int newIndex);

        // Selects the item at the index, then sends MessageSelectItem.
        void SelectAt(int index);

        // Clears the selection, then sends MessageClearSelection.
        void ClearSelection();

        // Adds a control from its model at the end, selects it, then sends MessageAddItem.
        void AddFromModel(IControlModel model);

        // Removes every item, disposes the orphans, then sends one MessageRemoveItem per item with index -1.
        void Clear();

        // Disposes the item when no list holds it.
        void Release(IControl item);
    }
}
