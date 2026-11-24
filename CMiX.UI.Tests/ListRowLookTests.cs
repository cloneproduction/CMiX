using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // Every row of a list has the same states: transparent, grey with the pointer over, blue when selected.
    // The rows of the standard list and of the editable list share one base theme, ListBoxItemBasic.
    public class ListRowLookTests
    {
        private static ListBoxItem FirstRow(ListBox list)
        {
            list.ItemsSource = new[] { "One", "Two" };
            list.ItemTemplate = new FuncDataTemplate<string>((text, _) => new TextBlock { Text = text });
            TestWindow.Show(list, 300, 300);
            return list.GetVisualDescendants().OfType<ListBoxItem>().First();
        }

        private static Color? Background(ListBoxItem row) => (Parts.Find<Border>(row, "Bd").Background as ISolidColorBrush)?.Color;

        private static void Pointer(ListBoxItem row, bool over)
        {
            PseudoClassInput.Set(row, ":pointerover", over);
            Dispatcher.UIThread.RunJobs();
        }

        private static ListBox EditableList() => new() { Theme = Tokens.Theme("ListBoxEditableList") };

        public static TheoryData<string> Lists => new() { "standard", "editable" };

        private static ListBox ListOf(string kind) => kind == "editable" ? EditableList() : new ListBox();

        [AvaloniaTheory]
        [MemberData(nameof(Lists))]
        public void ARow_IsTransparent_UntilThePointerOrTheSelectionTouchesIt(string kind)
        {
            var row = FirstRow(ListOf(kind));

            Assert.Equal(Colors.Transparent, Background(row));
        }

        [AvaloniaTheory]
        [MemberData(nameof(Lists))]
        public void ARowWithThePointerOver_IsGrey(string kind)
        {
            var row = FirstRow(ListOf(kind));

            Pointer(row, true);

            Assert.Equal(Tokens.Color("RowHoverBrush"), Background(row));
        }

        [AvaloniaTheory]
        [MemberData(nameof(Lists))]
        public void ASelectedRow_IsInactiveBlue(string kind)
        {
            var row = FirstRow(ListOf(kind));

            row.IsSelected = true;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(Tokens.Color("AccentInactiveBrush"), Background(row));
        }

        [AvaloniaTheory]
        [MemberData(nameof(Lists))]
        public void ASelectedRowWithThePointerOver_IsBlue(string kind)
        {
            var row = FirstRow(ListOf(kind));

            row.IsSelected = true;
            Pointer(row, true);

            Assert.Equal(Tokens.Color("StateOnHoverBrush"), Background(row));
        }

        [AvaloniaFact]
        public void TheStandardRow_IsOneControlHeightHigh_AndTheEditableRowIsAsHighAsItsContent()
        {
            var standard = FirstRow(new ListBox());
            var editable = FirstRow(EditableList());

            Assert.Equal(Tokens.RowHeight, standard.Bounds.Height);
            Assert.True(editable.Bounds.Height < Tokens.RowHeight);
        }
    }
}
