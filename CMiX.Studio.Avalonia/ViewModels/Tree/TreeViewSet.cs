// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    public sealed class TreeViewSet : IRowCloser
    {
        private readonly List<TreeViewHooks> views = new();

        public void Attach(TreeViewHooks view)
        {
            if (!views.Contains(view))
                views.Add(view);
        }

        public void Detach(TreeViewHooks view) => views.Remove(view);

        // Focus in the rows, else focus in the side panel, else the pointer, else the first view.
        public TreeViewHooks? ActiveView() =>
            views.Find(view => view.IsTreeFocused()) ?? views.Find(view => view.IsPanelFocused())
            ?? views.Find(view => view.IsPointerOverTree()) ?? (views.Count > 0 ? views[0] : null);

        public bool IsActiveView(TreeViewHooks view) => ReferenceEquals(ActiveView(), view);

        public bool IsPointerOverTree() => views.Exists(view => view.IsPointerOverTree());

        public bool IsTreeFocused() => views.Exists(view => view.IsTreeFocused());

        public bool IsTextBoxFocused() => views.Exists(view => view.IsTextBoxFocused());

        public bool IsFocusInOtherPanel() => views.Count > 0 && views.TrueForAll(view => view.IsFocusInOtherPanel());

        public bool AllowsTreeKey() =>
            !IsTextBoxFocused() && (IsTreeFocused() || IsPointerOverTree() && !IsFocusInOtherPanel());

        public void ToggleDragRow()
        {
            foreach (var view in views.ToArray())
                view.ToggleDragRow();
        }

        public void CancelDrag()
        {
            foreach (var view in views.ToArray())
                view.CancelDrag();
        }

        public void CloseRowsInView(TreeNode? anchor, Func<IReadOnlyList<TreeNode>> close)
        {
            if (ActiveView() is { } view)
                view.CloseRowsInView(anchor, close);
            else
                close();
        }
    }
}
