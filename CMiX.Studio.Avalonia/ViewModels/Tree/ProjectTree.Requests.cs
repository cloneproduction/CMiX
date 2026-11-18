// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    public sealed partial class ProjectTree
    {
        // The active view handles this request (see IsActiveView): it runs CollapseOthers and keeps the target row in view.
        public event EventHandler? CollapseOthersRequested;

        // The active view handles this request (see IsActiveView): it scrolls the row of the node to the middle of the view.
        public event EventHandler<TreeNode>? RevealRequested;

        // The active view handles this request (see IsActiveView): it starts the grab of the node.
        public event EventHandler<TreeNode>? GrabRequested;

        // The active view handles this request (see IsActiveView): it opens the Add panel.
        public event EventHandler? AddRequested;

        // The active view handles this request (see IsActiveView): it opens the menu.
        public event EventHandler? MenuRequested;

        // Gives the focus back to the rows of the tree, for example after Escape in an empty search box. The active view handles it.
        public void FocusTree() => TreeFocusRequested?.Invoke(this, EventArgs.Empty);

        public event EventHandler? TreeFocusRequested;

        // The active view handles this request (see IsActiveView): it starts the rename of the row of the node.
        public event EventHandler<TreeNode>? RenameRequested;

        public void FocusSearch()
        {
            if (!IsDragging)
                SearchFocusRequested?.Invoke(this, EventArgs.Empty);
        }

        // The search box of the panel of the active view handles this request: it takes the focus and selects its text.
        public event EventHandler? SearchFocusRequested;
    }
}
