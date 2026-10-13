// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using CMiX.UI.Controls.Parts;

namespace CMiX.UI.Controls.Vector
{
    public partial class Vector3 : CaptionedUserControl
    {
        public Vector3()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<float> XProperty =
            AvaloniaProperty.Register<Vector3, float>(nameof(X), 0.0f, defaultBindingMode: BindingMode.TwoWay);
        public float X
        {
            get => GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        public static readonly StyledProperty<float> YProperty =
            AvaloniaProperty.Register<Vector3, float>(nameof(Y), 0.0f, defaultBindingMode: BindingMode.TwoWay);
        public float Y
        {
            get => GetValue(YProperty);
            set => SetValue(YProperty, value);
        }

        public static readonly StyledProperty<float> ZProperty =
            AvaloniaProperty.Register<Vector3, float>(nameof(Z), 0.0f, defaultBindingMode: BindingMode.TwoWay);
        public float Z
        {
            get => GetValue(ZProperty);
            set => SetValue(ZProperty, value);
        }
    }
}
