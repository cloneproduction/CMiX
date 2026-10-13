// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;

namespace CMiX.UI.Behaviors
{
    // The distance rule of the behaviors that close a popup when the pointer goes away.
    internal static class PointerAway
    {
        public const double DefaultDistance = 64;

        // True when the pointer is not farther than the distance from the control.
        public static bool IsNear(Control? control, PointerEventArgs e, double distance)
        {
            if (control == null)
                return false;

            var size = control.Bounds.Size;
            var pos = e.GetPosition(control);

            return pos.X >= -distance && pos.Y >= -distance &&
                   pos.X <= size.Width + distance && pos.Y <= size.Height + distance;
        }

        // True when the pointer is farther than the distance from every control.
        public static bool IsFar(PointerEventArgs e, double distance, params Control?[] controls) =>
            !controls.Any(control => IsNear(control, e, distance));
    }
}
