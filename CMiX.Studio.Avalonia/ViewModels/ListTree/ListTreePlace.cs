// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // A row spans the full width of the panel, so the place has no x and no width.
    public sealed record ListTreePlace(TreeNode Node, double Y, double RowHeight, double AreaHeight, int Depth, double Indent, bool CanToggle);
}
