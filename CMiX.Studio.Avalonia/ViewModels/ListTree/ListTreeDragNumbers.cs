// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    public sealed record ListTreeDragNumbers(double PixelsPerLevel, double HorizontalWeight, double Hysteresis, double Threshold)
    {
        public static ListTreeDragNumbers Default { get; } = new(24, 1.5, 6, 4);
    }
}
