using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.Collection;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The list of rows in a popup. A long list scrolls inside the popup and does not leave the screen.
    public class ListItemPickerTests
    {
        private sealed record Item(string Name);

        private static double Token(string key) => System.Convert.ToDouble(Application.Current!.FindResource(key)!);

        private static (ListItemPicker List, ObservableCollection<Item> Items) Show(int count, ICommand? command = null)
        {
            var items = new ObservableCollection<Item>(Enumerable.Range(1, count).Select(i => new Item($"Entity {i}")));
            var list = new ListItemPicker { ItemsSource = items, TextPath = nameof(Item.Name), ItemCommand = command, Width = 200 };
            new Window { Content = list, Width = 400, Height = 1000 }.Show();
            Dispatcher.UIThread.RunJobs();
            return (list, items);
        }

        private static ScrollViewer Scroll(ListItemPicker list) =>
            list.GetVisualDescendants().OfType<ScrollViewer>().Single(scroll => scroll.Name == "PART_Scroll");

        private static IReadOnlyList<Button> Rows(ListItemPicker list) =>
            list.GetVisualDescendants().OfType<Button>().ToList();

        [AvaloniaFact]
        public void Show_ALongList_IsAsHighAsTheVisibleRowsAndScrolls()
        {
            var (list, _) = Show(40);

            var row = Token("ControlHeightMD");
            var visible = (int)Token("ComboBoxVisibleItems");
            var scroll = Scroll(list);

            Assert.Equal(visible * (row + 4) - 4, scroll.MaxHeight);
            Assert.True(scroll.Extent.Height > scroll.Viewport.Height);
            Assert.Equal(scroll.MaxHeight, scroll.Viewport.Height);
        }

        [AvaloniaFact]
        public void Show_AShortList_IsAsHighAsItsRowsAndDoesNotScroll()
        {
            var (list, _) = Show(3);

            var scroll = Scroll(list);

            Assert.Equal(scroll.Extent.Height, scroll.Viewport.Height);
            Assert.Equal(3, Rows(list).Count);
        }

        [AvaloniaFact]
        public void Show_ARow_ShowsTheTextOfThePath()
        {
            var (list, _) = Show(2);

            Assert.Equal(new object[] { "Entity 1", "Entity 2" }, Rows(list).Select(row => row.Content).ToArray());
        }

        [AvaloniaFact]
        public void Click_ARow_RunsTheCommandWithTheItem()
        {
            object? received = null;
            var (list, items) = Show(5, new RelayCommand<object>(item => received = item));

            Rows(list)[2].Command!.Execute(Rows(list)[2].CommandParameter);

            Assert.Same(items[2], received);
        }

        [AvaloniaFact]
        public void Add_AnItem_AddsARow()
        {
            var (list, items) = Show(2);

            items.Add(new Item("Entity 3"));
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(3, Rows(list).Count);
        }
    }
}
