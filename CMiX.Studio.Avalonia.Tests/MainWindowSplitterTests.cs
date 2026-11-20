using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.Views;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The tree and the tabs have no gap between them. The splitter between them lies over their edge, takes no room,
    // and shows the same bar as the splitters of the rows (GridSplitterCMiX).
    public class MainWindowSplitterTests
    {
        private static (Window Window, GridSplitter Splitter, Grid Grid, ProjectTreeView Tree, TabControl Tabs) Show()
        {
            var (_, window, _) = TestServiceProviderFactory.ShowMainWindow();
            var tree = window.GetVisualDescendants().OfType<ProjectTreeView>().Single();
            var grid = tree.GetVisualAncestors().OfType<Grid>().First(g => g.ColumnDefinitions.Count == 2);
            var splitter = grid.Children.OfType<GridSplitter>().Single();
            return (window, splitter, grid, tree, TestServiceProviderFactory.MainTabControl(window));
        }

        private static double Left(Visual control, Visual root) => control.TranslatePoint(new Point(0, 0), root)!.Value.X;

        [AvaloniaFact]
        public void Show_TheTreeAndTheTabs_TouchEachOther()
        {
            var (window, _, _, tree, tabs) = Show();

            Assert.Equal(Left(tabs, window), Left(tree, window) + tree.Bounds.Width, 0.5);
        }

        [AvaloniaFact]
        public void Show_TheColumns_FillTheWholeWidth()
        {
            var (_, _, grid, _, _) = Show();

            Assert.Equal(grid.Bounds.Width, grid.ColumnDefinitions.Sum(column => column.ActualWidth), 0.5);
        }

        [AvaloniaFact]
        public void Show_TheSplitter_LiesOverTheEdgeOfBothPanels()
        {
            var (window, splitter, grid, tree, tabs) = Show();
            var edge = Left(tabs, window);
            var splitterLeft = Left(splitter, window);

            Assert.Equal(Convert.ToDouble(Application.Current!.FindResource("SplitterSize")), splitter.Bounds.Width);
            Assert.True(splitterLeft < edge && splitterLeft + splitter.Bounds.Width > edge, "The splitter is not over the edge.");
            Assert.Same(GridResizeDirection.Columns.GetType(), splitter.ResizeDirection.GetType());
            Assert.Equal(GridResizeDirection.Columns, splitter.ResizeDirection);
            Assert.Same(Application.Current!.FindResource("GridSplitterCMiX"), splitter.Theme);

            // It is on top: it comes after the two side panels in the grid.
            Assert.All(grid.Children.OfType<PanelSide>(), host => Assert.True(grid.Children.IndexOf(splitter) > grid.Children.IndexOf(host)));
        }

        [AvaloniaFact]
        public void Drag_TheSplitter_MovesTheEdgeByTheDistanceOfTheMouse()
        {
            var (window, splitter, grid, _, _) = Show();
            var before = grid.ColumnDefinitions[0].ActualWidth;
            var start = splitter.TranslatePoint(new Point(splitter.Bounds.Width / 2, 100), window)!.Value;

            window.MouseDown(start, MouseButton.Left);
            window.MouseMove(start + new Vector(40, 0), RawInputModifiers.LeftMouseButton);
            window.MouseUp(start + new Vector(40, 0), MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();

            Assert.Equal(before + 40, grid.ColumnDefinitions[0].ActualWidth, 1);
            Assert.Equal(grid.Bounds.Width, grid.ColumnDefinitions.Sum(column => column.ActualWidth), 0.5);
        }

        [AvaloniaFact]
        public void Drag_TheSplitter_StopsAtTheLeastWidthOfAPanel()
        {
            var (window, splitter, grid, _, _) = Show();
            var start = splitter.TranslatePoint(new Point(splitter.Bounds.Width / 2, 100), window)!.Value;

            window.MouseDown(start, MouseButton.Left);
            window.MouseMove(start + new Vector(-2000, 0), RawInputModifiers.LeftMouseButton);
            window.MouseUp(start + new Vector(-2000, 0), MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Assert.Equal(grid.ColumnDefinitions[0].MinWidth, grid.ColumnDefinitions[0].ActualWidth, 0.5);

            var left = splitter.TranslatePoint(new Point(splitter.Bounds.Width / 2, 100), window)!.Value;
            window.MouseDown(left, MouseButton.Left);
            window.MouseMove(left + new Vector(4000, 0), RawInputModifiers.LeftMouseButton);
            window.MouseUp(left + new Vector(4000, 0), MouseButton.Left);
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();

            Assert.Equal(grid.ColumnDefinitions[1].MinWidth, grid.ColumnDefinitions[1].ActualWidth, 0.5);
        }

        [AvaloniaFact]
        public void Show_TheSplitter_LeavesTheScrollBarOfTheTreeFree()
        {
            var (window, splitter, _, tree, _) = Show();
            var bar = tree.GetVisualDescendants().OfType<global::Avalonia.Controls.Primitives.ScrollBar>().First(b => b.Orientation == global::Avalonia.Layout.Orientation.Vertical);

            var barRight = Left(bar, window) + bar.Bounds.Width;

            Assert.True(barRight <= Left(splitter, window) + 0.5, "The splitter is over the scroll bar of the tree.");
        }
    }
}
