using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Xunit;

namespace CMiX.UI.Tests
{
    // The text of the UI has the size of the token FontSizeMD, in a window and in a popup.
    public class FontSizeTests
    {
        [AvaloniaFact]
        public void TheText_OfAWindow_HasTheSizeOfTheToken()
        {
            var text = new TextBlock { Text = "text" };
            var box = new TextBox { Text = "box" };
            new Window { Content = new StackPanel { Children = { text, box } }, Width = 300, Height = 200 }.Show();
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(Tokens.Get<double>("FontSizeMD"), text.FontSize);
            Assert.Equal(Tokens.Get<double>("FontSizeMD"), box.FontSize);
        }

        [AvaloniaFact]
        public void ChangingTheToken_ChangesTheText()
        {
            var text = new TextBlock { Text = "text" };
            new Window { Content = text, Width = 300, Height = 200 }.Show();
            Dispatcher.UIThread.RunJobs();

            var application = Application.Current!;
            application.Resources["FontSizeMD"] = 20.0;
            try
            {
                Dispatcher.UIThread.RunJobs();
                Assert.Equal(20.0, text.FontSize);
            }
            finally
            {
                application.Resources.Remove("FontSizeMD");
            }
        }
    }
}
