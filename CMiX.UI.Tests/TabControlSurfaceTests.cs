using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;

namespace CMiX.UI.Tests
{
    // The content area of a tab control lets the surface behind it show through, so the cards on it stand out.
    public class TabControlSurfaceTests
    {
        [AvaloniaFact]
        public void TheContentAreaOfTheDefaultTabControl_IsTransparent()
        {
            var tabs = new TabControl { ItemsSource = new[] { new TabItem { Header = "one", Content = new TextBlock { Text = "page" } } } };
            new Window { Content = tabs, Width = 300, Height = 200 }.Show();
            Dispatcher.UIThread.RunJobs();

            var content = tabs.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "contentPanel");

            Assert.Equal(0, ((ISolidColorBrush)content.Background!).Color.A);
        }
    }
}
