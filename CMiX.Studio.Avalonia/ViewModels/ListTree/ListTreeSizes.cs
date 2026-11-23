// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    public sealed record ListTreeSizes(double RowHeight, double Gap, double Indent)
    {
        public static ListTreeSizes Default { get; } = new(32, 3, 16);

        public double Pitch => RowHeight + Gap;
    }
}
