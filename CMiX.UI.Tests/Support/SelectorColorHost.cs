using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using CMiX.UI.Controls.ColorPicker;
using Xunit;

namespace CMiX.UI.Tests.Support
{
    internal static class SelectorColorHost
    {
        // A color selector at the bottom left of the window. A click near the left end of the swatch opens its popup.
        public static (Window Window, SelectorColor Selector, ToggleButton Swatch, Popup Popup) ShowOpen(double width, double windowWidth, double windowHeight)
        {
            var selector = new SelectorColor
            {
                DataContext = new StringValue(),
                Width = width,
                Height = 40,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
            };
            var window = TestWindow.Show(selector, windowWidth, windowHeight);

            var swatch = selector.FindControl<ToggleButton>("popupToggle")!;
            var popup = selector.FindControl<Popup>("colorPickerPopup")!;
            PointerInput.Click(window, OnSwatch(window, swatch, 0.05));
            Assert.True(popup.IsOpen);
            return (window, selector, swatch, popup);
        }

        // A point on the swatch, at the given fraction of its width.
        public static Point OnSwatch(Window window, Control swatch, double fraction)
        {
            var top = swatch.TranslatePoint(new Point(0, 0), window)!.Value;
            return new Point(top.X + swatch.Bounds.Width * fraction, top.Y + swatch.Bounds.Height / 2);
        }
    }
}
