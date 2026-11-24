using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using AvaloniaInteraction = Avalonia.Xaml.Interactivity.Interaction;
using CMiX.UI.Behaviors;
using CMiX.UI.Controls.ColorPicker;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // A popup with ClosePopupBehavior closes when the pointer goes far from the popup.
    // The control that opened the popup counts as near, also when it is much wider than the popup.
    public class ClosePopupBehaviorTests
    {
        // The swatch is 540 wide and the popup 232, so the ends of the swatch are 154 from the popup.
        private static (Window Window, ToggleButton Swatch, Popup Popup) ShowWideSelectorWithOpenPopup()
        {
            var (window, _, swatch, popup) = SelectorColorHost.ShowOpen(900, 1000, 500);
            return (window, swatch, popup);
        }

        private static (Window Window, ToggleButton Swatch, Popup Popup) ShowSelectorWithOpenPopup()
        {
            var (window, _, swatch, popup) = SelectorColorHost.ShowOpen(300, 1000, 600);
            return (window, swatch, popup);
        }

        private static readonly Point Far = new(950, 20);

        [AvaloniaTheory]
        [InlineData(0.05)]
        [InlineData(0.5)]
        [InlineData(0.95)]
        public void AfterTheClick_AMoveOnTheSwatch_KeepsThePopupOpen(double clickAt)
        {
            var (window, swatch, popup) = ShowWideSelectorWithOpenPopup();
            popup.IsOpen = false;
            swatch.IsChecked = false;
            Dispatcher.UIThread.RunJobs();

            var click = SelectorColorHost.OnSwatch(window, swatch, clickAt);
            PointerInput.Click(window, click);
            PointerInput.Move(window, new Point(click.X + 2, click.Y + 1));

            Assert.True(popup.IsOpen);
        }

        [AvaloniaFact]
        public void APointerFarFromThePopupAndTheSwatch_ClosesThePopup()
        {
            var (window, _, popup) = ShowWideSelectorWithOpenPopup();

            PointerInput.Move(window, new Point(5, 5));

            Assert.False(popup.IsOpen);
        }

        [AvaloniaFact]
        public void TheDistance_IsSixtyFourByDefault()
        {
            Assert.Equal(64, new ClosePopupBehavior().PointerAwayDistance);
        }

        [AvaloniaFact]
        public void ABiggerDistance_KeepsThePopupOpenFartherAway()
        {
            var (window, _, popup) = ShowWideSelectorWithOpenPopup();
            var behavior = AvaloniaInteraction.GetBehaviors(popup).OfType<ClosePopupBehavior>().Single();

            behavior.PointerAwayDistance = 2000;
            PointerInput.Move(window, new Point(5, 5));

            Assert.True(popup.IsOpen);
        }

        // A popup above a button at the bottom left of the window, with a text box inside.
        private static (Window Window, Popup Popup, TextBox Box) ShowPopupWithTextBox()
        {
            var button = new Button { Content = "open", Width = 100, Height = 30, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Bottom };
            var box = new TextBox { Width = 150 };
            var popup = new Popup { PlacementTarget = button, Placement = PlacementMode.Top, IsLightDismissEnabled = false, Child = new Border { Child = box } };
            AvaloniaInteraction.SetBehaviors(popup, new BehaviorCollection { new ClosePopupBehavior() });
            var window = TestWindow.Show(new Panel { Children = { button, popup } }, 800, 500);

            popup.IsOpen = true;
            Dispatcher.UIThread.RunJobs();
            return (window, popup, box);
        }

        [AvaloniaFact]
        public void WithoutAFocusedTextBox_APointerFarAway_ClosesThePopup()
        {
            var (window, popup, _) = ShowPopupWithTextBox();

            PointerInput.Move(window, new Point(790, 5));

            Assert.False(popup.IsOpen);
        }

        [AvaloniaFact]
        public void ATextBoxWithFocusButNoTyping_APointerFarAway_ClosesThePopup()
        {
            var (window, popup, box) = ShowPopupWithTextBox();
            box.Focus();
            Dispatcher.UIThread.RunJobs();
            Assert.True(box.IsFocused);

            PointerInput.Move(window, new Point(790, 5));

            Assert.False(popup.IsOpen);
        }

        [AvaloniaFact]
        public void AfterTypingInATextBoxOfThePopup_APointerFarAway_KeepsThePopupOpen()
        {
            var (window, popup, box) = ShowPopupWithTextBox();
            box.Focus();
            window.KeyPress(Key.A, RawInputModifiers.None, PhysicalKey.A, "a");
            Dispatcher.UIThread.RunJobs();

            PointerInput.Move(window, new Point(790, 5));

            Assert.True(popup.IsOpen);
        }

        // A control of the popup that holds the pointer keeps the popup open. After the release, the next move away closes it.

        [AvaloniaFact]
        public void PointerAway_WhileAControlOfThePopupHoldsThePointer_KeepsThePopupOpen()
        {
            var (window, _, popup) = ShowSelectorWithOpenPopup();
            var pointer = PointerInput.NewPointer();
            pointer.Capture(popup.Child);

            PointerInput.Move(window, pointer, Far, window);

            Assert.True(popup.IsOpen);
        }

        [AvaloniaFact]
        public void Release_FarAway_ClosesThePopupOnTheNextMove()
        {
            var (window, _, popup) = ShowSelectorWithOpenPopup();
            var pointer = PointerInput.NewPointer();
            pointer.Capture(popup.Child);
            PointerInput.Move(window, pointer, Far, window);

            // The release itself closes nothing: the control that held the pointer ends its drag first.
            pointer.Capture(null);
            PointerInput.Release(window, pointer, Far, window);
            Assert.True(popup.IsOpen);

            PointerInput.Move(window, pointer, new Point(Far.X - 5, Far.Y), window);
            Assert.False(popup.IsOpen);
        }

        [AvaloniaFact]
        public void Release_NearThePopup_KeepsThePopupOpen()
        {
            var (window, swatch, popup) = ShowSelectorWithOpenPopup();
            var pointer = PointerInput.NewPointer();
            pointer.Capture(popup.Child);
            var near = swatch.TranslatePoint(new Point(swatch.Bounds.Width / 2, swatch.Bounds.Height / 2), window)!.Value;
            PointerInput.Move(window, pointer, Far, window);

            pointer.Capture(null);
            PointerInput.Release(window, pointer, near, window);

            Assert.True(popup.IsOpen);
        }

        [AvaloniaFact]
        public void PointerAway_WithoutAHeldPointer_ClosesThePopup()
        {
            var (window, _, popup) = ShowSelectorWithOpenPopup();

            PointerInput.Move(window, PointerInput.NewPointer(), Far, window);

            Assert.False(popup.IsOpen);
        }
    }
}
