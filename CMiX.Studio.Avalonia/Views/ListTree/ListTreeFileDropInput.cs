// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.UI.Behaviors;

namespace CMiX.Studio.Avalonia.Views
{
    // Files from disk over the rows of the new tree, with the rules of TreeFileDropRules, as TreeFileDropInput gives them to the old tree.
    internal sealed class ListTreeFileDropInput
    {
        private readonly Control host;
        private readonly PanelListTreeRows rows;

        private ProjectTree? tree;
        private DropFiles? dropFiles;

        public ListTreeFileDropInput(Control host, PanelListTreeRows rows)
        {
            this.host = host;
            this.rows = rows;
        }

        public void Attach(ProjectTree attached)
        {
            tree = attached;
            DragDrop.SetAllowDrop(host, true);
            host.AddHandler(DragDrop.DragEnterEvent, OnDragEnter);
            host.AddHandler(DragDrop.DragOverEvent, OnDragOver);
            host.AddHandler(DragDrop.DragLeaveEvent, OnDragLeave);
            host.AddHandler(DragDrop.DropEvent, OnDrop);
        }

        public void Detach()
        {
            host.RemoveHandler(DragDrop.DragEnterEvent, OnDragEnter);
            host.RemoveHandler(DragDrop.DragOverEvent, OnDragOver);
            host.RemoveHandler(DragDrop.DragLeaveEvent, OnDragLeave);
            host.RemoveHandler(DragDrop.DropEvent, OnDrop);
            rows.MarkDropTarget(null);
            dropFiles = null;
            tree = null;
        }

        private static bool IsFileDrag(DragEventArgs e) => e.Data.Contains(DataFormats.Files);

        // DragOver reads no disk.
        private DropFiles? FilesOf(DragEventArgs e)
        {
            if (tree is not { } shown)
                return null;

            if (dropFiles == null || !ReferenceEquals(dropFiles.Data, e.Data))
                dropFiles = DropFiles.Classify(e.Data, FileDropMode.FilesAndFolders, shown.AcceptsPath);
            return dropFiles;
        }

        private void OnDragEnter(object? sender, DragEventArgs e)
        {
            if (IsFileDrag(e))
                FilesOf(e);
        }

        // Over a child of a layer, or under the rows, nothing is marked.
        private void OnDragOver(object? sender, DragEventArgs e)
        {
            if (!IsFileDrag(e))
                return;

            e.Handled = true;
            e.DragEffects = DragDropEffects.None;
            rows.MarkDropTarget(null);
            if (tree is not { } shown || FilesOf(e) is not { IsAccepted: true })
                return;

            e.DragEffects = DragDropEffects.Copy;
            if (NodeOf(e.Source) is { } hovered && shown.IsPathTarget(hovered))
                rows.MarkDropTarget(hovered);
        }

        // A leave of one row inside the tree keeps the record.
        private void OnDragLeave(object? sender, DragEventArgs e)
        {
            rows.MarkDropTarget(null);
            if (DropFiles.LeavesControl(host, e))
                dropFiles = null;
        }

        private void OnDrop(object? sender, DragEventArgs e)
        {
            if (!IsFileDrag(e))
                return;

            e.Handled = true;
            rows.MarkDropTarget(null);
            var files = FilesOf(e);
            dropFiles = null;
            if (tree is { } shown && files != null)
                shown.DropPaths(NodeOf(e.Source), files.Accepted);
        }

        private static TreeNode? NodeOf(object? source) =>
            (source as Visual)?.FindAncestorOfType<RowListTree>(includeSelf: true)?.DataContext as TreeNode;
    }
}
