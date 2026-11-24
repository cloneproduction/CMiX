using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // The tabs: the icon tabs are lit when selected or under the pointer and dim at rest,
    // and the content area of a tab control is transparent.
    public class TabItemTests
    {
        // A strip of icon tabs follows the rule of the icon toggles: dim at rest, lit under the pointer and while selected.

        private static (TabItem first, TabItem second) Show()
        {
            var theme = Tokens.Theme("TabItemHorizontal");
            var tabs = new TabControl();
            tabs.Items.Add(new TabItem { Theme = theme, Header = "A" });
            tabs.Items.Add(new TabItem { Theme = theme, Header = "B" });
            TestWindow.Show(tabs, 300, 200);
            var items = tabs.GetVisualDescendants().OfType<TabItem>().ToList();
            return (items[0], items[1]);
        }

        private static double IconOpacity(TabItem item) => Parts.Find<ContentPresenter>(item, "PART_ContentPresenter").Opacity;

        [AvaloniaFact]
        public void Select_ATab_LightsItAndDimsTheOthers()
        {
            var (first, second) = Show();

            Assert.Equal(1.0, IconOpacity(first));
            Assert.Equal(Tokens.Get<double>("OpacityIconIdle"), IconOpacity(second));
        }

        [AvaloniaFact]
        public void Hover_ADimTab_LightsIt()
        {
            var (_, second) = Show();

            PseudoClassInput.Force(second, ":pointerover");

            Assert.Equal(1.0, IconOpacity(second));
        }

        // The content area of a tab control lets the surface behind it show through, so the cards on it stand out.

        [AvaloniaFact]
        public void Show_TheDefaultTabControl_HasATransparentContentArea()
        {
            var tabs = new TabControl { ItemsSource = new[] { new TabItem { Header = "one", Content = new TextBlock { Text = "page" } } } };
            TestWindow.Show(tabs, 300, 200);

            var content = tabs.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "contentPanel");

            Assert.Equal(0, ((ISolidColorBrush)content.Background!).Color.A);
        }
    }
}
