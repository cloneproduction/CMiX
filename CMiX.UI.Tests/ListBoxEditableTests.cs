using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Behaviors;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Collection;
using CMiX.UI.Panels;
using Xunit;

namespace CMiX.UI.Tests
{
    // ListBoxEditable shows its items with the template of the application, and a button only when
    // the command of that button is set.
    public class ListBoxEditableTests
    {
        private sealed class RecordingCommand : ICommand
        {
            public event EventHandler? CanExecuteChanged { add { } remove { } }

            public bool CanExecute(object? parameter) => true;

            public void Execute(object? parameter) { }
        }

        private sealed class FakeReorder : IReorderService
        {
            public bool DragHandleIsPressed { get; set; }
            public bool CanDrop(int sourceIndex, int insertIndex) => true;
            public void Dropped(int sourceIndex, int insertIndex) { }
        }

        private static ListBoxEditable NewList(params string[] items) => new()
        {
            ItemsSource = new ObservableCollection<string>(items),
            ItemTemplate = new FuncDataTemplate<string>((text, _) => new TextBlock { Text = text }),
        };

        private static void Show(ListBoxEditable list)
        {
            new Window { Content = list, Width = 400, Height = 500 }.Show();
            Dispatcher.UIThread.RunJobs();
        }

        private static Button ButtonWith(ListBoxEditable list, string? content) =>
            list.GetVisualDescendants().OfType<Button>().First(b => Equals(b.Content, content));

        private static IEnumerable<Button> ArrowButtons(ListBoxEditable list) =>
            list.GetVisualDescendants().OfType<Button>()
                .Where(b => b.Content is AppIcon { IconKey: "ArrowUp" or "ArrowDown" });

        private static IEnumerable<Button> DragHandles(ListBoxEditable list) =>
            list.GetVisualDescendants().OfType<Button>()
                .Where(b => b.Content is AppIcon { IconKey: "DragHandle" });

        private static Point Position(Visual visual, Visual relativeTo) =>
            visual.TranslatePoint(new Point(0, 0), relativeTo)!.Value;

        [AvaloniaFact]
        public void TheItems_AreShownWithTheTemplate()
        {
            var list = NewList("one", "two", "three");

            Show(list);

            var texts = list.GetVisualDescendants().OfType<TextBlock>().Select(t => t.Text).ToList();
            Assert.Contains("one", texts);
            Assert.Contains("two", texts);
            Assert.Contains("three", texts);
        }

        [AvaloniaFact]
        public void AButton_ShowsOnlyWhenItsCommandIsSet()
        {
            var list = NewList("one");
            Show(list);

            Assert.False(ButtonWith(list, "+").IsVisible);
            Assert.False(ButtonWith(list, "-").IsVisible);
            Assert.All(ArrowButtons(list), b => Assert.False(b.IsVisible));

            list.AddCommand = new RecordingCommand();
            list.RemoveCommand = new RecordingCommand();
            list.MoveUpCommand = new RecordingCommand();
            list.MoveDownCommand = new RecordingCommand();
            Dispatcher.UIThread.RunJobs();

            Assert.True(ButtonWith(list, "+").IsVisible);
            Assert.True(ButtonWith(list, "-").IsVisible);
            Assert.Equal(2, ArrowButtons(list).Count());
            Assert.All(ArrowButtons(list), b => Assert.True(b.IsVisible));
        }

        [AvaloniaFact]
        public void TheAddButton_GetsItsCommandAndItsParameter()
        {
            var list = NewList("one");
            var add = new RecordingCommand();
            list.AddCommand = add;
            list.AddCommandParameter = typeof(string);
            Show(list);

            var button = ButtonWith(list, "+");
            Assert.Same(add, button.Command);
            Assert.Equal(typeof(string), button.CommandParameter);
        }

        [AvaloniaFact]
        public void TheRemoveButton_GetsItsCommandAndTheSelectedItem()
        {
            var list = NewList("one", "two");
            var remove = new RecordingCommand();
            list.RemoveCommand = remove;
            Show(list);

            list.SelectedItem = "two";
            Dispatcher.UIThread.RunJobs();

            var button = ButtonWith(list, "-");
            Assert.Same(remove, button.Command);
            Assert.Equal("two", button.CommandParameter);
        }

        [AvaloniaFact]
        public void TheButtons_AreInAColumnOnTheRightOfTheList()
        {
            var list = NewList("one");
            list.AddCommand = new RecordingCommand();
            list.RemoveCommand = new RecordingCommand();
            Show(list);
            var box = list.GetVisualDescendants().OfType<ListBox>().Single();

            var add = Position(ButtonWith(list, "+"), list);
            var remove = Position(ButtonWith(list, "-"), list);

            Assert.True(add.X >= box.Bounds.Right - 1);
            Assert.Equal(add.X, remove.X);
            Assert.True(remove.Y > add.Y);
        }

