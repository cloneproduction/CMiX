using Avalonia;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Xunit;

namespace CMiX.UI.Tests
{
    // The cursor frame, the muted warning and the opacities are shared tokens with fixed values.
    public class CursorAndOpacityTokensTests
    {
        private static Color BrushColor(string key) => Tokens.Get<ISolidColorBrush>(key).Color;

        [AvaloniaFact]
        public void CursorFrameBrush_Token_IsTheActiveText()
        {
            Assert.Equal(Tokens.Get<Color>("ActiveText"), BrushColor("CursorFrameBrush"));
            Assert.Equal(BrushColor("ActiveForegroundBrush"), BrushColor("CursorFrameBrush"));
        }

        [AvaloniaFact]
        public void CursorFrameInactiveBrush_Token_IsGrey800()
        {
            Assert.Equal(Tokens.Get<Color>("Grey800"), BrushColor("CursorFrameInactiveBrush"));
        }

        [AvaloniaFact]
        public void CursorFrameThickness_Tokens_AreTwoAndOne()
        {
            Assert.Equal(new Thickness(2), Tokens.Get<Thickness>("CursorFrameThickness"));
            Assert.Equal(new Thickness(1), Tokens.Get<Thickness>("CursorFrameInactiveThickness"));
        }

        [AvaloniaTheory]
        [InlineData("OpacityDisabled", 0.5)]
        [InlineData("OpacityDragSource", 0.4)]
        [InlineData("OpacityDragGhost", 0.8)]
        [InlineData("OpacityIconIdle", 0.6)]
        [InlineData("OpacityIconAction", 0.9)]
        public void Opacity_Token_HasItsValue(string key, double expected)
        {
            Assert.Equal(expected, Tokens.Get<double>(key));
        }

        [AvaloniaFact]
        public void WarningMutedBrush_Token_IsNotThePopupCardWarning()
        {
            Assert.Equal(Color.Parse("#FFB4685C"), BrushColor("WarningMutedBrush"));
            Assert.NotEqual(BrushColor("PopupCardWarningBrush"), BrushColor("WarningMutedBrush"));
        }
    }
}
