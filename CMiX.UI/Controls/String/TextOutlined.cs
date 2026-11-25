// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;

namespace CMiX.UI.Controls.String
{
    // A short text with an outline: it draws the outline of the glyphs with a pen, then fills them.
    public class TextOutlined : Control
    {
        public static readonly StyledProperty<string?> TextProperty =
            AvaloniaProperty.Register<TextOutlined, string?>(nameof(Text));

        public static readonly StyledProperty<FontFamily> FontFamilyProperty =
            TextElement.FontFamilyProperty.AddOwner<TextOutlined>();

        public static readonly StyledProperty<double> FontSizeProperty =
            TextElement.FontSizeProperty.AddOwner<TextOutlined>();

        public static readonly StyledProperty<FontWeight> FontWeightProperty =
            TextElement.FontWeightProperty.AddOwner<TextOutlined>();

        public static readonly StyledProperty<IBrush?> ForegroundProperty =
            TextElement.ForegroundProperty.AddOwner<TextOutlined>();

        public static readonly StyledProperty<IBrush?> OutlineBrushProperty =
            AvaloniaProperty.Register<TextOutlined, IBrush?>(nameof(OutlineBrush));

        public static readonly StyledProperty<double> OutlineThicknessProperty =
            AvaloniaProperty.Register<TextOutlined, double>(nameof(OutlineThickness));

        private static readonly AvaloniaProperty[] MeasureProperties =
            { TextProperty, FontFamilyProperty, FontSizeProperty, FontWeightProperty, OutlineThicknessProperty };

        private FormattedText? formatted;
        private Geometry? glyphs;
        private Pen? outline;

        static TextOutlined()
        {
            AffectsMeasure<TextOutlined>(MeasureProperties);
            AffectsRender<TextOutlined>(ForegroundProperty, OutlineBrushProperty);
        }

        public string? Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public FontFamily FontFamily
        {
            get => GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public double FontSize
        {
            get => GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public FontWeight FontWeight
        {
            get => GetValue(FontWeightProperty);
            set => SetValue(FontWeightProperty, value);
        }

        public IBrush? Foreground
        {
            get => GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        public IBrush? OutlineBrush
        {
            get => GetValue(OutlineBrushProperty);
            set => SetValue(OutlineBrushProperty, value);
        }

        public double OutlineThickness
        {
            get => GetValue(OutlineThicknessProperty);
            set => SetValue(OutlineThicknessProperty, value);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (Array.IndexOf(MeasureProperties, change.Property) >= 0)
            {
                formatted = null;
                glyphs = null;
            }

            if (change.Property == OutlineBrushProperty || change.Property == OutlineThicknessProperty)
                outline = null;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var text = Formatted();
            return new Size(text.WidthIncludingTrailingWhitespace + OutlineThickness, text.Height + OutlineThickness);
        }

        public override void Render(DrawingContext context)
        {
            glyphs ??= Formatted().BuildGeometry(new Point(OutlineThickness / 2, OutlineThickness / 2));
            if (glyphs == null)
                return;

            if (OutlineBrush != null && OutlineThickness > 0)
            {
                outline ??= new Pen(OutlineBrush, OutlineThickness, lineJoin: PenLineJoin.Round);
                context.DrawGeometry(null, outline, glyphs);
            }
            context.DrawGeometry(Foreground, null, glyphs);
        }

        // The brush only makes the geometry: a null brush gives no geometry.
        private FormattedText Formatted() =>
            formatted ??= new FormattedText(Text ?? "", CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                new Typeface(FontFamily, FontStyle.Normal, FontWeight), FontSize, Brushes.Black);
    }
}