        private static CornerRadius CornersOf(Button button) =>
            button.GetVisualDescendants().OfType<Border>().First(b => b.Name == "border").CornerRadius;

        [AvaloniaFact]
        public void TheButtons_AreRoundedAtTheEndsOfTheirGroup()
        {
            var list = NewList("one");
            list.AddCommand = new RecordingCommand();
            list.RemoveCommand = new RecordingCommand();
            list.MoveUpCommand = new RecordingCommand();
            list.MoveDownCommand = new RecordingCommand();
            Show(list);

            var add = ButtonWith(list, "+");
            var remove = ButtonWith(list, "-");
            var up = ArrowButtons(list).First(b => ((AppIcon)b.Content!).IconKey == "ArrowUp");
            var down = ArrowButtons(list).First(b => ((AppIcon)b.Content!).IconKey == "ArrowDown");

            Assert.Equal(ControlPosition.Top, PositionedControl.GetPosition(add));
            Assert.Equal(ControlPosition.Bottom, PositionedControl.GetPosition(remove));
            Assert.Equal(ControlPosition.Top, PositionedControl.GetPosition(up));
            Assert.Equal(ControlPosition.Bottom, PositionedControl.GetPosition(down));

            Assert.Equal(new CornerRadius(2, 2, 0, 0), CornersOf(add));
            Assert.Equal(new CornerRadius(0, 0, 2, 2), CornersOf(remove));
            Assert.Equal(new CornerRadius(2, 2, 0, 0), CornersOf(up));
            Assert.Equal(new CornerRadius(0, 0, 2, 2), CornersOf(down));
        }

        [AvaloniaFact]
        public void ALoneButton_IsRoundedAllAround_AndAnEmptyGroupKeepsNoSpace()
        {
            var list = NewList("one");
            list.AddCommand = new RecordingCommand();
            Show(list);

            var add = ButtonWith(list, "+");
            Assert.Equal(ControlPosition.All, PositionedControl.GetPosition(add));
            Assert.Equal(new CornerRadius(2), CornersOf(add));
            Assert.False(ButtonWith(list, "-").IsVisible);
            Assert.All(ArrowButtons(list), b => Assert.False(b.IsVisible));

            // The remove button joins the add button: the corners follow.
            list.RemoveCommand = new RecordingCommand();
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(ControlPosition.Top, PositionedControl.GetPosition(add));
            Assert.Equal(ControlPosition.Bottom, PositionedControl.GetPosition(ButtonWith(list, "-")));
        }

        [AvaloniaFact]
        public void TheSelection_IsTwoWay()
        {
            var list = NewList("one", "two", "three");
            Show(list);
            var box = list.GetVisualDescendants().OfType<ListBox>().Single();

            box.SelectedIndex = 1;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("two", list.SelectedItem);

            list.SelectedItem = "three";
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(2, box.SelectedIndex);
        }

        [AvaloniaFact]
        public void ADragHandle_ShowsOnEachRowOnlyWhenAReorderServiceIsSet()
        {
            var list = NewList("one", "two");
            Show(list);

            Assert.All(DragHandles(list), h => Assert.False(h.IsVisible));

            list.ReorderService = new FakeReorder();
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(2, DragHandles(list).Count());
            Assert.All(DragHandles(list), h => Assert.True(h.IsVisible));
        }

        [AvaloniaFact]
        public void TheList_IsDark_AndHasNoScrollViewer()
        {
            var list = NewList("one", "two");
            Show(list);
            var box = list.GetVisualDescendants().OfType<ListBox>().Single();
            var dark = (ISolidColorBrush)Application.Current!.FindResource("ListBoxBackground")!;

            var background = (ISolidColorBrush)box.GetVisualChildren().OfType<Border>().Single().Background!;

            // A black with an opacity below 1 is darker than any panel behind it.
            Assert.Equal(dark.Color, background.Color);
            Assert.Equal(Colors.Black, background.Color);
            Assert.InRange(background.Opacity, 0.1, 0.9);
            Assert.Empty(list.GetVisualDescendants().OfType<ScrollViewer>());
        }

        [AvaloniaFact]
        public void ARow_IsTransparent_AndBlueWhenItIsSelected()
        {
            var list = NewList("one", "two");
            Show(list);
            var box = list.GetVisualDescendants().OfType<ListBox>().Single();
            var accent = (ISolidColorBrush)Application.Current!.FindResource("AccentInactiveBrush")!;

            var rows = box.GetVisualDescendants().OfType<ListBoxItem>().OrderBy(r => r.Bounds.Y).ToList();
            ISolidColorBrush? RowBackground(ListBoxItem row) =>
                row.GetVisualDescendants().OfType<Border>().First(b => b.Name == "Bd").Background as ISolidColorBrush;

            Assert.True(RowBackground(rows[0]) is null or { Color.A: 0 });

            box.SelectedIndex = 1;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(accent.Color, RowBackground(rows[1])!.Color);
            Assert.True(RowBackground(rows[0]) is null or { Color.A: 0 });
        }

