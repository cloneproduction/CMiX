// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
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
        private TreeNode? dropTarget;

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

        public void Select(TreeNode? node)
        {
            selected = node;
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
            var marks = new ListTreeMarks(selected, HoveredCard(rows), dropTarget);
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
