// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

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
