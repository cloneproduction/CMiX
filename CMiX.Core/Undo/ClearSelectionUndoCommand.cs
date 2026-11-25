// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Undo
{
    public class ClearSelectionUndoCommand : ItemListUndoCommand
    {
        public ClearSelectionUndoCommand(IItemList list) : base(list)
        {
        }

        protected override void Change() => List.ClearSelection();

        protected override void ChangeBack() { }
    }
}
