// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;
using CMiX.UI.Behaviors;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // Files from disk over the rows of the new tree, with the rules of TreeFileDropRules, as TreeFileDropInput gives them to the old tree.
    internal sealed class ListTreeFileDropInput : ListTreeBehavior
    {
        private ProjectTree? tree;
        private DropFiles? dropFiles;

        private Control Host => Context.Scroller;

        private PanelListTreeRows Rows => Context.Rows;

        protected override void OnWatch(ProjectTree attached)
        {
            tree = attached;
            DragDrop.SetAllowDrop(Host, true);
            Host.AddHandler(DragDrop.DragEnterEvent, OnDragEnter);
            Host.AddHandler(DragDrop.DragOverEvent, OnDragOver);
            Host.AddHandler(DragDrop.DragLeaveEvent, OnDragLeave);
            Host.AddHandler(DragDrop.DropEvent, OnDrop);
        }

        protected override void OnUnwatch()
        {
            Host.RemoveHandler(DragDrop.DragEnterEvent, OnDragEnter);
            Host.RemoveHandler(DragDrop.DragOverEvent, OnDragOver);
            Host.RemoveHandler(DragDrop.DragLeaveEvent, OnDragLeave);
            Host.RemoveHandler(DragDrop.DropEvent, OnDrop);
            Rows.Surfaces.MarkDropTarget(null);
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
            Rows.Surfaces.MarkDropTarget(null);
            Part<ListTreeAutoScroll>().Track(e.GetPosition(Host).Y);
            if (tree is not { } shown || FilesOf(e) is not { IsAccepted: true })
                return;

            e.DragEffects = DragDropEffects.Copy;
            if (NodeOf(e.Source) is { } hovered && shown.IsPathTarget(hovered))
                Rows.Surfaces.MarkDropTarget(hovered);
        }

        // A leave of one row inside the tree keeps the record.
        private void OnDragLeave(object? sender, DragEventArgs e)
        {
            Rows.Surfaces.MarkDropTarget(null);
            if (!DropFiles.LeavesControl(Host, e))
                return;

            dropFiles = null;
            Part<ListTreeAutoScroll>().Stop();
        }

        private void OnDrop(object? sender, DragEventArgs e)
        {
            if (!IsFileDrag(e))
                return;

            e.Handled = true;
            Rows.Surfaces.MarkDropTarget(null);
            Part<ListTreeAutoScroll>().Stop();
            var files = FilesOf(e);
            dropFiles = null;
            if (tree is { } shown && files != null)
                shown.DropPaths(NodeOf(e.Source), files.Accepted);
        }

        private static TreeNode? NodeOf(object? source) =>
            (source as Visual)?.FindAncestorOfType<RowListTree>(includeSelf: true)?.DataContext as TreeNode;
    }
}
