// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // The parts of one view of the project tree that its helpers read. Each getter reads the view at the moment of the call:
    // the scroll viewer exists only after the template applies, and the tree follows the data context.
    public sealed class TreeViewContext
    {
        private readonly Control owner;
        private DragAutoScroller? autoScroller;

        internal TreeViewContext(Control owner, TreeView treeView, TreeRowIndex rowIndex)
        {
            this.owner = owner;
            TreeView = treeView;
            RowIndex = rowIndex;
        }

        public TreeView TreeView { get; }

        internal TreeRowIndex RowIndex { get; }

        public ProjectTree? Tree => owner.DataContext as ProjectTree;

        public ScrollViewer? Scroller => TreeView.GetVisualDescendants().OfType<ScrollViewer>().FirstOrDefault();

        public double RowPitch => TreeRowMetrics.ReadRowPitch(owner.Resources);

        // The scroller is made at the first drag and goes away when the view leaves the window (see DisposeAutoScroller).
        public DragAutoScroller AutoScroller => autoScroller ??= new DragAutoScroller(this);

        public DragAutoScroller? ExistingAutoScroller => autoScroller;

        public void DisposeAutoScroller()
        {
            autoScroller?.Dispose();
            autoScroller = null;
        }
    }
}
