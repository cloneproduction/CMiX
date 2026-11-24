using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Popups;
using Xunit;

namespace CMiX.UI.Tests
{
    // The tip of a card: its size on each edge and the side of its point.
    public class PopupTipTests
    {
        [Theory]
        [InlineData(PopupSide.Top)]
        [InlineData(PopupSide.Bottom)]
        public void SizeOf_AHorizontalEdge_IsWideAndLow(PopupSide edge)
        {
            var size = PopupTip.SizeOf(edge);

            Assert.Equal(PopupTip.Base, size.Width);
            Assert.Equal(PopupTip.Depth + PopupTip.Overlap, size.Height);
        }

        [Theory]
        [InlineData(PopupSide.Left)]
        [InlineData(PopupSide.Right)]
        public void SizeOf_AVerticalEdge_IsNarrowAndHigh(PopupSide edge)
        {
            var size = PopupTip.SizeOf(edge);

            Assert.Equal(PopupTip.Depth + PopupTip.Overlap, size.Width);
            Assert.Equal(PopupTip.Base, size.Height);
        }

        [Fact]
        public void Depth_OfTheTip_FitsInTheRoomOfTheShadow()
        {
            Assert.True(PopupTip.Depth <= PopupFrame.ShadowMargin.Top);
        }

        [Theory]
        [InlineData(PopupSide.Top)]
        [InlineData(PopupSide.Bottom)]
        [InlineData(PopupSide.Left)]
        [InlineData(PopupSide.Right)]
        public void Outline_OfEachEdge_HasTheBaseOnTheCardAndThePointAwayFromIt(PopupSide edge)
        {
            var points = PopupTip.Outline(edge);
            var size = PopupTip.SizeOf(edge);

            // The point is in the middle of the base, at the far side of the box.
            switch (edge)
            {
                case PopupSide.Top:
                    Assert.Equal(0, points[1].Y);
                    Assert.Equal(PopupTip.Depth, points[0].Y);
                    Assert.Equal(PopupTip.Base / 2, points[1].X);
                    break;
                case PopupSide.Bottom:
                    Assert.Equal(size.Height, points[1].Y);
                    Assert.Equal(PopupTip.Overlap, points[0].Y);
                    Assert.Equal(PopupTip.Base / 2, points[1].X);
                    break;
                case PopupSide.Left:
                    Assert.Equal(0, points[1].X);
                    Assert.Equal(PopupTip.Depth, points[0].X);
                    Assert.Equal(PopupTip.Base / 2, points[1].Y);
                    break;
                default:
                    Assert.Equal(size.Width, points[1].X);
                    Assert.Equal(PopupTip.Overlap, points[0].X);
                    Assert.Equal(PopupTip.Base / 2, points[1].Y);
                    break;
            }

            Assert.Equal(PopupTip.Base, Distance(points[0], points[2]));
        }

        [AvaloniaFact]
        public void Measure_ATipOnEachEdge_TakesTheSizeOfTheEdge()
        {
            var tip = new PopupTip { Edge = PopupSide.Left };
            tip.Measure(Size.Infinity);

            Assert.Equal(PopupTip.SizeOf(PopupSide.Left), tip.DesiredSize);

            tip.Edge = PopupSide.Bottom;
            tip.Measure(Size.Infinity);

            Assert.Equal(PopupTip.SizeOf(PopupSide.Bottom), tip.DesiredSize);
        }

        [AvaloniaFact]
        public void Tip_OfTheCard_IgnoresThePointer()
        {
            Assert.False(new PopupTip().IsHitTestVisible);
        }

        private static double Distance(Point a, Point b) => Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
    }
}
