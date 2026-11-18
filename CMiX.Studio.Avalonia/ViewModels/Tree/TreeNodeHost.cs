// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    internal interface TreeNodeHost
    {
        bool IsLoading { get; }

        void RefreshSearch();

        void RowExpandedChanged(TreeNode row);

        void ListChanged();

        void NodeRemoved(TreeNode row);

        void SelectFromModel(TreeNode row);
    }
}