        [AvaloniaFact]
        public void TheRows_AreTwoApart_AndTheHighlightHasTheSizeOfTheContent()
        {
            var list = NewList("one", "two");
            Show(list);
            var box = list.GetVisualDescendants().OfType<ListBox>().Single();
            var rows = box.GetVisualDescendants().OfType<ListBoxItem>().OrderBy(r => r.Bounds.Y).ToList();
            var text = rows[0].GetVisualDescendants().OfType<TextBlock>().Single();

            Assert.Equal(Tokens.Get<double>("ListEditableSpacing"), rows[1].Bounds.Y - rows[0].Bounds.Bottom, 0.5);
            Assert.Equal(text.Bounds.Height, rows[0].Bounds.Height, 0.5);
        }

        [AvaloniaFact]
        public void TheList_HasItsPadding_AndRoomForTheRowsOfTheTokenEvenWhenItIsEmpty()
        {
            var list = NewList("one");
            Show(list);
            var box = list.GetVisualDescendants().OfType<ListBox>().Single();
            var row = box.GetVisualDescendants().OfType<ListBoxItem>().Single();
            var padding = Tokens.Get<Thickness>("ListEditablePadding");
            var rows = Tokens.Get<int>("ListEditableVisibleItems");
            var room = rows * Tokens.RowHeight + (rows - 1) * Tokens.Get<double>("ListEditableSpacing") + padding.Top + padding.Bottom;

            Assert.Equal(padding.Left, Position(row, box).X, 0.5);
            Assert.Equal(padding.Top, Position(row, box).Y, 0.5);
            Assert.True(box.Bounds.Height >= room - 0.5);

            ((ObservableCollection<string>)list.ItemsSource!).Clear();
            Dispatcher.UIThread.RunJobs();

            Assert.True(box.Bounds.Height >= room - 0.5);
        }

        private static double RoomForRows(int rows)
        {
            var padding = Tokens.Get<Thickness>("ListEditablePadding");
            return rows * Tokens.RowHeight + (rows - 1) * Tokens.Get<double>("ListEditableSpacing") + padding.Top + padding.Bottom;
        }

        [AvaloniaFact]
        public void MinItemCount_SetsTheLeastNumberOfRows_AndZeroTakesTheToken()
        {
            var list = NewList("one");
            Show(list);
            var box = list.GetVisualDescendants().OfType<ListBox>().Single();

            Assert.Equal(0, list.MinItemCount);
            Assert.Equal(RoomForRows(Tokens.Get<int>("ListEditableVisibleItems")), box.MinHeight, 0.5);

            list.MinItemCount = 7;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(RoomForRows(7), box.MinHeight, 0.5);

            list.MinItemCount = 0;
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(RoomForRows(Tokens.Get<int>("ListEditableVisibleItems")), box.MinHeight, 0.5);
        }

        [AvaloniaFact]
        public void TheList_GrowsWithItsItems()
        {
            var list = NewList("one");
            // Rows of the height of a CMiX control, as a DragValue has.
            list.ItemTemplate = new FuncDataTemplate<string>((text, _) => new Border { Height = Tokens.RowHeight });
            Show(list);
            var box = list.GetVisualDescendants().OfType<ListBox>().Single();
            var before = box.Bounds.Height;

            // Four rows fit in the minimum height, so it grows only after the fourth.
            var items = (ObservableCollection<string>)list.ItemsSource!;
            foreach (var name in new[] { "two", "three", "four", "five", "six", "seven" })
                items.Add(name);
            Dispatcher.UIThread.RunJobs();
            box.UpdateLayout();

            Assert.True(box.Bounds.Height > before);
        }

        [AvaloniaFact]
        public void EachRow_KnowsItsNumber_AlsoAfterAMove()
        {
            var list = NewList("one", "two", "three");
            Show(list);
            var box = list.GetVisualDescendants().OfType<ListBox>().Single();

            void AssertNumbers()
            {
                var rows = box.GetVisualDescendants().OfType<ListBoxItem>().ToList();
                Assert.Equal(3, rows.Count);
                Assert.All(rows, row => Assert.Equal(box.IndexFromContainer(row) + 1, ListBoxEditable.GetRowNumber(row)));
            }

            AssertNumbers();

            ((ObservableCollection<string>)list.ItemsSource!).Move(0, 2);
            Dispatcher.UIThread.RunJobs();

            AssertNumbers();
        }

        [AvaloniaFact]
        public void TheDragHandle_IsAtTheEndOfTheRow()
        {
            var list = NewList("one");
            list.ReorderService = new FakeReorder();
            Show(list);

            var handle = DragHandles(list).Single();
            var text = list.GetVisualDescendants().OfType<TextBlock>().Single(t => t.Text == "one");

            Assert.True(Position(handle, list).X > Position(text, list).X);
        }
    }
}
