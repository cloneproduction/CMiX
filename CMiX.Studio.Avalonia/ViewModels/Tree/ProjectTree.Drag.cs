// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    public sealed partial class ProjectTree
    {
        private bool isDragging;

        public bool IsDragging
        {
            get => isDragging;
            private set
            {
                if (isDragging == value)
                    return;

                isDragging = value;
                OnPropertyChanged();
                FocusSearchCommand.NotifyCanExecuteChanged();
                ClearSearchCommand.NotifyCanExecuteChanged();
            }
        }

        public TreeDropService DropService => dropService;

        public bool CanDrag(TreeNode node) => dragState.CanDrag(node);

        public bool CanDrop(TreeNode source, TreeNode target, DropPosition position) => dragState.CanDrop(source, target, position);

        public void BeginDrag(TreeNode source)
        {
            IsDragging = true;
            dragState.MarkRows(root, source);
        }

        public void EndDrag()
        {
            IsDragging = false;
            dragState.ClearRows();

            // A drop can bring a match to a new place: its parents open now that the drag is over.
            if (search.Matches.Count > 0)
                SyncSearchRows();
        }

        public DropPlace? Locate(TreeNode source, TreeNode hovered, double fraction) =>
            dropService.Locate(source, hovered, fraction);

        // The move and the selection of the moved item are one undo step.
        public async Task DropAsync(TreeNode source, TreeNode target, DropPosition position)
        {
            DropMove move;
            undoManager.BeginCapture();
            try
            {
                // Read before the drop: the drop removes the source node, and NodeRemoved passes the inspector up.
                var inspected = ReferenceEquals(inspectedNode, source);
                move = drop.Move(root, source, target, position);
                if (move.MovedNode is { } moved)
                {
                    Select(moved, SelectionOrigin.Program);
                    if (inspected)
                        Inspect(moved);
                }
            }
            finally
            {
                undoManager.EndCapture();
            }

            await drop.NoticeBlockAsync(source, target, move.Outcome);
        }

        // The moves of the rows of a block and the selection of the moved rows are one undo step. The pressed row stays the selected node.
        public async Task DropBlockAsync(IReadOnlyList<TreeNode> nodes, TreeNode pressed, TreeNode target, DropPosition position)
        {
            DropBlockMove move;
            var pressedItem = pressed.Item;
            var reverted = false;
            undoManager.BeginCapture();
            try
            {
                // Read before the drop: the drop removes the source nodes, and NodeRemoved passes the inspector up.
                var inspected = nodes.FirstOrDefault(source => ReferenceEquals(inspectedNode, source))?.Item;
                move = drop.MoveBlock(root, nodes, target, position);
                if (move.Refusal != null && move.MovedNodes.Count > 0)
                {
                    // A block moves whole or not at all: the rows that moved before the refusal go back.
                    reverted = true;
                    undoManager.RevertCapture();
                }
                else if (move.MovedNodes.Count > 0)
                {
                    SelectBlock(MovedNodeOf(move, pressedItem) ?? move.MovedNodes[^1], move.MovedNodes);
                    if (inspected != null && MovedNodeOf(move, inspected) is { } moved)
                        Inspect(moved);
                }
            }
            finally
            {
                if (!reverted)
                    undoManager.EndCapture();
            }

            if (move.Refusal is { } refusal)
                await drop.NoticeBlockAsync(refusal.Source, refusal.Target, refusal.Outcome);
        }

        private static TreeNode? MovedNodeOf(DropBlockMove move, object? item) =>
            move.MovedNodes.FirstOrDefault(node => ReferenceEquals(node.Item, item));
    }
}
