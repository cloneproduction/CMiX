using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // The marks of the keyboard focus: the accent border of a text box and the focus line of a panel.
    public class FocusLookTests
    {
        // A text box that has the keyboard focus has an accent border, the color of the focus line of a panel.

        private static (TextBox Box, Border Border) ShowTextBox(string? themeKey)
        {
            var box = new TextBox { Text = "Text", Width = 120, Height = 24 };
            if (themeKey != null)
                box.Theme = Tokens.Theme(themeKey);

            // The button takes the focus first, so the text box starts without it.
            var other = new Button { Content = "Other" };
            TestWindow.Show(new StackPanel { Children = { other, box } }, 300, 200);
            other.Focus();
            Dispatcher.UIThread.RunJobs();
            return (box, Parts.Find<Border>(box, "border"));
        }

        [AvaloniaTheory]
        [InlineData(null)]
        [InlineData("TextBoxNumbersOnly")]
        public void Focus_ATextBox_GivesItTheAccentBorder(string? themeKey)
        {
            var (box, border) = ShowTextBox(themeKey);
            Assert.NotEqual(Tokens.Color("AccentBrush"), ((ISolidColorBrush)border.BorderBrush!).Color);

            box.Focus();
            Dispatcher.UIThread.RunJobs();

            Assert.True(box.IsFocused);
            Assert.Equal(Tokens.Color("AccentBrush"), ((ISolidColorBrush)border.BorderBrush!).Color);
        }

        // CMiXTheme has the panel focus line: a Border.focusline in a .focuspanel shows only while the panel has the keyboard focus.

        private static (Border Line, TextBox Inside, Button Outside) ShowPanel()
        {
            var line = new Border { Classes = { "focusline" } };
            var inside = new TextBox();
            var panel = new StackPanel { Classes = { "focuspanel" } };
            panel.Children.Add(line);
            panel.Children.Add(inside);

            var outside = new Button { Content = "Other" };
            var root = new StackPanel();
            root.Children.Add(panel);
            root.Children.Add(outside);
            TestWindow.Show(root, 300, 200);
            return (line, inside, outside);
        }

        [AvaloniaFact]
        public void FocusLine_FocusInAndOut_ShowsTheAccentLineOnlyWithTheFocus()
        {
            var (line, inside, outside) = ShowPanel();

            outside.Focus();
            Dispatcher.UIThread.RunJobs();
            Assert.False(line.IsEffectivelyVisible);

            inside.Focus();
            Dispatcher.UIThread.RunJobs();
            Assert.True(line.IsEffectivelyVisible);
            Assert.Equal(Tokens.Get<double>("SplitterIndicatorSize"), line.Bounds.Height);
            Assert.Equal(Tokens.Color("AccentBrush"), ((ISolidColorBrush)line.Background!).Color);
        }
    }
}
