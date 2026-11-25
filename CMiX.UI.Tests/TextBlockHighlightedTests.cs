// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using CMiX.UI.Controls.String;
using CMiX.UI.Tests.Support;
using Xunit;

namespace CMiX.UI.Tests
{
    // A TextBlock that draws the letters of its highlight text in its own brush.
    public class TextBlockHighlightedTests
    {
        private static string[] RunTextsOf(TextBlockHighlighted block) =>
            block.Inlines?.OfType<Run>().Select(run => run.Text ?? "").ToArray() ?? [];

        [AvaloniaFact]
        public void HighlightText_APartOfTheText_DrawsThoseLettersInTheHighlightForeground()
        {
            var block = new TextBlockHighlighted { Text = "Blur", HighlightForeground = Brushes.Gold };

            block.HighlightText = "lu";

            Assert.Equal(new[] { "B", "lu", "r" }, RunTextsOf(block));
            Assert.Same(Brushes.Gold, block.HighlightRun!.Foreground);
            Assert.Equal("Blur", block.Text);
        }

        [AvaloniaFact]
        public void HighlightText_OtherCase_DrawsTheLettersOfTheText()
        {
            var block = new TextBlockHighlighted { Text = "Blur", HighlightText = "LU" };

            Assert.Equal(new[] { "B", "lu", "r" }, RunTextsOf(block));
        }

        [AvaloniaFact]
        public void HighlightForeground_NotSet_IsTheAccentHighlightBrush()
        {
            var block = new TextBlockHighlighted { Text = "Blur", HighlightText = "lu" };
            var window = TestWindow.Show(block, 200, 100);
            var app = Application.Current!;
            Assert.True(app.TryFindResource("AccentHighlightBrush", app.ActualThemeVariant, out var brush));

            Assert.Same(brush, block.HighlightRun!.Foreground);
            window.Close();
        }

        [AvaloniaFact]
        public void HighlightText_APartOfTheText_KeepsTheWidthOfThePlainText()
        {
            var plain = new TextBlockHighlighted { Text = "Blur" };
            var highlighted = new TextBlockHighlighted { Text = "Blur", HighlightText = "lu" };

            plain.Measure(Size.Infinity);
            highlighted.Measure(Size.Infinity);

            Assert.True(plain.DesiredSize.Width > 0);
            Assert.Equal(plain.DesiredSize.Width, highlighted.DesiredSize.Width, 0.5);
        }

        [AvaloniaFact]
        public void HighlightText_NotInTheText_DrawsThePlainText()
        {
            var block = new TextBlockHighlighted { Text = "Blur", HighlightText = "rx" };

            Assert.Empty(RunTextsOf(block));
            Assert.Null(block.HighlightRun);
        }

        [AvaloniaFact]
        public void HighlightText_NoMatchAndNoRunBefore_KeepsTheInlines()
        {
            var block = new TextBlockHighlighted { Text = "Blur" };
            var inlines = block.Inlines;

            block.HighlightText = "rx";
            block.Text = "Glow";

            Assert.Same(inlines, block.Inlines);
        }

        [AvaloniaFact]
        public void Text_ANewTextDuringAHighlight_DrawsTheLettersInTheNewText()
        {
            var block = new TextBlockHighlighted { Text = "Blur", HighlightText = "l" };

            block.Text = "Glow";

            Assert.Equal(new[] { "G", "l", "ow" }, RunTextsOf(block));
            Assert.Equal("Glow", block.Text);
        }

        [AvaloniaFact]
        public void HighlightText_Null_RemovesTheRuns()
        {
            var block = new TextBlockHighlighted { Text = "Blur", HighlightText = "Bl" };

            block.HighlightText = null;

            Assert.Empty(RunTextsOf(block));
            Assert.Null(block.HighlightRun);
            Assert.Equal("Blur", block.Text);
        }
    }
}
