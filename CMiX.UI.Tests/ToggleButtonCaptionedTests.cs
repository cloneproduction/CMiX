using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.Boolean;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // ToggleButtonCaptioned is a real ToggleButton with a Caption, so it keeps the whole ToggleButton API.
    public class ToggleButtonCaptionedTests
    {
        private static (Window window, ToggleButtonCaptioned toggle) Show(string caption = "Antialiasing")
        {
            var toggle = new ToggleButtonCaptioned { Caption = caption };
            return (TestWindow.Show(toggle, 400, 100), toggle);
        }

        private static Border Box(Control toggle) => toggle.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "box");

        private static Color BackgroundOf(Border border) => ((ISolidColorBrush)border.Background!).Color;

        [AvaloniaFact]
        public void IsARealToggleButton()
        {
            Assert.IsAssignableFrom<ToggleButton>(new ToggleButtonCaptioned());
        }

        [AvaloniaFact]
        public void ShowsItsCaption_BesideTheBox()
        {
            var (_, toggle) = Show("Antialiasing");

            var text = toggle.GetVisualDescendants().OfType<TextBlock>().Single(block => block.Text == toggle.Caption);

            Assert.Equal("Antialiasing", text.Text);
            Assert.True(text.TranslatePoint(new Point(0, 0), toggle)!.Value.X > Box(toggle).TranslatePoint(new Point(0, 0), toggle)!.Value.X);
        }

        [AvaloniaFact]
        public void TheBoxSitsInTheValueColumn_ToTheRightOfTheMiddle()
        {
            var (_, toggle) = Show();

            var boxLeft = Box(toggle).TranslatePoint(new Point(0, 0), toggle)!.Value.X;

            Assert.True(boxLeft >= toggle.Bounds.Width * 0.39, $"The box starts at {boxLeft} of {toggle.Bounds.Width}.");
        }

        [AvaloniaFact]
        public void ClickOnTheBox_TogglesIt()
        {
            var (window, toggle) = Show();

            PointerInput.Click(window, Box(toggle), MouseButton.Left);

            Assert.True(toggle.IsChecked);
        }

        [AvaloniaFact]
        public void ClickOnTheCaption_TogglesIt()
        {
            var (window, toggle) = Show();
            var text = toggle.GetVisualDescendants().OfType<TextBlock>().Single(block => block.Text == toggle.Caption);

            PointerInput.Click(window, text, MouseButton.Left);

            Assert.True(toggle.IsChecked);
        }

        [AvaloniaFact]
        public void ClickInTheEmptyCaptionColumn_DoesNotToggle()
        {
            var (window, toggle) = Show();

            PointerInput.Click(window, toggle, MouseButton.Left, new Point(10, toggle.Bounds.Height / 2));

            Assert.False(toggle.IsChecked);
        }

        [AvaloniaFact]
        public void TheBoxLooksLikeTheOldCheckBox_WhenOffAndWhenOn()
        {
            var (_, toggle) = Show();
            var box = Box(toggle);
            var icon = toggle.GetVisualDescendants().OfType<CMiX.UI.Controls.AppIcon>().Single();

            Assert.Equal(Tokens.Color("StateIdleBrush"), BackgroundOf(box));
            Assert.Equal(0, icon.Opacity);

            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(Tokens.Color("AccentInactiveBrush"), BackgroundOf(box));
            Assert.Equal(1, icon.Opacity);
        }

        [AvaloniaFact]
        public void TheBoxAndTheRowHaveTheOldSize()
        {
            var (_, toggle) = Show();
            var xs = Tokens.Get<double>("ControlHeightXS");
            var md = Tokens.Get<double>("ControlHeightMD");

            Assert.Equal(xs, Box(toggle).Bounds.Width);
            Assert.Equal(xs, Box(toggle).Bounds.Height);
            Assert.Equal(md, toggle.Bounds.Height);
        }

        [AvaloniaFact]
        public void ADisabledToggle_DimsTheBox()
        {
            var (_, toggle) = Show();

            toggle.IsEnabled = false;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(0.5, Box(toggle).Opacity);
        }

        [AvaloniaFact]
        public void KeepsTheToggleButtonEvents()
        {
            var (window, toggle) = Show();
            var checkedRaised = 0;
            toggle.IsCheckedChanged += (_, _) => checkedRaised++;

            PointerInput.Click(window, Box(toggle), MouseButton.Left);

            Assert.Equal(1, checkedRaised);
        }
    }
}
