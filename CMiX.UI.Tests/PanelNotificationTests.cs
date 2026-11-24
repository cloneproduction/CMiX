using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Popups;
using CMiX.UI.Tests.Support;
using CMiX.UI.Themes;
using Xunit;

namespace CMiX.UI.Tests
{
    // The notification of a window: one at a time, at the bottom center, with the undo key cap, no focus, no click, and it hides itself.
    public class PanelNotificationTests
    {
        private static (PanelNotification Host, Border Behind, Window Window, ManualTimeProvider Clock) Show()
        {
            var clock = new ManualTimeProvider();
            var host = new PanelNotification { TimeProvider = clock };
            // A focusable area with a fill under the notification. A fill makes it hit by the pointer.
            var behind = new Border { Background = Brushes.Gray, Focusable = true };
            var area = new Grid();
            area.Children.Add(behind);
            area.Children.Add(host);
            return (host, behind, TestWindow.Show(area, 600, 400), clock);
        }

        private static string? KeyText(Notification notification) =>
            notification.GetVisualDescendants().OfType<TextBlock>().SingleOrDefault(text => text.Name == "PART_KeyText")?.Text;

        [AvaloniaFact]
        public void Duration_ANewPanel_IsTheNotificationDurationOfMotion()
        {
            Assert.Equal(Motion.NotificationDuration, new PanelNotification().Duration);
        }

        [AvaloniaFact]
        public void Show_WithAnUndoGesture_ShowsTheTextTheKeyCapAndTheUndoWord()
        {
            var (host, _, _, _) = Show();

            host.Show("Removed Layer 1", "Ctrl+Z");
            Dispatcher.UIThread.RunJobs();

            Assert.True(host.IsShowing);
            var texts = host.Notification.GetVisualDescendants().OfType<TextBlock>().Where(text => text.IsEffectivelyVisible).Select(text => text.Text).ToArray();
            Assert.Contains("Removed Layer 1", texts);
            Assert.Contains(Notification.UndoWord, texts);
            Assert.True(host.Notification.KeyCap.IsEffectivelyVisible);
            Assert.Equal("Ctrl+Z", KeyText(host.Notification));
        }

        [AvaloniaFact]
        public void Show_WithoutAnUndoGesture_ShowsNoKeyCap()
        {
            var (host, _, _, _) = Show();

            host.Show("Hidden: Layer 1");
            Dispatcher.UIThread.RunJobs();

            Assert.True(host.IsShowing);
            Assert.False(host.Notification.KeyCap.IsEffectivelyVisible);
            Assert.DoesNotContain(host.Notification.GetVisualDescendants().OfType<TextBlock>(), text => text.IsEffectivelyVisible && text.Text == Notification.UndoWord);
        }

        [AvaloniaFact]
        public void Show_InAnArea_PlacesTheNotificationAtTheBottomCenter()
        {
            var (host, _, window, _) = Show();

            host.Show("Removed Layer 1", "Ctrl+Z");
            Dispatcher.UIThread.RunJobs();

            var bounds = host.Notification.Bounds;
            var origin = host.Notification.TranslatePoint(default, window)!.Value;
            var margin = Tokens.Get<Thickness>("NotificationMargin");
            Assert.Equal(window.Bounds.Width / 2, origin.X + bounds.Width / 2, 0.5);
            Assert.Equal(window.Bounds.Height - margin.Bottom, origin.Y + bounds.Height, 0.5);
        }

        [AvaloniaFact]
        public void Show_ASecondNotification_ReplacesTheFirst()
        {
            var (host, _, _, clock) = Show();

            host.Show("Removed Layer 1", "Ctrl+Z");
            clock.Advance(Motion.NotificationDuration / 2);
            host.Show("Removed Layer 2");
            Dispatcher.UIThread.RunJobs();

            Assert.Single(host.GetVisualDescendants().OfType<Notification>());
            Assert.Equal("Removed Layer 2", host.Notification.Text);
            Assert.False(host.Notification.KeyCap.IsEffectivelyVisible);

            // The timer of the first notification does not hide the second notification.
            clock.Advance(Motion.NotificationDuration / 2 + TimeSpan.FromMilliseconds(100));
            Dispatcher.UIThread.RunJobs();
            Assert.True(host.IsShowing);
        }

        [AvaloniaFact]
        public void Show_AfterTheDuration_HidesTheNotification()
        {
            var (host, _, _, clock) = Show();

            host.Show("Removed Layer 1", "Ctrl+Z");
            clock.Advance(Motion.NotificationDuration - TimeSpan.FromMilliseconds(1));
            Dispatcher.UIThread.RunJobs();
            Assert.True(host.IsShowing);

            clock.Advance(TimeSpan.FromMilliseconds(1));
            Dispatcher.UIThread.RunJobs();
            Assert.False(host.IsShowing);
        }

        [AvaloniaFact]
        public void Show_WithTheFocusBehind_KeepsTheFocusAndLetsTheClickThrough()
        {
            var (host, behind, window, _) = Show();
            behind.Focus();

            host.Show("Removed Layer 1", "Ctrl+Z");
            Dispatcher.UIThread.RunJobs();

            Assert.Same(behind, window.FocusManager!.GetFocusedElement());
            Assert.All(host.GetSelfAndVisualDescendants().OfType<InputElement>(), element => Assert.False(element.Focusable));
            Assert.False(host.IsHitTestVisible);

            // A click in the middle of the notification goes to the area behind it.
            var pressed = false;
            behind.PointerPressed += (_, _) => pressed = true;
            TestWindow.Render();
            var center = host.Notification.TranslatePoint(new Point(host.Notification.Bounds.Width / 2, host.Notification.Bounds.Height / 2), window)!.Value;
            window.MouseDown(center, MouseButton.Left);
            window.MouseUp(center, MouseButton.Left);
            Assert.True(pressed);
        }
    }
}
