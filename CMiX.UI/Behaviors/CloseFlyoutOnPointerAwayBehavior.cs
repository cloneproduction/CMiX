// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace CMiX.UI.Behaviors
{
    // Closes a flyout when the pointer is far from it and from the control that opened it.
    // It does not close the flyout after the user typed in a text box in it.
    public class CloseFlyoutOnPointerAwayBehavior : Behavior<PopupFlyoutBase>
    {
        public static readonly StyledProperty<double> DistanceProperty =
            AvaloniaProperty.Register<CloseFlyoutOnPointerAwayBehavior, double>(nameof(Distance), PointerAway.DefaultDistance);

        public double Distance
        {
            get => GetValue(DistanceProperty);
            set => SetValue(DistanceProperty, value);
        }

        private readonly TypingGuard typingGuard = new();
        private TopLevel? watchedTopLevel;

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
            StopWatching();
        }

        private void OnOpened(object? sender, EventArgs e)
        {
            StopWatching();
            var target = AssociatedObject?.Target;
            watchedTopLevel = target == null ? null : TopLevel.GetTopLevel(target);
            watchedTopLevel?.AddHandler(InputElement.PointerMovedEvent, OnPointerMoved, RoutingStrategies.Tunnel);
            typingGuard.Watch(PresenterContent());
        }

        private void OnClosed(object? sender, EventArgs e) => StopWatching();

        private void StopWatching()
        {
            watchedTopLevel?.RemoveHandler(InputElement.PointerMovedEvent, OnPointerMoved);
            watchedTopLevel = null;
            typingGuard.Stop();
        }

        // The popup host gives the presenter, the control that holds the flyout content.
        private Control? PresenterContent() => (AssociatedObject as IPopupHostProvider)?.PopupHost?.Presenter?.Child;

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            var flyout = AssociatedObject;
            var presenter = PresenterContent();
            if (flyout == null || presenter == null)
                return;

            if (PointerAway.IsFar(e, Distance, presenter, flyout.Target) && !typingGuard.IsTyping)
                flyout.Hide();
        }
    }
}
