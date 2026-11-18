// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    public sealed record DragAim(TreeViewItem? Hovered, TreeViewItem? Item, TreeNode? Target, DropPosition? Position)
    {
        public static DragAim NoDropOver(TreeViewItem? hovered) => new(hovered, null, null, null);
    }
}
