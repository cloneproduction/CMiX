// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    public readonly record struct DragLook(bool IsSource, bool HasTargetBelow, string? BlockReason)
    {
        public static readonly DragLook None = new(false, false, null);
    }
}
