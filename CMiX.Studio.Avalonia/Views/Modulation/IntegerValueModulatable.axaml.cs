// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;

namespace CMiX.Studio.Avalonia.Views.Modulation
{
    public partial class IntegerValueModulatable : UserControlLimitedModulatable
    {
        static IntegerValueModulatable()
        {
            MinimumProperty.OverrideDefaultValue<IntegerValueModulatable>(1.0);
        }

        public IntegerValueModulatable()
        {
            InitializeComponent();
        }
    }
}
