using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
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
    // The look of a row in every list: the fill states, the height, the inset of the content and the corners.
    // The rows of the standard list and of the editable list share one base theme, ListBoxItemBasic.
    public class RowLookTests
    {
        // Every row of a list has the same states: transparent, grey with the pointer over, blue when selected.
        // The rows of the standard list and of the editable list share one base theme, ListBoxItemBasic.

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
        public void Hover_ARow_MakesItGrey(string kind)
        {
            var row = FirstRow(ListOf(kind));

            Pointer(row, true);

            Assert.Equal(Tokens.Color("RowHoverBrush"), Background(row));
        }

        [AvaloniaTheory]
        [MemberData(nameof(Lists))]
        public void Select_ARow_MakesItInactiveBlue(string kind)
        {
            var row = FirstRow(ListOf(kind));

            row.IsSelected = true;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(Tokens.Color("AccentInactiveBrush"), Background(row));
        }

        [AvaloniaTheory]
        [MemberData(nameof(Lists))]
        public void Hover_ASelectedRow_MakesItBlue(string kind)
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

        // Every row starts its content 8 from its left edge and has the round corners of the theme: a list row and a combo row.
        // ContextMenuLookTests tests the menu row.

        private static T Show<T>(T control) where T : Control
        {
            TestWindow.Show(control, 300, 200);
            return control;
        }

        private static CornerRadius Round => Tokens.Get<CornerRadius>("CornerRadiusAll");

        [AvaloniaFact]
        public void Show_ARowOfAList_StartsItsContentAtEight()
        {
            var list = Show(new ListBox { ItemsSource = new[] { "Row" }, Width = 140, Height = 40 });

            var row = list.GetVisualDescendants().OfType<ListBoxItem>().First();
            var border = Parts.Find<Border>(row, "Bd");

            Assert.Equal(8, border.Padding.Left);
            Assert.Equal(Round, border.CornerRadius);
        }

        [AvaloniaFact]
        public void Show_ARowOfAComboBox_StartsItsContentAtEight()
        {
            var item = Show(new ComboBoxItem { Content = "Item", Width = 140 });

            var presenter = Parts.Find<ContentPresenter>(item, "PART_ContentPresenter");
            var border = Parts.Find<Border>(item, "border");

            Assert.Equal(8, presenter.Margin.Left);
            Assert.Equal(Round, border.CornerRadius);
        }
    }
}
