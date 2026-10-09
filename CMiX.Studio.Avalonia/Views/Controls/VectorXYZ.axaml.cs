// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class VectorXYZ : CaptionedUserControl
    {
        public VectorXYZ()
        {
            InitializeComponent();
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
