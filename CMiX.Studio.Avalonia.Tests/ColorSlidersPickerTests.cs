using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Views.Controls;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // Reproduces the layout regression fixed in commit fc493d82: the HSV, RGB and HEX mode
    // panels used to be swapped in with IsVisible, which collapses layout space, so the whole
    // picker resized every time the mode changed. The fix hides the inactive panels with the
    // hiddenMode class (opacity, not IsVisible) so they keep their layout space.
    public class ColorSlidersPickerTests
    {
        [AvaloniaFact]
        public void SwitchingColorModes_DoesNotChangePickerHeight()
        {
            var picker = new ColorSliders
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            var window = new Window
            {
                Content = picker,
                Width = 400,
                Height = 800,
                SizeToContent = SizeToContent.Manual
            };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var radios = picker.GetVisualDescendants().OfType<RadioButton>().ToList();
            var hsvRadio = radios.Single(r => r.Name == "HSVRadio");
            var rgbRadio = radios.Single(r => r.Name == "RGBRadio");
            var hexRadio = radios.Single(r => r.Name == "HEXRadio");

            hsvRadio.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            var baselineHeight = picker.Bounds.Height;
            Assert.True(baselineHeight > 0, "Expected the picker to have measured a positive height.");

            foreach (var radio in new[] { rgbRadio, hexRadio, hsvRadio })
            {
                radio.IsChecked = true;
                Dispatcher.UIThread.RunJobs();
                Assert.Equal(baselineHeight, picker.Bounds.Height);
            }
        }
    }
}
