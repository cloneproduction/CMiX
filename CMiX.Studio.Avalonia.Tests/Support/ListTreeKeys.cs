// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Tests.Support
{
    // The key that a snapshot of the live tree gives to a node. The roots are the children of the hidden root.
    internal static class ListTreeKeys
    {
        public static ListTreeRowKey Of(TreeNode node) =>
            ListTreeRowKey.Of(node, node.Parent is { Parent: not null } parent ? Of(parent) : null);
    }
}
