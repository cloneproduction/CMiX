// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Services;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    public sealed class TreeEdit
    {
        private readonly UndoManager undoManager;
        private readonly IToastService toasts;

        public TreeEdit(UndoManager undoManager, IToastService toasts)
        {
            this.undoManager = undoManager;
            this.toasts = toasts;
        }

        public bool CanMoveUp(TargetPlace place) => place.Index > 0;

        public bool CanMoveDown(TargetPlace place) => place.Index < place.List.ManagerData.Items.Count - 1;

        public void Duplicate(TargetPlace place) => place.List.DuplicateItem(place.Item);

        public void MoveUp(TargetPlace place) => place.List.MoveItemTo(place.Item, place.Index - 1);

        public void MoveDown(TargetPlace place) => place.List.MoveItemTo(place.Item, place.Index + 1);

        public void Remove(TargetPlace place, string name)
        {
            place.List.RemoveItem(place.Item);
            toasts.ShowWithUndo($"Removed {name}");
        }

        public void ToggleHide(TreeNode target)
        {
            if (target.HiddenValue is not { } value)
                return;

            value.Value = !value.Value;
            toasts.ShowWithUndo(value.Value ? $"Hidden: {target.Header}" : $"Shown: {target.Header}");
        }

        public void ShowAll(IEnumerable<TreeNode> roots)
        {
            var hidden = TreeWalk.AllRows(roots).Select(node => node.HiddenValue).Where(value => value is { Value: true }).Distinct().ToList();
            if (hidden.Count == 0)
                return;

            undoManager.BeginCapture();
            try
            {
                foreach (var value in hidden)
                    value!.Value = false;
            }
            finally
            {
                undoManager.EndCapture();
            }

            toasts.ShowWithUndo("All rows shown");
        }
    }
}
