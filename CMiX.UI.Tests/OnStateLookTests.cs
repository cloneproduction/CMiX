using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Mathematics;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // A control that is on and a row that is selected lie on the accent: AccentInactiveBrush at rest, StateOnHoverBrush under the pointer.
    // Their text is ActiveForegroundBrush, so it can be read on the two fills.
    public class OnStateLookTests
    {
        private static Color TextColorOf(Control control) =>
            ((ISolidColorBrush)control.GetVisualDescendants().OfType<TextBlock>().First().Foreground!).Color;

        private static Color FillOf(Control control, string part) =>
            ((ISolidColorBrush)control.GetVisualDescendants().OfType<Border>().First(border => border.Name == part).Background!).Color;

        [AvaloniaTheory]
        [InlineData("AccentInactiveBrush")]
        [InlineData("StateOnHoverBrush")]
        public void TheActiveText_OnTheFillOfAnOnState_CanBeRead(string fill)
        {
            Assert.True(ColorMix.Contrast(Tokens.Color("ActiveForegroundBrush"), Tokens.Color(fill)) >= 4.5);
        }

        [AvaloniaFact]
        public void Check_AToggleButton_MakesItsTextTheActiveText()
        {
            var toggle = new ToggleButton { Content = "Toggle", Width = 100 };
            TestWindow.Show(toggle, 300, 200);
            Assert.Equal(Tokens.Color("ForegroundBrush"), TextColorOf(toggle));

            toggle.IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(Tokens.Color("ActiveForegroundBrush"), TextColorOf(toggle));
            Assert.Equal(Tokens.Color("AccentInactiveBrush"), FillOf(toggle, "border"));
        }

        [AvaloniaFact]
        public void Hover_AToggleButtonThatIsOn_HasTheOnHoverFill()
        {
            var toggle = new ToggleButton { Content = "Toggle", Width = 100, IsChecked = true };
            TestWindow.Show(toggle, 300, 200);

            PseudoClassInput.Force(toggle, ":pointerover");

            Assert.Equal(Tokens.Color("StateOnHoverBrush"), FillOf(toggle, "border"));
            Assert.Equal(Tokens.Color("ActiveForegroundBrush"), TextColorOf(toggle));
        }

        [AvaloniaFact]
        public void Check_ARadioButton_MakesItsTextTheActiveText()
        {
            var radio = new RadioButton { Content = "Radio", Width = 100 };
            TestWindow.Show(radio, 300, 200);
            Assert.Equal(Tokens.Color("ForegroundBrush"), TextColorOf(radio));

            radio.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(Tokens.Color("ActiveForegroundBrush"), TextColorOf(radio));

            PseudoClassInput.Force(radio, ":pointerover");
            Assert.Equal(Tokens.Color("StateOnHoverBrush"), FillOf(radio, "radioButtonBorder"));
        }

        [AvaloniaFact]
        public void Select_ARowOfAList_MakesItsTextTheActiveText()
        {
            var list = new ListBox { ItemsSource = new[] { "First", "Second" }, Width = 140, Height = 80 };
            TestWindow.Show(list, 300, 200);
            list.SelectedIndex = 0;
            Dispatcher.UIThread.RunJobs();
            var rows = list.GetVisualDescendants().OfType<ListBoxItem>().ToList();

            Assert.Equal(Tokens.Color("ActiveForegroundBrush"), TextColorOf(rows[0]));
            Assert.Equal(Tokens.Color("ForegroundBrush"), TextColorOf(rows[1]));

            PseudoClassInput.Force(rows[0], ":pointerover");
            Assert.Equal(Tokens.Color("StateOnHoverBrush"), FillOf(rows[0], "Bd"));
        }

        [AvaloniaFact]
        public void Select_ARowOfAComboBox_MakesItsTextTheActiveText()
        {
            var item = new ComboBoxItem { Content = "Item", Width = 140 };
            TestWindow.Show(item, 300, 200);
            Assert.Equal(Tokens.Color("ForegroundBrush"), TextColorOf(item));

            item.IsSelected = true;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(Tokens.Color("ActiveForegroundBrush"), TextColorOf(item));

            PseudoClassInput.Force(item, ":pointerover");
            Assert.Equal(Tokens.Color("StateOnHoverBrush"), FillOf(item, "border"));
        }

        // The header of an expander is a toggle that is checked while the expander is open. It is not on the accent: its text does not change.
        [AvaloniaFact]
        public void Open_AnExpander_KeepsTheTextOfItsHeader()
        {
            var expander = new Expander { Header = "Header", Content = "Content", IsExpanded = true, Width = 200 };
            TestWindow.Show(expander, 300, 200);

            var header = expander.GetVisualDescendants().OfType<ToggleButton>().First();

            Assert.True(header.IsChecked);
            Assert.Equal(Tokens.Color("ForegroundBrush"), TextColorOf(header));
        }
    }
}
