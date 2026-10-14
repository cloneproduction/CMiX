using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using CMiX.UI.AttachedProperties;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;

namespace CMiX.UI.Tests
{
    // The wheel moves the list of a ComboBox by whole items, so no row is cut at the top or at the bottom.
    public class ScrollStepTests
    {
        private static double ItemHeight => Tokens.RowHeight;

        private static (ComboBox combo, ScrollViewer scroll) OpenCombo(int itemCount, int selectedIndex = 0)
        {
            var combo = new ComboBox
            {
                ItemsSource = Enumerable.Range(1, itemCount).Select(i => "Item " + i).ToList(),
                SelectedIndex = selectedIndex,
                Width = 200,
            };
            new Window { Content = combo, Width = 400, Height = 600 }.Show();
            Dispatcher.UIThread.RunJobs();

            combo.IsDropDownOpen = true;
            Dispatcher.UIThread.RunJobs();

            var popup = combo.GetVisualDescendants().OfType<Popup>().Single();
            return (combo, popup.Child!.GetVisualDescendants().OfType<ScrollViewer>().First());
        }

        private static void Wheel(ScrollViewer scroll, double delta)
        {
            var presenter = scroll.GetVisualDescendants().OfType<ScrollContentPresenter>().First();
            var pointer = new Pointer(Pointer.GetNextFreeId(), PointerType.Mouse, true);
            var args = new PointerWheelEventArgs(
                presenter, pointer, TopLevel.GetTopLevel(scroll)!, new Point(50, 50), 0,
                new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.Other),
                KeyModifiers.None, new Vector(0, delta))
            { RoutedEvent = InputElement.PointerWheelChangedEvent };

            presenter.RaiseEvent(args);
            Dispatcher.UIThread.RunJobs();
        }

        [AvaloniaFact]
        public void TheDropDownScrollViewer_UsesTheHeightOfAnItem()
        {
            var (_, scroll) = OpenCombo(30);

            Assert.Equal(ItemHeight, ScrollStep.GetItemHeight(scroll));
        }

        [AvaloniaFact]
        public void TheDropDown_ShowsTheNumberOfRowsOfTheToken()
        {
            var (_, scroll) = OpenCombo(30);

            Assert.Equal(Tokens.Get<int>("ComboBoxVisibleItems") * ItemHeight, scroll.Viewport.Height);
        }

        [AvaloniaFact]
        public void OneNotch_MovesExactlyOneItem()
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
        public void TheOffset_IsAlwaysAWholeNumberOfItems()
        {
            var (_, scroll) = OpenCombo(30);

            foreach (var delta in new double[] { -1, -1, -1, 1, -1, -1, 1, 1, -1 })
            {
                Wheel(scroll, delta);
                Assert.Equal(0, scroll.Offset.Y % ItemHeight);
            }
        }

        [AvaloniaFact]
        public void AtTheEnd_TheLastItemIsWhole_AndTheStartStopsAtZero()
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
        public void OpeningWithAnItemFarDown_StartsOnAWholeItem(int selectedIndex)
        {
            var (_, scroll) = OpenCombo(30, selectedIndex);

            Assert.True(scroll.Offset.Y % ItemHeight == 0, "offset=" + scroll.Offset.Y);
            if (selectedIndex >= 12)
                Assert.True(scroll.Offset.Y > 0, "no scroll to the selection: offset=" + scroll.Offset.Y);
        }

        [AvaloniaFact]
        public void AShortList_DoesNotScroll()
        {
            var (_, scroll) = OpenCombo(5);

            Wheel(scroll, -1);

            Assert.Equal(0, scroll.Offset.Y);
        }
    }
}
