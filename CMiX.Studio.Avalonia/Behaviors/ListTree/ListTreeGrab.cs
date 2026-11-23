// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.Studio.Avalonia.Views;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // The grab of a row of the new tree (Tree.Grab), with the keys and the rules of TreeGrab: the drag starts with no pressed button.
    // The keys step through the candidates of the drag. A pointer move past TreeGrab.MouseTakeOver gives the mark to the pointer.
    internal sealed class ListTreeGrab : ListTreeBehavior
    {
        // While a grab runs, the view shows the accent frame (grabFrame).
        public const string GrabbingClass = "grabbing";

        private ProjectTree? tree;
        private bool isMarkFromKey;
        private Point keyMovePoint;
        private Point lastPointer;

        // A right click asks for a menu on the release.
        private bool menuBlocked;

        public bool IsGrabbing => Input.IsAttached && Session.IsRunning;

        // Until the release of the last grab click, no press starts a pointer drag.
        public bool HoldsThePointer => Input.IsAttached;

        internal ListTreeDrag? Drag => IsGrabbing ? Session.Drag : null;

        private ListTreeDragSession Session => Part<ListTreeDragSession>();

        private ListTreeGrabInput Input => Part<ListTreeGrabInput>();

        private PanelListTreeRows Rows => Context.Rows;

        protected override void OnWatch(ProjectTree attached)
        {
            tree = attached;
            Session.Ended += OnSessionEnded;
        }

        protected override void OnUnwatch()
        {
            Detach();
            Session.Ended -= OnSessionEnded;
            tree = null;
        }

        public bool TakeMenuBlock()
        {
            var blocked = menuBlocked || IsGrabbing;
            menuBlocked = false;
            return blocked;
        }

        public void Start(TreeNode node)
        {
            if (Input.IsAttached || Session.IsRunning || tree is not { } project || !project.CanDrag(node)
                || TopLevel.GetTopLevel(Context.Owner) is not { } window || PlaceOf(node) is not { } place)
                return;

            var pointer = PointerInRows();
            Session.Begin(node, PressAt(pointer, place));
            if (!Session.IsRunning)
                return;

            isMarkFromKey = true;
            keyMovePoint = pointer;
            lastPointer = pointer;
            menuBlocked = false;
            Input.Attach(window);
            ShowAt(StartIndex());
            ShowGrabbing();
        }

        // Every row goes back to its place, and nothing moves.
        public void Cancel()
        {
            if (Input.IsAttached)
                Session.Cancel();
        }

        // Every handler goes, also when a release is still awaited.
        public void Detach()
        {
            Cancel();
            Input.Detach();
        }

        public void StepUp() => Step(-1);

        public void StepDown() => Step(1);

        // At the start place a confirm ends the grab with no move, as in Blender.
        public void Confirm()
        {
            if (Session.HasDropPlace)
                Session.Drop();
            else if (isMarkFromKey && Session.Drag?.IsAtItsOwnPlace() == true)
                Cancel();
        }

        // The rows can move between the last move and the click, so the drop reads the shown place.
        public void Click(PointerPointProperties properties)
        {
            menuBlocked = true;
            if (properties.IsRightButtonPressed)
                Cancel();
            else if (properties.IsLeftButtonPressed && Session.HasDropPlace)
                Session.Drop();
        }

        public void TrackPointer(Point inRows, Point inScroller)
        {
            lastPointer = inRows;
            if (isMarkFromKey && ((Vector)(inRows - keyMovePoint)).Length < TreeGrab.MouseTakeOver)
                return;

            isMarkFromKey = false;
            // Out of the tree the mark stays, so a short move out and back keeps the place.
            if (!new Rect(Context.Scroller.Bounds.Size).Contains(inScroller))
                return;

            Session.Track(new ListTreePoint(inRows.X, inRows.Y));
            Part<ListTreeAutoScroll>().Track(inScroller.Y);
        }

        private ListTreePlace? PlaceOf(TreeNode node)
        {
            Rows.ApplyTreeChanges();
            return Rows.RowOf(node)?.Place;
        }

        private Point PointerInRows()
        {
            var last = Part<ListTreeLastPointer>().Last;
            return Context.Scroller.TranslatePoint(last, Rows) ?? last;
        }

        // The press is at the middle of the row, so the row follows the pointer to the place under it.
        private static ListTreePress PressAt(Point pointer, ListTreePlace place) =>
            new(new ListTreePoint(pointer.X, place.Y + place.RowHeight / 2), place.RowHeight / 2, place.Depth);

        private int StartIndex() =>
            Session.Drag is { } drag ? IndexOf(drag, candidate => ReferenceEquals(candidate.Slot.Target, drag.Node)) : -1;

        private static int IndexOf(ListTreeDrag drag, Func<ListTreeCandidate, bool> match)
        {
            for (var i = 0; i < drag.Candidates.Count; i++)
                if (match(drag.Candidates[i]))
                    return i;

            return -1;
        }

        private int ShownIndex(ListTreeDrag drag)
        {
            var index = drag.Look is { } look ? IndexOf(drag, candidate => candidate.Slot == look.Candidate.Slot) : -1;
            return index >= 0 ? index : StartIndex();
        }

        // The candidates are in tree order, and a step stops at the first and at the last.
        private void Step(int direction)
        {
            if (Session.Drag is not { Candidates.Count: > 0 } drag)
                return;

            var index = ShownIndex(drag);
            var last = drag.Candidates.Count - 1;
            isMarkFromKey = true;
            keyMovePoint = lastPointer;
            ShowAt(direction > 0 ? Math.Min(index + 1, last) : index < 0 ? last : Math.Max(index - 1, 0));
        }

        private void ShowAt(int index)
        {
            if (Session.Drag is not { } drag || index < 0 || index >= drag.Candidates.Count)
                return;

            var candidate = drag.Candidates[index];
            Session.ShowCandidate(candidate);
            Part<ListTreeRowReveal>().RevealPlace(candidate.Place);
        }

        // A drop, a cancel and a cancel from the tree all end the session.
        private void OnSessionEnded(object? sender, EventArgs e)
        {
            if (!Input.IsAttached)
                return;

            isMarkFromKey = false;
            ShowGrabbing();
            Input.DetachAfterRelease();
        }

        private void ShowGrabbing()
        {
            Context.Owner.Classes.Set(GrabbingClass, IsGrabbing);
            Part<ListTreeHints>().Update();
        }
    }
}
