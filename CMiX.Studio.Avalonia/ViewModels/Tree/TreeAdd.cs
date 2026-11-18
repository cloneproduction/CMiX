// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    // Enabled is false for an entry that the node cannot take: it has no action and the Add panel hides it.
    public sealed record TreeAdd(string Id, string Header, Action? Execute = null, TreeCategory? Category = null, bool Enabled = true);
}
