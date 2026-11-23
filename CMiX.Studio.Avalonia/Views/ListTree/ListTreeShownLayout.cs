// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // What the rows of the new tree show: the arrangement of the live tree, or the look of a drag until the drag ends.
    internal sealed class ListTreeShownLayout
    {
        private readonly ListTreeRowControls rowControls;
        private readonly ListTreeSurfaces surfaces;

        private ListTreeArrangement arrangement = ListTreeArrangement.Empty;
        private ListTreeDragLook? dragLook;

        public ListTreeShownLayout(ListTreeRowControls rowControls, ListTreeSurfaces surfaces)
        {
            this.rowControls = rowControls;
            this.surfaces = surfaces;
        }

        public ListTreeLayout LiveLayout => arrangement.Layout;

        public ListTreeLayout Layout => dragLook?.Layout ?? arrangement.Layout;

        public bool IsDragShown => dragLook != null;

        public void Arrange(IEnumerable<TreeNode> roots, ListTreeSizes sizes)
        {
            arrangement = ListTreeArrangement.Of(roots, sizes);
            Show();
        }

        // The rows take the places of the candidate of a drag, until the drag ends.
        public void ShowDrag(ListTreeDragLook look)
        {
            dragLook = look;
            Show();
        }

        public void EndDrag()
        {
            dragLook = null;
            surfaces.EndDrag();
        }

        public void Show()
        {
            if (dragLook is { } look)
            {
                rowControls.ShowDrag(look);
                surfaces.ShowDrag(look);
            }
            else
            {
                rowControls.Show(arrangement.Rows, arrangement.Layout);
                surfaces.Show(arrangement.Rows, arrangement.Layout);
            }
        }
    }
}
