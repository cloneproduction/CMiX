using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Boolean;
using CMiX.UI.Controls.Collection;
using CMiX.UI.Controls.Number;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // The opacity tokens in use: OpacityIconAction for the icon of a button that does an action, OpacityIconIdle
    // for a quiet icon, OpacityDisabled for a disabled control.
    public class OpacityTokensTests
    {
        // An icon has one of two opacities: OpacityIconAction for the icon of a button that does an action, OpacityIconIdle for a quiet icon.

        private static T ShowAtTheLeft<T>(T control) where T : Control
        {
            TestWindow.Show(new StackPanel { HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left, Children = { control } }, 300, 200);
            return control;
        }

        private static AppIcon IconOf(Control control) => control.GetVisualDescendants().OfType<AppIcon>().First();

        [AvaloniaFact]
        public void Show_AButtonIcon_HasTheActionOpacity()
        {
            var button = ShowAtTheLeft(new ButtonIcon { IconKey = "Add" });

            Assert.Equal(Tokens.Get<double>("OpacityIconAction"), IconOf(button).Opacity);
        }

        [AvaloniaFact]
        public void Show_AWindowButtonIcon_HasTheIdleOpacityAndTheLargeSize()
        {
            var button = ShowAtTheLeft(new ButtonIcon { IconKey = "Close", Classes = { "window" } });

            Assert.Equal(Tokens.Get<double>("OpacityIconIdle"), IconOf(button).Opacity);
            Assert.Equal(Tokens.Get<double>("ControlHeightXL"), button.Bounds.Width);
            Assert.Equal(Tokens.Get<double>("ControlHeightXL"), button.Bounds.Height);
        }

        [AvaloniaFact]
        public void Show_AHandleButtonIcon_HasTheHandCursor()
        {
            var button = ShowAtTheLeft(new ButtonIcon { IconKey = "DragHandle", Classes = { "handle" } });

            Assert.NotNull(button.Cursor);
            Assert.Equal(Tokens.Get<double>("OpacityIconAction"), IconOf(button).Opacity);
        }

        [AvaloniaTheory]
        [InlineData("SubButton")]
        [InlineData("AddButton")]
        public void Show_AStepButtonOfADragValue_HasTheActionOpacity(string name)
        {
            var drag = ShowAtTheLeft(new DragValue { Minimum = 0, Maximum = 1, Value = 0.5, Width = 160 });
            var button = drag.GetVisualDescendants().OfType<Button>().First(part => part.Name == name);
            button.IsVisible = true;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(Tokens.Get<double>("OpacityIconAction"), IconOf(button).Opacity);
        }

        [AvaloniaFact]
        public void Show_AListItemIcon_HasTheIdleOpacity()
        {
            var item = ShowAtTheLeft(new ListItemIcon { Icon = "Folder", Title = "Item" });

            Assert.Equal(Tokens.Get<double>("OpacityIconIdle"), IconOf(item).Opacity);
        }

        [AvaloniaFact]
        public void Show_TheIconOfATextBoxSearch_HasTheIdleOpacity()
        {
            var box = ShowAtTheLeft(new TextBoxSearch { Width = 160 });

            Assert.Equal(Tokens.Get<double>("OpacityIconIdle"), IconOf(box).Opacity);
        }

        // The styles take their opacities from the tokens: a disabled control has OpacityDisabled, a dim icon has OpacityIconIdle.

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
