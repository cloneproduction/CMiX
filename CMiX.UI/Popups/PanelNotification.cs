// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Threading;
using CMiX.UI.Themes;

namespace CMiX.UI.Popups
{
    // The place of the one notification of a window: at the bottom center of its area, over the other controls.
    // Show puts a new notification in place of the old one. The notification hides itself after Duration.
    // The host and its notification take no focus and no pointer input, so a click goes to the control under them.
    public class PanelNotification : Decorator
    {
        public static readonly StyledProperty<TimeSpan> DurationProperty =
            AvaloniaProperty.Register<PanelNotification, TimeSpan>(nameof(Duration), Motion.NotificationDuration);

        // The time that a notification shows.
        public TimeSpan Duration
        {
            get => GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }

        private readonly Notification notification = new();
        private ITimer? timer;

        // The number of the notification that shows. A timer of an older notification does not hide a newer one.
        private int shown;

        public PanelNotification()
        {
            Focusable = false;
            IsHitTestVisible = false;
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Bottom;
            this.Bind(MarginProperty, this.GetResourceObservable("NotificationMargin"));
            notification.IsVisible = false;
            Child = notification;
        }

        // The clock of the hide timer. A test sets a clock that it moves by hand.
        public TimeProvider TimeProvider { get; set; } = TimeProvider.System;

        public Notification Notification => notification;

        public bool IsShowing => notification.IsVisible;

        // Shows the text, and the undo gesture as a key cap when it is not null. The notification replaces the notification that shows.
        public void Show(string text, string? undoGesture = null)
        {
            Dispatcher.UIThread.VerifyAccess();
            notification.Text = text;
            notification.UndoGesture = undoGesture;
            notification.IsVisible = true;

            // The timer posts the hide to the dispatcher of the host. It never posts to a dispatcher that comes later (a new test).
            var number = ++shown;
            var dispatcher = Dispatcher.UIThread;
            timer?.Dispose();
            timer = TimeProvider.CreateTimer(
                _ => dispatcher.Post(() => { if (number == shown) Hide(); }),
                null, Duration, Timeout.InfiniteTimeSpan);
        }

        // Stops the hide timer when the host leaves the window. A hide that the timer posted before then does nothing.
        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            shown++;
            timer?.Dispose();
            timer = null;
            base.OnDetachedFromVisualTree(e);
        }

        public void Hide()
        {
            timer?.Dispose();
            timer = null;
            notification.IsVisible = false;
        }
    }
}
