using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The mask toggle is the power toggle of the library with the icon of the Studio.
    // It looks like the other icon toggles: a dim icon while off, a lit icon while on, in the same color.
    public class MaskToggleTests
    {
        private static ToggleButton Show()
        {
            var toggle = new ToggleButton { Theme = (ControlTheme)Application.Current!.FindResource("ToggleButtonMask")! };
            new Window { Content = toggle, Width = 200, Height = 100 }.Show();
            Dispatcher.UIThread.RunJobs();
            return toggle;
        }

        private static Border Box(Control toggle) => toggle.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "border");

        private static AppIcon Icon(Control toggle) => toggle.GetVisualDescendants().OfType<AppIcon>().Single();

        private static Color BrushColor(string key) => ((ISolidColorBrush)Application.Current!.FindResource(key)!).Color;

        [AvaloniaFact]
        public void ShowsTheMaskIcon()
        {
            Assert.Equal("Mask", Icon(Show()).IconKey);
        }

        [AvaloniaFact]
        public void WhileOff_TheIconIsDim()
        {
            var toggle = Show();

            Assert.Equal(0.6, Box(toggle).Opacity);
            Assert.Equal(BrushColor("ForegroundBrush"), ((ISolidColorBrush)Icon(toggle).Foreground!).Color);
        }

        [AvaloniaFact]
        public void WhileOn_TheIconIsLit_InTheSameColor()
        {
            var toggle = Show();

            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(1, Box(toggle).Opacity);
            Assert.Equal(BrushColor("ForegroundBrush"), ((ISolidColorBrush)Icon(toggle).Foreground!).Color);
        }
    }
}
