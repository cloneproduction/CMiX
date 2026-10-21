using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Collection;
using Xunit;

namespace CMiX.UI.Tests
{
    // The lists of CMiX show a whole number of rows, and the wheel moves them by one row.
    public class ListScrollStepTests
    {
        private static double RowHeight => Tokens.RowHeight;

        private static T Show<T>(T list, int itemCount = 30) where T : ListBox
        {
            list.ItemsSource = Enumerable.Range(1, itemCount).Select(i => "Item " + i).ToList();
            list.ItemTemplate = new FuncDataTemplate<string>((text, _) => new TextBlock { Text = text });
            new Window { Content = list, Width = 400, Height = 800 }.Show();
            Dispatcher.UIThread.RunJobs();
            return list;
        }

        private static ScrollViewer ScrollOf(Visual list) => list.GetVisualDescendants().OfType<ScrollViewer>().First();

        private static bool Wheel(ScrollViewer scroll, double delta)
        {
            var presenter = scroll.GetVisualDescendants().OfType<ScrollContentPresenter>().First();
            var args = new PointerWheelEventArgs(
                presenter, new Pointer(Pointer.GetNextFreeId(), PointerType.Mouse, true), TopLevel.GetTopLevel(scroll)!,
                new Point(20, 20), 0, new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.Other),
                KeyModifiers.None, new Vector(0, delta))
            { RoutedEvent = InputElement.PointerWheelChangedEvent };

            presenter.RaiseEvent(args);
            Dispatcher.UIThread.RunJobs();
            return args.Handled;
        }

        [AvaloniaFact]
        public void AListBoxCMiX_WithFiveVisibleItems_ShowsFiveWholeRows()
        {
            var list = Show(new ListBoxCMiX { MinItemCount = Tokens.ListVisibleItems });
            var scroll = ScrollOf(list);

            Assert.Equal(Tokens.ListVisibleItems * RowHeight, scroll.Viewport.Height);
            Assert.Equal(Tokens.ListHeight(Tokens.ListVisibleItems), list.MinHeight);
        }

        [AvaloniaFact]
        public void AListBoxCMiX_WithoutVisibleItems_KeepsItsMinHeight()
        {
            // A height that is not a whole number of rows, to see that the list leaves it alone.
            var minimum = Tokens.ListHeight(3) + 1;
            var list = Show(new ListBoxCMiX { MinHeight = minimum });

            Assert.Equal(minimum, list.MinHeight);
            Assert.Equal(0, list.MinItemCount);
        }

        [AvaloniaFact]
        public void AListBoxRepository_ShowsFiveWholeRows()
        {
            var detail = new ListBoxRepository
            {
                ItemsSource = Enumerable.Range(1, 30).Select(i => "Item " + i).ToList(),
            };
            new Window { Content = detail, Width = 400, Height = 800 }.Show();
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(Tokens.ListVisibleItems * RowHeight, ScrollOf(detail).Viewport.Height);
        }

        [AvaloniaFact]
        public void TheWheel_MovesAListBoxCMiXByOneRow()
        {
            var scroll = ScrollOf(Show(new ListBoxCMiX { MinItemCount = Tokens.ListVisibleItems }));

            Wheel(scroll, -1);
            Assert.Equal(RowHeight, scroll.Offset.Y);

            Wheel(scroll, -1);
            Wheel(scroll, -1);
            Assert.Equal(3 * RowHeight, scroll.Offset.Y);
        }

        [AvaloniaFact]
        public void TheWheel_MovesAPlainListBoxByOneRow()
        {
            var list = Show(new ListBox { MaxHeight = Tokens.ListHeight(Tokens.ListVisibleItems) });
            var scroll = ScrollOf(list);

            Wheel(scroll, -1);

            Assert.Equal(RowHeight, scroll.Offset.Y);
        }

        [AvaloniaFact]
        public void AtTheEndOfTheList_TheWheelGoesOnToThePanelAround()
        {
            var scroll = ScrollOf(Show(new ListBox { MaxHeight = Tokens.ListHeight(Tokens.ListVisibleItems) }));

            for (var i = 0; i < 40; i++)
                Wheel(scroll, -1);
            var atTheEnd = scroll.Offset.Y;

            Assert.True(atTheEnd > 0);
            Assert.False(Wheel(scroll, -1));
            Assert.Equal(atTheEnd, scroll.Offset.Y);

            // The other way, the wheel is taken again.
            Assert.True(Wheel(scroll, 1));
        }

