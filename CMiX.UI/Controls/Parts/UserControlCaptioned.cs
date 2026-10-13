// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace CMiX.UI.Controls.Parts
{
    // Every BaseControl exposes a Caption with the same three lines of boilerplate - this is
    // that declaration made once instead of copy-pasted into each one (and drifting, as the
    // BindingMode inconsistency across them showed before this existed). SliderCMiX is the one
    // exception: it has to inherit Avalonia's own Slider for its drag/track behavior, so it
    // can't also inherit this and keeps its own independent Caption property.
    public class UserControlCaptioned : UserControl
    {
        public static readonly StyledProperty<string> CaptionProperty =
            AvaloniaProperty.Register<UserControlCaptioned, string>(nameof(Caption), string.Empty, defaultBindingMode: BindingMode.TwoWay);

        public string Caption
        {
            get => GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }
    }
}
