using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Panels;
using Xunit;

namespace CMiX.UI.Tests
{
    // An ExpanderIndented is a card: rounded corners, a little space around it, and the same outline
    // when it is folded and when it is open.
    public class ExpanderIndentedCardTests
    {
        private static (StackPanel stack, ExpanderIndented first, ExpanderIndented second) Show()
        {
            var first = new ExpanderIndented { Header = "Transform", Content = new TextBlock { Text = "first body", Height = 40 } };
            var second = new ExpanderIndented { Header = "Texture", Content = new TextBlock { Text = "second body", Height = 40 } };
            var stack = new StackPanel { Children = { first, second } };
            new Window { Content = stack, Width = 400, Height = 400 }.Show();
            Dispatcher.UIThread.RunJobs();
            return (stack, first, second);
        }

        private static Border Outline(Control expander) => expander.GetVisualDescendants().OfType<Border>().First();

        private static T Token<T>(string key) => (T)Application.Current!.FindResource(key)!;

        [AvaloniaFact]
        public void TheCard_HasTheRoundedCornersOfTheToken_AndClipsItsContentToThem()
        {
            var (_, first, _) = Show();
            var outline = Outline(first);

            Assert.Equal(Token<CornerRadius>("CornerRadiusCard"), outline.CornerRadius);
            Assert.Equal(Token<CornerRadius>("CornerRadiusCard"), RoundedClip.GetCornerRadius(outline));
            Assert.NotNull(outline.Clip);
        }

        [AvaloniaFact]
        public void TheCard_HasTheSpaceAroundItOfTheToken()
        {
            var (_, first, _) = Show();

            Assert.Equal(Token<Thickness>("MarginCard"), first.Margin);
        }

        [AvaloniaFact]
        public void TwoCardsInAStack_DoNotTouch()
        {
            var (_, first, second) = Show();
            var margin = Token<Thickness>("MarginCard");

            var gap = second.Bounds.Top - first.Bounds.Bottom;

            Assert.Equal(margin.Top + margin.Bottom, gap);
        }

        [AvaloniaFact]
        public void FoldedAndOpen_TheCardKeepsItsRoundedOutline()
        {
            var (_, first, _) = Show();
            var radius = Token<CornerRadius>("CornerRadiusCard");

            first.IsExpanded = false;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(radius, Outline(first).CornerRadius);

            first.IsExpanded = true;
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(radius, Outline(first).CornerRadius);
            Assert.NotNull(Outline(first).Clip);
        }

        [AvaloniaFact]
        public void AMarginSetByTheApplication_StillWins()
        {
            var expander = new ExpanderIndented { Header = "x", Margin = new Thickness(0) };
            new Window { Content = expander, Width = 300, Height = 200 }.Show();
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(new Thickness(0), expander.Margin);
        }

        private static int Brightness(string brushKey)
        {
            var color = ((Avalonia.Media.ISolidColorBrush)Application.Current!.FindResource(brushKey)!).Color;
            return color.R + color.G + color.B;
        }

        [AvaloniaFact]
        public void TheSurfaceBehindTheCards_IsDarkerThanTheCards()
        {
            Assert.True(Brightness("PanelBackgroundLevel0Brush") < Brightness("PanelBackgroundLevel1Brush"),
                "The cards would not stand out from the surface behind them.");
        }

        [AvaloniaFact]
        public void AFirstLevelCard_HasTheLevelOneBackground_NotTheSurface()
        {
            var (_, first, _) = Show();
            var header = first.GetVisualDescendants().OfType<Border>().First(b => b.Name == "mainBorder");

            var color = ((Avalonia.Media.ISolidColorBrush)header.Background!).Color;
            var level1 = ((Avalonia.Media.ISolidColorBrush)Application.Current!.FindResource("PanelBackgroundLevel1Brush")!).Color;

            Assert.Equal(level1, color);
        }
    }
}
