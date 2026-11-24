using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.Boolean;
using CMiX.UI.Tests.Support;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace CMiX.UI.Tests
{
    // ButtonMomentary is a real Button. Its Value is true only while the pointer holds it down.
    public class ButtonMomentaryTests
    {
        [AvaloniaFact]
        public void Create_TheMomentaryButton_IsAButton()
        {
            Assert.IsAssignableFrom<Button>(new ButtonMomentary());
        }

        [AvaloniaFact]
        public void Press_SetsTheValueTrue_AndReleaseSetsItFalse()
        {
            var button = new ButtonMomentary();
            var window = TestWindow.Show(button, 300, 100);

            PointerInput.Press(window, button, MouseButton.Left);
            Assert.True(button.Value);

            PointerInput.Release(window, button, MouseButton.Left);
            Assert.False(button.Value);
        }

        [AvaloniaFact]
        public void Value_SetFromOutside_ShowsTheHeldState()
        {
            var button = new ButtonMomentary();
            TestWindow.Show(button, 300, 100);
            var border = button.GetVisualDescendants().OfType<Border>().First(b => b.Name == "border");
            Color Current() => ((ISolidColorBrush)border.Background!).Color;

            button.Value = true;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(Tokens.Get<ISolidColorBrush>("AccentBrush").Color, Current());

            button.Value = false;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(Tokens.Get<ISolidColorBrush>("StateIdleBrush").Color, Current());
        }

        [AvaloniaFact]
        public void Button_WithoutResetCommand_HasNoMenu()
        {
            var button = new ButtonMomentary();
            TestWindow.Show(button, 300, 100);

            Assert.Null(button.ContextMenu);
        }

        [AvaloniaFact]
        public void Button_WithResetCommand_HasAResetMenuThatRunsIt()
        {
            var ran = false;
            var button = new ButtonMomentary { ResetCommand = new RelayCommand(() => ran = true) };
            TestWindow.Show(button, 300, 100);

            button.ContextMenu!.Items.OfType<MenuItem>().Single(i => Equals(i.Header, "Reset")).Command!.Execute(null);

            Assert.True(ran);
        }
    }
}
