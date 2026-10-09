// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    // Generic "caption on the left, arbitrary content on the right" shell, replacing the same
    // two-column layout every BaseControl used to hand-roll individually.
    public class LabeledContent : ContentControl
    {
        public static readonly StyledProperty<string> CaptionProperty =
            AvaloniaProperty.Register<LabeledContent, string>(nameof(Caption), string.Empty);

        public string Caption
        {
            get => GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }
    }
}
