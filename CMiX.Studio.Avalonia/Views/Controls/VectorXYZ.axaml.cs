// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class VectorXYZ : UserControl
    {
        public VectorXYZ()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<string> CaptionProperty =
            AvaloniaProperty.Register<VectorXYZ, string>(nameof(Caption), string.Empty, defaultBindingMode: BindingMode.TwoWay);
        public string Caption
        {
            get => GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        public static readonly StyledProperty<float> XProperty =
            AvaloniaProperty.Register<VectorXYZ, float>(nameof(X), 0.0f, defaultBindingMode: BindingMode.TwoWay);
        public float X
        {
            get => GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        public static readonly StyledProperty<float> YProperty =
            AvaloniaProperty.Register<VectorXYZ, float>(nameof(Y), 0.0f, defaultBindingMode: BindingMode.TwoWay);
        public float Y
        {
            get => GetValue(YProperty);
            set => SetValue(YProperty, value);
        }

        public static readonly StyledProperty<float> ZProperty =
            AvaloniaProperty.Register<VectorXYZ, float>(nameof(Z), 0.0f, defaultBindingMode: BindingMode.TwoWay);
        public float Z
        {
            get => GetValue(ZProperty);
            set => SetValue(ZProperty, value);
        }
    }
}
