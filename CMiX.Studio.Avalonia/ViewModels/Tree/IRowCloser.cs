// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    internal interface IRowCloser
    {
        void CloseRowsInView(TreeNode? anchor, Func<IReadOnlyList<TreeNode>> close);
    }
}
