using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using CMiX.UI.Panels;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // A panel is open when it appears.
    public class ExpanderDefaultTests
    {
        [AvaloniaFact]
        public void Show_AnIndentedExpander_IsOpenByDefault()
        {
            Assert.True(new ExpanderIndented().IsExpanded);
        }

        [AvaloniaFact]
        public void Show_AnExpanderInTheTheme_IsOpenByDefault()
        {
            var expander = new Expander { Header = "Peers", Content = new Border() };
            TestWindow.Show(expander, 200, 200);

            Assert.True(expander.IsExpanded);
        }
    }
}
