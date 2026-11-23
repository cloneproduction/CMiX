// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // The card areas of the new tree and the marks on its rows and areas: the selection, the hover and the drop target.
    // Each rebuild makes the areas again in tree order, so an inner area draws over its outer area.
    internal sealed class ListTreeSurfaces
    {
        private readonly Panel panel;
        private readonly List<FrameListTreeCard> areas = new();
        private readonly FrameListTreeOutline outline = new() { IsHitTestVisible = false, IsVisible = false, ZIndex = ListTreeLayers.DropOutline };

        private TreeNode? selected;
        private IReadOnlySet<TreeNode> alsoSelected = ImmutableHashSet<TreeNode>.Empty;
        private TreeNode? dropTarget;
        private bool isDragging;

        public ListTreeSurfaces(Panel panel)
        {
            this.panel = panel;
        }

        public IReadOnlyList<FrameListTreeCard> Areas => areas;

        public FrameListTreeOutline Outline => outline;

        public TreeNode? DropTarget => dropTarget;

        public void Show(IReadOnlyList<ListTreeRow> rows, ListTreeLayout shown)
        {
            panel.Children.RemoveAll(areas);
            areas.Clear();
            foreach (var row in rows)
                if (shown.PlaceOf(row.Key) is { CanToggle: true } place)
                    Add(row, place);

            if (!panel.Children.Contains(outline))
                panel.Children.Add(outline);
            ShowMarks();
        }

        // During a drag, the hover and the selection marks hide and the holder of the candidate is the drop target.
        public void ShowDrag(ListTreeDragLook look)
        {
            isDragging = true;
            dropTarget = look.DropTarget;
            Show(look.Rows, look.Layout);
        }

        public void EndDrag()
        {
            isDragging = false;
            dropTarget = null;
        }

        public void Select(TreeNode? node)
        {
            selected = node;
            ShowMarks();
        }

        public void SelectAlso(IEnumerable<TreeNode> nodes)
        {
            alsoSelected = new HashSet<TreeNode>(nodes, ReferenceEqualityComparer.Instance);
            ShowMarks();
        }

        public void MarkDropTarget(TreeNode? node)
        {
            if (ReferenceEquals(node, dropTarget))
                return;

            dropTarget = node;
            ShowMarks();
        }

        public void Watch(RowListTree row) => row.PropertyChanged += OnRowChanged;

        public void Unwatch(RowListTree row) => row.PropertyChanged -= OnRowChanged;

        private void OnRowChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == InputElement.IsPointerOverProperty)
                ShowMarks();
        }

        public void ShowMarks()
        {
            var rows = panel.Children.OfType<RowListTree>().ToList();
            var marks = isDragging ? new ListTreeMarks(null, null, dropTarget) : new ListTreeMarks(selected, HoveredCard(rows), dropTarget) { AlsoSelected = alsoSelected };
            foreach (var row in rows)
                row.ShowMarks(marks);
            foreach (var area in areas)
                area.ShowMarks(marks);
            outline.Show(dropTarget, PlaceOf(rows, dropTarget));
        }

        private static TreeNode? HoveredCard(IEnumerable<RowListTree> rows) =>
            rows.FirstOrDefault(row => row.IsPointerOver && row.Place is { CanToggle: true })?.DataContext as TreeNode;

        private static ListTreePlace? PlaceOf(IEnumerable<RowListTree> rows, TreeNode? node) =>
            node == null ? null : rows.FirstOrDefault(row => ReferenceEquals(row.DataContext, node))?.Place;

        private void Add(ListTreeRow row, ListTreePlace place)
        {
            var area = new FrameListTreeCard { Place = place, DataContext = row.Node, ZIndex = ListTreeLayers.CardArea };
            areas.Add(area);
            panel.Children.Add(area);
        }
    }
}
