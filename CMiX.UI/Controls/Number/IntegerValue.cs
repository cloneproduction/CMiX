// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;

namespace CMiX.UI.Controls.Number
{
    // A DragValue for whole numbers. It starts as a whole number editor with steps of 1.
    // It takes the same bindings as DragValue: Value and ResetCommand.
    public class IntegerValue : DragValue
    {
        static IntegerValue()
        {
            IsIntegerProperty.OverrideDefaultValue<IntegerValue>(true);
            SmallChangeProperty.OverrideDefaultValue<IntegerValue>(1.0);
            LargeChangeProperty.OverrideDefaultValue<IntegerValue>(1.0);
        }

        protected override Type StyleKeyOverride => typeof(DragValue);
    }
}
