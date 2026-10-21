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

namespace CMiX.UI.Behaviors
{
    public class ClosePopupBehavior : Behavior<Popup>
    {
        // The popup closes when the pointer is farther than this from the popup and from the control that opened it.
        public static readonly StyledProperty<double> DistanceProperty =
            AvaloniaProperty.Register<ClosePopupBehavior, double>(nameof(Distance), PointerAwayCloser.DefaultDistance);

        public double Distance
        {
            get => GetValue(DistanceProperty);
            set => SetValue(DistanceProperty, value);
        }

        // False: the pointer going away does not close the popup. A press outside the popup and a click on a button of it still do.
        public static readonly StyledProperty<bool> ClosesOnPointerAwayProperty =
            AvaloniaProperty.Register<ClosePopupBehavior, bool>(nameof(ClosesOnPointerAway), true);

        public bool ClosesOnPointerAway
        {
            get => GetValue(ClosesOnPointerAwayProperty);
            set => SetValue(ClosesOnPointerAwayProperty, value);
        }

        static ClosePopupBehavior()
        {
            ClosesOnPointerAwayProperty.Changed.AddClassHandler<ClosePopupBehavior>((behavior, _) => behavior.UpdateCloser());
        }

        private readonly PointerAwayCloser closer;
        private Control? subscribedChild;
        private bool parentHandlersAdded;

        public ClosePopupBehavior()
        {
            closer = new PointerAwayCloser(
                () => Distance,
                () => ClosesOnPointerAway,
                () => AssociatedObject?.Child,
                // The control that opened the popup can be much wider than the popup.
                () => AssociatedObject?.PlacementTarget ?? AssociatedObject?.Parent as Control,
                ClosePopup);
        }

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
            closer.Stop();
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
        }

        private void RemoveParentWindowHandlers()
        {
            var topLevel = ParentTopLevel;
            if (topLevel == null || !parentHandlersAdded)
                return;

            parentHandlersAdded = false;
            topLevel.RemoveHandler(InputElement.PointerPressedEvent, OnParentPointerPressed);
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
            UpdateCloser();
        }

        // The closer runs while the popup is open. It starts only while ClosesOnPointerAway is true.
        private void UpdateCloser()
        {
            if (AssociatedObject?.IsOpen == true)
                closer.Start(ParentTopLevel);
            else
                closer.Stop();
        }

        private void OnClosed(object? sender, EventArgs e)
        {
            closer.Stop();
            RemoveParentWindowHandlers();
            RemoveChildHandlers();
            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            mainWindow?.Focus();
        }

        // The pointer is back in the content, so the pointer going away closes the popup again.
        private void OnPointerEntered(object? sender, PointerEventArgs e)
        {
            RemoveParentWindowHandlers();
            UpdateCloser();
        }
        private void OnPointerExited(object? sender, PointerEventArgs e) => AddParentWindowHandlers();

        private void OnChildPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            var sourceButton = (e.Source as global::Avalonia.Visual)?.FindAncestorOfType<Button>(true);
            if (e.InitialPressMouseButton == MouseButton.Left && sourceButton != null && sourceButton is not ToggleButton)
                global::Avalonia.Threading.Dispatcher.UIThread.Post(ClosePopup);
        }

        // A press outside ends the edit of a value of the popup, and that is all it does. The editor commits the
        // value in the bubble phase, after this tunnel handler. The next press outside closes the popup.
        private void OnParentPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (PointerAwayCloser.IsEditing(AssociatedObject?.Child))
            {
                closer.Stop();
                return;
            }

            ClosePopup();
            e.Handled = true;
        }
    }
}
