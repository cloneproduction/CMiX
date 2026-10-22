using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Panels;
using Xunit;

namespace CMiX.UI.Tests
{
    // Expanders on top of each other take the colors of their level: the outer one level 1, the next one level 2, and the innermost level 3.
    public class ExpanderDepthTests
    {
        private static Color Level(int level) =>
            ((ISolidColorBrush)Application.Current!.FindResource($"PanelBackgroundLevel{level}Brush")!).Color;

        private static (ExpanderIndented outer, ExpanderIndented inner2, ExpanderIndented inner3) Show()
        {
            var inner3 = new ExpanderIndented { Header = "3", IsExpanded = true, Content = new TextBlock { Text = "x" } };
            var inner2 = new ExpanderIndented { Header = "2", IsExpanded = true, Content = inner3 };
            var outer = new ExpanderIndented { Header = "1", IsExpanded = true, Content = inner2 };
            new Window { Content = outer, Width = 400, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();
            return (outer, inner2, inner3);
        }

        private static Color Content(ExpanderIndented expander) =>
            ((ISolidColorBrush)expander.GetVisualDescendants().OfType<Border>().First(border => border.Name == "contentBackground").Background!).Color;

        private static Color Header(ExpanderIndented expander) =>
            ((ISolidColorBrush)expander.GetVisualDescendants().OfType<Border>().First(border => border.Name == "mainBorder").Background!).Color;

        [AvaloniaFact]
        public void TheContent_TakesTheColorOfItsLevel()
        {
            var (outer, inner2, inner3) = Show();

            Assert.Equal(Level(1), Content(outer));
            Assert.Equal(Level(2), Content(inner2));
            Assert.Equal(Level(3), Content(inner3));
        }

        [AvaloniaFact]
        public void TheHeader_TakesTheColorOfItsLevel()
        {
            var (outer, inner2, inner3) = Show();

            Assert.Equal(Level(1), Header(outer));
            Assert.Equal(Level(2), Header(inner2));
            Assert.Equal(Level(3), Header(inner3));
        }

        [AvaloniaFact]
        public void ABackgroundSetByTheCaller_OnTheOuterExpander_IsKept()
        {
            var grey = Colors.Orange;
            var expander = new ExpanderIndented { Header = "1", IsExpanded = true, Background = new SolidColorBrush(grey), Content = new TextBlock { Text = "x" } };
            new Window { Content = expander, Width = 400, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(grey, Content(expander));
            Assert.Equal(grey, Header(expander));
        }

        [AvaloniaFact]
        public void ABackgroundSetByTheCaller_OnANestedExpander_IsKeptAtEveryDepth()
        {
            var orange = Colors.Orange;
            var inner = new ExpanderIndented { Header = "2", IsExpanded = true, Background = new SolidColorBrush(orange), Content = new TextBlock { Text = "x" } };
            var outer = new ExpanderIndented { Header = "1", IsExpanded = true, Content = inner };
            new Window { Content = outer, Width = 400, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(orange, Content(inner));
            Assert.Equal(orange, Header(inner));
            Assert.Equal(Level(1), Content(outer));
        }
    }
}
