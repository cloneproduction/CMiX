// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Undo
{
    public class SelectItemUndoCommand : ItemListUndoCommand
    {
        private readonly int _newIndex;

        public SelectItemUndoCommand(IItemList list, int newIndex) : base(list)
        {
            _newIndex = newIndex;
        }

        protected override void Change() => List.SelectAt(_newIndex);

        protected override void ChangeBack() { }
    }
}
