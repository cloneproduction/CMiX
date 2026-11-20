// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace CMiX.UI.Controls.Parts
{
    // One quiet line at the bottom of a panel: a short list of keys and what they do. Each key is a key cap (ContentControlKeyCap), followed by
    // its words. The keys of one hint are apart by a slash. The text is static: the owner sets new hints when its mode changes.
    // The strip has no hit test and takes no focus, so a click goes through it. A hint that does not fit at the right is cut.
    public class KeyHintStrip : Border
    {
        // The slash between two keys of one hint.
        public const string KeySeparator = "/";

        public static readonly StyledProperty<IReadOnlyList<KeyHint>?> HintsProperty =
            AvaloniaProperty.Register<KeyHintStrip, IReadOnlyList<KeyHint>?>(nameof(Hints));

        public IReadOnlyList<KeyHint>? Hints
        {
            get => GetValue(HintsProperty);
            set => SetValue(HintsProperty, value);
        }

        private readonly StackPanel line = new() { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };

        public KeyHintStrip()
        {
            Focusable = false;
            IsHitTestVisible = false;
            ClipToBounds = true;
            this.Bind(HeightProperty, this.GetResourceObservable("HintStripHeight"));
            this.Bind(PaddingProperty, this.GetResourceObservable("HintStripPadding"));
            this.Bind(BackgroundProperty, this.GetResourceObservable("HintStripBackgroundBrush"));
            line.Bind(StackPanel.SpacingProperty, line.GetResourceObservable("HintStripSpacing"));
            Child = line;
        }

        protected override Type StyleKeyOverride => typeof(Border);

        // The line of the hints. A test reads it.
        internal StackPanel Line => line;

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == HintsProperty)
                ShowHints();
        }

        private void ShowHints()
        {
            line.Children.Clear();
            foreach (var hint in Hints ?? Array.Empty<KeyHint>())
            {
                var item = new StackPanel { Orientation = Orientation.Horizontal };
                item.Bind(StackPanel.SpacingProperty, item.GetResourceObservable("SpacingSM"));
                for (var i = 0; i < hint.Keys.Count; i++)
                {
                    if (i > 0)
                        item.Children.Add(Words(KeySeparator));

                    var cap = new ContentControl { Focusable = false, Content = hint.Keys[i] };
                    cap.Bind(ThemeProperty, cap.GetResourceObservable("ContentControlKeyCap"));
                    item.Children.Add(cap);
                }

                item.Children.Add(Words(hint.Words));
                line.Children.Add(item);
            }
        }

        private static TextBlock Words(string text)
        {
            var words = new TextBlock { Text = text };
            words.Bind(TextBlock.ForegroundProperty, words.GetResourceObservable("HintStripTextBrush"));
            return words;
        }
    }
}
