// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // One drag of a row. A starter such as TreeRowDrag gives it the points, so it reads no pointer event.
    public sealed class TreeDragSession
    {
        // The system cursor for a blocked drop does not fit the look of the app.
        public const StandardCursorType MoveCursorType = StandardCursorType.DragMove;

        public static readonly TimeSpan DefaultRestDelay = TimeSpan.FromSeconds(0.6);

        private readonly TreeViewContext context;
        private readonly TreeDropMarker marker;
        private readonly TreeScrollAnchor scrollAnchor;
        private readonly TreeDragGhost ghost = new();

        private TreeNode? source;
        private Cursor? cursorBefore;
        private Control? blockTip;

        private TreeViewItem? hoveredItem;
        private TreeNode? hoveredNode;

        private TreeNode? resting;
        private DispatcherTimer? restTimer;

        private TreeNode? closedByKey;

        private readonly List<TreeNode> openedRows = new();

        internal TreeDragSession(TreeViewContext context, TreeDropMarker marker, TreeScrollAnchor scrollAnchor)
        {
            this.context = context;
            this.marker = marker;
            this.scrollAnchor = scrollAnchor;
        }

        public bool IsRunning => source != null;

        public TreeNode? Source => source;

        public bool IsDropping { get; private set; }

        public Cursor MoveCursor { get; } = new(MoveCursorType);

        public DropPlace? Slot =>
            marker.Marked is { DataContext: TreeNode target } && marker.MarkedPosition is { } position ? new DropPlace(target, position) : null;

        internal TreeDragGhost Ghost => ghost;

        internal IReadOnlyList<TreeNode> OpenedRows => openedRows;

        internal TimeSpan RestDelay { get; set; } = DefaultRestDelay;

        internal TreeNode? RestingNode => resting;
        internal TimeSpan? RestTimerInterval => restTimer is { IsEnabled: true } timer ? timer.Interval : null;

        internal void Start(TreeNode node, Point position)
        {
            if (source != null)
                return;

            source = node;
            openedRows.Clear();

            // Take the picture before the rows change their look for the drag.
            ShowGhost(node, position);
            context.Tree?.BeginDrag(node);
            cursorBefore = context.TreeView.Cursor;
            context.TreeView.Cursor = MoveCursor;
        }

        private void ShowGhost(TreeNode node, Point position)
        {
            var item = context.RowIndex.FindItem(node);
            if (item != null && TreeRows.RowBorderOf(item) is Border row)
                ghost.Show(row, node.IconKey, node.Header, context.TreeView, position);
        }

        // The label moves after the mark, so it can keep clear of the drop line.
        internal void Track(Point position)
        {
            if (source == null)
                return;

            Aim(position);
            ShowBlockReason(hoveredItem);
            TrackRest(hoveredItem);
            ghost.Ghost.IsVisible = true;
            ghost.Move(context.TreeView, position, DropLineTop());
            context.AutoScroller.Track(context.Scroller is { } view ? context.TreeView.TranslatePoint(position, view)?.Y : null);
        }

        // A starter calls this when the drop comes at a point that Track did not see.
        internal void Aim(Point position)
        {
            if (source == null)
                return;

            var aim = ReadAim(position);
            hoveredItem = aim.Hovered;
            if (aim.Item != null && aim.Target != null && aim.Position != null && CanDrop(aim.Target, aim.Position.Value))
                marker.Mark(aim.Item, aim.Position);
            else
                marker.Mark(null, null);
        }

        // The label hides because the pointer is not at the place that the key gives.
        internal void AimAt(TreeNode target, DropPosition position)
        {
            if (source == null)
                return;

            StopRest();
            var item = context.RowIndex.FindItem(target);
            hoveredItem = item;
            hoveredNode = item != null ? target : null;
            closedByKey = null;
            if (item != null && CanDrop(target, position))
                marker.Mark(item, position);
            else
                marker.Mark(null, null);

            ShowBlockReason(null);
            ghost.Ghost.IsVisible = false;
        }

        internal void ScrollByWheel(int steps)
        {
            if (source != null)
                context.AutoScroller.ScrollByWheel(steps);
        }

        internal void End(TreeDragEnd end)
        {
            if (source is not { } node)
                return;

            if (end == TreeDragEnd.Drop)
                _ = DropAsync(node);
            else
                Finish();
        }

        private async Task DropAsync(TreeNode node)
        {
            try
            {
                var drop = StartDrop(node);
                Finish();
                if (drop != null)
                    await drop;
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"TreeViewProject: the drop failed. {exception}");
            }
            finally
            {
                // The drag also ends when the drop fails before Finish.
                if (ReferenceEquals(source, node))
                    Finish();
            }
        }

        // The move must finish before the first await of ProjectTree.DropAsync,
        // so the first row in view stays where it is.
        private Task? StartDrop(TreeNode node)
        {
            if (context.Tree is not { } project || Slot is not { } slot || !CanDrop(slot.Target, slot.Position))
                return null;

            var anchor = scrollAnchor.CaptureAnchor(node);
            IsDropping = true;
            try
            {
                return project.DropAsync(node, slot.Target, slot.Position);
            }
            finally
            {
                IsDropping = false;
                // The target and its parents stay open to show where the node went.
                scrollAnchor.KeepStill(anchor, () => CloseOpenedRows(slot.Target));
                Dispatcher.UIThread.Post(() => scrollAnchor.RestoreAnchor(anchor), DispatcherPriority.Loaded);
            }
        }

        private void Finish()
        {
            if (source == null)
                return;

            // A cancel or a blocked drop did not close the opened rows.
            if (openedRows.Count > 0)
            {
                scrollAnchor.KeepStill(scrollAnchor.CaptureAnchor(source), () => CloseOpenedRows(null));
            }

            source = null;
            StopRest();
            hoveredItem = null;
            hoveredNode = null;
            closedByKey = null;
            ShowBlockReason(null);
            context.Tree?.EndDrag();
            context.AutoScroller.StopNow();
            marker.Mark(null, null);
            ghost.Hide();
            ghost.Ghost.IsVisible = true;
            context.TreeView.Cursor = cursorBefore;
            cursorBefore = null;
        }

        // A line below a row starts at the bottom of its border, not of its item.
        private double? DropLineTop()
        {
            if (marker.Line is not { } line)
                return null;

            if (!line.Below)
                return line.Item.TranslatePoint(default, context.TreeView)?.Y;

            return TreeRows.RowBorderOf(line.Item) is { } row ? row.TranslatePoint(new Point(0, row.Bounds.Height), context.TreeView)?.Y : null;
        }

        private bool CanDrop(TreeNode target, DropPosition position) =>
            source != null && context.Tree is { } project && project.CanDrop(source, target, position);

        // A point between two rows or left of a row keeps the last drop place.
        private DragAim ReadAim(Point position)
        {
            var hoveredItem = TreeRows.ItemOf(context.TreeView.InputHitTest(position));
            if (source == null || context.Tree is not { } project)
                return DragAim.NoDropOver(hoveredItem);

            if (hoveredItem?.DataContext is not TreeNode hovered)
                return IsAmongTheRows(position) ? MarkedAim(hoveredItem) : DragAim.NoDropOver(hoveredItem);

            var header = TreeRows.RowBorderOf(hoveredItem);
            var fraction = header == null || header.Bounds.Height <= 0
                ? 0.5
                : (context.TreeView.TranslatePoint(position, header)?.Y ?? 0) / header.Bounds.Height;

            if (fraction < 0 || fraction > 1)
                return MarkedAim(hoveredItem);

            if (project.Locate(source, hovered, fraction) is not { } place)
                return DragAim.NoDropOver(hoveredItem);

            return new DragAim(hoveredItem, TreeRows.ItemOfNode(hoveredItem, place.Target), place.Target, place.Position);
        }

        private DragAim MarkedAim(TreeViewItem? hovered) =>
            marker.Marked is { DataContext: TreeNode node } marked && marker.MarkedPosition is { } position
                ? new DragAim(hovered, marked, node, position)
                : DragAim.NoDropOver(hovered);

        // The room under the last row is not among the rows.
        private bool IsAmongTheRows(Point position) =>
            context.TreeView.ItemsPanelRoot is { } panel
            && context.TreeView.TranslatePoint(position, panel) is { } point
            && new Rect(panel.Bounds.Size).Contains(point);

        // A row that the key closed does not open on a rest until the pointer leaves it.
        private void TrackRest(TreeViewItem? hovered)
        {
            hoveredNode = hovered?.DataContext as TreeNode;
            if (!ReferenceEquals(hoveredNode, closedByKey))
                closedByKey = null;

            var node = hoveredNode != null && OpensOnRest(hoveredNode) && !ReferenceEquals(hoveredNode, closedByKey) ? hoveredNode : null;
            if (ReferenceEquals(node, resting))
                return;

            StopRest();
            resting = node;
            if (node == null)
                return;

            if (restTimer == null)
            {
                restTimer = new DispatcherTimer();
                restTimer.Tick += (_, _) => OpenRested();
            }

            restTimer.Interval = RestDelay;
            restTimer.Start();
        }

        // A row that can also take the node still opens, to show the places between its children.
        private static bool OpensOnRest(TreeNode node) =>
            node is { IsDragActive: true, HasDropTargetBelow: true, IsExpanded: false, Children.Count: > 0 };

        private void StopRest()
        {
            restTimer?.Stop();
            resting = null;
        }

        // A test calls this to skip the wait.
        internal void OpenRested()
        {
            var node = resting;
            StopRest();
            if (source == null || node == null || !OpensOnRest(node))
                return;

            node.IsExpanded = true;
            openedRows.Add(node);
        }

        // A row that the user opened before the drag stays open.
        internal void ToggleHovered()
        {
            if (source == null || hoveredNode is not { } node)
                return;

            if (OpensOnRest(node))
            {
                StopRest();
                closedByKey = null;
                node.IsExpanded = true;
                openedRows.Add(node);
            }
            else if (node.IsExpanded && openedRows.Remove(node))
            {
                StopRest();
                closedByKey = node;
                node.IsExpanded = false;
            }
        }

        private List<TreeNode> CloseOpenedRows(TreeNode? kept)
        {
            var closed = openedRows.Where(node => kept == null || !kept.IsOrIsUnder(node)).ToList();
            foreach (var node in closed)
                node.IsExpanded = false;

            openedRows.Clear();
            return closed;
        }

        // The drag sends no pointer events to the rows, so the tooltip opens from here.
        private void ShowBlockReason(TreeViewItem? hovered)
        {
            var row = hovered?.DataContext is TreeNode { IsDropBlocked: true } ? TreeRows.RowBorderOf(hovered) : null;
            if (ReferenceEquals(row, blockTip))
                return;

            if (blockTip != null)
                ToolTip.SetIsOpen(blockTip, false);

            blockTip = row;
            if (row != null)
                ToolTip.SetIsOpen(row, true);
        }
    }
}
