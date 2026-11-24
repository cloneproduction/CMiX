using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.Boolean;
using CMiX.UI.Tests.Support;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace CMiX.UI.Tests
{
    // ButtonCaptioned is a real Button. Its Caption is the label, unless a Content is set.
    public class ButtonCaptionedTests
    {
        [AvaloniaFact]
        public void Create_AButtonCaptioned_IsAButton()
        {
            Assert.IsAssignableFrom<Button>(new ButtonCaptioned());
        }

        [AvaloniaFact]
        public void Caption_ATextSet_IsTheLabel()
        {
            var button = new ButtonCaptioned { Caption = "Apply" };

            Assert.Equal("Apply", button.Content);
        }

        [AvaloniaFact]
        public void Caption_ANewText_ChangesTheLabel()
        {
            var button = new ButtonCaptioned { Caption = "Apply" };

            button.Caption = "Validate";

            Assert.Equal("Validate", button.Content);
        }

        [AvaloniaFact]
        public void Caption_AContentThatIsSet_IsNotReplaced()
        {
            var button = new ButtonCaptioned { Content = "Own label" };

            button.Caption = "Apply";

            Assert.Equal("Own label", button.Content);
        }

        [AvaloniaFact]
        public void Click_TheButton_RunsTheCommand()
        {
            var ran = 0;
            var button = new ButtonCaptioned { Caption = "Apply", Command = new RelayCommand(() => ran++) };
            var window = TestWindow.Show(button, 300, 100);

            PointerInput.Click(window, button, MouseButton.Left);

            Assert.Equal(1, ran);
        }

        [AvaloniaFact]
        public void Show_ACaption_ShowsItOnScreen()
        {
            var button = new ButtonCaptioned { Caption = "Apply" };
            TestWindow.Show(button, 300, 100);

            var text = button.GetVisualDescendants().OfType<TextBlock>().Single();

            Assert.Equal("Apply", text.Text);
        }
    }
}
