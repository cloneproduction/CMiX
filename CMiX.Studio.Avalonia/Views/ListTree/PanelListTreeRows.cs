// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.UI.Themes;

namespace CMiX.Studio.Avalonia.Views
{
    // The rows of the new tree, each one a direct child, placed by ListTreeLayout. Only the rows near the view of the scroller have a control.
    public sealed class PanelListTreeRows : Panel
    {
        public static readonly StyledProperty<IEnumerable<TreeNode>?> RootsProperty =
            AvaloniaProperty.Register<PanelListTreeRows, IEnumerable<TreeNode>?>(nameof(Roots));

        public static readonly StyledProperty<double> RowHeightProperty =
            AvaloniaProperty.Register<PanelListTreeRows, double>(nameof(RowHeight), ListTreeSizes.Default.RowHeight);

        public static readonly StyledProperty<double> RowGapProperty =
            AvaloniaProperty.Register<PanelListTreeRows, double>(nameof(RowGap), ListTreeSizes.Default.Gap);

        public static readonly StyledProperty<double> IndentProperty =
            AvaloniaProperty.Register<PanelListTreeRows, double>(nameof(Indent), ListTreeSizes.Default.Indent);

        public static readonly StyledProperty<TreeNode?> SelectedNodeProperty =
            AvaloniaProperty.Register<PanelListTreeRows, TreeNode?>(nameof(SelectedNode));

        public static readonly StyledProperty<IReadOnlyList<TreeNode>?> SelectedNodesProperty =
            AvaloniaProperty.Register<PanelListTreeRows, IReadOnlyList<TreeNode>?>(nameof(SelectedNodes));

        // The time of a row move. A test sets zero, so the rows are at their place at once.
        public static readonly StyledProperty<TimeSpan> MoveDurationProperty =
            AvaloniaProperty.Register<PanelListTreeRows, TimeSpan>(nameof(MoveDuration), Motion.Duration);

        // The top of a row in the layout. ListTreeRowSlide slides its render to it.
        public static readonly AttachedProperty<double> TopProperty =
            AvaloniaProperty.RegisterAttached<PanelListTreeRows, Control, double>("Top");

        private readonly ListTreeNodeWatch watch = new();
        private readonly ListTreeSurfaces surfaces;
        private readonly ListTreeRowControls rowControls;
        private readonly ListTreeShownLayout shown;
        private readonly ListTreeRebuildQueue rebuildQueue;

        private bool isAttached;
        private ScrollViewer? scroller;

        static PanelListTreeRows()
        {
            AffectsParentArrange<PanelListTreeRows>(TopProperty);
        }

        public PanelListTreeRows()
        {
            surfaces = new ListTreeSurfaces(this);
            rowControls = new ListTreeRowControls(this, surfaces);
            shown = new ListTreeShownLayout(rowControls, surfaces);
            rebuildQueue = new ListTreeRebuildQueue(RebuildWhenAttached);
        }

        public IEnumerable<TreeNode>? Roots
        {
            get => GetValue(RootsProperty);
            set => SetValue(RootsProperty, value);
        }

        public double RowHeight
        {
            get => GetValue(RowHeightProperty);
            set => SetValue(RowHeightProperty, value);
        }

        public double RowGap
        {
            get => GetValue(RowGapProperty);
            set => SetValue(RowGapProperty, value);
        }

        public double Indent
        {
            get => GetValue(IndentProperty);
            set => SetValue(IndentProperty, value);
        }

        public TreeNode? SelectedNode
        {
            get => GetValue(SelectedNodeProperty);
            set => SetValue(SelectedNodeProperty, value);
        }

        public IReadOnlyList<TreeNode>? SelectedNodes
        {
            get => GetValue(SelectedNodesProperty);
            set => SetValue(SelectedNodesProperty, value);
        }

        public TimeSpan MoveDuration
        {
            get => GetValue(MoveDurationProperty);
            set => SetValue(MoveDurationProperty, value);
        }

        public static double GetTop(Control row) => row.GetValue(TopProperty);

        public static void SetTop(Control row, double top) => row.SetValue(TopProperty, top);

        public ListTreeLayout Layout => shown.LiveLayout;

        public ListTreeSizes Sizes => new(RowHeight, RowGap, Indent);

        public RowListTree? RowOf(ListTreeRowKey key) => rowControls.RowOf(key);

