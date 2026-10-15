using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using CMiX.UI.Behaviors;
using Xunit;
using AvaloniaInteraction = Avalonia.Xaml.Interactivity.Interaction;

namespace CMiX.UI.Tests
{
    // A flyout with CloseFlyoutOnPointerAwayBehavior closes when the pointer goes far from the flyout
    // and from the button that opened it. It stays open after the user typed in a text box in it.
    public class CloseFlyoutOnPointerAwayBehaviorTests
    {
        private static PopupFlyoutBase CreateFlyout(string kind, TextBox? box = null) => kind == "menu"
            ? new MenuFlyout { ItemsSource = new[] { "one", "two", "three" } }
            : new Flyout { Content = new StackPanel { Width = 200, Children = { box ?? new TextBox() } } };

        // The button is wide and sits at the top left of the window. The flyout opens below it.
        private static (Window Window, Button Button, PopupFlyoutBase Flyout) ShowWithOpenFlyout(string kind, TextBox? box = null, Action<CloseFlyoutOnPointerAwayBehavior>? configure = null)
        {
            var flyout = CreateFlyout(kind, box);
            flyout.Placement = PlacementMode.Bottom;
            var behavior = new CloseFlyoutOnPointerAwayBehavior();
            configure?.Invoke(behavior);
            AvaloniaInteraction.SetBehaviors(flyout, new BehaviorCollection { behavior });
            var button = new Button { Content = "open", Width = 700, Height = 30, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
            var window = new Window { Content = button, Width = 800, Height = 500 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            flyout.ShowAt(button);
            Dispatcher.UIThread.RunJobs();
            Assert.True(flyout.IsOpen);
            return (window, button, flyout);
        }

        private static void Move(Window window, Point point)
        {
            window.MouseMove(point);
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaTheory]
        [InlineData("flyout")]
        [InlineData("menu")]
        public void APointerFarFromTheFlyoutAndTheButton_ClosesTheFlyout(string kind)
        {
            var (window, _, flyout) = ShowWithOpenFlyout(kind);

            Move(window, new Point(790, 490));

            Assert.False(flyout.IsOpen);
        }

        [AvaloniaTheory]
        [InlineData("flyout")]
        [InlineData("menu")]
        public void APointerOnTheFarEndOfAWideButton_KeepsTheFlyoutOpen(string kind)
        {
            var (window, _, flyout) = ShowWithOpenFlyout(kind);

            Move(window, new Point(680, 15));

            Assert.True(flyout.IsOpen);
        }

        [AvaloniaTheory]
        [InlineData("flyout")]
        [InlineData("menu")]
        public void ABiggerDistance_KeepsTheFlyoutOpenFartherAway(string kind)
        {
            var (window, _, flyout) = ShowWithOpenFlyout(kind);
            AvaloniaInteraction.GetBehaviors(flyout).OfType<CloseFlyoutOnPointerAwayBehavior>().Single().Distance = 2000;

            Move(window, new Point(790, 490));

            Assert.True(flyout.IsOpen);
        }

        [AvaloniaTheory]
        [InlineData("flyout")]
        [InlineData("menu")]
        public void WithoutClosesOnPointerAway_TheFlyoutStaysOpen(string kind)
        {
            var (window, _, flyout) = ShowWithOpenFlyout(kind, configure: behavior => behavior.ClosesOnPointerAway = false);

            Move(window, new Point(790, 490));

            Assert.True(flyout.IsOpen);
        }

        [AvaloniaTheory]
        [InlineData("flyout")]
        [InlineData("menu")]
        public void TheSwitch_CanChangeWhileTheFlyoutIsOpen(string kind)
        {
            var (window, _, flyout) = ShowWithOpenFlyout(kind);

            AvaloniaInteraction.GetBehaviors(flyout).OfType<CloseFlyoutOnPointerAwayBehavior>().Single().ClosesOnPointerAway = false;
            Move(window, new Point(790, 490));

            Assert.True(flyout.IsOpen);
        }

        [AvaloniaFact]
        public void ClosesOnPointerAway_IsOnByDefault()
        {
            Assert.True(new CloseFlyoutOnPointerAwayBehavior().ClosesOnPointerAway);
        }

        [AvaloniaFact]
        public void TheDistance_IsSixtyFourByDefault()
        {
            Assert.Equal(64, new CloseFlyoutOnPointerAwayBehavior().Distance);
        }

        [AvaloniaFact]
        public void AfterTypingInATextBoxOfTheFlyout_APointerFarAway_KeepsTheFlyoutOpen()
        {
            var box = new TextBox();
            var (window, _, flyout) = ShowWithOpenFlyout("flyout", box);
            box.Focus();
            window.KeyPress(Key.A, RawInputModifiers.None, PhysicalKey.A, "a");
            Dispatcher.UIThread.RunJobs();
            Assert.True(box.IsFocused);

            Move(window, new Point(790, 490));

            Assert.True(flyout.IsOpen);
        }

        [AvaloniaTheory]
        [InlineData("flyout")]
        [InlineData("menu")]
        public void AfterTheFlyoutIsOpenedAgain_APointerFarAway_ClosesItAgain(string kind)
        {
            var (window, button, flyout) = ShowWithOpenFlyout(kind);
            flyout.Hide();
            Dispatcher.UIThread.RunJobs();
            Move(window, new Point(790, 490));

            flyout.ShowAt(button);
            Dispatcher.UIThread.RunJobs();
            Assert.True(flyout.IsOpen);
            Move(window, new Point(780, 480));

            Assert.False(flyout.IsOpen);
        }
    }
}
