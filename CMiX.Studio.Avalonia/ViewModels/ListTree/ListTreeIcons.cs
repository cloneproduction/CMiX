// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    public static class ListTreeIcons
    {
        public const string GroupKey = "Folder";

        public static string KeyOf(ListTreeRow row) => row.IsGroup ? GroupKey : row.Node?.IconKey ?? GroupKey;
    }
}
