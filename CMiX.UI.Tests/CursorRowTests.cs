using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.AttachedProperties;
using Xunit;

namespace CMiX.UI.Tests
{
    // CursorRow.IsCursor sets the class cursor. A cursor row of a list shows CursorFrame: a thick bright frame while the list
    // has the focus, a thin grey frame when it does not. The fill of the row does not change.
    public class CursorRowTests
    {
        private static Color Brush(string key) => ((ISolidColorBrush)Application.Current!.FindResource(key)!).Color;

        private static Thickness Thickness(string key) => (Thickness)Application.Current!.FindResource(key)!;

        public static TheoryData<string> Lists => new() { "standard", "editable" };

        private static ListBox ListOf(string kind) => kind == "editable"
            ? new ListBox { Theme = (ControlTheme)Application.Current!.FindResource("ListBoxEditableList")! }
            : new ListBox();

        // A list with two rows and a button beside it that can take the focus.
        private static (ListBox List, Button Other) Show(string kind)
        {
            var list = ListOf(kind);
            list.ItemsSource = new[] { "One", "Two" };
            list.ItemTemplate = new FuncDataTemplate<string>((text, _) => new TextBlock { Text = text });
            var other = new Button { Content = "Other" };
            var panel = new StackPanel();
            panel.Children.Add(list);
            panel.Children.Add(other);
            new Window { Content = panel, Width = 300, Height = 300 }.Show();
            Dispatcher.UIThread.RunJobs();
            return (list, other);
        }

        private static ListBoxItem Row(ListBox list, int index) => list.GetVisualDescendants().OfType<ListBoxItem>().ElementAt(index);

        private static Border Frame(ListBoxItem row) => row.GetVisualDescendants().OfType<Border>().First(border => border.Name == "CursorFrame");

        [AvaloniaFact]
        public void IsCursor_SetAndCleared_SetsAndClearsThePseudoClass()
        {
            var row = new Border();

            CursorRow.SetIsCursor(row, true);
            Assert.Contains(CursorRow.CursorPseudoClass, row.Classes);

            CursorRow.SetIsCursor(row, false);
            Assert.DoesNotContain(CursorRow.CursorPseudoClass, row.Classes);
        }

        [AvaloniaTheory]
        [MemberData(nameof(Lists))]
        public void CursorFrame_RowIsNotTheCursor_IsHidden(string kind)
        {
            var (list, _) = Show(kind);

            Assert.False(Frame(Row(list, 0)).IsVisible);
        }

        [AvaloniaTheory]
        [MemberData(nameof(Lists))]
        public void CursorFrame_ListFocused_IsTheThickBrightFrame(string kind)
        {
            var (list, _) = Show(kind);
            var row = Row(list, 0);
            CursorRow.SetIsCursor(row, true);

            Row(list, 1).Focus();
            Dispatcher.UIThread.RunJobs();

            var frame = Frame(row);
            Assert.True(list.IsKeyboardFocusWithin);
            Assert.True(frame.IsVisible);
            Assert.Equal(Brush("CursorFrameBrush"), ((ISolidColorBrush)frame.BorderBrush!).Color);
            Assert.Equal(Thickness("CursorFrameThickness"), frame.BorderThickness);
        }

        [AvaloniaTheory]
        [MemberData(nameof(Lists))]
        public void CursorFrame_FocusOutsideTheList_IsTheThinGreyFrame(string kind)
        {
            var (list, other) = Show(kind);
            var row = Row(list, 0);
            CursorRow.SetIsCursor(row, true);

            other.Focus();
            Dispatcher.UIThread.RunJobs();

            var frame = Frame(row);
            Assert.False(list.IsKeyboardFocusWithin);
            Assert.True(frame.IsVisible);
            Assert.Equal(Brush("CursorFrameInactiveBrush"), ((ISolidColorBrush)frame.BorderBrush!).Color);
            Assert.Equal(Thickness("CursorFrameInactiveThickness"), frame.BorderThickness);
        }

        [AvaloniaTheory]
        [MemberData(nameof(Lists))]
        public void CursorFrame_CursorRow_KeepsTheTransparentFill(string kind)
        {
            var (list, _) = Show(kind);
            var row = Row(list, 0);

            CursorRow.SetIsCursor(row, true);
            Dispatcher.UIThread.RunJobs();

            var fill = row.GetVisualDescendants().OfType<Border>().First(border => border.Name == "Bd").Background as ISolidColorBrush;
            Assert.Equal(Colors.Transparent, fill?.Color);
        }
    }
}
