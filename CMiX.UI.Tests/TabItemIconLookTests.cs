using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // A strip of icon tabs follows the rule of the icon toggles: dim at rest, lit under the pointer and while selected.
    public class TabItemIconLookTests
    {
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
        public void TheSelectedTab_IsLit_AndTheOthersAreDim()
        {
            var (first, second) = Show();

            Assert.Equal(1.0, IconOpacity(first));
            Assert.Equal(Tokens.Get<double>("OpacityIconIdle"), IconOpacity(second));
        }

        [AvaloniaFact]
        public void ADimTab_IsLitUnderThePointer()
        {
            var (_, second) = Show();

            PseudoClassInput.Force(second, ":pointerover");

            Assert.Equal(1.0, IconOpacity(second));
        }
    }
}
