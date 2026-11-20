// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using Avalonia;
using Avalonia.Controls;

namespace CMiX.UI.AttachedProperties
{
    // Marks the cursor row of a list or a tree: the row that the arrow keys move to.
    // IsCursor sets the pseudo class :cursor on the row. The style of the row draws the cursor frame for this pseudo class.
    // A style setter can not set a pseudo class, so the theme of the row binds IsCursor to a value of the model.
    // The fill of a row shows the selection. The frame shows the cursor.
    public static class CursorRow
    {
        public const string CursorPseudoClass = ":cursor";

        public static readonly AttachedProperty<bool> IsCursorProperty =
            AvaloniaProperty.RegisterAttached<Control, bool>(
                "IsCursor",
                typeof(CursorRow));

        public static bool GetIsCursor(Control obj) => obj.GetValue(IsCursorProperty);
        public static void SetIsCursor(Control obj, bool value) => obj.SetValue(IsCursorProperty, value);

        static CursorRow()
        {
            IsCursorProperty.Changed.AddClassHandler<Control>((row, e) => ((IPseudoClasses)row.Classes).Set(CursorPseudoClass, e.GetNewValue<bool>()));
        }
    }
}
