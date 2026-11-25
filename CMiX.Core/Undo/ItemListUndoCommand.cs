// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Undo
{
    // A step that changes a list. Undo and Redo restore the selection of the list exactly.
    public abstract class ItemListUndoCommand : IUndoCommand
    {
        private IControl _selectedBefore;
        private IControl _selectedAfter;
        private bool _executedOnce;

        protected ItemListUndoCommand(IItemList list) => List = list;

        protected IItemList List { get; }

        public void Execute()
        {
            if (_executedOnce)
            {
                Change();
                RestoreSelection(_selectedAfter);
                return;
            }

            _selectedBefore = List.SelectedItem;
            Change();
            _selectedAfter = List.SelectedItem;
            _executedOnce = true;
        }

        public void Undo()
        {
            ChangeBack();
            RestoreSelection(_selectedBefore);
        }

        // The list change of the step.
        protected abstract void Change();

        // The inverse list change.
        protected abstract void ChangeBack();

        // Selects the item, or nothing, only when the list does not show it already.
        private void RestoreSelection(IControl item)
        {
            if (ReferenceEquals(List.SelectedItem, item)) return;
            if (item == null) { List.ClearSelection(); return; }
            var index = List.IndexOf(item);
            if (index >= 0) List.SelectAt(index);
        }
    }
}
