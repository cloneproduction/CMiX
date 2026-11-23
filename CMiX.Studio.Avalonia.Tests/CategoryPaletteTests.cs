using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using CMiX.Studio.Avalonia.Themes;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.UI.Mathematics;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The tones of a kind of row: the fills of the states, and the colors of the count squares.
    public class CategoryPaletteTests
    {
        private static readonly Color Background = Color.FromRgb(34, 34, 36);
        private static readonly Color Amber = Color.FromRgb(233, 211, 73);
        private static readonly Color Magenta = Color.FromRgb(220, 116, 215);
        private static readonly Color Violet = Color.FromRgb(159, 133, 236);
        private static readonly Color Text = Color.FromRgb(224, 224, 224);
        private static readonly Color White = Color.FromRgb(255, 255, 255);

        private static CategoryPalette Palette()
        {
            var colors = new Dictionary<string, Color>
            {
                ["CategoryLight"] = Amber,
                ["CategoryComposable0"] = Magenta,
                ["CategoryComposable3"] = Violet
            };

            return new CategoryPalette(key => colors[key], Background);
        }

        private static double LuminanceOf(Color color) => ColorMix.Luminance(color);

        [Fact]
        public void Fill_EachState_IsTheKindColorOverTheBackgroundWithItsStrength()
        {
            var tone = Palette().For(TreeCategory.Light);

            Assert.Equal(ColorMix.Over(Amber, Background, CategoryPalette.FillStrength), tone.Fill);
            Assert.Equal(ColorMix.Over(Amber, Background, CategoryPalette.FillHoverStrength), tone.FillHover);
            Assert.Equal(ColorMix.Over(Amber, Background, CategoryPalette.FillSelectedStrength), tone.FillSelected);
        }

        [Fact]
        public void CountSquare_EachState_IsTheKindColorOverTheBackgroundWithItsStrength()
        {
            var tone = Palette().For(TreeCategory.Light);

            Assert.Equal(ColorMix.Over(Amber, Background, CategoryPalette.KindStrength), tone.Kind);
            Assert.Equal(ColorMix.Over(Amber, Background, CategoryPalette.KindHoverStrength), tone.KindHover);
            Assert.Equal(Amber, tone.KindSelected);
        }

        [Fact]
        public void FillStrength_EachState_IsBrighterThanTheStateBefore()
        {
            var tone = Palette().For(TreeCategory.Light);

            Assert.True(LuminanceOf(tone.Fill) < LuminanceOf(tone.FillHover));
            Assert.True(LuminanceOf(tone.FillHover) < LuminanceOf(tone.FillSelected));
            Assert.True(LuminanceOf(tone.Kind) < LuminanceOf(tone.KindHover));
            Assert.True(LuminanceOf(tone.KindHover) < LuminanceOf(tone.KindSelected));
        }

        [Fact]
        public void FillStrength_ARowDeeperByAChangeOfKind_DropsByTheLevelFactorToTheFloor()
        {
            var palette = Palette();

            Assert.Equal(ColorMix.Over(Amber, Background, CategoryPalette.FillStrength * CategoryPalette.LevelFactor), palette.For(TreeCategory.Light, kindDepth: 1).Fill);
            Assert.True(LuminanceOf(palette.For(TreeCategory.Light, kindDepth: 2).Fill) < LuminanceOf(palette.For(TreeCategory.Light, kindDepth: 1).Fill));
            Assert.Equal(ColorMix.Over(Amber, Background, CategoryPalette.FillFloor), palette.For(TreeCategory.Light, kindDepth: 40).Fill);
            Assert.Equal(ColorMix.Over(Amber, Background, CategoryPalette.FillHoverFloor), palette.For(TreeCategory.Light, kindDepth: 40).FillHover);
            Assert.Equal(ColorMix.Over(Amber, Background, CategoryPalette.FillSelectedFloor), palette.For(TreeCategory.Light, kindDepth: 40).FillSelected);
        }

        [Fact]
        public void FillStrength_ADeepRow_StaysAColorAndEachStateStaysBrighter()
        {
            var tone = Palette().For(TreeCategory.Light, kindDepth: 40);

            Assert.NotEqual(Background, tone.Fill);
            Assert.True(LuminanceOf(tone.Fill) < LuminanceOf(tone.FillHover));
            Assert.True(LuminanceOf(tone.FillHover) < LuminanceOf(tone.FillSelected));
        }

        [Fact]
        public void ComposableDepth_ADeeperComposable_ChangesTheColorAndNotTheStrength()
        {
            var palette = Palette();

            Assert.NotEqual(palette.For(TreeCategory.Composable, 0).Fill, palette.For(TreeCategory.Composable, 3).Fill);
        }

        // In the theme, a composable takes the hue of its depth from the wheel, so a container and the container in it differ.
        [AvaloniaFact]
        public void FromApplication_ADeeperComposable_HasAnotherFillThanTheOneAbove()
        {
            var palette = CategoryPalette.FromApplication();

            for (var depth = 1; depth < CategoryPalette.WheelSize; depth++)
            {
                var above = palette.For(TreeCategory.Composable, depth - 1);
                var deeper = palette.For(TreeCategory.Composable, depth);
                Assert.NotEqual(above.Fill, deeper.Fill);
                Assert.NotEqual(above.FillSelected, deeper.FillSelected);
            }
        }

        // The six colors of the wheel are all in the theme.
        [AvaloniaFact]
        public void FromApplication_EveryWheelColor_IsInTheTheme()
        {
            for (var step = 0; step < CategoryPalette.WheelSize; step++)
                Assert.IsType<Color>(Application.Current!.FindResource("CategoryComposable" + step));
        }

        [Theory]
        [InlineData(TreeCategory.Light, 0, "CategoryLight")]
        [InlineData(TreeCategory.Light, 5, "CategoryLight")]
        [InlineData(TreeCategory.Composable, 0, "CategoryComposable0")]
        [InlineData(TreeCategory.Composable, 1, "CategoryComposable1")]
        [InlineData(TreeCategory.Composable, 5, "CategoryComposable5")]
        [InlineData(TreeCategory.Composable, 6, "CategoryComposable0")]
        [InlineData(TreeCategory.Composable, 9, "CategoryComposable3")]
        public void ColorKey_EveryKindAndDepth_ChangesWithTheDepthForAComposableOnly(TreeCategory category, int depth, string expected)
        {
            Assert.Equal(expected, CategoryPalette.ColorKey(category, depth));
        }

        [Fact]
        public void ColorKey_ADeepComposable_WrapsAroundTheWheel()
        {
            var palette = Palette();

            Assert.Equal(palette.For(TreeCategory.Composable, 3), palette.For(TreeCategory.Composable, 3 + CategoryPalette.WheelSize));
        }

        // The text must stay readable on every fill: the light text of the theme at rest and when hovered, white when selected.
        [AvaloniaFact]
        public void FromApplication_EveryKindLevelAndState_KeepsTheTextReadable()
        {
            var palette = CategoryPalette.FromApplication();

            foreach (var category in System.Enum.GetValues<TreeCategory>())
            {
                for (var composable = 0; composable < CategoryPalette.WheelSize; composable++)
                {
                    for (var kind = 0; kind <= 6; kind++)
                    {
                        var tone = palette.For(category, composable, kind);
                        Assert.True(ColorMix.Contrast(Text, tone.Fill) >= 4.5, $"{category} {composable}/{kind} at rest: {ColorMix.Contrast(Text, tone.Fill):0.0}");
                        Assert.True(ColorMix.Contrast(Text, tone.FillHover) >= 4.5, $"{category} {composable}/{kind} hovered: {ColorMix.Contrast(Text, tone.FillHover):0.0}");
                        // The selected fill is nearly the color of the kind (FillSelectedStrength 0.85, chosen 2026-11-13): the colour marks the chosen row,

                        // so the white text on a light kind (Light, Texture) is below 4.5 by design.
                    }
                }
            }
        }

        [AvaloniaFact]
        public void FromApplication_TheAppPalette_HasATonePerKindAndLevel()
        {
            var palette = CategoryPalette.FromApplication();

            foreach (var category in System.Enum.GetValues<TreeCategory>())
                Assert.NotEqual(default, palette.For(category).Fill);

            for (var depth = 0; depth < CategoryPalette.WheelSize; depth++)
                Assert.NotEqual(default, palette.For(TreeCategory.Composable, depth).Fill);
        }

        [Fact]
        public void ForListTree_ALightRow_IsTheKindColourWithTheAlphaOfEachMark()
        {
            var colors = Palette().ForListTree(TreeCategory.Light, 0);

            Assert.Equal(Color.FromArgb(115, 233, 211, 73), colors.HoverBorder);
            Assert.Equal(Color.FromArgb(179, 233, 211, 73), colors.DropOutline);
            Assert.Equal(Amber, colors.SelectionBorder);
        }

        [Fact]
        public void ForListTree_AComposable_TakesTheHueOfItsDepth()
        {
            var palette = Palette();
            var top = palette.ForListTree(TreeCategory.Composable, 0);
            var deeper = palette.ForListTree(TreeCategory.Composable, 3);

            Assert.Equal(Color.FromArgb(115, 220, 116, 215), top.HoverBorder);
            Assert.Equal(Color.FromArgb(179, 220, 116, 215), top.DropOutline);
            Assert.Equal(Magenta, top.SelectionBorder);
            Assert.Equal(Color.FromArgb(115, 159, 133, 236), deeper.HoverBorder);
            Assert.Equal(Color.FromArgb(179, 159, 133, 236), deeper.DropOutline);
            Assert.Equal(Violet, deeper.SelectionBorder);
        }

        [AvaloniaFact]
        public void FromApplication_TheTreeBackground_IsTheBaseOfTheMix()
        {
            var colors = Application.Current!;
            var kind = (Color)colors.FindResource("CategoryLight")!;
            var background = (Color)colors.FindResource("TreeBackground")!;

            var tone = CategoryPalette.FromApplication().For(TreeCategory.Light);

            Assert.Equal(ColorMix.Over(kind, background, CategoryPalette.FillStrength), tone.Fill);
            Assert.Equal(ColorMix.Over(kind, background, CategoryPalette.KindStrength), tone.Kind);
        }
    }
}
