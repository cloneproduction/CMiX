using Avalonia.Controls;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // A ComboBox with a search field filters its rows by their text, in any case, and Enter picks the first row that is left.
    public class ComboBoxSearchTests
    {
        private static readonly string[] Modes = { "Add", "Multiply", "Screen", "Overlay", "Soft Light" };

        private static (Window Window, ComboBox Combo) Show(bool searchable)
        {
            var combo = new ComboBox { ItemsSource = Modes, Width = 200 };
            ComboBoxSearch.SetIsSearchable(combo, searchable);
            var window = TestWindow.Show(combo, 400, 400);
            combo.IsDropDownOpen = true;
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            return (window, combo);
        }

        private static TextBoxSearch BoxOf(ComboBox combo) =>
            ((Visual)combo.GetVisualDescendants().OfType<Popup>().Single().Child!).GetVisualDescendants().OfType<TextBoxSearch>().Single(box => box.Name == ComboBoxSearch.PartName);

        private static List<string> VisibleRows(ComboBox combo) =>
            Enumerable.Range(0, combo.ItemCount)
                .Where(index => combo.ContainerFromIndex(index)?.IsVisible == true)
                .Select(index => (string)combo.Items[index]!)
                .ToList();

        [AvaloniaFact]
        public void Open_WhenNotSearchable_HidesTheSearchField()
        {
            var (_, combo) = Show(searchable: false);

            Assert.False(BoxOf(combo).IsVisible);
        }

        [AvaloniaFact]
        public void Open_WhenSearchable_ShowsTheSearchFieldAndEveryRow()
        {
            var (_, combo) = Show(searchable: true);

            Assert.True(BoxOf(combo).IsVisible);
            Assert.Equal(Modes, VisibleRows(combo));
        }

        [AvaloniaFact]
        public void Type_InTheSearchField_KeepsTheRowsThatHoldTheText()
        {
            var (_, combo) = Show(searchable: true);

            BoxOf(combo).Text = "I";
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(new[] { "Multiply", "Soft Light" }, VisibleRows(combo));
        }

        [AvaloniaFact]
        public void Open_Again_StartsWithAnEmptyField()
        {
            var (_, combo) = Show(searchable: true);
            BoxOf(combo).Text = "add";
            Dispatcher.UIThread.RunJobs();

            combo.IsDropDownOpen = false;
            combo.IsDropDownOpen = true;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(string.Empty, BoxOf(combo).Text);
            Assert.Equal(Modes, VisibleRows(combo));
        }


        private static void Press(ComboBox combo, Key key) => KeyInput.Press(BoxOf(combo), key);

        [AvaloniaFact]
        public void Press_DownAfterASearch_MovesTheHighlightAndEnterPicksThatRow()
        {
            var (_, combo) = Show(searchable: true);
            BoxOf(combo).Text = "o";
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(new[] { "Overlay", "Soft Light" }, VisibleRows(combo));
            Assert.Contains("highlighted", combo.ContainerFromIndex(3)!.Classes);

            Press(combo, Key.Down);
            Assert.DoesNotContain("highlighted", combo.ContainerFromIndex(3)!.Classes);
            Assert.Contains("highlighted", combo.ContainerFromIndex(4)!.Classes);

            Press(combo, Key.Down);
            Assert.Contains("highlighted", combo.ContainerFromIndex(4)!.Classes);

            Press(combo, Key.Up);
            Assert.Contains("highlighted", combo.ContainerFromIndex(3)!.Classes);

            Press(combo, Key.Down);
            Press(combo, Key.Enter);

            Assert.Equal("Soft Light", combo.SelectedItem);
            Assert.False(combo.IsDropDownOpen);
        }

        [AvaloniaFact]
        public void Press_EnterWithoutAnArrow_PicksTheFirstRowThatIsLeft()
        {
            var (_, combo) = Show(searchable: true);
            BoxOf(combo).Text = "sc";
            Dispatcher.UIThread.RunJobs();

            Press(combo, Key.Enter);

            Assert.Equal("Screen", combo.SelectedItem);
        }
        [AvaloniaFact]
        public void Matches_NoText_KeepsEveryItem()
        {
            Assert.True(ComboBoxSearch.Matches("Add", null));
            Assert.True(ComboBoxSearch.Matches("Add", ""));
            Assert.False(ComboBoxSearch.Matches("Add", "z"));
        }
    }
}
