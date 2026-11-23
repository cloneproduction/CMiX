// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The open state of a card and of every card under it, by model: a move to another list builds new nodes for the same models.
    public sealed record ListTreeOpenState(object Model, bool IsExpanded, IReadOnlyList<ListTreeOpenState> Children)
    {
        public static ListTreeOpenState Of(TreeNode node) => new(ListTreeRowKey.ModelOf(node), node.IsExpanded, node.Children.Select(Of).ToArray());

        public void RestoreOn(TreeNode node)
        {
            node.IsExpanded = IsExpanded;
            foreach (var child in node.Children)
                if (Children.FirstOrDefault(state => ReferenceEquals(state.Model, ListTreeRowKey.ModelOf(child))) is { } state)
                    state.RestoreOn(child);
        }
    }
}
