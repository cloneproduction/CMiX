// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class Vector2 : UserControl
    {
        public Vector2()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<string> CaptionProperty =
            AvaloniaProperty.Register<Vector2, string>(nameof(Caption), string.Empty, defaultBindingMode: BindingMode.TwoWay);
        public string Caption
        {
            get => GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }
    }
}
