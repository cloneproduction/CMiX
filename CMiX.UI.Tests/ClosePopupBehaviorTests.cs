using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;
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
            var selector = new ColorSelector
            {
                DataContext = new StringValue(),
                Width = 900,
                Height = 40,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
            };
            var window = new Window { Content = selector, Width = 1000, Height = 500 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var swatch = selector.FindControl<ToggleButton>("PopupToggle")!;
            var popup = selector.FindControl<Popup>("colorPickerPopup")!;
            Click(window, OnSwatch(window, swatch, 0.05));
            Assert.True(popup.IsOpen);
            return (window, swatch, popup);
        }

        private static Point OnSwatch(Window window, Control swatch, double fraction)
        {
            var top = swatch.TranslatePoint(new Point(0, 0), window)!.Value;
            return new Point(top.X + swatch.Bounds.Width * fraction, top.Y + swatch.Bounds.Height / 2);
        }

        private static void Click(Window window, Point point)
        {
            window.MouseMove(point);
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
        }

        private static void Move(Window window, Point point)
        {
            window.MouseMove(point);
            Dispatcher.UIThread.RunJobs();
        }

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

            var click = OnSwatch(window, swatch, clickAt);
            Click(window, click);
            Move(window, new Point(click.X + 2, click.Y + 1));

            Assert.True(popup.IsOpen);
        }

        [AvaloniaFact]
        public void APointerFarFromThePopupAndTheSwatch_ClosesThePopup()
        {
            var (window, _, popup) = ShowWideSelectorWithOpenPopup();

            Move(window, new Point(5, 5));

            Assert.False(popup.IsOpen);
        }
    }
}
