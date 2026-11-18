// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Threading.Tasks;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    public sealed partial class ProjectTree
    {
        private bool isDragging;

        // The search is off while a drag runs (see FocusSearch): the view binds the search box to it.
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
            if (searchMatches.Count > 0)
                SyncSearchRows();
        }

        public DropPlace? Locate(TreeNode source, TreeNode hovered, double fraction) =>
            dropService.Locate(source, hovered, fraction);

        // The move and the selection of the moved item are one undo step. The moved node keeps the inspector.
        public async Task DropAsync(TreeNode source, TreeNode target, DropPosition position)
        {
            DropMove move;
            undoManager.BeginCapture();
            try
            {
                // Read before the drop: the drop removes the source node, which passes the inspector to the parent (NodeRemoved).
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
    }
}
