// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Input;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // The running drag of a row of the tree: its candidates, the look of the rows, the near card and the note.
    internal sealed class ListTreeDragSession : ListTreeBehavior
    {
        private readonly ListTreePick pick = new(ListTreeDragNumbers.Default);

        private ProjectTree? tree;
        private ListTreeDrag? drag;
        private ListTreeDragScreen? screen;
        private ListTreePoint lastPoint = new(0, 0);
        private IReadOnlyList<ListTreeOpenState> openBefore = Array.Empty<ListTreeOpenState>();
        private ListTreeSlot? keyedSlot;
        private readonly List<TreeNode> openedByDrag = new();

        public event EventHandler? Began;

        public event EventHandler? Ended;

        public bool IsRunning => drag != null;

        public ListTreePick Pick => pick;

        internal ListTreeDrag? Drag => drag;

        // The slot of the row itself moves nothing, so it is no drop place.
        public bool HasDropPlace => drag is { Look: not null } running && !running.IsAtItsOwnPlace();

        private PanelListTreeRows Rows => Context.Rows;

        private ListTreeDragScreen Screen => screen ??= new ListTreeDragScreen(Context.Scroller, Context.DragNote);

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

            drag = ListTreeDrag.Start(dragged, node, press, pick, Rows.Sizes);
            openBefore = drag.Nodes.Nodes.Select(ListTreeOpenState.Of).ToArray();
            dragged.BeginDrag(drag.Nodes.Nodes);
            Screen.ShowMoveCursor();
            Began?.Invoke(this, EventArgs.Empty);
        }

        public void Track(ListTreePoint point)
        {
            lastPoint = point;
            keyedSlot = null;
            var shown = drag?.Look;
            if (drag is not { } dragging || dragging.PickAt(point) is not { } look)
            {
                Rows.ShowLiveTree();
                Rest.Rest(null);
                Screen.ShowNote(null);
                return;
            }

            if (!ReferenceEquals(look, shown))
                Rows.ShowDrag(look);

            var near = ListTreeNearCard.Of(look, dragging.Press, point);
            Rest.Rest(near);
            Screen.ShowNote(NoteOf(look, near));
        }

        public void TrackPointer(Point inScroller)
        {
            Track(Context.ToRows(inScroller));
            Part<ListTreeAutoScroll>().Track(inScroller.Y);
        }

        public void ScrollByWheel(PointerWheelEventArgs e)
        {
            Part<ListTreeAutoScroll>().ScrollByWheel(ListTreeAutoScroll.WheelSteps(e));
            e.Handled = true;
        }

        // The near card of a key waits for the key Tree.Expand: a rest needs the pointer.
        public void ShowCandidate(ListTreeCandidate candidate)
        {
            var shown = drag?.Look;
            if (drag?.PickCandidate(candidate) is not { } look)
                return;

            keyedSlot = candidate.Slot;
            Rest.Rest(null);
            if (!ReferenceEquals(look, shown))
                Rows.ShowDrag(look);

            Screen.ShowNote(NoteOf(look, KeyNearCard()));
        }

        private TreeNode? KeyNearCard() =>
            keyedSlot != null && drag is { Look: { } look } keyed ? ListTreeNearCard.OfKey(look, keyed.Press) : null;

        private string NoteOf(ListTreeDragLook look, TreeNode? near) =>
            ListTreeDragNote.WithNearCard(look, near, tree?.Shortcuts["Tree.Expand"]?.ToString());

        private void OnRested(object? sender, EventArgs e) => OpenNearCard();

        // The timer and the key Tree.Expand open the card. It stays open after the drop and after a cancel.
        public void OpenNearCard()
        {
            var card = Rest.TakeResting() ?? KeyNearCard();
            if (drag == null || card == null)
                return;

            openedByDrag.Add(card);
            SetOpen(card, true);
        }

        // The key Tree.Expand: it opens the near card, else it closes the card of the landing place when this drag opened it.
        public void ToggleNearCard()
        {
            if (Rest.RestingNode != null || KeyNearCard() != null)
            {
                OpenNearCard();
                return;
            }

            if (OpenedCardOfTheLandingPlace() is not { } card)
                return;

            openedByDrag.Remove(card);
            SetOpen(card, false);
        }

        // The innermost card on the path of the holder that this drag opened. A card open before the drag stays open.
        private TreeNode? OpenedCardOfTheLandingPlace()
        {
            for (var node = drag?.Look?.HolderNode; node != null; node = node.Parent)
                if (openedByDrag.Contains(node))
                    return node;
            return null;
        }

        private void SetOpen(TreeNode card, bool isOpen)
        {
            if (drag is not { } running || tree is not { } opened)
                return;

            card.IsExpanded = isOpen;
            Rows.ApplyTreeChanges();
            running.MakeCandidates(ListTreeSnapshot.Of(opened.Roots));
            ShowAgain(running);
        }

        private void ShowAgain(ListTreeDrag made)
        {
            if (keyedSlot is { } slot && made.Candidates.FirstOrDefault(candidate => candidate.Slot == slot) is { } keyed)
                ShowCandidate(keyed);
            else
                Track(lastPoint);
        }

        public void Drop()
        {
            if (!HasDropPlace || drag is not { Look: { } look } ended || tree is not { } project)
                return;

            var dropping = Part<ListTreeDrop>().Start(project, ended.Nodes, look.Candidate.Slot, openBefore);
            Finish();
            Part<ListTreeDrop>().Show(dropping);
        }

        public void DropOrCancel()
        {
            if (HasDropPlace)
                Drop();
            else
                Cancel();
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
            openBefore = Array.Empty<ListTreeOpenState>();
            keyedSlot = null;
            openedByDrag.Clear();
            Rest.Rest(null);
            Part<ListTreeAutoScroll>().Stop();
            Rows.ShowLiveTree();
            Screen.ShowNote(null);
            tree?.EndDrag();
            Screen.RestoreCursor();
            Ended?.Invoke(this, EventArgs.Empty);
        }
    }
}
