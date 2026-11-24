// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // The drop of a drag of the tree: one undo step that selects the moved items, the open states of before the drag, and a flash on each row.
    internal sealed class ListTreeDrop : ListTreeBehavior
    {
        public Dropping Start(ProjectTree project, ListTreeDragNodes nodes, ListTreeSlot slot, IReadOnlyList<ListTreeOpenState> openBefore)
        {
            var items = nodes.Nodes.Select(node => node.Item).ToArray();
            var drop = nodes.IsBlock
                ? project.DropBlockAsync(nodes.Nodes, nodes.Pressed, slot.Target, slot.Position)
                : project.DropAsync(nodes.Pressed, slot.Target, slot.Position);
            var dropped = project.SelectedNodesOf(items);
            foreach (var node in dropped)
                OpenStateOf(openBefore, node)?.RestoreOn(node);
            return new Dropping(drop, dropped);
        }

        // The session ends between the start and the show, so the flash shows on the rows of the live tree.
        public void Show(Dropping dropping)
        {
            foreach (var dropped in dropping.Dropped)
                Part<ListTreeRowReveal>().ShownRowOf(dropped)?.Flash();
            _ = ObserveAsync(dropping.Drop);
        }

        private static ListTreeOpenState? OpenStateOf(IReadOnlyList<ListTreeOpenState> states, TreeNode node) =>
            states.FirstOrDefault(state => ReferenceEquals(state.Model, ListTreeRowKey.ModelOf(node)));

        private static async Task ObserveAsync(Task drop)
        {
            try
            {
                await drop;
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"ListTreeProject: the drop failed. {exception}");
            }
        }

        public sealed record Dropping(Task Drop, IReadOnlyList<TreeNode> Dropped);
    }
}
