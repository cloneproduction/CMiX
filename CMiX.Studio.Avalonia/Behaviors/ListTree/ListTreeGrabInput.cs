// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Views;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // The input of the window during a grab of the new tree. It takes every click and every key, as TreeGrab does.
    internal sealed class ListTreeGrabInput : ListTreeBehavior
    {
        private TopLevel? top;

        // The release of a grab click must not reach a row.
        private bool releasePending;

        public bool IsAttached => top != null;

        private ListTreeGrab Grab => Part<ListTreeGrab>();

        protected override void OnUnwatch() => Detach();

        public void Attach(TopLevel window)
        {
            top = window;
            window.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
            window.AddHandler(InputElement.PointerMovedEvent, OnPointerMoved, RoutingStrategies.Tunnel);
            window.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
            window.AddHandler(InputElement.PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Tunnel);
            window.AddHandler(InputElement.PointerWheelChangedEvent, OnPointerWheel, RoutingStrategies.Tunnel);
            window.AddHandler(InputElement.GotFocusEvent, OnGotFocus, RoutingStrategies.Bubble, handledEventsToo: true);
            if (window is WindowBase windowBase)
                windowBase.Deactivated += OnDeactivated;
            if (window is Window closable)
                closable.Closed += OnDeactivated;
        }

        public void DetachAfterRelease()
        {
            if (!releasePending)
                Detach();
        }

        public void Detach()
        {
            if (top is not { } window)
                return;

            window.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
            window.RemoveHandler(InputElement.PointerMovedEvent, OnPointerMoved);
            window.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressed);
            window.RemoveHandler(InputElement.PointerReleasedEvent, OnPointerReleased);
            window.RemoveHandler(InputElement.PointerWheelChangedEvent, OnPointerWheel);
            window.RemoveHandler(InputElement.GotFocusEvent, OnGotFocus);
            if (window is WindowBase windowBase)
                windowBase.Deactivated -= OnDeactivated;
            if (window is Window closable)
                closable.Closed -= OnDeactivated;
            top = null;
            releasePending = false;
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (!Grab.IsGrabbing)
                return;

            switch (e.Key)
            {
                case Key.Up when e.KeyModifiers == KeyModifiers.None:
                    Grab.StepUp();
                    e.Handled = true;
                    break;
                case Key.Down when e.KeyModifiers == KeyModifiers.None:
                    Grab.StepDown();
                    e.Handled = true;
                    break;
                case Key.Enter:
                    Grab.Confirm();
                    e.Handled = true;
                    break;
                case Key.Escape:
                    Grab.Cancel();
                    e.Handled = true;
                    break;
                case Key.E:
                    // Tree.Expand goes on to the key binding.
                    break;
                default:
                    // Another key would change the rows under the grab.
                    e.Handled = true;
                    break;
            }
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (Grab.IsGrabbing)
                Grab.TrackPointer(e.GetPosition(Context.Rows), e.GetPosition(Context.Scroller));
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (!Grab.IsGrabbing)
                return;

            e.Handled = true;
            releasePending = true;
            Grab.Click(e.GetCurrentPoint(Context.Rows).Properties);
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (!releasePending)
                return;

            e.Handled = true;
            releasePending = false;
            if (!Grab.IsGrabbing)
                Detach();
        }

        private void OnPointerWheel(object? sender, PointerWheelEventArgs e)
        {
            if (!Grab.IsGrabbing)
                return;

            Part<ListTreeAutoScroll>().ScrollByWheel(TreeRows.WheelSteps(e));
            e.Handled = true;
        }

        private void OnGotFocus(object? sender, GotFocusEventArgs e)
        {
            var owner = Context.Owner;
            if (Grab.IsGrabbing && e.Source is Visual focused && !ReferenceEquals(focused, owner) && !owner.IsVisualAncestorOf(focused))
                Grab.Cancel();
        }

        private void OnDeactivated(object? sender, EventArgs e) => Grab.Detach();
    }
}
