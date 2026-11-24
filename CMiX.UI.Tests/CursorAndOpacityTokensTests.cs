using Avalonia;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Xunit;

namespace CMiX.UI.Tests
{
    // The muted warning and the opacities are shared tokens with fixed values.
    public class CursorAndOpacityTokensTests
    {
        private static Color BrushColor(string key) => Tokens.Get<ISolidColorBrush>(key).Color;

        [AvaloniaTheory]
        [InlineData("OpacityDisabled", 0.5)]
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
