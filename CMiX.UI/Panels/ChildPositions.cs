// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using Avalonia.Controls;
using CMiX.UI.AttachedProperties;

namespace CMiX.UI.Panels
{
    // Gives a row of controls their positions: the first and the last take the outer corners.
    internal static class ChildPositions
    {
        public static void Assign(IReadOnlyList<Control> row, ControlPosition first, ControlPosition last)
        {
            for (int i = 0; i < row.Count; i++)
                PositionedControl.SetPosition(row[i], PositionAt(i, row.Count, first, last));
        }

        private static ControlPosition PositionAt(int index, int count, ControlPosition first, ControlPosition last)
        {
            if (count == 1)
                return ControlPosition.All;
            if (index == 0)
                return first;
            if (index == count - 1)
                return last;
            return ControlPosition.None;
        }
    }
}
