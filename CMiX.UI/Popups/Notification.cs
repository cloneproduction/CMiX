// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using CMiX.UI.Controls.Parts;

namespace CMiX.UI.Popups
{
    // A short message on a popup card, for example "Removed Layer 1". When it has an undo gesture, the gesture shows as a key cap
    // (ContentControlKeyCap) followed by the word "undo". A PanelNotification shows it. It has no tip and takes no focus.
    public class Notification : PopupCard
    {
        public const string UndoWord = "undo";

        public static readonly StyledProperty<string?> TextProperty =
            AvaloniaProperty.Register<Notification, string?>(nameof(Text));

        public string? Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly StyledProperty<string?> UndoGestureProperty =
            AvaloniaProperty.Register<Notification, string?>(nameof(UndoGesture));

        // The text of the undo gesture, such as "Ctrl+Z". Null shows no key cap and no undo word.
        public string? UndoGesture
        {
            get => GetValue(UndoGestureProperty);
            set => SetValue(UndoGestureProperty, value);
        }

        private readonly TextBlock message = new();
        private readonly ContentControl keyCap = new() { Focusable = false, IsVisible = false };
        private readonly TextBlock undoWord = new() { Text = UndoWord, IsVisible = false };

        public Notification()
        {
            Focusable = false;
            ShowsTip = false;
            keyCap.Bind(ThemeProperty, keyCap.GetResourceObservable("ContentControlKeyCap"));

            var line = new StackPanel { Orientation = Orientation.Horizontal };
            line.Bind(StackPanel.SpacingProperty, line.GetResourceObservable("SpacingMD"));
            line.Children.Add(message);
            line.Children.Add(keyCap);
            line.Children.Add(undoWord);
            Content = line;
        }

        protected override Type StyleKeyOverride => typeof(PopupCard);

        // The key cap of the undo gesture. A test reads it.
        internal ContentControl KeyCap => keyCap;

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == TextProperty)
            {
                message.Text = Text;
            }
            else if (change.Property == UndoGestureProperty)
            {
                var hasUndo = !string.IsNullOrEmpty(UndoGesture);
                keyCap.Content = UndoGesture;
                keyCap.IsVisible = hasUndo;
                undoWord.IsVisible = hasUndo;
            }
        }
    }
}
