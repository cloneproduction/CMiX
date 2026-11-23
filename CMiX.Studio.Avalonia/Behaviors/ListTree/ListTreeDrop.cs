// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // The drop of a drag of the new tree: one undo step that selects the moved item, the open states of before the drag, and a flash.
    internal sealed class ListTreeDrop : ListTreeBehavior
    {
        public Dropping Start(ProjectTree project, TreeNode source, ListTreeSlot slot, ListTreeOpenState? openBefore)
        {
            var drop = project.DropAsync(source, slot.Target, slot.Position);
            var dropped = DroppedNodeOf(project, source);
            if (dropped != null)
                openBefore?.RestoreOn(dropped);
            return new Dropping(drop, dropped);
        }

        // The session ends between the start and the show, so the flash shows on the rows of the live tree.
        public void Show(Dropping dropping)
        {
            if (dropping.Dropped is { } dropped && Part<ListTreeRowReveal>().ShownRowOf(dropped) is { } row)
                row.Flash();
            _ = ObserveAsync(dropping.Drop);
        }

        // The drop selects the moved item. A blocked drop leaves the source selected.
        private static TreeNode? DroppedNodeOf(ProjectTree project, TreeNode source) =>
            project.SelectedNode is { } selected && ReferenceEquals(selected.Item, source.Item) ? selected : null;

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

        public sealed record Dropping(Task Drop, TreeNode? Dropped);
    }
}
