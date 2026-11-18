// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia.Controls;
using Avalonia.Input;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.UI.Behaviors;

namespace CMiX.Studio.Avalonia.Views
{
    // A row is dragged with the pointer (see TreeRowDrag), so a system drag here is a file drag.
    internal sealed class TreeFileDropInput
    {
        private readonly TreeViewContext context;
        private readonly TreeDropMarker marker;

        private DropFiles? dropFiles;

        internal TreeFileDropInput(TreeViewContext context, TreeDropMarker marker)
        {
            this.context = context;
            this.marker = marker;
        }

        internal void Attach(TreeView treeView)
        {
            DragDrop.SetAllowDrop(treeView, true);
            treeView.AddHandler(DragDrop.DragEnterEvent, OnDragEnter);
            treeView.AddHandler(DragDrop.DragOverEvent, OnDragOver);
            treeView.AddHandler(DragDrop.DragLeaveEvent, OnDragLeave);
            treeView.AddHandler(DragDrop.DropEvent, OnDrop);
        }

        private static bool IsFileDrag(DragEventArgs e) => e.Data.Contains(DataFormats.Files);

        // DragOver reads no disk.
        private DropFiles? FilesOf(DragEventArgs e)
        {
            if (context.Tree is not { } tree)
                return null;

            if (dropFiles == null || !ReferenceEquals(dropFiles.Data, e.Data))
                dropFiles = DropFiles.Classify(e.Data, FileDropMode.FilesAndFolders, tree.AcceptsPath);
            return dropFiles;
        }

        private void TrackFilesPointer(DragEventArgs e) =>
            context.AutoScroller.Track(context.Scroller is { } scroller ? e.GetPosition(scroller).Y : null);

        // A leave of one row comes with the enter of the next row.
        private void OnDragEnter(object? sender, DragEventArgs e)
        {
            if (!IsFileDrag(e))
                return;

            FilesOf(e);
            TrackFilesPointer(e);
        }

        // Over a child of a layer, or under the rows, nothing is marked.
        private void OnDragOver(object? sender, DragEventArgs e)
        {
            if (!IsFileDrag(e))
                return;

            TrackFilesPointer(e);
            e.Handled = true;
            e.DragEffects = DragDropEffects.None;
            marker.Mark(null, null);

            if (context.Tree is not { } tree || FilesOf(e) is not { IsAccepted: true })
                return;

            e.DragEffects = DragDropEffects.Copy;
            var hoveredItem = TreeRows.ItemOf(e.Source);
            if (hoveredItem?.DataContext is TreeNode hovered && tree.IsPathTarget(hovered))
                marker.Mark(hoveredItem, DropPosition.Inside);
        }

        private void OnDragLeave(object? sender, DragEventArgs e)
        {
            context.ExistingAutoScroller?.StopNow();
            marker.Mark(null, null);
            // A leave of a row inside the tree keeps the record.
            if (DropFiles.LeavesControl(context.TreeView, e))
                dropFiles = null;
        }

        private void OnDrop(object? sender, DragEventArgs e)
        {
            if (!IsFileDrag(e))
                return;

            context.ExistingAutoScroller?.StopNow();
            e.Handled = true;
            marker.Mark(null, null);
            var files = FilesOf(e);
            dropFiles = null;
            if (context.Tree is { } tree && files != null)
                tree.DropPaths(TreeRows.ItemOf(e.Source)?.DataContext as TreeNode, files.Accepted);
        }
    }
}
