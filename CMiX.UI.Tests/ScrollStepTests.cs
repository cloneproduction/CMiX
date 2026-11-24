using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Controls;
using CMiX.UI.Controls.Collection;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // The attached property ScrollStep: the lists of CMiX show a whole number of rows, and the wheel moves them
    // by one row, in a combo box, a plain list and a ListBoxCMiX. At an end the wheel goes on to the panel around.
    public class ScrollStepTests
    {
        // The lists of CMiX show a whole number of rows, and the wheel moves them by one row.

        private static double RowPitch => Tokens.RowPitch;

        private static T Show<T>(T list, int itemCount = 30) where T : ListBox
        {
            list.ItemsSource = Enumerable.Range(1, itemCount).Select(i => "Item " + i).ToList();
            list.ItemTemplate = new FuncDataTemplate<string>((text, _) => new TextBlock { Text = text });
            TestWindow.Show(list, 400, 800);
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

            Assert.Equal(Tokens.ListHeight(Tokens.ListVisibleItems) - Tokens.ListMargin, scroll.Viewport.Height);
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
        public void Show_AListBoxRepository_ShowsFiveWholeRows()
        {
            var detail = new ListBoxRepository
            {
                ItemsSource = Enumerable.Range(1, 30).Select(i => "Item " + i).ToList(),
            };
            TestWindow.Show(detail, 400, 800);

            Assert.Equal(Tokens.ListHeight(Tokens.ListVisibleItems) - Tokens.ListMargin, ScrollOf(detail).Viewport.Height);
        }

        [AvaloniaFact]
        public void Wheel_OnAListBoxCMiX_MovesOneRow()
        {
            var scroll = ScrollOf(Show(new ListBoxCMiX { MinItemCount = Tokens.ListVisibleItems }));

            Wheel(scroll, -1);
            Assert.Equal(RowPitch, scroll.Offset.Y);

            Wheel(scroll, -1);
            Wheel(scroll, -1);
            Assert.Equal(3 * RowPitch, scroll.Offset.Y);
        }

        [AvaloniaFact]
        public void Wheel_OnAPlainListBox_MovesOneRow()
        {
            var list = Show(new ListBox { MaxHeight = Tokens.ListHeight(Tokens.ListVisibleItems) });
            var scroll = ScrollOf(list);

            Wheel(scroll, -1);

            Assert.Equal(RowPitch, scroll.Offset.Y);
        }

        [AvaloniaFact]
        public void Wheel_AtTheEndOfAList_GoesOnToThePanelAround()
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
        public void Wheel_OnAListThatDoesNotScroll_GoesOnToThePanelAround()
        {
            var scroll = ScrollOf(Show(new ListBox { MaxHeight = Tokens.ListHeight(Tokens.ListVisibleItems) }, itemCount: 3));

            Assert.False(Wheel(scroll, -1));
            Assert.Equal(0, scroll.Offset.Y);
        }

        // ListBoxCMiX forwards the wheel to the scroll viewer around it when it is at its start or end.
        // The step of a row must not stop this.
        [AvaloniaFact]
        public void Wheel_AtTheEndOfAListBoxCMiX_ScrollsThePanelAround()
        {
            var list = new ListBoxCMiX { MinItemCount = Tokens.ListVisibleItems };
            list.ItemsSource = Enumerable.Range(1, 30).Select(i => "Item " + i).ToList();
            list.ItemTemplate = new FuncDataTemplate<string>((text, _) => new TextBlock { Text = text });
            var around = new ScrollViewer
            {
                Content = new StackPanel { Children = { new Border { Height = 1500 }, list } },
            };
            TestWindow.Show(around, 400, 400);
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
            var window = TestWindow.Show(list, 400, 1500);

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
        public void Show_AListBoxCMiX_HasAResizeStripUnderTheList()
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
        public void Resize_AnyDistance_MakesTheListAWholeNumberOfRowsTaller(double distance)
        {
            var height = Resize(distance);

            Assert.Equal(0, (height - Tokens.ListMargin + Tokens.ListSpacing) % RowPitch, 6);
            Assert.True(height >= Tokens.ListHeight(Tokens.ListVisibleItems));
            // The height is the nearest whole number of rows to where the pointer is.
            var wanted = Tokens.ListHeight(Tokens.ListVisibleItems) + distance;
            Assert.True(Math.Abs(height - wanted) <= RowPitch / 2 + 0.5);
        }

        [AvaloniaFact]
        public void Resize_Up_StopsAtTheMinimum()
        {
            Assert.Equal(Tokens.ListHeight(Tokens.ListVisibleItems), Resize(-300));
        }

        [AvaloniaFact]
        public void Resize_AFarDistance_HasNoLargestHeight()
        {
            var height = Resize(1000);

            Assert.True(height > Tokens.ListHeight(Tokens.ListVisibleItems) + 900);
        }

        [AvaloniaFact]
        public void CaptureLost_DuringAResize_StopsResizing()
        {
            var list = new ListBoxCMiX { MinItemCount = Tokens.ListVisibleItems, ItemsSource = Enumerable.Range(1, 30).Select(i => "Item " + i).ToList() };
            var window = TestWindow.ShowWithoutClips(list, 400, 1500);
            var strip = Parts.Find<Border>(list, "resizeBorder");
            var pointer = PointerInput.WatchPressedPointer(strip);
            var start = strip.TranslatePoint(new Point(strip.Bounds.Width / 2, strip.Bounds.Height / 2), window)!.Value;
            var before = HeightBorderOf(list).Height;

            window.MouseMove(start);
            window.MouseDown(start, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            Assert.NotNull(pointer()!.Captured);

            pointer()!.Capture(null);
            Dispatcher.UIThread.RunJobs();

            // A move over the strip, as when the mouse comes back to it.
            PointerInput.Move(strip, PointerInput.NewPointer(), new Point(start.X, start.Y + 200), window);

            Assert.Equal(before, HeightBorderOf(list).Height);
        }

        // The wheel moves the list of a ComboBox by whole items, so no row is cut at the top or at the bottom.

        private static double ItemHeight => Tokens.RowHeight;

        private static (ComboBox combo, ScrollViewer scroll) OpenCombo(int itemCount, int selectedIndex = 0)
        {
            var combo = new ComboBox
            {
                ItemsSource = Enumerable.Range(1, itemCount).Select(i => "Item " + i).ToList(),
                SelectedIndex = selectedIndex,
                Width = 200,
            };
            TestWindow.Show(combo, 400, 600);

            combo.IsDropDownOpen = true;
            Dispatcher.UIThread.RunJobs();

            var popup = combo.GetVisualDescendants().OfType<Popup>().Single();
            return (combo, popup.Child!.GetVisualDescendants().OfType<ScrollViewer>().First());
        }

        [AvaloniaFact]
        public void Open_AComboBox_GivesTheScrollTheHeightOfAnItem()
        {
            var (_, scroll) = OpenCombo(30);

            Assert.Equal(ItemHeight, ScrollStep.GetItemHeight(scroll));
        }

        [AvaloniaFact]
        public void Open_AComboBox_ShowsTheNumberOfRowsOfTheToken()
        {
            var (_, scroll) = OpenCombo(30);

            Assert.Equal(Tokens.Get<int>("ComboBoxVisibleItems") * ItemHeight, scroll.Viewport.Height);
        }

        [AvaloniaFact]
        public void Wheel_OneNotchInAComboBox_MovesExactlyOneItem()
        {
            var (_, scroll) = OpenCombo(30);

            Wheel(scroll, -1);
            Assert.Equal(ItemHeight, scroll.Offset.Y);

            Wheel(scroll, -1);
            Wheel(scroll, -1);
            Assert.Equal(3 * ItemHeight, scroll.Offset.Y);

            Wheel(scroll, 1);
            Assert.Equal(2 * ItemHeight, scroll.Offset.Y);
        }

        [AvaloniaFact]
        public void Wheel_AnyNotchesInAComboBox_KeepAWholeNumberOfItems()
        {
            var (_, scroll) = OpenCombo(30);

            foreach (var delta in new double[] { -1, -1, -1, 1, -1, -1, 1, 1, -1 })
            {
                Wheel(scroll, delta);
                Assert.Equal(0, scroll.Offset.Y % ItemHeight);
            }
        }

        [AvaloniaFact]
        public void Wheel_PastTheEndsOfAComboBox_StopsOnWholeItems()
        {
            var (_, scroll) = OpenCombo(30);

            for (var i = 0; i < 40; i++)
                Wheel(scroll, -1);

            Assert.Equal(scroll.Extent.Height - scroll.Viewport.Height, scroll.Offset.Y);

            for (var i = 0; i < 40; i++)
                Wheel(scroll, 1);

            Assert.Equal(0, scroll.Offset.Y);
        }

        [AvaloniaTheory]
        [InlineData(0)]
        [InlineData(7)]
        [InlineData(12)]
        [InlineData(20)]
        [InlineData(29)]
        public void Open_WithAnItemFarDown_StartsOnAWholeItem(int selectedIndex)
        {
            var (_, scroll) = OpenCombo(30, selectedIndex);

            Assert.True(scroll.Offset.Y % ItemHeight == 0, "offset=" + scroll.Offset.Y);
            if (selectedIndex >= 12)
                Assert.True(scroll.Offset.Y > 0, "no scroll to the selection: offset=" + scroll.Offset.Y);
        }

        [AvaloniaFact]
        public void Wheel_AShortComboBox_DoesNotScroll()
        {
            var (_, scroll) = OpenCombo(5);

            Wheel(scroll, -1);

            Assert.Equal(0, scroll.Offset.Y);
        }
    }
}
