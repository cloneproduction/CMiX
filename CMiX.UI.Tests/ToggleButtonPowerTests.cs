using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // The power switch: a dim icon while off, a lit icon while on.
    public class ToggleButtonPowerTests
    {
        private static (Window window, ToggleButton toggle) Show()
        {
            var toggle = new ToggleButton { Theme = (ControlTheme)Application.Current!.FindResource("ToggleButtonPower")! };
            var window = new Window { Content = toggle, Width = 200, Height = 100 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (window, toggle);
        }

        private static Border Box(Control toggle) => toggle.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "border");

        private static AppIcon Icon(Control toggle) => toggle.GetVisualDescendants().OfType<AppIcon>().Single();

        private static Color BrushColor(string key) => ((ISolidColorBrush)Application.Current!.FindResource(key)!).Color;

        [AvaloniaFact]
        public void ShowsThePowerIcon()
        {
            var (_, toggle) = Show();

            Assert.Equal("Power", Icon(toggle).IconKey);
        }

        [AvaloniaFact]
        public void WhileOff_TheIconIsDim()
        {
            var (_, toggle) = Show();

            Assert.Equal(0.4, Box(toggle).Opacity);
            Assert.Equal(BrushColor("ForegroundBrush"), ((ISolidColorBrush)Icon(toggle).Foreground!).Color);
        }

        [AvaloniaFact]
        public void WhileOn_TheIconIsLitInTheAccentColor()
        {
            var (_, toggle) = Show();

            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(1, Box(toggle).Opacity);
            Assert.Equal(BrushColor("AccentBrush"), ((ISolidColorBrush)Icon(toggle).Foreground!).Color);
        }

        [AvaloniaFact]
        public void AClick_SwitchesItOn_AndOff()
        {
            var (window, toggle) = Show();

            PointerInput.Click(window, Box(toggle), MouseButton.Left);
            Assert.True(toggle.IsChecked);

            PointerInput.Click(window, Box(toggle), MouseButton.Left);
            Assert.False(toggle.IsChecked);
        }
    }
}
