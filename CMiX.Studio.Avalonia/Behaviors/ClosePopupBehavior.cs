// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace CMiX.Studio.Avalonia.Behaviors
{
    public class ClosePopupBehavior : Behavior<Popup>
    {
        // The Avalonia Popup control itself receives no pointer input, the popup content does.
        // Pointer handlers that WPF attached to the Popup are therefore attached to Popup.Child
        // while the popup is open. The subscribed child is tracked so handlers are removed from
        // the same element they were added to even if the child changes while closed.
        private Control? subscribedChild;
        private bool parentHandlersAdded;

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject == null)
                return;

            AssociatedObject.Opened += OnOpened;
            AssociatedObject.Closed += OnClosed;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
            {
                AssociatedObject.Opened -= OnOpened;
                AssociatedObject.Closed -= OnClosed;
            }
            RemoveChildHandlers();
            RemoveParentWindowHandlers();
        }

        private TopLevel? ParentTopLevel => AssociatedObject == null ? null : TopLevel.GetTopLevel(AssociatedObject);

        private void AddChildHandlers()
        {
            var child = AssociatedObject?.Child;
            if (child == null || subscribedChild != null)
                return;

            subscribedChild = child;
            child.PointerEntered += OnPointerEntered;
            child.PointerExited += OnPointerExited;
            // WPF PreviewMouseLeftButtonUp maps to a tunneling PointerReleased handler.
            child.AddHandler(InputElement.PointerReleasedEvent, OnChildPointerReleased, RoutingStrategies.Tunnel);
        }

        private void RemoveChildHandlers()
        {
            if (subscribedChild == null)
                return;

            subscribedChild.PointerEntered -= OnPointerEntered;
            subscribedChild.PointerExited -= OnPointerExited;
            subscribedChild.RemoveHandler(InputElement.PointerReleasedEvent, OnChildPointerReleased);
            subscribedChild = null;
        }

        private void AddParentWindowHandlers()
        {
            var topLevel = ParentTopLevel;
            if (topLevel == null || parentHandlersAdded)
                return;

            parentHandlersAdded = true;
            // WPF Mouse.AddPreviewMouseDownHandler and AddPreviewMouseMoveHandler map to
            // tunneling pointer handlers on the owning top level.
            topLevel.AddHandler(InputElement.PointerPressedEvent, OnParentPointerPressed, RoutingStrategies.Tunnel);
            topLevel.AddHandler(InputElement.PointerMovedEvent, OnParentPointerMoved, RoutingStrategies.Tunnel);
        }

        private void RemoveParentWindowHandlers()
        {
            var topLevel = ParentTopLevel;
            if (topLevel == null || !parentHandlersAdded)
                return;

            parentHandlersAdded = false;
            topLevel.RemoveHandler(InputElement.PointerPressedEvent, OnParentPointerPressed);
            topLevel.RemoveHandler(InputElement.PointerMovedEvent, OnParentPointerMoved);
        }

        private void ClosePopup()
        {
            if (AssociatedObject == null)
                return;

            if (AssociatedObject.PlacementTarget is ToggleButton tb)
                tb.IsChecked = false;
            AssociatedObject.IsOpen = false;
        }

        private void OnOpened(object? sender, EventArgs e)
        {
            AddChildHandlers();
            AddParentWindowHandlers();
        }

        private void OnClosed(object? sender, EventArgs e)
        {
            RemoveParentWindowHandlers();
            RemoveChildHandlers();
            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            mainWindow?.Focus();
        }

        private void OnPointerEntered(object? sender, PointerEventArgs e) => RemoveParentWindowHandlers();
        private void OnPointerExited(object? sender, PointerEventArgs e) => AddParentWindowHandlers();

        private void OnChildPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (e.InitialPressMouseButton == MouseButton.Left && e.Source is Button)
                ClosePopup();
        }

        private void OnParentPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            ClosePopup();
            e.Handled = true;
        }

        private void OnParentPointerMoved(object? sender, PointerEventArgs e)
        {
            var child = AssociatedObject?.Child;
            if (child == null)
                return;

            var size = child.Bounds.Size;
            var pos = e.GetPosition(child);

            if (pos.X < -64 || pos.Y < -64 ||
                pos.X > size.Width + 64 || pos.Y > size.Height + 64)
            {
                ClosePopup();
                RemoveParentWindowHandlers();
            }
        }
    }
}
