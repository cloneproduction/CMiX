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
using Xunit;

namespace CMiX.UI.Tests
{
    // The icon toggles (power, update, eye) look the same: a dim icon while off, a lit icon while on.
    public class ToggleButtonPowerTests
    {
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
        public void AThemeBasedOnPower_ShowsItsOwnIcon(string theme, string iconKey)
        {
            var (_, toggle) = Show(theme);

            Assert.Equal(iconKey, Icon(toggle).IconKey);
        }

        [AvaloniaTheory]
        [InlineData("ToggleButtonPower")]
        [InlineData("ToggleButtonUpdate")]
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
        public void WhileOn_TheIconIsLit_InTheSameColorAsWhileOff(string theme)
        {
            var (_, toggle) = Show(theme);

            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(1, Box(toggle).Opacity);
            Assert.Equal(BrushColor("ForegroundBrush"), ((ISolidColorBrush)Icon(toggle).Foreground!).Color);
        }

        // A value on the template element would beat every style, so a style of the application could not change the opacity,
        // and the on and hover states of the theme would not show either.
        [AvaloniaTheory]
        [InlineData("ToggleButtonPower")]
        [InlineData("ToggleButtonUpdate")]
        [InlineData("ToggleButtonVisibility")]
        [InlineData("ToggleButtonPlayPause")]
        public void TheOpacityOfAnIconToggle_IsAStyle_NotALocalValue(string theme)
        {
            var (_, toggle) = Show(theme);

            toggle.Styles.Add(new Style(x => x.OfType<ToggleButton>().Template().OfType<Border>().Name("border"))
            {
                Setters = { new Setter(Visual.OpacityProperty, 0.123) }
            });
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(0.123, Box(toggle).Opacity);
        }

        [AvaloniaFact]
        public void ThePlayPauseToggle_IsDimAtRest_AndShowsStopWhenOn()
        {
            var (_, toggle) = Show("ToggleButtonPlayPause");
            Assert.Equal(0.6, Box(toggle).Opacity);
            Assert.Equal("Play", Icon(toggle).IconKey);

            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("Stop", Icon(toggle).IconKey);
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
