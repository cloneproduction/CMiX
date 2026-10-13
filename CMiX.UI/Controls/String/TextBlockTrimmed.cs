// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace CMiX.UI.Controls.String
{
    // A TextBlock for a name that can be long, such as the name of a file or a folder. It stays on one line
    // and ends with "..." when it does not fit. The full text shows on hover, but only while it is cut.
    // ToolTipText replaces the hover text, for example with the whole path of a file.
    public class TextBlockTrimmed : TextBlock
    {
        static TextBlockTrimmed()
        {
            TextTrimmingProperty.OverrideDefaultValue<TextBlockTrimmed>(TextTrimming.CharacterEllipsis);
            TextWrappingProperty.OverrideDefaultValue<TextBlockTrimmed>(TextWrapping.NoWrap);
            ToolTipTextProperty.Changed.AddClassHandler<TextBlockTrimmed>((block, e) => block.UpdateToolTip(block.Bounds.Width));
        }

        // The styles of the TextBlock apply to this control as well.
        protected override Type StyleKeyOverride => typeof(TextBlock);

        public static readonly StyledProperty<string?> ToolTipTextProperty =
            AvaloniaProperty.Register<TextBlockTrimmed, string?>(nameof(ToolTipText));

        public string? ToolTipText
        {
            get => GetValue(ToolTipTextProperty);
            set => SetValue(ToolTipTextProperty, value);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var size = base.ArrangeOverride(finalSize);
            UpdateToolTip(size.Width);
            return size;
        }

        private void UpdateToolTip(double width)
        {
            if (!string.IsNullOrEmpty(ToolTipText))
            {
                ToolTip.SetTip(this, ToolTipText);
                return;
            }

            ToolTip.SetTip(this, IsCut(width) ? Text : null);
        }

        private bool IsCut(double width)
        {
            if (string.IsNullOrEmpty(Text))
                return false;

            var typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
            var full = new FormattedText(Text, CultureInfo.CurrentCulture, FlowDirection, typeface, FontSize, null);
            return full.Width > width + 0.5;
        }
    }
}
