// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Threading;
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

        // The time of a row move. A test sets zero, so the rows are at their place at once.
        public static readonly StyledProperty<TimeSpan> MoveDurationProperty =
            AvaloniaProperty.Register<PanelListTreeRows, TimeSpan>(nameof(MoveDuration), Motion.Duration);

        // The top of a row. It moves with a transition, so a row slides to its new place.
        public static readonly AttachedProperty<double> TopProperty =
            AvaloniaProperty.RegisterAttached<PanelListTreeRows, Control, double>("Top");

        private readonly ListTreeNodeWatch watch = new();
        private readonly Dictionary<ListTreeRowKey, RowListTree> rowsByKey = new();

        private ListTreeLayout layout = ListTreeLayout.Of(new ListTreeSnapshot(Array.Empty<ListTreeRow>()), ListTreeSizes.Default);
        private bool isAttached;
        private bool isRebuildPosted;

        static PanelListTreeRows()
        {
            AffectsParentArrange<PanelListTreeRows>(TopProperty);
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

        public TimeSpan MoveDuration
        {
            get => GetValue(MoveDurationProperty);
            set => SetValue(MoveDurationProperty, value);
        }

        public static double GetTop(Control row) => row.GetValue(TopProperty);

        public static void SetTop(Control row, double top) => row.SetValue(TopProperty, top);

        public ListTreeLayout Layout => layout;

        public RowListTree? RowOf(ListTreeRowKey key) => rowsByKey.GetValueOrDefault(key);

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            isAttached = true;
            ListTreeToggleSizes.Of(this).Write(Resources);
            watch.Changed += OnTreeChanged;
            Rebuild();
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            isAttached = false;
            watch.Changed -= OnTreeChanged;
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
                ShowSelection();
        }

        // One rebuild for a burst of changes, such as a close of all cards.
        private void OnTreeChanged(object? sender, EventArgs e)
        {
            if (isRebuildPosted)
                return;

            isRebuildPosted = true;
            Dispatcher.UIThread.Post(() =>
            {
                isRebuildPosted = false;
                if (isAttached)
                    Rebuild();
            });
        }

        private void Rebuild()
        {
            var roots = Roots ?? Array.Empty<TreeNode>();
            watch.Watch(roots);
            var snapshot = ListTreeSnapshot.Of(roots);
            layout = ListTreeLayout.Of(snapshot, new ListTreeSizes(RowHeight, RowGap, Indent));
            ShowRows(snapshot.VisibleRows());
            InvalidateMeasure();
        }

        private void ShowRows(IReadOnlyList<ListTreeRow> rows)
        {
            foreach (var row in rows)
                ShowRow(row, layout.PlaceOf(row.Key)!);

            foreach (var key in rowsByKey.Keys.Where(key => layout.PlaceOf(key) == null).ToArray())
                RemoveRow(key);
        }

        private void ShowRow(ListTreeRow row, ListTreePlace place)
        {
            var control = rowsByKey.GetValueOrDefault(row.Key) ?? AddRow(row.Key, place);
            control.DataContext = row.Node;
            control.IconKey = ListTreeIcons.KeyOf(row);
            control.ItemCount = row.Node.Children.Count;
            control.IsSelected = IsSelected(row.Node);
            control.Place = place;
            SetTop(control, place.Y);
        }

        private void ShowSelection()
        {
            foreach (var control in rowsByKey.Values)
                control.IsSelected = control.DataContext is TreeNode node && IsSelected(node);
        }

        private bool IsSelected(TreeNode node) => ReferenceEquals(node, SelectedNode);

        // A new row shows at its place at once. Only later moves slide.
        private RowListTree AddRow(ListTreeRowKey key, ListTreePlace place)
        {
            var control = new RowListTree { ZIndex = ListTreeLayers.Row };
            SetTop(control, place.Y);
            if (MoveDuration > TimeSpan.Zero)
                control.Transitions = new Transitions
                {
                    new DoubleTransition { Property = TopProperty, Duration = MoveDuration, Easing = Motion.Easing }
                };

            rowsByKey[key] = control;
            Children.Add(control);
            return control;
        }

        private void RemoveRow(ListTreeRowKey key)
        {
            Children.Remove(rowsByKey[key]);
            rowsByKey.Remove(key);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var rowSize = new Size(availableSize.Width, RowHeight);
            foreach (var child in Children)
                child.Measure(rowSize);

            var width = double.IsInfinity(availableSize.Width) ? Children.Select(child => child.DesiredSize.Width).DefaultIfEmpty(0).Max() : availableSize.Width;
            return new Size(width, layout.TotalHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var child in Children)
                child.Arrange(new Rect(0, GetTop(child), finalSize.Width, RowHeight));

            return finalSize;
        }
    }
}
