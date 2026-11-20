// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using CMiX.UI.Controls.Parts;
using CMiX.UI.Converters;

namespace CMiX.UI.Controls.String
{
    // Shows a key gesture and records the next key combination while it has the focus.
    public partial class TextBoxKeyGesture : UserControl, IValueText
    {
        public static readonly StyledProperty<KeyGesture?> GestureProperty =
            AvaloniaProperty.Register<TextBoxKeyGesture, KeyGesture?>(nameof(Gesture), defaultBindingMode: BindingMode.TwoWay);
        public KeyGesture? Gesture
        {
            get => GetValue(GestureProperty);
            set => SetValue(GestureProperty, value);
        }

        public static readonly StyledProperty<ICommand?> ResetCommandProperty =
            AvaloniaProperty.Register<TextBoxKeyGesture, ICommand?>(nameof(ResetCommand));
        public ICommand? ResetCommand
        {
            get => GetValue(ResetCommandProperty);
            set => SetValue(ResetCommandProperty, value);
        }

        // The text that shows while the box records.
        internal const string RecordingText = "Press keys";

        // The text that shows when there is no gesture.
        internal const string NoneText = "None";

        public TextBoxKeyGesture()
        {
            InitializeComponent();
            ValueMenu.Follow(this, ResetCommandProperty);
            UpdateText();
        }

        // The text that shows now. For the tests.
        internal string? ShownText => gestureText.Text;

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == GestureProperty)
                UpdateText();
        }

        protected override void OnGotFocus(GotFocusEventArgs e)
        {
            base.OnGotFocus(e);
            PseudoClasses.Set(":recording", true);
            UpdateText();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            PseudoClasses.Set(":recording", false);
            UpdateText();
        }

        // A left click gives the focus. A right click opens the menu.
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                return;

            Focus(NavigationMethod.Pointer);
            e.Handled = true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!IsFocused)
            {
                base.OnKeyDown(e);
                return;
            }

            e.Handled = true;

            if (IsModifierKey(e.Key))
                return;

            if (e.Key == Key.Escape)
            {
                LeaveFocus();
                return;
            }

            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                Gesture = null;
                return;
            }

            Gesture = new KeyGesture(e.Key, e.KeyModifiers);
            LeaveFocus();
        }

        private static bool IsModifierKey(Key key) => key is
            Key.LeftCtrl or Key.RightCtrl or
            Key.LeftShift or Key.RightShift or
            Key.LeftAlt or Key.RightAlt or
            Key.LWin or Key.RWin;

        private void LeaveFocus()
        {
            TopLevel.GetTopLevel(this)?.FocusManager?.ClearFocus();
        }

        private void UpdateText()
        {
            gestureText.Text = IsFocused ? RecordingText : Gesture is { } gesture ? KeyGestureTextConverter.TextOf(gesture) : NoneText;
        }

        bool IValueText.CanCopyValueText() => Gesture != null;

        string IValueText.CopyValueText() => Gesture is { } gesture ? KeyGestureTextConverter.TextOf(gesture) : string.Empty;

        bool IValueText.CanPasteValueText(string? text) => TryParse(text, out _);

        void IValueText.PasteValueText(string? text)
        {
            if (TryParse(text, out var gesture))
                Gesture = gesture;
        }

        private static bool TryParse(string? text, out KeyGesture? gesture)
        {
            gesture = null;
            if (string.IsNullOrWhiteSpace(text))
                return false;

            try
            {
                gesture = KeyGestureTextConverter.Parse(text.Trim());
                return gesture.Key != Key.None;
            }
            catch (Exception e) when (e is ArgumentException or FormatException)
            {
                return false;
            }
        }
    }
}
