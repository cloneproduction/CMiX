// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Data;
using CMiX.UI.Controls.Parts;

namespace CMiX.UI.Controls.Vector
{
    public partial class AxisSelector3 : UserControlCaptioned
    {
        public AxisSelector3()
        {
            InitializeComponent();
        }

        public static readonly StyledProperty<bool> XIsCheckedProperty =
            AvaloniaProperty.Register<AxisSelector3, bool>(nameof(XIsChecked), true, defaultBindingMode: BindingMode.TwoWay);
        public bool XIsChecked
        {
            get => GetValue(XIsCheckedProperty);
            set => SetValue(XIsCheckedProperty, value);
        }

        public static readonly StyledProperty<bool> YIsCheckedProperty =
            AvaloniaProperty.Register<AxisSelector3, bool>(nameof(YIsChecked), false, defaultBindingMode: BindingMode.TwoWay);
        public bool YIsChecked
        {
            get => GetValue(YIsCheckedProperty);
            set => SetValue(YIsCheckedProperty, value);
        }

        public static readonly StyledProperty<bool> ZIsCheckedProperty =
            AvaloniaProperty.Register<AxisSelector3, bool>(nameof(ZIsChecked), false, defaultBindingMode: BindingMode.TwoWay);
        public bool ZIsChecked
        {
            get => GetValue(ZIsCheckedProperty);
            set => SetValue(ZIsCheckedProperty, value);
        }
    }
}
