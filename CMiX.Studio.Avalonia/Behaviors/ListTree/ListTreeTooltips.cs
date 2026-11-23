// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Studio.Avalonia.Views;

namespace CMiX.Studio.Avalonia.Behaviors.ListTree
{
    // The tooltips of the rows of the new tree. It owns one TreeTooltips, made when the view first goes into the visual tree.
    internal sealed class ListTreeTooltips : ListTreeBehavior
    {
        private TreeTooltips? tooltips;

        public TreeTooltips Tooltips => Owned();

        protected override void OnAttachedToVisualTree()
        {
            Owned();
            base.OnAttachedToVisualTree();
        }

        private TreeTooltips Owned() => tooltips ??= new TreeTooltips(Context.Rows);
    }
}
