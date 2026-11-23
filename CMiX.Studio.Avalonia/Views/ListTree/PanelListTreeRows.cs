// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.UI.Themes;

namespace CMiX.Studio.Avalonia.Views
{
    // The rows of the new tree, each one a direct child, placed by ListTreeLayout. A row control lives as long as the key of its row.
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

        // The top of a row. It moves with a transition, so a row slides to its new place.
        public static readonly AttachedProperty<double> TopProperty =
            AvaloniaProperty.RegisterAttached<PanelListTreeRows, Control, double>("Top");

        private readonly ListTreeNodeWatch watch = new();
        private readonly ListTreeSurfaces surfaces;
        private readonly ListTreeRowControls rowControls;
        private readonly ListTreeShownLayout shown;
        private readonly ListTreeRebuildQueue rebuildQueue;

        private bool isAttached;

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
            ListTreeToggleSizes.Of(this).Write(Resources);
            watch.Changed += OnTreeChanged;
            watch.HiddenChanged += OnHiddenChanged;
            Rebuild();
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            isAttached = false;
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

        private double HeightOf(Control child) => child is FrameListTreeCard { Place: { } place } ? place.AreaHeight : RowHeight;
    }
}
