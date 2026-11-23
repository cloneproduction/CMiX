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
        private ListTreePoint keyMovePoint = new(0, 0);
        private ListTreePoint lastPointer = new(0, 0);

        // A right click asks for a menu on the release.
        private bool menuBlocked;

        public bool IsGrabbing => HoldsTheInput && Session.IsRunning;

        internal ListTreeDrag? Drag => IsGrabbing ? Session.Drag : null;

        private ListTreeDragSession Session => Part<ListTreeDragSession>();

        private ListTreeGrabInput Input => Part<ListTreeGrabInput>();

        // Until the release of the last grab click, no press starts a pointer drag.
        private bool HoldsTheInput => Context.InputOwner == ListTreeInputOwner.Grab;

        private PanelListTreeRows Rows => Context.Rows;

        protected override void OnWatch(ProjectTree attached)
        {
            tree = attached;
            Session.Ended += OnSessionEnded;
        }

        protected override void OnUnwatch()
        {
            DetachFromWindow();
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
            if (Context.InputOwner != ListTreeInputOwner.None || Session.IsRunning || tree is not { } project || !project.CanDrag(node)
                || TopLevel.GetTopLevel(Context.Owner) is not { } window || PlaceOf(node) is not { } place)
                return;

            var pointer = Context.ToRows(Part<ListTreeLastPointer>().Last);
            Session.Begin(node, ListTreePress.AtMiddleOf(place, pointer.X));
            if (!Session.IsRunning)
                return;

            isMarkFromKey = true;
            keyMovePoint = pointer;
            lastPointer = pointer;
            menuBlocked = false;
            Input.AttachToWindow(window);
            ShowAt(Session.Drag is { } drag ? ListTreeGrabSteps.StartIndexOf(drag) : -1);
            ShowGrabbing();
        }

        // Every row goes back to its place, and nothing moves.
        public void Cancel()
        {
            if (HoldsTheInput)
                Session.Cancel();
        }

        // Every handler goes, also when a release is still awaited.
        public void DetachFromWindow()
        {
            Cancel();
            Input.DetachFromWindow();
        }

        public void StepUp()
        {
            if (Session.Drag is { Candidates.Count: > 0 } drag)
                StepTo(ListTreeGrabSteps.IndexAbove(drag));
        }

        public void StepDown()
        {
            if (Session.Drag is { Candidates.Count: > 0 } drag)
                StepTo(ListTreeGrabSteps.IndexBelow(drag));
        }

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

        public void TrackPointer(Point inScroller)
        {
            var inRows = Context.ToRows(inScroller);
            lastPointer = inRows;
            if (isMarkFromKey && ListTreeGrabTakeOver.KeepsTheKeyMark(keyMovePoint, inRows, TreeGrab.MouseTakeOver))
                return;

            isMarkFromKey = false;
            // Out of the tree the mark stays, so a short move out and back keeps the place.
            if (!new Rect(Context.Scroller.Bounds.Size).Contains(inScroller))
                return;

            Session.Track(inRows);
            Part<ListTreeAutoScroll>().Track(inScroller.Y);
        }

        private ListTreePlace? PlaceOf(TreeNode node)
        {
            Rows.ApplyTreeChanges();
            return Rows.RowOf(node)?.Place;
        }

        private void StepTo(int index)
        {
            isMarkFromKey = true;
            keyMovePoint = lastPointer;
            ShowAt(index);
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
            if (!HoldsTheInput)
                return;

            isMarkFromKey = false;
            ShowGrabbing();
            Input.DetachFromWindowAfterRelease();
        }

        private void ShowGrabbing()
        {
            Context.Owner.Classes.Set(GrabbingClass, IsGrabbing);
            Part<ListTreeHints>().Update();
        }
    }
}
