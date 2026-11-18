// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    // Each view has its own hooks, so two views can show the same tree.
    public sealed class TreeViewHooks
    {
        public Func<bool> IsPointerOverTree { get; init; } = () => false;

        public Func<bool> IsTreeFocused { get; init; } = () => false;

        public Func<bool> IsTextBoxFocused { get; init; } = () => false;

        public Func<bool> IsFocusInOtherPanel { get; init; } = () => false;

        public Func<bool> IsPanelFocused { get; init; } = () => false;

        public Action ToggleDragRow { get; init; } = () => { };

        public Action CancelDrag { get; init; } = () => { };

        public Action<TreeNode?, Func<IReadOnlyList<TreeNode>>> CloseRowsInView { get; init; } = (_, close) => close();
    }
}
