// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls.Documents;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using CMiX.UI.Controls.String;
using Xunit;

namespace CMiX.UI.Tests
{
    // A TextBlock that draws a range of its text in its own brush.
    public class TextBlockHighlightedTests
    {
        private static string[] RunTextsOf(TextBlockHighlighted block) =>
            block.Inlines?.OfType<Run>().Select(run => run.Text ?? "").ToArray() ?? [];

        [AvaloniaFact]
        public void HighlightStart_ARange_DrawsThoseLettersInTheHighlightForeground()
        {
            var block = new TextBlockHighlighted { Text = "Blur", HighlightForeground = Brushes.Gold };

            block.HighlightStart = 1;
            block.HighlightLength = 2;

            Assert.Equal(new[] { "B", "lu", "r" }, RunTextsOf(block));
            Assert.Same(Brushes.Gold, block.HighlightRun!.Foreground);
            Assert.Equal("Blur", block.Text);
        }

        [AvaloniaFact]
        public void HighlightStart_ARange_KeepsTheWidthOfThePlainText()
        {
            var plain = new TextBlockHighlighted { Text = "Blur" };
            var highlighted = new TextBlockHighlighted { Text = "Blur", HighlightStart = 1, HighlightLength = 2 };

            plain.Measure(Size.Infinity);
            highlighted.Measure(Size.Infinity);

            Assert.True(plain.DesiredSize.Width > 0);
            Assert.Equal(plain.DesiredSize.Width, highlighted.DesiredSize.Width, 0.5);
        }

        [AvaloniaFact]
        public void HighlightStart_ARangePastTheEnd_DrawsThePlainText()
        {
            var block = new TextBlockHighlighted { Text = "Blur", HighlightStart = 3, HighlightLength = 2 };

            Assert.Empty(RunTextsOf(block));
            Assert.Null(block.HighlightRun);
        }

        [AvaloniaFact]
        public void Text_ANewTextDuringAHighlight_DrawsTheRangeOfTheNewText()
        {
            var block = new TextBlockHighlighted { Text = "Blur", HighlightStart = 0, HighlightLength = 2 };

            block.Text = "Glow";

            Assert.Equal(new[] { "", "Gl", "ow" }, RunTextsOf(block));
            Assert.Equal("Glow", block.Text);
        }

        [AvaloniaFact]
        public void HighlightLength_Zero_RemovesTheRuns()
        {
            var block = new TextBlockHighlighted { Text = "Blur", HighlightStart = 0, HighlightLength = 2 };

            block.HighlightLength = 0;

            Assert.Empty(RunTextsOf(block));
            Assert.Equal("Blur", block.Text);
        }
    }
}
