using Avalonia.Headless.XUnit;
using Xunit;

namespace CMiX.UI.Tests
{
    // The sizes of the UI are one scale. XAML cannot add two tokens, so the tokens that follow ControlHeightMD
    // are written as numbers in Geometry.axaml, and this test keeps them in step. To make the UI bigger or smaller,
    // change ControlHeightMD and move the others by the same amount.
    public class ScaleTokensTests
    {
        private static double Size(string key) => Tokens.Get<double>(key);

        [AvaloniaFact]
        public void TheControlHeights_AreOffsetsOfControlHeightMD()
        {
            var md = Size("ControlHeightMD");

            Assert.Equal(md - 8, Size("ControlHeightXS"));
            Assert.Equal(md - 4, Size("ControlHeightSM"));
            Assert.Equal(md + 6, Size("ControlHeightXL"));
            Assert.Equal(md + 10, Size("ControlHeightXXL"));
        }

        [AvaloniaFact]
        public void TheIconSizes_FollowTheControlHeights()
        {
            var md = Size("ControlHeightMD");

            // An icon sits in a row of ControlHeightMD with 4 on each side.
            Assert.Equal(md - 8, Size("IconSizeXS"));
            Assert.Equal(Size("IconSizeXS"), Size("IconSizeMD"));
            Assert.Equal(Size("IconSizeXS") - 4, Size("IconSizeSM"));
            Assert.Equal(md, Size("IconSizeLG"));
        }

        [AvaloniaFact]
        public void TheTextAreas_AreWholeLinesOfARow()
        {
            var line = Size("ControlHeightMD") - 2;

            Assert.Equal(2 * line, Size("TextAreaHeight2"));
            Assert.Equal(4 * line, Size("TextAreaHeight4"));
        }

        [AvaloniaFact]
        public void TheFontSize_IsInProportionToTheRow()
        {
            // About half a row, rounded: 12 in a row of 24.
            Assert.InRange(Size("FontSizeMD"), Size("ControlHeightMD") / 2 - 0.5, Size("ControlHeightMD") / 2 + 0.5);
        }
    }
}
