// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    // How one row looks while a node is dragged (see TreeNode.ShowDragLook).
    public sealed record DragLook(bool IsSource, bool IsCandidate, bool IsNeighbor, bool HasTargetBelow, string? BlockReason)
    {
        public static readonly DragLook None = new(false, false, false, false, null);
    }
}
