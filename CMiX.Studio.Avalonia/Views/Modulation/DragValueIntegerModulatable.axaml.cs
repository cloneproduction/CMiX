// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;

namespace CMiX.Studio.Avalonia.Views.Modulation
{
    public partial class DragValueIntegerModulatable : UserControlLimitedModulatable
    {
        static DragValueIntegerModulatable()
        {
            MinimumProperty.OverrideDefaultValue<DragValueIntegerModulatable>(1.0);
        }

        public DragValueIntegerModulatable()
        {
            InitializeComponent();
        }
    }
}
