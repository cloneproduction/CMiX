// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactivity;

namespace CMiX.UI.Behaviors
{
    // Closes a flyout when the pointer is far from it and from the control that opened it.
    // It does not close the flyout after the user typed in a text box in it.
    public class CloseFlyoutOnPointerAwayBehavior : Behavior<PopupFlyoutBase>
    {
        public static readonly StyledProperty<double> DistanceProperty =
            AvaloniaProperty.Register<CloseFlyoutOnPointerAwayBehavior, double>(nameof(Distance), PointerAwayCloser.DefaultDistance);

        public double Distance
        {
            get => GetValue(DistanceProperty);
            set => SetValue(DistanceProperty, value);
        }

        private readonly PointerAwayCloser closer;

        public CloseFlyoutOnPointerAwayBehavior()
        {
            closer = new PointerAwayCloser(
                () => Distance,
                // The popup host gives the presenter, the control that holds the flyout content.
                () => (AssociatedObject as IPopupHostProvider)?.PopupHost?.Presenter?.Child,
                () => AssociatedObject?.Target,
                () => AssociatedObject?.Hide());
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
            closer.Stop();
        }

        private void OnOpened(object? sender, EventArgs e)
        {
            var target = AssociatedObject?.Target;
            closer.Start(target == null ? null : TopLevel.GetTopLevel(target));
        }

        private void OnClosed(object? sender, EventArgs e) => closer.Stop();
    }
}
