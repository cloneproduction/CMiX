// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace CMiX.UI.Controls.Boolean
{
    // A Button whose Caption is its label. A Content that is set on its own replaces the Caption.
    public class ButtonCaptioned : Button
    {
        static ButtonCaptioned()
        {
            CaptionProperty.Changed.AddClassHandler<ButtonCaptioned>((button, e) => button.OnCaptionChanged((string?)e.OldValue, (string?)e.NewValue));
        }

        public static readonly StyledProperty<string> CaptionProperty =
            AvaloniaProperty.Register<ButtonCaptioned, string>(nameof(Caption), string.Empty, defaultBindingMode: BindingMode.TwoWay);

        public string Caption
        {
            get => GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        private void OnCaptionChanged(string? oldCaption, string? newCaption)
        {
            if (Content == null || Equals(Content, oldCaption))
                Content = newCaption;
        }
    }
}
