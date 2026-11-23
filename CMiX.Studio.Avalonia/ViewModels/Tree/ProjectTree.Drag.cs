// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

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
    }
}
