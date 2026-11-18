// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    // How one row looks while a search runs (see TreeNode.ShowSearchLook). A row that does not match is dimmed.
    public sealed record SearchLook(bool IsMatch);
}
