// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class Integer2Value : UserControl
    {
        public Integer2Value()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<string> CaptionProperty =
            AvaloniaProperty.Register<Integer2Value, string>(nameof(Caption), string.Empty);
        public string Caption
        {
            get => GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }
    }
}
