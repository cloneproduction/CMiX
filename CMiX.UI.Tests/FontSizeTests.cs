using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using CMiX.UI.Tests.Support;
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
            TestWindow.Show(new StackPanel { Children = { text, box } }, 300, 200);

            Assert.Equal(Tokens.Get<double>("FontSizeMD"), text.FontSize);
            Assert.Equal(Tokens.Get<double>("FontSizeMD"), box.FontSize);
        }
    }
}
