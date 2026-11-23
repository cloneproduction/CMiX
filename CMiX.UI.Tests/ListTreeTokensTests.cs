// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using CMiX.UI.Mathematics;
using Xunit;

namespace CMiX.UI.Tests
{
    // The tokens of the new list tree have the values of the spec in docs/tree-port-plan.md.
    public class ListTreeTokensTests
    {
        private static Color BrushColor(string key) => Tokens.Get<ISolidColorBrush>(key).Color;

        private static Color WhiteOverTheTreeBackground(double strength) =>
            ColorMix.Over(Colors.White, Tokens.Get<Color>("TreeBackground"), strength);

        [AvaloniaTheory]
        [InlineData("ListTreeRowHeight", 32)]
        [InlineData("ListTreeRowGap", 3)]
        [InlineData("ListTreeIndent", 16)]
        [InlineData("ListTreeLeftBorderWidth", 3)]
        [InlineData("ListTreeDropOutlineThickness", 1.5)]
        [InlineData("ListTreeDropOutlineOffset", 2)]
        [InlineData("ListTreeToggleSize", 32)]
        [InlineData("ListTreeToggleGap", 8)]
        [InlineData("ListTreeToggleColumnWidth", 72)]
        public void Size_EachKey_HasTheValueOfTheSpec(string key, double expected)
        {
            Assert.Equal(expected, Tokens.Get<double>(key));
        }

        [AvaloniaFact]
        public void Space_EachKey_HasTheValueOfTheSpec()
        {
            Assert.Equal(new Thickness(2, 0, 0, 0), Tokens.Get<Thickness>("ListTreeRowContentMargin"));
            Assert.Equal(new Thickness(8, 0), Tokens.Get<Thickness>("ListTreeIconMargin"));
            Assert.Equal(new Thickness(6, 0), Tokens.Get<Thickness>("ListTreeNamePadding"));
            Assert.Equal(new Thickness(8, 8, 0, 8), Tokens.Get<Thickness>("ListTreeMargin"));
            Assert.Equal(new Thickness(4), Tokens.Get<Thickness>("ListTreeToolbarPadding"));
        }

        [AvaloniaFact]
        public void ToggleColumnWidth_Token_IsTwoSlotsAndTheGap()
        {
            var slot = Tokens.Get<double>("ListTreeToggleSize");

            Assert.Equal(slot + Tokens.Get<double>("ListTreeToggleGap") + slot, Tokens.Get<double>("ListTreeToggleColumnWidth"));
        }

        [AvaloniaTheory]
        [InlineData("ListTreeRowHoverBrush", 0.05)]
        [InlineData("ListTreeCardHoverBrush", 0.05)]
        [InlineData("ListTreeRowSelectedBrush", 0.07)]
        [InlineData("ListTreeRowSelectedHoverBrush", 0.10)]
        [InlineData("ListTreeRowDraggedBrush", 0.16)]
        [InlineData("ListTreeCardDropTargetBrush", 0.05)]
        public void Surface_EachOpaqueKey_IsWhiteOverTheTreeBackground(string key, double strength)
        {
            Assert.Equal(WhiteOverTheTreeBackground(strength), BrushColor(key));
        }

        [AvaloniaTheory]
        [InlineData("ListTreeRowHoverBrush", "#FF1B1B1E")]
        [InlineData("ListTreeRowSelectedBrush", "#FF202023")]
        [InlineData("ListTreeRowSelectedHoverBrush", "#FF27272A")]
        [InlineData("ListTreeRowDraggedBrush", "#FF353538")]
        [InlineData("ListTreeRowRestBandBrush", "#0DFFFFFF")]
        [InlineData("ListTreeRowDimmedBandBrush", "#05FFFFFF")]
        [InlineData("ListTreeRowHoverInSelectionBandBrush", "#0DFFFFFF")]
        [InlineData("ListTreeCardHoverInSelectionBandBrush", "#08FFFFFF")]
        [InlineData("ListTreeTextRestBrush", "#FFB6B6BB")]
        [InlineData("ListTreeTextDimmedBrush", "#FF9A9A9F")]
        [InlineData("ListTreeTextHiddenBrush", "#FF8A8A8F")]
        [InlineData("ListTreeIconHiddenBrush", "#FF6C6C6F")]
        public void Brush_EachKey_HasTheColourOfTheSpec(string key, string expected)
        {
            Assert.Equal(Color.Parse(expected), BrushColor(key));
        }

        [AvaloniaTheory]
        [InlineData("ListTreeTextActiveBrush", "TreeTextRest")]
        [InlineData("ListTreeTextSelectedBrush", "ActiveText")]
        [InlineData("ListTreeIconDimmedBrush", "Grey800")]
        [InlineData("ListTreeBackgroundBrush", "TreeBackground")]
        [InlineData("ListTreeToolbarBackgroundBrush", "Grey300")]
        public void Brush_EachSharedKey_IsTheColourOfTheTree(string key, string colour)
        {
            Assert.Equal(Tokens.Get<Color>(colour), BrushColor(key));
        }
    }
}
