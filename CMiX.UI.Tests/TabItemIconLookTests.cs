using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;

namespace CMiX.UI.Tests
{
    // A strip of icon tabs follows the rule of the icon toggles: dim at rest, lit under the pointer and while selected.
    public class TabItemIconLookTests
    {
        private static (TabItem first, TabItem second) Show()
        {
            var theme = (ControlTheme)Application.Current!.FindResource("TabItemHorizontal")!;
            var tabs = new TabControl();
            tabs.Items.Add(new TabItem { Theme = theme, Header = "A" });
            tabs.Items.Add(new TabItem { Theme = theme, Header = "B" });
            new Window { Content = tabs, Width = 300, Height = 200 }.Show();
            Dispatcher.UIThread.RunJobs();
            var items = tabs.GetVisualDescendants().OfType<TabItem>().ToList();
            return (items[0], items[1]);
        }

        private static double IconOpacity(TabItem item) =>
            item.GetVisualDescendants().OfType<ContentPresenter>().First(presenter => presenter.Name == "PART_ContentPresenter").Opacity;

        [AvaloniaFact]
        public void TheSelectedTab_IsLit_AndTheOthersAreDim()
        {
            var (first, second) = Show();

            Assert.Equal(1.0, IconOpacity(first));
            Assert.Equal(0.6, IconOpacity(second));
        }

        [AvaloniaFact]
        public void ADimTab_IsLitUnderThePointer()
        {
            var (_, second) = Show();

            foreach (var part in second.GetSelfAndVisualDescendants().OfType<Control>())
                ((IPseudoClasses)part.Classes).Set(":pointerover", true);
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(1.0, IconOpacity(second));
        }
    }
}
