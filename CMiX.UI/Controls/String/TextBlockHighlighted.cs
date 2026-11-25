// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;

namespace CMiX.UI.Controls.String
{
    // A TextBlock that draws a range of its text, such as the letters of a search match, in HighlightForeground.
    // The range comes from properties only, so a control that is used again for other data shows the range of that data.
    public class TextBlockHighlighted : TextBlock
    {
        public static readonly StyledProperty<int> HighlightStartProperty =
            AvaloniaProperty.Register<TextBlockHighlighted, int>(nameof(HighlightStart), -1);

        public int HighlightStart
        {
            get => GetValue(HighlightStartProperty);
            set => SetValue(HighlightStartProperty, value);
        }

        public static readonly StyledProperty<int> HighlightLengthProperty =
            AvaloniaProperty.Register<TextBlockHighlighted, int>(nameof(HighlightLength));

        public int HighlightLength
        {
            get => GetValue(HighlightLengthProperty);
            set => SetValue(HighlightLengthProperty, value);
        }

        public static readonly StyledProperty<IBrush?> HighlightForegroundProperty =
            AvaloniaProperty.Register<TextBlockHighlighted, IBrush?>(nameof(HighlightForeground));

        public IBrush? HighlightForeground
        {
            get => GetValue(HighlightForegroundProperty);
            set => SetValue(HighlightForegroundProperty, value);
        }

        // The styles of the TextBlock apply to this control as well.
        protected override Type StyleKeyOverride => typeof(TextBlock);

        // The run of the highlighted letters, or null when no letter is highlighted. For the tests.
        internal Run? HighlightRun { get; private set; }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == TextProperty || change.Property == HighlightStartProperty
                || change.Property == HighlightLengthProperty || change.Property == HighlightForegroundProperty)
                ShowHighlight();
        }

        private bool HasHighlight =>
            Text != null && HighlightStart >= 0 && HighlightLength > 0 && HighlightStart + HighlightLength <= Text.Length;

        // A new collection each time: an Add to the collection of the TextBlock moves its Text into a run and empties the Text.
        private void ShowHighlight()
        {
            if (!HasHighlight)
            {
                HighlightRun = null;
                Inlines = new InlineCollection();
                return;
            }

            var text = Text!;
            HighlightRun = new Run(text.Substring(HighlightStart, HighlightLength)) { Foreground = HighlightForeground };
            Inlines = new InlineCollection
            {
                new Run(text.Substring(0, HighlightStart)),
                HighlightRun,
                new Run(text.Substring(HighlightStart + HighlightLength)),
            };
        }
    }
}
