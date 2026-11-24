using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Tests.Support;
using Xunit;
using Avalonia.Headless;
using Avalonia.Input;

namespace CMiX.UI.Tests
{
    // One splitter theme for the rows and for the columns. The bar that shows when the pointer is over the splitter
    // is the same in both: 2 wide and as long as the splitter.
    public class GridSplitterThemeTests
    {
        private static ResourceDictionaryHelper Theme => new();

        private sealed class ResourceDictionaryHelper
        {
            public ControlTheme Splitter => Tokens.Theme("GridSplitterCMiX");
            public double IndicatorSize => Tokens.Get<double>("SplitterIndicatorSize");
            public double Size => Tokens.Get<double>("SplitterSize");
        }

        private static (Window Window, GridSplitter Splitter) ShowColumns()
        {
            var grid = new Grid { Width = 300, Height = 100 };
            grid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
            grid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
            var splitter = new GridSplitter
            {
                Theme = Theme.Splitter,
                Width = Theme.Size,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                ResizeDirection = GridResizeDirection.Columns
            };
            grid.Children.Add(splitter);
            return (TestWindow.Show(grid, 400, 200), splitter);
        }

        private static (Window Window, GridSplitter Splitter) ShowRows()
        {
            var grid = new Grid { Width = 300, Height = 100 };
            grid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));
            grid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));
            var splitter = new GridSplitter
            {
                Theme = Theme.Splitter,
                Height = Theme.Size,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom
            };
            grid.Children.Add(splitter);
            return (TestWindow.Show(grid, 400, 200), splitter);
        }

        private static Border Indicator(GridSplitter splitter) =>
            splitter.GetVisualDescendants().OfType<Border>().First(border => border.Name == "indicator");

        private static Border Backdrop(GridSplitter splitter) =>
            splitter.GetVisualDescendants().OfType<Border>().First(border => border.Name == "border");

        [AvaloniaFact]
        public void Show_ASplitterOfColumns_HasAVerticalBarOfItsLength()
        {
            var (_, splitter) = ShowColumns();

            var bar = Indicator(splitter);

            Assert.Equal(Theme.IndicatorSize, bar.Bounds.Width);
            Assert.Equal(splitter.Bounds.Height, bar.Bounds.Height);
        }

        [AvaloniaFact]
        public void Show_ASplitterOfRows_HasAHorizontalBarOfItsLength()
        {
            var (_, splitter) = ShowRows();

            var bar = Indicator(splitter);

            Assert.Equal(Theme.IndicatorSize, bar.Bounds.Height);
            Assert.Equal(splitter.Bounds.Width, bar.Bounds.Width);
        }

        [AvaloniaTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void Hover_ASplitterInEitherDirection_ShowsTheBarAndTheBack(bool columns)
        {
            var (window, splitter) = columns ? ShowColumns() : ShowRows();
            var idleBar = Assert.IsAssignableFrom<ISolidColorBrush>(Indicator(splitter).Background ?? Brushes.Transparent);
            Assert.Equal(Colors.Transparent, idleBar.Color);

            window.MouseMove(splitter.TranslatePoint(new Point(splitter.Bounds.Width / 2, splitter.Bounds.Height / 2), window)!.Value);
            Dispatcher.UIThread.RunJobs();

            var grey600 = Tokens.Color("Grey600Brush");
            var grey100 = Tokens.Color("Grey100Brush");
            Assert.Equal(grey600, Assert.IsAssignableFrom<ISolidColorBrush>(Indicator(splitter).Background).Color);
            Assert.Equal(grey100, Assert.IsAssignableFrom<ISolidColorBrush>(Backdrop(splitter).Background).Color);
        }
    }
}
