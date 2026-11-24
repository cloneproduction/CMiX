using Avalonia.Controls;
using Avalonia.Media;
using CMiX.UI.Controls.ColorPicker;

namespace CMiX.UI.Tests.Support
{
    internal static class ColorWheelHost
    {
        // A red wheel of 200 by 200 in a window without clips, and the colors that it gives.
        public static (ColorPickerWheel Wheel, Window Window, List<Color> Colors) Show()
        {
            // The wheel draws its cursor from its theme, as ColorPickerFull sets it.
            var wheel = new ColorPickerWheel
            {
                Width = 200,
                Height = 200,
                Theme = Tokens.Theme("ColorPickerWheel"),
                SelectedColor = Colors.Red,
            };
            var window = TestWindow.ShowWithoutClips(wheel, 400, 400);
            var colors = new List<Color>();
            wheel.ColorChanged += (_, e) => colors.Add(e.Color);
            return (wheel, window, colors);
        }
    }
}
