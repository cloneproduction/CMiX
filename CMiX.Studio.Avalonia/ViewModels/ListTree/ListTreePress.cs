// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The grab offset is the pointer y minus the top of the pressed row. The depth is the depth of the pressed row.
    public sealed record ListTreePress(ListTreePoint Pointer, double GrabOffsetY, int Depth);
}
