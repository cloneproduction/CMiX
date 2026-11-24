using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Panels;
using CMiX.UI.Tests.Support;
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
            TestWindow.Show(stack, 400, 400);
            return (stack, first, second);
        }

        private static Border Outline(Control expander) => expander.GetVisualDescendants().OfType<Border>().First();

        [AvaloniaFact]
        public void TheCard_HasTheBaseRoundedCorners_AndClipsItsContentToThem()
        {
            var (_, first, _) = Show();
            var outline = Outline(first);

            Assert.Equal(Tokens.Get<CornerRadius>("CornerRadiusAll"), outline.CornerRadius);
            Assert.Equal(Tokens.Get<CornerRadius>("CornerRadiusAll"), RoundedClip.GetCornerRadius(outline));
            Assert.NotNull(outline.Clip);
        }

        [AvaloniaFact]
        public void TheCard_HasTheSpaceAroundItOfTheToken()
        {
            var (_, first, _) = Show();

            Assert.Equal(Tokens.Get<Thickness>("MarginCard"), first.Margin);
        }

        [AvaloniaFact]
        public void TwoCardsInAStack_DoNotTouch()
        {
            var (_, first, second) = Show();
            var margin = Tokens.Get<Thickness>("MarginCard");

            var gap = second.Bounds.Top - first.Bounds.Bottom;

            Assert.Equal(margin.Top + margin.Bottom, gap);
        }

        [AvaloniaFact]
        public void FoldedAndOpen_TheCardKeepsItsRoundedOutline()
        {
            var (_, first, _) = Show();
            var radius = Tokens.Get<CornerRadius>("CornerRadiusAll");

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
            TestWindow.Show(expander, 300, 200);

            Assert.Equal(new Thickness(0), expander.Margin);
        }

        [AvaloniaFact]
        public void AFirstLevelCard_HasTheLevelOneBackground_NotTheSurface()
        {
            var (_, first, _) = Show();
            var header = first.GetVisualDescendants().OfType<Border>().First(b => b.Name == "mainBorder");

            var color = ((Avalonia.Media.ISolidColorBrush)header.Background!).Color;
            var level1 = Tokens.Color("PanelBackgroundLevel1Brush");

            Assert.Equal(level1, color);
        }

        [AvaloniaFact]
        public void Indentation_SetToFour_GivesTheToggleAMarginOfFour()
        {
            var expander = new ExpanderIndented { Header = "x", Indentation = 4 };
            TestWindow.Show(expander, 300, 200);

            var toggle = expander.GetVisualDescendants().OfType<ToggleButton>().First(t => t.Name == "PART_toggle");

            Assert.Equal(new Thickness(4, 0, 0, 0), toggle.Margin);
        }

        [AvaloniaFact]
        public void TheCardMargin_IsMadeOfTheSpacingScale()
        {
            var margin = Tokens.Get<Thickness>("MarginCard");

            Assert.Equal(Tokens.Get<double>("SpacingXS"), margin.Left);
            Assert.Equal(Tokens.Get<double>("SpacingXS"), margin.Right);
            Assert.Equal(Tokens.Get<double>("SpacingXXS"), margin.Top);
            Assert.Equal(Tokens.Get<double>("SpacingXXS"), margin.Bottom);
        }
    }
}
