using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;
using CMiX.UI.Controls.ColorPicker;

namespace CMiX.UI.Tests
{
    // The picker hides the inactive mode panels (HSV, RGB, HEX) with the class hiddenMode, an opacity and not IsVisible,
    // so each panel keeps its room and the picker keeps its height when the mode changes.
    public class ColorPickerSlidersTests
    {
        [AvaloniaFact]
        public void ColorMode_EachMode_KeepsTheHeightOfThePicker()
        {
            var picker = new ColorPickerSliders
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
            var hsvRadio = radios.Single(r => r.Name == "hsvRadio");
            var rgbRadio = radios.Single(r => r.Name == "rgbRadio");
            var hexRadio = radios.Single(r => r.Name == "hexRadio");

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
