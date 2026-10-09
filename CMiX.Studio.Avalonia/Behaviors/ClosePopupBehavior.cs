// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace CMiX.Studio.Avalonia.Behaviors
{
    public class ClosePopupBehavior : Behavior<Popup>
    {
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
            var sourceButton = (e.Source as global::Avalonia.Visual)?.FindAncestorOfType<Button>(true);
            if (e.InitialPressMouseButton == MouseButton.Left && sourceButton != null && sourceButton is not ToggleButton)
                global::Avalonia.Threading.Dispatcher.UIThread.Post(ClosePopup);
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
