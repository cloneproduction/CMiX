using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // The styles take their opacities from the tokens: a disabled control has OpacityDisabled, a dim icon has OpacityIconIdle.
    public class OpacityTokenUseTests
    {
        private static T Show<T>(T control) where T : Control
        {
            TestWindow.Show(control, 300, 200);
            return control;
        }

        [AvaloniaTheory]
        [InlineData(null)]
        [InlineData("TextBoxNumbersOnly")]
        public void TextBox_Disabled_HasTheDisabledOpacity(string? themeKey)
        {
            var box = new TextBox { IsEnabled = false };
            if (themeKey != null)
                box.Theme = Tokens.Theme(themeKey);
            Show(box);

            Assert.Equal(Tokens.Get<double>("OpacityDisabled"), Parts.Find<Border>(box, "border").Opacity);
        }

        [AvaloniaFact]
        public void TabItem_Disabled_HasTheDisabledOpacity()
        {
            var theme = Tokens.Theme("TabItemCMiX");
            var tabs = new TabControl();
            tabs.Items.Add(new TabItem { Theme = theme, Header = "A" });
            tabs.Items.Add(new TabItem { Theme = theme, Header = "B", IsEnabled = false });
            Show(tabs);
            var off = tabs.GetVisualDescendants().OfType<TabItem>().ToList()[1];

            Assert.Equal(Tokens.Get<double>("OpacityDisabled"), Parts.Find<ContentPresenter>(off, "PART_ContentPresenter").Opacity);
        }

        [AvaloniaFact]
        public void TabItemHorizontal_NotSelected_HasTheIdleIconOpacity()
        {
            var theme = Tokens.Theme("TabItemHorizontal");
            var tabs = new TabControl();
            tabs.Items.Add(new TabItem { Theme = theme, Header = "A" });
            tabs.Items.Add(new TabItem { Theme = theme, Header = "B" });
            Show(tabs);
            var idle = tabs.GetVisualDescendants().OfType<TabItem>().ToList()[1];

            Assert.Equal(Tokens.Get<double>("OpacityIconIdle"), Parts.Find<ContentPresenter>(idle, "PART_ContentPresenter").Opacity);
        }

        [AvaloniaFact]
        public void QuietButtonIcon_AtRest_HasTheIdleIconOpacity()
        {
            var button = Show(new CMiX.UI.Controls.Boolean.ButtonIcon { IconKey = "Close", Classes = { "quiet" } });

            Assert.Equal(Tokens.Get<double>("OpacityIconIdle"), button.GetVisualDescendants().OfType<CMiX.UI.Controls.AppIcon>().Single().Opacity);
        }

        [AvaloniaFact]
        public void ToggleButtonIcon_AtRest_HasTheIdleIconOpacity()
        {
            var theme = Tokens.Theme("ToggleButtonIcon");
            var toggle = Show(new ToggleButton { Theme = theme });

            Assert.Equal(Tokens.Get<double>("OpacityIconIdle"), Parts.Find<Border>(toggle, "border").Opacity);
        }

        [AvaloniaFact]
        public void TextBlock_Disabled_HasTheDisabledOpacity()
        {
            var text = Show(new TextBlock { Text = "A", IsEnabled = false });

            Assert.Equal(Tokens.Get<double>("OpacityDisabled"), text.Opacity);
        }
    }
}
