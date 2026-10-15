using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls;
using CMiX.UI.Tests.Support;
using CMiX.UI.Themes;
using Material.Icons;
using Xunit;

namespace CMiX.UI.Tests
{
    // The icon toggles (power, update, mask, eye) look the same: a dim icon while off, a lit icon while on.
    public class ToggleButtonPowerTests
    {
        // The Mask key belongs to the Studio app. The test registers the same key.
        static ToggleButtonPowerTests() => Icons.Register("Mask", MaterialIconKind.CircleBox);

        private static (Window window, ToggleButton toggle) Show(string theme = "ToggleButtonPower")
        {
            var toggle = new ToggleButton { Theme = (ControlTheme)Application.Current!.FindResource(theme)! };
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

        [AvaloniaTheory]
        [InlineData("ToggleButtonUpdate", "Update")]
        [InlineData("ToggleButtonMask", "Mask")]
        public void AThemeBasedOnPower_ShowsItsOwnIcon(string theme, string iconKey)
        {
            var (_, toggle) = Show(theme);

            Assert.Equal(iconKey, Icon(toggle).IconKey);
        }

        [AvaloniaTheory]
        [InlineData("ToggleButtonPower")]
        [InlineData("ToggleButtonUpdate")]
        [InlineData("ToggleButtonMask")]
        [InlineData("ToggleButtonVisibility")]
        public void WhileOff_TheIconIsDim(string theme)
        {
            var (_, toggle) = Show(theme);

            Assert.Equal(0.6, Box(toggle).Opacity);
            Assert.Equal(BrushColor("ForegroundBrush"), ((ISolidColorBrush)Icon(toggle).Foreground!).Color);
        }

        [AvaloniaTheory]
        [InlineData("ToggleButtonPower")]
        [InlineData("ToggleButtonUpdate")]
        [InlineData("ToggleButtonMask")]
        public void WhileOn_TheIconIsLit_InTheSameColorAsWhileOff(string theme)
        {
            var (_, toggle) = Show(theme);

            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(1, Box(toggle).Opacity);
            Assert.Equal(BrushColor("ForegroundBrush"), ((ISolidColorBrush)Icon(toggle).Foreground!).Color);
        }

        [AvaloniaFact]
        public void TheEye_IsLitUnderThePointer()
        {
            var (window, toggle) = Show("ToggleButtonVisibility");

            window.MouseMove(Box(toggle).TranslatePoint(new Point(4, 4), window)!.Value);
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(1, Box(toggle).Opacity);
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
