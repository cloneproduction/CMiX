// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using CMiX.UI.Search;

namespace CMiX.UI.Controls.String
{
    // A TextBlock that draws the letters of HighlightText in its text in HighlightForeground, AccentHighlightBrush by default.
    public class TextBlockHighlighted : TextBlock
    {
        private const string HighlightBrushKey = "AccentHighlightBrush";

        public static readonly StyledProperty<string?> HighlightTextProperty =
            AvaloniaProperty.Register<TextBlockHighlighted, string?>(nameof(HighlightText));

        public string? HighlightText
        {
            get => GetValue(HighlightTextProperty);
            set => SetValue(HighlightTextProperty, value);
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

            if (change.Property == TextProperty || change.Property == HighlightTextProperty || change.Property == HighlightForegroundProperty)
                ShowHighlight();
        }

        // A new collection each time: an Add to the collection of the TextBlock moves its Text into a run and empties the Text.
        private void ShowHighlight()
        {
            var start = SearchRules.IndexOfMatch(HighlightText, Text);
            if (start < 0)
            {
                ClearHighlight();
                return;
            }

            var text = Text!;
            var length = HighlightText!.Length;
            HighlightRun = CreateHighlightRun(text.Substring(start, length));
            Inlines = new InlineCollection
            {
                new Run(text.Substring(0, start)),
                HighlightRun,
                new Run(text.Substring(start + length)),
            };
        }

        private void ClearHighlight()
        {
            if (HighlightRun == null)
                return;

            HighlightRun = null;
            Inlines = new InlineCollection();
        }

        private Run CreateHighlightRun(string letters)
        {
            var run = new Run(letters);
            if (HighlightForeground != null)
                run.Foreground = HighlightForeground;
            else
                run.Bind(TextElement.ForegroundProperty, run.GetResourceObservable(HighlightBrushKey));

            return run;
        }
    }
}