        public RowListTree? RowOf(TreeNode node) => rowControls.RowOf(node);

        // Null for a row outside the row window: no control is made.
        public RowListTree? RealizedRowOf(ListTreeRowKey key) => rowControls.RealizedRowOf(key);

        public IEnumerable<RowListTree> RealizedRows => rowControls.RealizedRows;

        public ListTreePlace? ShownPlaceOf(TreeNode node) => rowControls.PlaceOf(node);

        // With no scroller, every row has a control.
        public ListTreeRowWindow RowWindow => scroller == null ? ListTreeRowWindow.All : ListTreeRowWindow.Around(ViewTop, ViewHeight);

        private double ViewTop => (scroller?.Offset.Y ?? 0) - Bounds.Y;

        // Before the first layout the scroller has no viewport, so the window height stands for it.
        private double ViewHeight => scroller is { Viewport.Height: > 0 } shown ? shown.Viewport.Height : TopLevel.GetTopLevel(this)?.ClientSize.Height ?? 0;

        internal ListTreeSurfaces Surfaces => surfaces;

        public void ShowDrag(ListTreeDragLook look)
        {
            shown.ShowDrag(look);
            InvalidateMeasure();
        }

        public void ShowLiveTree()
        {
            if (!shown.IsDragShown)
                return;

            shown.EndDrag();
            if (rebuildQueue.IsPosted)
                ApplyTreeChanges();
            else
                ShowArrangement();
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            isAttached = true;
            scroller = this.FindAncestorOfType<ScrollViewer>();
            if (scroller != null)
                scroller.ScrollChanged += OnScrollChanged;
            ListTreeToggleSizes.Of(this).Write(Resources);
            watch.Changed += OnTreeChanged;
            watch.HiddenChanged += OnHiddenChanged;
            Rebuild();
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            isAttached = false;
            if (scroller != null)
                scroller.ScrollChanged -= OnScrollChanged;
            scroller = null;
            watch.Changed -= OnTreeChanged;
            watch.HiddenChanged -= OnHiddenChanged;
            watch.Unwatch();
            base.OnDetachedFromVisualTree(e);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (isAttached && (change.Property == RootsProperty || change.Property == RowHeightProperty
                || change.Property == RowGapProperty || change.Property == IndentProperty))
                Rebuild();
            else if (change.Property == SelectedNodeProperty)
                surfaces.Select(SelectedNode);
            else if (change.Property == SelectedNodesProperty)
                surfaces.SelectAlso(SelectedNodes ?? Array.Empty<TreeNode>());
        }

        private void OnTreeChanged(object? sender, EventArgs e) => rebuildQueue.Post();

        private void OnHiddenChanged(object? sender, EventArgs e) => surfaces.ShowMarks();

        private void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            if (!rowControls.Window.Holds(ViewTop, ViewHeight))
                ShowArrangement();
        }

        public void ApplyTreeChanges() => rebuildQueue.Run();

        private void RebuildWhenAttached()
        {
            if (isAttached)
                Rebuild();
        }

        private void Rebuild()
        {
            var roots = Roots ?? Array.Empty<TreeNode>();
            watch.Watch(roots);
            shown.Arrange(roots, Sizes);
            InvalidateMeasure();
        }

        private void ShowArrangement()
        {
            shown.Show();
            InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var child in Children)
                child.Measure(new Size(availableSize.Width, HeightOf(child)));

            var width = double.IsInfinity(availableSize.Width) ? Children.Select(child => child.DesiredSize.Width).DefaultIfEmpty(0).Max() : availableSize.Width;
            return new Size(width, shown.Layout.TotalHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var child in Children)
                child.Arrange(child is FrameListTreeOutline outline ? outline.RectIn(finalSize.Width) : new Rect(0, TopOf(child), finalSize.Width, HeightOf(child)));

            return finalSize;
        }

        private static double TopOf(Control child) => child is FrameListTreeCard { Place: { } place } ? place.Y : GetTop(child);

        // A row made outside a layout pass gets its bounds at once.
        internal void PlaceRowNow(RowListTree row)
        {
            row.Measure(new Size(Bounds.Width, RowHeight));
            row.Arrange(new Rect(0, TopOf(row), Bounds.Width, RowHeight));
        }

        private double HeightOf(Control child) => child is FrameListTreeCard { Place: { } place } ? place.AreaHeight : RowHeight;
    }
}
