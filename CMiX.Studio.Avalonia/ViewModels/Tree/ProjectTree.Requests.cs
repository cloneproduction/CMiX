// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    public sealed partial class ProjectTree
    {
        // Only the active view handles these requests (see IsActiveView).
        public event EventHandler? CollapseOthersRequested;

        public event EventHandler<TreeNode>? RevealRequested;

        public event EventHandler<TreeNode>? GrabRequested;

        public event EventHandler? AddRequested;

        public event EventHandler? MenuRequested;

        public void FocusTree() => TreeFocusRequested?.Invoke(this, EventArgs.Empty);

        public event EventHandler? TreeFocusRequested;

        public event EventHandler<TreeNode>? RenameRequested;

        public event EventHandler? SearchFocusRequested;
    }
}
