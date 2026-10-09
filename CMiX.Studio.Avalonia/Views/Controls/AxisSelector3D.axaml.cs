// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace CMiX.Studio.Avalonia.Views.Controls
{
    public partial class AxisSelector3D : CaptionedUserControl
    {
        public AxisSelector3D()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<bool> XIsCheckedProperty =
            AvaloniaProperty.Register<AxisSelector3D, bool>(nameof(XIsChecked), true, defaultBindingMode: BindingMode.TwoWay);
        public bool XIsChecked
        {
            get => GetValue(XIsCheckedProperty);
            set => SetValue(XIsCheckedProperty, value);
        }

        public static readonly StyledProperty<bool> YIsCheckedProperty =
            AvaloniaProperty.Register<AxisSelector3D, bool>(nameof(YIsChecked), false, defaultBindingMode: BindingMode.TwoWay);
        public bool YIsChecked
        {
            get => GetValue(YIsCheckedProperty);
            set => SetValue(YIsCheckedProperty, value);
        }

        public static readonly StyledProperty<bool> ZIsCheckedProperty =
            AvaloniaProperty.Register<AxisSelector3D, bool>(nameof(ZIsChecked), false, defaultBindingMode: BindingMode.TwoWay);
        public bool ZIsChecked
        {
            get => GetValue(ZIsCheckedProperty);
            set => SetValue(ZIsCheckedProperty, value);
        }
    }
}
