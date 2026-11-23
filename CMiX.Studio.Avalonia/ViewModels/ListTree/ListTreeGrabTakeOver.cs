// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // After a key of a grab, a pointer move takes the mark only when it goes farther than the distance.
    public static class ListTreeGrabTakeOver
    {
        public static bool KeepsTheKeyMark(ListTreePoint keyMove, ListTreePoint pointer, double distance)
        {
            var dx = pointer.X - keyMove.X;
            var dy = pointer.Y - keyMove.Y;
            return Math.Sqrt(dx * dx + dy * dy) < distance;
        }
    }
}
