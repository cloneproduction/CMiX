// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Globalization;
using Avalonia;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using CMiX.UI.Controls.String;
using Xunit;

namespace CMiX.UI.Tests
{
    public class TextOutlinedTests
    {
        private static Size TextSize(TextOutlined control)
        {
            var text = new FormattedText(control.Text!, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                new Typeface(control.FontFamily, FontStyle.Normal, control.FontWeight), control.FontSize, null);
            return new Size(text.WidthIncludingTrailingWhitespace, text.Height);
        }

        [AvaloniaFact]
        public void TextOutlined_ACount_MeasuresTheTextAndTheOutline()
        {
            var control = new TextOutlined { Text = "12", FontSize = 9, FontWeight = FontWeight.Bold, OutlineThickness = 2.5 };

            control.Measure(Size.Infinity);
            var text = TextSize(control);

            Assert.InRange(control.DesiredSize.Width - (text.Width + 2.5), -0.01, 1);
            Assert.InRange(control.DesiredSize.Height - (text.Height + 2.5), -0.01, 1);
        }

        [AvaloniaFact]
        public void TextOutlined_ALongerText_MeasuresAgain()
        {
            var control = new TextOutlined { Text = "1", FontSize = 9, OutlineThickness = 2.5 };
            control.Measure(Size.Infinity);
            var one = control.DesiredSize.Width;

            control.Text = "100";
            control.Measure(Size.Infinity);

            Assert.True(control.DesiredSize.Width > one);
        }
    }
}
