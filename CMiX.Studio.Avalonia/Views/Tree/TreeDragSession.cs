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
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // One drag of a row of the project tree, from its start to its end: the dragged node, its label at the pointer, the mark of the
    // drop, the rows that the drag opened, and the drop.
    // The session does not know what started the drag and it reads no pointer event. A starter gives it the points (see TreeRowDrag
    // for the drag with a pressed button). So another starter can use the same drag.
    // One session serves every drag of the tree. Only one drag runs at a time.
    public sealed class TreeDragSession
    {
        // The cursor of a row drag. It stays the same over a blocked row: the frame of the row tells, not the cursor.
        // The system cursor for a drop that is not possible does not fit the look of the app.
        public const StandardCursorType MoveCursorType = StandardCursorType.DragMove;

        // A collapsed row with a place for the node under it opens when the pointer rests on it this long (see OpensOnRest).
        public static readonly TimeSpan DefaultRestDelay = TimeSpan.FromSeconds(0.6);

        private readonly TreeView treeView;
        private readonly TreeViewContext context;
        private readonly TreeDropMarker marker;
        private readonly TreeScrollAnchor scrollAnchor;
        private readonly TreeDragGhost ghost = new();

        private TreeNode? source;
        private Cursor? cursorBefore;
        private Control? blockTip;

        // The row under the last tracked point.
        private TreeViewItem? hoveredItem;
        private TreeNode? hoveredNode;

        // The row that the pointer rests on, and the timer that opens it.
        private TreeNode? resting;
        private DispatcherTimer? restTimer;

        // The row that the key closed last. It does not open on a rest until the pointer leaves it.
        private TreeNode? closedByKey;

        // The rows that this drag opened, in the order that they opened. A new drag starts a new list.
        private readonly List<TreeNode> openedRows = new();

        internal TreeDragSession(TreeView treeView, TreeViewContext context, TreeDropMarker marker, TreeScrollAnchor scrollAnchor)
        {
            this.treeView = treeView;
            this.context = context;
            this.marker = marker;
            this.scrollAnchor = scrollAnchor;
        }

        // True from the start of the drag to its end.
        public bool IsRunning => source != null;

        // The dragged node, or null when no drag runs.
        public TreeNode? Source => source;

        // True while the drop moves the item. The view keeps its scroll where it is (see ProjectTreeView).
        public bool IsDropping { get; private set; }

        // The cursor of the tree during the drag.
        public Cursor MoveCursor { get; } = new(MoveCursorType);

        // The place where a drop puts the node now: the marked row and the position at it. Null when no drop is offered.
        // The drop reads this place only, not the pointer.
        public DropPlace? Slot =>
            marker.Marked is { DataContext: TreeNode target } && marker.MarkedPosition is { } position ? new DropPlace(target, position) : null;

        // The label of the dragged row that follows the pointer. A test reads it.
        internal TreeDragGhost Ghost => ghost;

        internal IReadOnlyList<TreeNode> OpenedRows => openedRows;

        // A test can change the time of the rest.
        internal TimeSpan RestDelay { get; set; } = DefaultRestDelay;

        // The row that waits to open, or null, and the time that the timer waits. A test reads them.
        internal TreeNode? RestingNode => resting;
        internal TimeSpan? RestTimerInterval => restTimer is { IsEnabled: true } timer ? timer.Interval : null;

        // Starts the drag of the node. The position is in the tree view. The tree shows every row that can take the node for as long
        // as the drag goes on.
        internal void Start(TreeNode node, Point position)
        {
            if (source != null)
                return;

            source = node;
            openedRows.Clear();

            // The picture is taken before the rows change their look for the drag, so it is the row as the user saw it.
            ShowGhost(node, position);
            context.Tree?.BeginDrag(node);
            cursorBefore = treeView.Cursor;
            treeView.Cursor = MoveCursor;
        }

        private void ShowGhost(TreeNode node, Point position)
        {
            var item = ItemOf(node);
            if (item != null && TreeRows.RowBorderOf(item) is Border row)
                ghost.Show(row, node.IconKey, node.Header, treeView, position);
        }

        // The pointer is at the position: marks the place of the drop under it and keeps the scroll of the drag going.
        // The label of the drag moves after the mark, so it can keep clear of the line of the drop.
        internal void Track(Point position)
        {
            if (source == null)
                return;

            Aim(position);
            ShowBlockReason(hoveredItem);
            TrackRest(hoveredItem);
            ghost.Ghost.IsVisible = true;
            ghost.Move(treeView, position, DropLineTop());
            context.AutoScroller.Track(context.Scroller is { } view ? treeView.TranslatePoint(position, view)?.Y : null);
        }

        // Sets the slot from the position and nothing more. A starter calls it when the drop comes at a point that Track did not see.
        internal void Aim(Point position)
        {
            if (source == null)
                return;

            var aim = Read(position);
            hoveredItem = aim.Hovered;
            if (aim.Item != null && aim.Target != null && aim.Position != null && CanDrop(aim.Target, aim.Position.Value))
                marker.Mark(aim.Item, aim.Position);
            else
                marker.Mark(null, null);
        }

        // Marks the place that a key gives (see TreeGrab), not the place under the pointer. The row of the place is the hovered row, so
        // Tree.Expand opens or closes it. The label hides: the mark shows the place, and the pointer is not where the place is.
        internal void AimAt(TreeNode target, DropPosition position)
        {
            if (source == null)
                return;

            StopRest();
            var item = ItemOf(target);
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

        private TreeViewItem? ItemOf(TreeNode node) =>
            treeView.GetVisualDescendants().OfType<TreeViewItem>().FirstOrDefault(row => ReferenceEquals(row.DataContext, node));

        // The wheel during the drag: positive steps scroll up (see DragAutoScroller.ScrollByWheel).
        internal void ScrollByWheel(int steps)
        {
            if (source != null)
                context.AutoScroller.ScrollByWheel(steps);
        }

        // Ends the drag. A drop puts the node in the slot first. The drop is one undo step, and it tells the user why a move is
        // blocked (see ProjectTree.DropAsync). The session does not run when this returns.
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
                Debug.WriteLine($"ProjectTreeView: the drop failed. {exception}");
            }
            finally
            {
                // The drag ends also when the drop failed before Finish. A later drag is not touched.
                if (ReferenceEquals(source, node))
                    Finish();
            }
        }

        // Starts the drop in the slot. Null when no drop is offered.
        // A drop changes the number of rows above and under the view. The tree must not jump: the first row in view stays where it
        // is, and the tree does not scroll to the moved row. The move is done before the first await of ProjectTree.DropAsync.
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
                // The rows that the drag opened close in the same anchor, so the first row in view stays where it is. The target and
                // its parents stay open: they show where the node went, also for a drop before or after a child of an opened row.
                var closed = CloseOpenedRows(slot.Target);
                scrollAnchor.RestoreAnchor(anchor);
                scrollAnchor.HoldWhileClosing(anchor, closed);
                Dispatcher.UIThread.Post(() => scrollAnchor.RestoreAnchor(anchor), DispatcherPriority.Loaded);
            }
        }

        // The rows get their look back, the scroll stops, the mark goes and the cursor is back.
        private void Finish()
        {
            if (source == null)
                return;

            // No drop closed them (a cancel, or a drop where no drop is offered): every row that the drag opened closes, and the
            // first row in view stays where it is.
            if (openedRows.Count > 0)
            {
                var anchor = scrollAnchor.CaptureAnchor(source);
                var closed = CloseOpenedRows(null);
                scrollAnchor.RestoreAnchor(anchor);
                scrollAnchor.HoldWhileClosing(anchor, closed);
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
            treeView.Cursor = cursorBefore;
            cursorBefore = null;
        }

        // The top of the line of the drop between two rows, in the coordinates of the tree view, or null when no line shows. A line in the
        // space under a row starts at the bottom of the row. A line at the top of a row starts at the top of its item.
        private double? DropLineTop()
        {
            if (marker.Line is not { } line)
                return null;

            if (!line.Below)
                return line.Item.TranslatePoint(default, treeView)?.Y;

            return TreeRows.RowBorderOf(line.Item) is { } row ? row.TranslatePoint(new Point(0, row.Bounds.Height), treeView)?.Y : null;
        }

        private bool CanDrop(TreeNode target, DropPosition position) =>
            source != null && context.Tree is { } project && project.CanDrop(source, target, position);

        // The row under the point, the item that takes the drop, its node, and the place of the drop. The item that takes the drop
        // is the row under the point or one of its parents (see ProjectTree.Locate). It has no place when no drop is offered here.
        // The point among the rows but on no row (between two rows, or left of a row): the drop stays where it was.
        private DragAim Read(Point position)
        {
            var hoveredItem = TreeRows.ItemOf(treeView.InputHitTest(position));
            if (source == null || context.Tree is not { } project)
                return DragAim.NoDropOver(hoveredItem);

            if (hoveredItem?.DataContext is not TreeNode hovered)
                return IsAmongTheRows(position) ? Marked(hoveredItem) : DragAim.NoDropOver(hoveredItem);

            var header = TreeRows.RowBorderOf(hoveredItem);
            var fraction = header == null || header.Bounds.Height <= 0
                ? 0.5
                : (treeView.TranslatePoint(position, header)?.Y ?? 0) / header.Bounds.Height;

            if (fraction < 0 || fraction > 1)
                return Marked(hoveredItem);

            if (project.Locate(source, hovered, fraction) is not { } place)
                return DragAim.NoDropOver(hoveredItem);

            return new DragAim(hoveredItem, TreeRows.ItemOfNode(hoveredItem, place.Target), place.Target, place.Position);
        }

        // The place that the marker keeps.
        private DragAim Marked(TreeViewItem? hovered) =>
            marker.Marked is { DataContext: TreeNode node } marked && marker.MarkedPosition is { } position
                ? new DragAim(hovered, marked, node, position)
                : DragAim.NoDropOver(hovered);

        // True when the point is in the panel of the rows: on a row, between two rows, or left of a row. The room under the last row is not in it.
        private bool IsAmongTheRows(Point position) =>
            treeView.ItemsPanelRoot is { } panel
            && treeView.TranslatePoint(position, panel) is { } point
            && new Rect(panel.Bounds.Size).Contains(point);

        // The rest starts when the pointer comes on a row that opens on a rest, and goes on while the pointer moves inside that row.
        // Another row, or the end of the drag, stops it. A row that the key closed does not open on a rest until the pointer leaves it.
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

        // A collapsed row with a place for the node under it. The row can take the node itself too, such as a layer for an entity:
        // open, it shows the places between its children.
        private static bool OpensOnRest(TreeNode node) =>
            node is { IsDragActive: true, HasDropTargetBelow: true, IsExpanded: false, Children.Count: > 0 };

        private void StopRest()
        {
            restTimer?.Stop();
            resting = null;
        }

        // Opens the row that the pointer rests on. A test calls it to skip the wait.
        internal void OpenRested()
        {
            var node = resting;
            StopRest();
            if (source == null || node == null || !OpensOnRest(node))
                return;

            node.IsExpanded = true;
            openedRows.Add(node);
        }

        // The key of Tree.Expand during the drag (see ProjectTree.ToggleDragRow). A collapsed row that opens on a rest opens at once.
        // An open row that this drag opened closes again. A row that the user opened before the drag stays open.
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

        // Closes the rows that the drag opened, except the kept node and its parents. The rows that were open before the drag are not
        // in the list, so they never close here.
        // Returns the rows that closed.
        private List<TreeNode> CloseOpenedRows(TreeNode? kept)
        {
            var closed = openedRows.Where(node => kept == null || !kept.IsOrIsUnder(node)).ToList();
            foreach (var node in closed)
                node.IsExpanded = false;

            openedRows.Clear();
            return closed;
        }

        // The drag sends no pointer events to the rows, so the tooltip of a blocked row under the pointer opens from here. Null closes it.
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