        [AvaloniaFact]
        public void AListThatDoesNotScroll_LetsTheWheelGoOnToThePanelAround()
        {
            var scroll = ScrollOf(Show(new ListBox { MaxHeight = Tokens.ListHeight(Tokens.ListVisibleItems) }, itemCount: 3));

            Assert.False(Wheel(scroll, -1));
            Assert.Equal(0, scroll.Offset.Y);
        }

        // ListBoxCMiX forwards the wheel to the scroll viewer around it when it is at its start or end.
        // The step of a row must not stop this.
        [AvaloniaFact]
        public void AtTheEndOfAListBoxCMiX_ThePanelAroundScrolls()
        {
            var list = new ListBoxCMiX { MinItemCount = Tokens.ListVisibleItems };
            list.ItemsSource = Enumerable.Range(1, 30).Select(i => "Item " + i).ToList();
            list.ItemTemplate = new FuncDataTemplate<string>((text, _) => new TextBlock { Text = text });
            var around = new ScrollViewer
            {
                Content = new StackPanel { Children = { new Border { Height = 1500 }, list } },
            };
            new Window { Content = around, Width = 400, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();
            var scroll = ScrollOf(list);

            for (var i = 0; i < 40; i++)
                Wheel(scroll, -1);
            var panelBefore = around.Offset.Y;

            Wheel(scroll, -1);

            Assert.True(around.Offset.Y > panelBefore, $"panel {panelBefore} -> {around.Offset.Y}");
        }

        private static Border HeightBorderOf(ListBoxCMiX list) =>
            ScrollOf(list).GetVisualAncestors().OfType<Border>().First(b => b.Name == "Bd");

        // Drags the strip under the list by the given distance and returns the height of the list.
        private static double Resize(double distance)
        {
            var list = new ListBoxCMiX { MinItemCount = Tokens.ListVisibleItems };
            list.ItemsSource = Enumerable.Range(1, 30).Select(i => "Item " + i).ToList();
            list.ItemTemplate = new FuncDataTemplate<string>((text, _) => new TextBlock { Text = text });
            var window = new Window { Content = list, Width = 400, Height = 1500 };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var strip = list.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "resizeBorder");
            var start = strip.TranslatePoint(new Point(strip.Bounds.Width / 2, strip.Bounds.Height / 2), window)!.Value;

            window.MouseDown(start, MouseButton.Left);
            window.MouseMove(new Point(start.X, start.Y + distance));
            Dispatcher.UIThread.RunJobs();
            window.MouseUp(new Point(start.X, start.Y + distance), MouseButton.Left);
            Dispatcher.UIThread.RunJobs();

            return HeightBorderOf(list).Height;
        }

        [AvaloniaFact]
        public void AListBoxCMiX_HasAResizeStripUnderTheList()
        {
            var list = Show(new ListBoxCMiX { MinItemCount = Tokens.ListVisibleItems });

            var strip = list.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "resizeBorder");

            Assert.Equal(Tokens.Get<double>("ListResizeHandleHeight"), strip.Bounds.Height);
            Assert.True(strip.Bounds.Y >= HeightBorderOf(list).Bounds.Bottom - 0.5);
        }

        [AvaloniaTheory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(37)]
        [InlineData(100)]
        [InlineData(251)]
        public void DraggingTheStrip_MakesTheListAWholeNumberOfRowsTaller(double distance)
        {
            var height = Resize(distance);

            Assert.Equal(0, (height - Tokens.ListMargin) % RowHeight, 6);
            Assert.True(height >= Tokens.ListHeight(Tokens.ListVisibleItems));
            // The height is the nearest whole number of rows to where the pointer is.
            var wanted = Tokens.ListHeight(Tokens.ListVisibleItems) + distance;
            Assert.True(Math.Abs(height - wanted) <= RowHeight / 2 + 0.5);
        }

        [AvaloniaFact]
        public void DraggingTheStripUp_NeverGoesBelowTheMinimum()
        {
            Assert.Equal(Tokens.ListHeight(Tokens.ListVisibleItems), Resize(-300));
        }

        [AvaloniaFact]
        public void ThereIsNoLargestHeight()
        {
            var height = Resize(1000);

            Assert.True(height > Tokens.ListHeight(Tokens.ListVisibleItems) + 900);
        }
    }
}
