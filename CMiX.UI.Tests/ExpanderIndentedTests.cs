using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.AttachedProperties;
using CMiX.UI.Panels;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // The expander card: open by default, a rounded card with the margin token, the colors of the levels,
    // a background set by the caller wins, the indentation, and one PanelCollapse per theme.
    public class ExpanderIndentedTests
    {
        // A panel is open when it appears.

        [AvaloniaFact]
        public void Show_AnIndentedExpander_IsOpenByDefault()
        {
            Assert.True(new ExpanderIndented().IsExpanded);
        }

        [AvaloniaFact]
        public void Show_AnExpanderInTheTheme_IsOpenByDefault()
        {
            var expander = new Expander { Header = "Peers", Content = new Border() };
            TestWindow.Show(expander, 200, 200);

            Assert.True(expander.IsExpanded);
        }

        // An ExpanderIndented is a card: rounded corners, a little space around it, and the same outline
        // when it is folded and when it is open.

        private static (StackPanel stack, ExpanderIndented first, ExpanderIndented second) ShowTwoCards()
        {
            var first = new ExpanderIndented { Header = "Transform", Content = new TextBlock { Text = "first body", Height = 40 } };
            var second = new ExpanderIndented { Header = "Texture", Content = new TextBlock { Text = "second body", Height = 40 } };
            var stack = new StackPanel { Children = { first, second } };
            TestWindow.Show(stack, 400, 400);
            return (stack, first, second);
        }

        private static Border Outline(Control expander) => expander.GetVisualDescendants().OfType<Border>().First();

        [AvaloniaFact]
        public void Show_ACard_HasTheBaseRoundedCornersAndClipsItsContent()
        {
            var (_, first, _) = ShowTwoCards();
            var outline = Outline(first);

            Assert.Equal(Tokens.Get<CornerRadius>("CornerRadiusAll"), outline.CornerRadius);
            Assert.Equal(Tokens.Get<CornerRadius>("CornerRadiusAll"), RoundedClip.GetCornerRadius(outline));
            Assert.NotNull(outline.Clip);
        }

        [AvaloniaFact]
        public void Show_ACard_HasTheSpaceAroundItOfTheToken()
        {
            var (_, first, _) = ShowTwoCards();

            Assert.Equal(Tokens.Get<Thickness>("MarginCard"), first.Margin);
        }

        [AvaloniaFact]
        public void Show_TwoCardsInAStack_DoNotTouch()
        {
            var (_, first, second) = ShowTwoCards();
            var margin = Tokens.Get<Thickness>("MarginCard");

            var gap = second.Bounds.Top - first.Bounds.Bottom;

            Assert.Equal(margin.Top + margin.Bottom, gap);
        }

        [AvaloniaFact]
        public void IsExpanded_FoldedAndOpen_KeepsTheRoundedOutline()
        {
            var (_, first, _) = ShowTwoCards();
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
        public void Margin_SetByTheApplication_Wins()
        {
            var expander = new ExpanderIndented { Header = "x", Margin = new Thickness(0) };
            TestWindow.Show(expander, 300, 200);

            Assert.Equal(new Thickness(0), expander.Margin);
        }

        [AvaloniaFact]
        public void Show_AFirstLevelCard_HasTheLevelOneBackground()
        {
            var (_, first, _) = ShowTwoCards();
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
        public void CardMargin_TheToken_IsMadeOfTheSpacingScale()
        {
            var margin = Tokens.Get<Thickness>("MarginCard");

            Assert.Equal(Tokens.Get<double>("SpacingXS"), margin.Left);
            Assert.Equal(Tokens.Get<double>("SpacingXS"), margin.Right);
            Assert.Equal(Tokens.Get<double>("SpacingXXS"), margin.Top);
            Assert.Equal(Tokens.Get<double>("SpacingXXS"), margin.Bottom);
        }

        // Expanders on top of each other take the colors of their level: the outer one level 1, the next one level 2, and the innermost level 3.

        private static Color Level(int level) => Tokens.Color($"PanelBackgroundLevel{level}Brush");

        private static (ExpanderIndented outer, ExpanderIndented inner2, ExpanderIndented inner3) ShowNested()
        {
            var inner3 = new ExpanderIndented { Header = "3", IsExpanded = true, Content = new TextBlock { Text = "x" } };
            var inner2 = new ExpanderIndented { Header = "2", IsExpanded = true, Content = inner3 };
            var outer = new ExpanderIndented { Header = "1", IsExpanded = true, Content = inner2 };
            TestWindow.Show(outer, 400, 400);
            return (outer, inner2, inner3);
        }

        private static Color Content(ExpanderIndented expander) =>
            ((ISolidColorBrush)expander.GetVisualDescendants().OfType<Border>().First(border => border.Name == "contentBackground").Background!).Color;

        private static Color Header(ExpanderIndented expander) =>
            ((ISolidColorBrush)expander.GetVisualDescendants().OfType<Border>().First(border => border.Name == "mainBorder").Background!).Color;

        [AvaloniaFact]
        public void Show_NestedExpanders_GiveTheContentTheColorOfItsLevel()
        {
            var (outer, inner2, inner3) = ShowNested();

            Assert.Equal(Level(1), Content(outer));
            Assert.Equal(Level(2), Content(inner2));
            Assert.Equal(Level(3), Content(inner3));
        }

        // A panel inside a panel is one step brighter than its parent, at every depth.
        [AvaloniaFact]
        public void Show_NestedExpanders_MakeEachLevelBrighterThanTheLevelOutside()
        {
            double Brightness(Color color) => color.R + color.G + color.B;

            Assert.True(Brightness(Level(2)) > Brightness(Level(1)));
            Assert.True(Brightness(Level(3)) > Brightness(Level(2)));
        }

        [AvaloniaFact]
        public void Show_NestedExpanders_GiveTheHeaderTheColorOfItsLevel()
        {
            var (outer, inner2, inner3) = ShowNested();

            Assert.Equal(Level(1), Header(outer));
            Assert.Equal(Level(2), Header(inner2));
            Assert.Equal(Level(3), Header(inner3));
        }

        [AvaloniaFact]
        public void Background_SetOnTheOuterExpander_IsKept()
        {
            var grey = Colors.Orange;
            var expander = new ExpanderIndented { Header = "1", IsExpanded = true, Background = new SolidColorBrush(grey), Content = new TextBlock { Text = "x" } };
            TestWindow.Show(expander, 400, 400);

            Assert.Equal(grey, Content(expander));
            Assert.Equal(grey, Header(expander));
        }

        [AvaloniaFact]
        public void Background_SetOnANestedExpander_IsKeptAtEveryDepth()
        {
            var orange = Colors.Orange;
            var inner = new ExpanderIndented { Header = "2", IsExpanded = true, Background = new SolidColorBrush(orange), Content = new TextBlock { Text = "x" } };
            var outer = new ExpanderIndented { Header = "1", IsExpanded = true, Content = inner };
            TestWindow.Show(outer, 400, 400);

            Assert.Equal(orange, Content(inner));
            Assert.Equal(orange, Header(inner));
            Assert.Equal(Level(1), Content(outer));
        }

        // The expanders open and close their content with the animation of PanelCollapse.

        private static Control ShowExpander(string kind)
        {
            Control view = kind switch
            {
                "indented" => new ExpanderIndented { Header = "Header", Content = new Border { Height = 80 } },
                "ExpanderItem" => new Expander { Theme = Tokens.Theme("ExpanderItem"), Header = "Header", Content = new Border { Height = 80 } },
                _ => new Expander { Header = "Header", Content = new Border { Height = 80 } }
            };
            TestWindow.Show(new StackPanel { Children = { view } }, 300, 400);
            return view;
        }

        public static TheoryData<string> Kinds => new() { "default", "ExpanderItem", "indented" };

        [AvaloniaTheory]
        [MemberData(nameof(Kinds))]
        public void Show_AnExpanderOfEachTheme_HasOnePanelCollapse(string kind)
        {
            var view = ShowExpander(kind);

            Assert.Single(view.GetVisualDescendants().OfType<PanelCollapse>());
        }
    }
}
