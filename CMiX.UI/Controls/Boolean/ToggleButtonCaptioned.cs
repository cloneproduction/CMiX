// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia;
using Avalonia.Data;

namespace CMiX.UI.Controls.Boolean
{
    // A ToggleButtonCMiX with a Caption. The box sits in the value column and the Caption is
    // beside it. A click on the Caption toggles it, like a click on the box.
    public class ToggleButtonCaptioned : ToggleButtonCMiX
    {
        public static readonly StyledProperty<string> CaptionProperty =
            AvaloniaProperty.Register<ToggleButtonCaptioned, string>(nameof(Caption), string.Empty, defaultBindingMode: BindingMode.TwoWay);

        public string Caption
        {
            get => GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        protected override Type StyleKeyOverride => typeof(ToggleButtonCaptioned);
    }
}
