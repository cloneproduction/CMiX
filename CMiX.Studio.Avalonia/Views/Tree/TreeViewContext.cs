// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // The scroll viewer exists only after the template applies, so the first call after that finds it.
    public sealed class TreeViewContext
    {
        private readonly Control owner;
        private DragAutoScroller? autoScroller;
        private ScrollViewer? scroller;
        private double? rowPitch;

        internal TreeViewContext(Control owner, TreeView treeView, TreeRowIndex rowIndex)
        {
            this.owner = owner;
            TreeView = treeView;
            RowIndex = rowIndex;
            treeView.TemplateApplied += (_, _) => scroller = null;
        }

        public TreeView TreeView { get; }

        internal TreeRowIndex RowIndex { get; }

        public ProjectTree? Tree => owner.DataContext as ProjectTree;

        internal bool IsTreeFocused => owner.IsKeyboardFocusWithin;

        public ScrollViewer? Scroller => scroller ??= TreeView.GetVisualDescendants().OfType<ScrollViewer>().FirstOrDefault();

        public double RowPitch
        {
            get => rowPitch ?? TreeRowMetrics.ReadRowPitch(owner);
            internal set => rowPitch = value;
        }

        public DragAutoScroller AutoScroller => autoScroller ??= new DragAutoScroller(this);

        public DragAutoScroller? ExistingAutoScroller => autoScroller;

        public void DisposeAutoScroller()
        {
            autoScroller?.Dispose();
            autoScroller = null;
        }
    }
}
