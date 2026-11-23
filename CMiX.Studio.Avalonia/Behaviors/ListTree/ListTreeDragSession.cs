// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia.Controls;
using Avalonia.Input;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // The running drag of a row of the new tree: its candidates, the look of the rows, the near card and the note.
    internal sealed class ListTreeDragSession : ListTreeBehavior
    {
        private readonly ListTreePick pick = new(ListTreeDragNumbers.Default);
        private readonly Cursor moveCursor = new(TreeDragSession.MoveCursorType);

        private ProjectTree? tree;
        private ListTreeDrag? drag;
        private Cursor? cursorBefore;
        private ListTreePoint lastPoint = new(0, 0);
        private ListTreeOpenState? openBefore;

        public event EventHandler? Ended;

        public bool IsRunning => drag != null;

        public ListTreePick Pick => pick;

        internal ListTreeDrag? Drag => drag;

        // The slot of the row itself moves nothing, so it is no drop place.
        public bool HasDropPlace => drag is { Look: not null } running && !running.IsAtItsOwnPlace();

        private PanelListTreeRows Rows => Context.Rows;

        private ScrollViewer Scroller => Context.Scroller;

        private ListTreeDragRest Rest => Part<ListTreeDragRest>();

        protected override void OnWatch(ProjectTree attached)
        {
            tree = attached;
            Rest.Rested += OnRested;
        }

        protected override void OnUnwatch()
        {
            Cancel();
            Rest.Rested -= OnRested;
            tree = null;
        }

        public void Begin(TreeNode node, ListTreePress press)
        {
            if (drag != null || tree is not { } dragged)
                return;

            var service = dragged.DropService;
            drag = new ListTreeDrag(new ListTreeCandidates(new ListTreeSlots(service), service, Rows.Sizes), pick, node, press);
            openBefore = ListTreeOpenState.Of(node);
            dragged.BeginDrag(node);
            cursorBefore = Scroller.Cursor;
            Scroller.Cursor = moveCursor;
            drag.MakeCandidates(ListTreeSnapshot.Of(dragged.Roots));
        }

        public void Track(ListTreePoint point)
        {
            lastPoint = point;
            var shown = drag?.Look;
            if (drag is not { } dragging || dragging.PickAt(point) is not { } look)
            {
                Rows.ShowLiveTree();
                Rest.Rest(null);
                ShowNote(null);
                return;
            }

            if (!ReferenceEquals(look, shown))
                Rows.ShowDrag(look);

            var near = ListTreeNearCard.Of(look, dragging.Press, point);
            Rest.Rest(near);
            ShowNote(NoteOf(look, near));
        }

        private string NoteOf(ListTreeDragLook look, TreeNode? near) =>
            near != null && tree?.Shortcuts["Tree.Expand"] is { } key ? ListTreeDragNote.OfNearCard(look, near, key.ToString()) : ListTreeDragNote.Of(look);

        private void OnRested(object? sender, EventArgs e) => OpenNearCard();

        // The timer and the key Tree.Expand open the card. It stays open after the drop and after a cancel.
        public void OpenNearCard()
        {
            var card = Rest.TakeResting();
            if (drag == null || tree is not { } opened || card == null)
                return;

            card.IsExpanded = true;
            Rows.ApplyTreeChanges();
            drag.MakeCandidates(ListTreeSnapshot.Of(opened.Roots));
            Track(lastPoint);
        }

        public void Drop()
        {
            if (!HasDropPlace || drag is not { Look: { } look } ended || tree is not { } project)
                return;

            var dropping = Part<ListTreeDrop>().Start(project, ended.Node, look.Candidate.Slot, openBefore);
            Finish();
            Part<ListTreeDrop>().Show(dropping);
        }

        // Every row goes back to its place, and nothing moves.
        public void Cancel()
        {
            if (drag != null)
                Finish();
        }

        private void Finish()
        {
            drag = null;
            openBefore = null;
            Rest.Rest(null);
            Part<ListTreeAutoScroll>().Stop();
            Rows.ShowLiveTree();
            ShowNote(null);
            tree?.EndDrag();
            Scroller.Cursor = cursorBefore;
            cursorBefore = null;
            Ended?.Invoke(this, EventArgs.Empty);
        }

        private void ShowNote(string? text)
        {
            Context.DragNote.Text = text;
            Context.DragNote.IsVisible = text != null;
        }
    }
}
