using Avalonia;
using Avalonia.Headless.XUnit;
using Xunit;

namespace CMiX.UI.Tests
{
    // XAML cannot add two tokens. These tests keep the tokens of Geometry.axaml that are sums of other tokens in step.
    public class GeometryTokenTests
    {
        private static double Size(string key) => Tokens.Get<double>(key);

        private static Thickness Space(string key) => Tokens.Get<Thickness>(key);

        [AvaloniaFact]
        public void ScrollBarSize_Token_IsTheThicknessPlusTwoMargins()
        {
            var margin = Space("ScrollBarMargin");

            Assert.Equal(Size("ScrollBarSize"), Size("ScrollBarThickness") + margin.Left + margin.Right);
        }

        [AvaloniaFact]
        public void ScrollBarGutter_Token_IsTheScrollBarSizeAtTheRight()
        {
            Assert.Equal(new Thickness(0, 0, Size("ScrollBarSize"), 0), Space("ScrollBarGutter"));
        }

        [AvaloniaFact]
        public void SplitterRowOverlap_Token_IsMinusHalfTheSplitterSize()
        {
            Assert.Equal(new Thickness(0, -Size("SplitterSize") / 2), Space("SplitterRowOverlap"));
        }

        [AvaloniaFact]
        public void SplitterColumnOverlap_Token_IsMinusTheSplitterSizeLessTheScrollBarMargin()
        {
            var margin = Space("ScrollBarMargin");

            Assert.Equal(new Thickness(0, 0, -(Size("SplitterSize") - margin.Right), 0), Space("SplitterColumnOverlap"));
        }
    }
}
