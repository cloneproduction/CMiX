// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    // Every BaseControl exposes a Caption with the same three lines of boilerplate - this is
    // that declaration made once instead of copy-pasted into each one (and drifting, as the
    // BindingMode inconsistency across them showed before this existed). CMiXSlider is the one
    // exception: it has to inherit Avalonia's own Slider for its drag/track behavior, so it
    // can't also inherit this and keeps its own independent Caption property.
    public class CaptionedUserControl : UserControl
    {
        public static readonly StyledProperty<string> CaptionProperty =
            AvaloniaProperty.Register<CaptionedUserControl, string>(nameof(Caption), string.Empty, defaultBindingMode: BindingMode.TwoWay);

        public string Caption
        {
            get => GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }
    }
}
