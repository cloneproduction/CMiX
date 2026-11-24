using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Popups;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // The card of a tooltip and of a panel: its look from the tokens, the room of the shadow, and the place of the tip on each edge.
    public class PopupCardTests
    {
        private static (PopupCard Card, Border Box, PopupTip Tip) Show()
        {
            var card = new PopupCard
            {
                Content = new TextBlock { Text = "A card with some text" },
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };
            TestWindow.Show(card, 400, 300);
            card.ApplyTemplate();
            Dispatcher.UIThread.RunJobs();

            var box = card.GetVisualDescendants().OfType<Border>().First(border => border.Name == "PART_Surface");
            var tip = card.GetVisualDescendants().OfType<PopupTip>().First();
            return (card, box, tip);
        }

        private static PlacementResult Result(PopupSide side, double offset) => new(default, side, offset, false);

        [AvaloniaFact]
        public void Card_InAWindow_HasTheLookOfTheTokens()
        {
            var (_, box, _) = Show();

            Assert.Equal(Tokens.Color("PopupBackgroundBrush"), ((ISolidColorBrush)box.Background!).Color);
            Assert.Equal(new Thickness(0), box.BorderThickness);
            Assert.Equal(Tokens.Get<CornerRadius>("CornerRadiusAll"), box.CornerRadius);
        }

        [AvaloniaFact]
        public void Card_InAWindow_KeepsTheRoomOfTheShadowAroundTheBox()
        {
            var (card, box, _) = Show();

            var at = box.TranslatePoint(default, card)!.Value;

            Assert.Equal(PopupFrame.ShadowMargin.Left, at.X);
            Assert.Equal(PopupFrame.ShadowMargin.Top, at.Y);
            Assert.Equal(card.Bounds.Width, box.Bounds.Width + PopupFrame.ShadowMargin.Left + PopupFrame.ShadowMargin.Right);
        }

        [AvaloniaFact]
        public void Card_WithoutAPlacement_ShowsNoTip()
        {
            var (_, _, tip) = Show();

            Assert.False(tip.IsEffectivelyVisible);
        }

        [AvaloniaFact]
        public void Place_ATopTip_SitsAboveTheTopEdgeAtTheOffset()
        {
            var (card, box, tip) = Show();

            card.Place(Result(PopupSide.Bottom, 40));
            Dispatcher.UIThread.RunJobs();

            var at = tip.TranslatePoint(default, box)!.Value;
            Assert.True(tip.IsEffectivelyVisible);
            Assert.Equal(PopupSide.Top, tip.Edge);
            Assert.Equal(40 - PopupTip.Base / 2, at.X);
            Assert.Equal(-PopupTip.Depth, at.Y);
        }

        [AvaloniaFact]
        public void Place_ABottomTip_SitsUnderTheBottomEdgeAtTheOffset()
        {
            var (card, box, tip) = Show();

            card.Place(Result(PopupSide.Top, 40));
            Dispatcher.UIThread.RunJobs();

            var at = tip.TranslatePoint(default, box)!.Value;
            Assert.Equal(PopupSide.Bottom, tip.Edge);
            Assert.Equal(40 - PopupTip.Base / 2, at.X);
            Assert.Equal(box.Bounds.Height - PopupTip.Overlap, at.Y);
        }

        [AvaloniaFact]
        public void Place_ALeftTip_SitsBeforeTheLeftEdgeAtTheOffset()
        {
            var (card, box, tip) = Show();

            card.Place(Result(PopupSide.Right, 12));
            Dispatcher.UIThread.RunJobs();

            var at = tip.TranslatePoint(default, box)!.Value;
            Assert.Equal(PopupSide.Left, tip.Edge);
            Assert.Equal(-PopupTip.Depth, at.X);
            Assert.Equal(12 - PopupTip.Base / 2, at.Y);
        }

        [AvaloniaFact]
        public void Place_ARightTip_SitsAfterTheRightEdgeAtTheOffset()
        {
            var (card, box, tip) = Show();

            card.Place(Result(PopupSide.Left, 12));
            Dispatcher.UIThread.RunJobs();

            var at = tip.TranslatePoint(default, box)!.Value;
            Assert.Equal(PopupSide.Right, tip.Edge);
            Assert.Equal(box.Bounds.Width - PopupTip.Overlap, at.X);
            Assert.Equal(12 - PopupTip.Base / 2, at.Y);
        }

        [AvaloniaTheory]
        [InlineData(PopupSide.Top)]
        [InlineData(PopupSide.Bottom)]
        [InlineData(PopupSide.Left)]
        [InlineData(PopupSide.Right)]
        public void Place_ATipOnAnyEdge_StaysInsideTheRoomOfTheShadow(PopupSide side)
        {
            var (card, _, tip) = Show();

            card.Place(Result(side, 20));
            Dispatcher.UIThread.RunJobs();

            var at = tip.TranslatePoint(default, card)!.Value;
            Assert.True(at.X >= 0 && at.Y >= 0);
            Assert.True(at.X + tip.Bounds.Width <= card.Bounds.Width);
            Assert.True(at.Y + tip.Bounds.Height <= card.Bounds.Height);
        }
    }
}
