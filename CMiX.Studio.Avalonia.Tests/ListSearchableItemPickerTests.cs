using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.Collection;
using CommunityToolkit.Mvvm.Input;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The list with a search field. The rows are the items whose text holds the text of the field.
    public class ListSearchableItemPickerTests
    {
        private sealed record Item(string Name);

        private static readonly string[] Names = { "Cube", "Camera Rig", "Sphere", "Cylinder", "Plane" };

        private static (Window Window, ListSearchableItemPicker List, ObservableCollection<Item> Items) Show(Action<object?>? onPick = null)
        {
            var items = new ObservableCollection<Item>(Names.Select(name => new Item(name)));
            var list = new ListSearchableItemPicker
            {
                ItemsSource = items,
                TextPath = nameof(Item.Name),
                ItemCommand = onPick == null ? null : new RelayCommand<object>(item => onPick(item)),
                Width = 200
            };
            var window = new Window { Content = list, Width = 400, Height = 600 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (window, list, items);
        }

        private static TextBox Search(ListSearchableItemPicker list) =>
            list.GetVisualDescendants().OfType<TextBox>().Single(box => box.Name == "PART_Search");

        private static TextBlock NoMatch(ListSearchableItemPicker list) =>
            list.GetVisualDescendants().OfType<TextBlock>().Single(text => text.Name == "PART_NoMatch");

        private static string[] Rows(ListSearchableItemPicker list) =>
            list.GetVisualDescendants().OfType<Button>().Select(row => (string)row.Content!).ToArray();

        private static void Type(ListSearchableItemPicker list, string text)
        {
            Search(list).Text = text;
            Dispatcher.UIThread.RunJobs();
        }

        private static void Press(TextBox box, Key key)
        {
            box.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = key });
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void Type_NoText_ShowsEveryItem()
        {
            var (_, list, _) = Show();

            Assert.Equal(Names, Rows(list));
            Assert.False(NoMatch(list).IsVisible);
        }

        [AvaloniaFact]
        public void Type_SomeText_KeepsTheMatchingItemsInSourceOrder()
        {
            var (_, list, _) = Show();

            Type(list, "c");

            Assert.Equal(new[] { "Cube", "Camera Rig", "Cylinder" }, Rows(list));
        }

        [AvaloniaFact]
        public void Type_TextInAnotherCase_FindsTheItems()
        {
            var (_, list, _) = Show();

            Type(list, "CAM");

            Assert.Equal(new[] { "Camera Rig" }, Rows(list));
        }

        [AvaloniaFact]
        public void Type_TextThatNothingHolds_ShowsTheNoMatchLine()
        {
            var (_, list, _) = Show();

            Type(list, "zzz");

            Assert.Empty(Rows(list));
            Assert.True(NoMatch(list).IsVisible);
        }

        [AvaloniaFact]
        public void Show_AnEmptySource_HidesTheNoMatchLine()
        {
            var (_, list, items) = Show();

            items.Clear();
            Dispatcher.UIThread.RunJobs();

            Assert.False(NoMatch(list).IsVisible);
        }

        [AvaloniaFact]
        public void Add_AnItemWhileFiltered_ShowsItOnlyWhenItMatches()
        {
            var (_, list, items) = Show();
            Type(list, "cube");

            items.Add(new Item("Cube 2"));
            items.Add(new Item("Torus"));
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(new[] { "Cube", "Cube 2" }, Rows(list));
        }

        [AvaloniaFact]
        public void Press_Enter_PicksTheFirstRow()
        {
            object? picked = null;
            var (_, list, items) = Show(item => picked = item);
            Type(list, "cy");

            Press(Search(list), Key.Enter);

            Assert.Same(items[3], picked);
        }

        [AvaloniaFact]
        public void Enter_WithoutAMatch_PicksNothing()
        {
            object? picked = null;
            var (_, list, _) = Show(item => picked = item);
            Type(list, "zzz");

            Press(Search(list), Key.Enter);

            Assert.Null(picked);
        }

        [AvaloniaFact]
        public void Press_Escape_ClearsTheText()
        {
            var (_, list, _) = Show();
            Type(list, "cu");

            Press(Search(list), Key.Escape);

            Assert.Equal(string.Empty, Search(list).Text);
            Assert.Equal(Names, Rows(list));
        }

        [AvaloniaFact]
        public void Show_Again_StartsWithoutAFilter()
        {
            var (window, list, _) = Show();
            Type(list, "cu");

            window.Content = null;
            Dispatcher.UIThread.RunJobs();
            window.Content = list;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(string.Empty, Search(list).Text);
            Assert.Equal(Names, Rows(list));
        }

        private static Button[] HighlightedRows(ListSearchableItemPicker list) =>
            list.GetVisualDescendants().OfType<Button>().Where(row => row.Classes.Contains("highlighted")).ToArray();

        [AvaloniaFact]
        public void Show_ByDefault_HighlightsTheFirstRow()
        {
            var (_, list, _) = Show();

            Assert.Equal(0, list.HighlightedIndex);
            Assert.Equal("Cube", (string)Assert.Single(HighlightedRows(list)).Content!);
        }

        [AvaloniaFact]
        public void Down_MovesTheHighlight_AndEnterPicksThatRow()
        {
            object? picked = null;
            var (_, list, items) = Show(item => picked = item);

            Press(Search(list), Key.Down);
            Press(Search(list), Key.Down);

            Assert.Equal("Sphere", (string)Assert.Single(HighlightedRows(list)).Content!);

            Press(Search(list), Key.Enter);

            Assert.Same(items[2], picked);
        }

        [AvaloniaFact]
        public void Press_Up_MovesTheHighlightBack()
        {
            var (_, list, _) = Show();

            Press(Search(list), Key.Down);
            Press(Search(list), Key.Down);
            Press(Search(list), Key.Up);

            Assert.Equal(1, list.HighlightedIndex);
        }

        [AvaloniaFact]
        public void Press_UpOrDownAtTheEnds_StopsTheHighlight()
        {
            var (_, list, _) = Show();

            Press(Search(list), Key.Up);
            Assert.Equal(0, list.HighlightedIndex);

            for (var i = 0; i < 20; i++)
                Press(Search(list), Key.Down);
            Assert.Equal(Names.Length - 1, list.HighlightedIndex);
        }

        [AvaloniaFact]
        public void Press_Down_MovesInsideTheRowsThatMatch()
        {
            object? picked = null;
            var (_, list, items) = Show(item => picked = item);
            Type(list, "c");

            Press(Search(list), Key.Down);
            Press(Search(list), Key.Enter);

            Assert.Equal(new[] { "Cube", "Camera Rig", "Cylinder" }, Rows(list));
            Assert.Same(items[1], picked);
        }

        [AvaloniaFact]
        public void Type_AgainAfterAMove_HighlightsTheFirstRow()
        {
            var (_, list, _) = Show();
            Press(Search(list), Key.Down);
            Press(Search(list), Key.Down);

            Type(list, "c");

            Assert.Equal(0, list.HighlightedIndex);
            Assert.Equal("Cube", (string)Assert.Single(HighlightedRows(list)).Content!);
        }

        [AvaloniaFact]
        public void Type_TextWithoutAMatch_HighlightsNothingAndDisablesTheKeys()
        {
            object? picked = null;
            var (_, list, _) = Show(item => picked = item);
            Type(list, "zzz");

            Press(Search(list), Key.Down);
            Press(Search(list), Key.Enter);

            Assert.Equal(-1, list.HighlightedIndex);
            Assert.Empty(HighlightedRows(list));
            Assert.Null(picked);
        }

        [AvaloniaFact]
        public void Press_DownPastTheView_ScrollsTheRowIntoView()
        {
            var items = new ObservableCollection<Item>(Enumerable.Range(1, 40).Select(i => new Item($"Entity {i}")));
            var list = new ListSearchableItemPicker { ItemsSource = items, TextPath = nameof(Item.Name), Width = 200 };
            new Window { Content = list, Width = 400, Height = 800 }.Show();
            Dispatcher.UIThread.RunJobs();

            for (var i = 0; i < 15; i++)
                Press(Search(list), Key.Down);
            Dispatcher.UIThread.RunJobs();

            var scroll = list.GetVisualDescendants().OfType<ScrollViewer>().Single(s => s.Name == "PART_Scroll");
            var row = Assert.Single(HighlightedRows(list));
            var top = row.TranslatePoint(new Point(0, 0), scroll)!.Value.Y;

            Assert.Equal("Entity 16", (string)row.Content!);
            Assert.True(top >= -0.5 && top + row.Bounds.Height <= scroll.Viewport.Height + 0.5, $"The row is at {top} in a viewport of {scroll.Viewport.Height}.");
        }

        [AvaloniaFact]
        public void Show_TheSearchIcon_IsInsideTheFieldBeforeTheText()
        {
            var (_, list, _) = Show();
            var box = Search(list);
            var icon = list.GetVisualDescendants().OfType<CMiX.UI.Controls.AppIcon>().Single(i => i.Name == "PART_SearchIcon");

            var origin = icon.TranslatePoint(new Point(0, 0), box)!.Value;

            Assert.Equal("Search", icon.IconKey);
            Assert.True(origin.X >= 0 && origin.X + icon.Bounds.Width <= box.Bounds.Width);
            Assert.True(origin.Y >= 0 && origin.Y + icon.Bounds.Height <= box.Bounds.Height);
            Assert.True(box.Padding.Left >= origin.X + icon.Bounds.Width, "The text would start under the icon.");
            Assert.False(icon.IsHitTestVisible);
        }

        // The keys go through the window, as a user's keys do.
        [AvaloniaFact]
        public void RealKeys_AfterTypingText_MoveTheHighlightBothWays()
        {
            object? picked = null;
            var (window, list, items) = Show(item => picked = item);
            window.KeyTextInput("c");
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(new[] { "Cube", "Camera Rig", "Cylinder" }, Rows(list));

            window.KeyPressQwerty(PhysicalKey.ArrowDown, RawInputModifiers.None);
            Assert.Equal(1, list.HighlightedIndex);

            window.KeyPressQwerty(PhysicalKey.ArrowDown, RawInputModifiers.None);
            Assert.Equal(2, list.HighlightedIndex);

            window.KeyPressQwerty(PhysicalKey.ArrowUp, RawInputModifiers.None);
            Assert.Equal(1, list.HighlightedIndex);

            window.KeyPressQwerty(PhysicalKey.ArrowUp, RawInputModifiers.None);
            Assert.Equal(0, list.HighlightedIndex);

            window.KeyPressQwerty(PhysicalKey.ArrowDown, RawInputModifiers.None);
            window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
            Assert.Same(items[1], picked);
        }

        private static Button RowButton(ListSearchableItemPicker list, int index) =>
            list.GetVisualDescendants().OfType<Button>().ElementAt(index);

        private static Color BackgroundOf(Button row)
        {
            var border = row.GetVisualDescendants().OfType<Border>().First(b => b.Name == "border");
            return (border.Background as ISolidColorBrush)?.Color ?? Colors.Transparent;
        }

        // The pointer moves onto the row, then the left button goes down and up, as the user does it.
        private static void Click(Window window, Button row)
        {
            var point = row.TranslatePoint(new Point(10, 10), window)!.Value;
            window.MouseMove(point);
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
        }

        private static void MouseOver(Window window, Button row)
        {
            window.MouseMove(row.TranslatePoint(new Point(10, 10), window)!.Value);
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void Hover_ARow_MovesTheHighlightAndEnterPicksIt()
        {
            object? picked = null;
            var (window, list, items) = Show(item => picked = item);

            MouseOver(window, RowButton(list, 3));

            Assert.Equal(3, list.HighlightedIndex);
            Assert.Equal("Cylinder", (string)Assert.Single(HighlightedRows(list)).Content!);

            window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
            Assert.Same(items[3], picked);
        }

        [AvaloniaFact]
        public void Press_ArrowsUnderAStillPointer_LightsOnlyOneRow()
        {
            var (window, list, _) = Show();
            MouseOver(window, RowButton(list, 0));

            window.KeyPressQwerty(PhysicalKey.ArrowDown, RawInputModifiers.None);
            window.KeyPressQwerty(PhysicalKey.ArrowDown, RawInputModifiers.None);
            Dispatcher.UIThread.RunJobs();

            var underPointer = RowButton(list, 0);
            var plain = RowButton(list, 4);
            var highlighted = Assert.Single(HighlightedRows(list));

            Assert.True(underPointer.IsPointerOver);
            Assert.Equal("Sphere", (string)highlighted.Content!);
            Assert.Equal(BackgroundOf(plain), BackgroundOf(underPointer));
            Assert.NotEqual(BackgroundOf(plain), BackgroundOf(highlighted));
        }

        [AvaloniaFact]
        public void Press_Enter_RunsTheCommandAndThenRaisesPicked()
        {
            var order = new List<string>();
            var (_, list, _) = Show(_ => order.Add("command"));
            list.Picked += (_, _) => order.Add("picked");

            Press(Search(list), Key.Enter);

            Assert.Equal(new[] { "command", "picked" }, order);
        }

        [AvaloniaFact]
        public void Enter_WithoutAMatch_DoesNotRaisePicked()
        {
            var picks = 0;
            var (_, list, _) = Show(_ => { });
            list.Picked += (_, _) => picks++;
            Type(list, "zzz");

            Press(Search(list), Key.Enter);

            Assert.Equal(0, picks);
        }

        [AvaloniaFact]
        public void Click_ARow_RunsTheCommandAndThenRaisesPicked()
        {
            var order = new List<string>();
            var (window, list, _) = Show(_ => order.Add("command"));
            list.Picked += (_, _) => order.Add("picked");

            Click(window, RowButton(list, 1));

            Assert.Equal(new[] { "command", "picked" }, order);
        }

        [AvaloniaFact]
        public void Show_TheList_GivesTheFocusToTheField()
        {
            var (_, list, _) = Show();
            Dispatcher.UIThread.RunJobs();

            Assert.True(Search(list).IsFocused);
        }

        // A source that counts the handlers on its change event.
        private sealed class CountingSource : IEnumerable<Item>, INotifyCollectionChanged
        {
            private NotifyCollectionChangedEventHandler? changed;
            public List<Item> Items { get; } = new() { new Item("Cube"), new Item("Sphere") };
            public int HandlerCount => changed?.GetInvocationList().Length ?? 0;

            public event NotifyCollectionChangedEventHandler? CollectionChanged
            {
                add => changed += value;
                remove => changed -= value;
            }

            public void Add(Item item)
            {
                Items.Add(item);
                changed?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            public IEnumerator<Item> GetEnumerator() => Items.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [AvaloniaFact]
        public void Detach_WhileWatchingAList_RemovesTheHandler()
        {
            var source = new CountingSource();
            var list = new ListSearchableItemPicker { ItemsSource = source, TextPath = nameof(Item.Name) };
            var window = new Window { Content = list, Width = 400, Height = 600 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(1, source.HandlerCount);

            window.Content = null;
            Dispatcher.UIThread.RunJobs();
            source.Add(new Item("Plane"));

            Assert.Equal(0, source.HandlerCount);
            Assert.Equal(2, list.Matches.Count);
        }

        [AvaloniaFact]
        public void Attach_Again_WatchesTheListAgain()
        {
            var source = new CountingSource();
            var list = new ListSearchableItemPicker { ItemsSource = source, TextPath = nameof(Item.Name) };
            var window = new Window { Content = list, Width = 400, Height = 600 };
            window.Show();
            window.Content = null;
            Dispatcher.UIThread.RunJobs();
            source.Add(new Item("Plane"));

            window.Content = list;
            Dispatcher.UIThread.RunJobs();
            source.Add(new Item("Cone"));

            Assert.Equal(1, source.HandlerCount);
            Assert.Equal(4, list.Matches.Count);
        }
    }
}
