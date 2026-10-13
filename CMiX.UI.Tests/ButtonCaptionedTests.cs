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
        private static Window Show(Control content)
        {
            var window = new Window { Content = content, Width = 300, Height = 100 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return window;
        }

        [AvaloniaFact]
        public void IsARealButton()
        {
            Assert.IsAssignableFrom<Button>(new ButtonCaptioned());
        }

        [AvaloniaFact]
        public void TheCaption_IsTheLabel()
        {
            var button = new ButtonCaptioned { Caption = "Apply" };

            Assert.Equal("Apply", button.Content);
        }

        [AvaloniaFact]
        public void ChangingTheCaption_ChangesTheLabel()
        {
            var button = new ButtonCaptioned { Caption = "Apply" };

            button.Caption = "Validate";

            Assert.Equal("Validate", button.Content);
        }

        [AvaloniaFact]
        public void AContentThatIsSet_IsNotReplacedByTheCaption()
        {
            var button = new ButtonCaptioned { Content = "Own label" };

            button.Caption = "Apply";

            Assert.Equal("Own label", button.Content);
        }

        [AvaloniaFact]
        public void ClickRunsTheCommand()
        {
            var ran = 0;
            var button = new ButtonCaptioned { Caption = "Apply", Command = new RelayCommand(() => ran++) };
            var window = Show(button);

            PointerInput.Click(window, button, MouseButton.Left);

            Assert.Equal(1, ran);
        }

        [AvaloniaFact]
        public void ShowsTheCaptionOnScreen()
        {
            var button = new ButtonCaptioned { Caption = "Apply" };
            Show(button);

            var text = button.GetVisualDescendants().OfType<TextBlock>().Single();

            Assert.Equal("Apply", text.Text);
        }
    }
}
