// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    // How many items of one kind a row holds. A collapsed row shows one of these for each kind it holds.
    public sealed record KindCount(TreeCategory Category, int Count)
    {
        // The text of the tooltip of the count, such as "2 Entities" or "1 Filter".
        public string Description => Count + " " + TreeCategories.Label(Category, Count);

        // The icon of the kind, shown in the square of a collapsed row (the count is in the tooltip). The keys are those of StudioIcons.
        public string IconKey => Category switch
        {
            TreeCategory.Composable => "Composition",
            TreeCategory.Light => "MeshLight",
            _ => Category.ToString()
        };
    }
}
