using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Core.Compositing;
using CMiX.Studio.Avalonia.Tests.Support;
using CMiX.Studio.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;
using Layer = CMiX.Core.Compositing.Layer;

namespace CMiX.Studio.Avalonia.Tests
{
    // The scroll bar of a list or of the tree lies in a room on the right of the content. The content has the same width with a bar and without one.
    public class ScrollBarOverlayTests : IDisposable
    {
        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        private static ScrollBar VerticalBarOf(Control control) =>
            control.GetVisualDescendants().OfType<ScrollBar>().First(bar => bar.Orientation == global::Avalonia.Layout.Orientation.Vertical);

        // The width of the first row of a list with the number of rows. Many rows need a bar, few rows do not.
        private static (double Width, bool BarShown) FirstRowOfAList(int count)
        {
            var list = new ListBox { Width = 200, Height = 96 };
            for (var i = 0; i < count; i++)
                list.Items.Add(new Border { Height = 24, Tag = "row" });
            var window = new Window { Content = list, Width = 300, Height = 200 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();

            var row = list.GetVisualDescendants().OfType<ListBoxItem>().First();
            var width = row.Bounds.Width;
            var shown = VerticalBarOf(list).IsVisible;
            window.Close();
            return (width, shown);
        }

        [AvaloniaFact]
        public void Show_AListWithManyRows_ShowsTheBarAndKeepsTheRowWidth()
        {
            var few = FirstRowOfAList(2);
            var many = FirstRowOfAList(20);

            Assert.False(few.BarShown);
            Assert.True(many.BarShown);
            Assert.Equal(few.Width, many.Width);
        }

        [AvaloniaFact]
        public void Show_TheBarOfAList_LiesInTheRoomOnTheRight()
        {
            var list = new ListBox { Width = 200, Height = 96 };
            for (var i = 0; i < 20; i++)
                list.Items.Add(new Border { Height = 24 });
            var window = new Window { Content = list, Width = 300, Height = 200 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();

            var bar = VerticalBarOf(list);
            var presenter = list.GetVisualDescendants().OfType<ScrollContentPresenter>().First();
            var viewer = list.GetVisualDescendants().OfType<ScrollViewer>().First();
            var barBox = new Rect(bar.TranslatePoint(new Point(0, 0), window)!.Value, bar.Bounds.Size);
            var presenterBox = new Rect(presenter.TranslatePoint(new Point(0, 0), window)!.Value, presenter.Bounds.Size);
            var viewerBox = new Rect(viewer.TranslatePoint(new Point(0, 0), window)!.Value, viewer.Bounds.Size);

            Assert.True(bar.IsVisible);
            Assert.True(bar.Bounds.Width > 0);
            Assert.True(barBox.Left >= presenterBox.Right - 0.5, "The bar is over the rows.");
            Assert.True(barBox.Right <= viewerBox.Right + 0.5, "The bar is outside the scroll viewer.");
            window.Close();
        }

        // The room on the right of the content is the width of the bar and of its margins. If one of them changes, the other must too.
        [AvaloniaFact]
        public void Show_TheRoomOnTheRight_IsTheWidthOfTheBarAndItsMargins()
        {
            var list = new ListBox { Width = 200, Height = 96 };
            for (var i = 0; i < 20; i++)
                list.Items.Add(new Border { Height = 24 });
            var window = new Window { Content = list, Width = 300, Height = 200 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();

            var bar = VerticalBarOf(list);
            var gutter = Assert.IsType<Thickness>(Application.Current!.FindResource("ScrollBarGutter"));

            Assert.Equal(gutter.Right, bar.Width + bar.Margin.Left + bar.Margin.Right);
            Assert.Equal(0, gutter.Left + gutter.Top + gutter.Bottom);
            window.Close();
        }

        [AvaloniaFact]
        public void Show_TheThumbOfTheBar_UsesADarkerThemeColor()
        {
            var list = new ListBox { Width = 200, Height = 96 };
            for (var i = 0; i < 20; i++)
                list.Items.Add(new Border { Height = 24 });
            var window = new Window { Content = list, Width = 300, Height = 200 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();

            var thumb = list.GetVisualDescendants().OfType<Thumb>().First(t => t.Name == "thumb");
            var idle = Assert.IsAssignableFrom<global::Avalonia.Media.ISolidColorBrush>(thumb.Background).Color;
            var grey500 = Assert.IsAssignableFrom<global::Avalonia.Media.ISolidColorBrush>(Application.Current!.FindResource("Grey500Brush")).Color;
            var before = Assert.IsAssignableFrom<global::Avalonia.Media.ISolidColorBrush>(Application.Current!.FindResource("StateIdleBrush")).Color;

            Assert.Equal(grey500, idle);
            Assert.True(idle.R + idle.G + idle.B < before.R + before.G + before.B, "The thumb is not darker than before.");
            window.Close();
        }

        // The tree with a composition and the number of layers: many layers need a bar.
        private (double RowWidth, bool BarShown) FirstRowOfTheTree(int layers)
        {
            var (provider, window, viewModel) = windows.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.SelectedItem;
            for (var i = 0; i < layers; i++)
                composition.LayerManager.AddItem(typeof(Layer));
            TestServiceProviderFactory.ExpandAll(viewModel.ProjectTree);
            TestServiceProviderFactory.Pump();
            Support.ListTreeTestScreen.Settle(window);

            var tree = window.GetVisualDescendants().OfType<ListTreeProject>().Single();
            var row = tree.Rows.RealizedRows.First();
            var result = (row.Bounds.Width, VerticalBarOf(tree.Scroller).IsVisible);
            window.Close();
            return result;
        }

        [AvaloniaFact]
        public void Show_ATreeWithManyRows_ShowsTheBarAndKeepsTheRowWidth()
        {
            var few = FirstRowOfTheTree(1);
            var many = FirstRowOfTheTree(60);

            Assert.False(few.BarShown);
            Assert.True(many.BarShown);
            Assert.Equal(few.RowWidth, many.RowWidth);
        }
    }
}
