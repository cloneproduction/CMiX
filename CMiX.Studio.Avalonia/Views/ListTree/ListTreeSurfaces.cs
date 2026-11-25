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
    // The card areas and the marks on rows and areas (selection, hover, drop target), in tree order.
    internal sealed class ListTreeSurfaces
    {
        private readonly PanelListTreeRows panel;
        private readonly List<FrameListTreeCard> areas = new();
        private readonly ListTreeSpareControls<FrameListTreeCard> spareAreas = new();
        private readonly FrameListTreeOutline outline = new() { IsHitTestVisible = false, IsVisible = false, ZIndex = ListTreeLayers.DropOutline };

        private TreeNode? selected;
        private IReadOnlySet<TreeNode> alsoSelected = ImmutableHashSet<TreeNode>.Empty;
        private TreeNode? dropTarget;
        private bool isDragging;

        public ListTreeSurfaces(PanelListTreeRows panel)
        {
            this.panel = panel;
        }

        public IReadOnlyList<FrameListTreeCard> Areas => areas;

        public FrameListTreeOutline Outline => outline;

        public TreeNode? DropTarget => dropTarget;

        public FrameListTreeCard? CardAreaOf(ListTreeRowKey key) => areas.FirstOrDefault(area => area.Place?.Key == key);

        public void Show(IReadOnlyList<ListTreeRow> rows, ListTreeLayout shown)
        {
            var window = panel.RowWindow;
            var cards = new List<(ListTreeRow Row, ListTreePlace Place)>();
            var cardKeys = new HashSet<ListTreeRowKey>();
            foreach (var row in rows)
            {
                if (shown.PlaceOf(row.Key) is { CanToggle: true } place && window.Overlaps(place.Y, place.AreaHeight))
                {
                    cards.Add((row, place));
                    cardKeys.Add(row.Key);
                }
            }

            var kept = new Dictionary<ListTreeRowKey, FrameListTreeCard>();
            foreach (var area in areas)
            {
                if (cardKeys.Contains(area.Place!.Key))
                    kept.Add(area.Place.Key, area);
                else
                    spareAreas.Keep(area);
            }

            areas.Clear();
            foreach (var card in cards)
                areas.Add(AreaOf(kept, card.Row, card.Place));

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
            var rows = panel.RealizedRows.ToList();
            var marks = isDragging ? new ListTreeMarks(null, null, dropTarget) : new ListTreeMarks(selected, HoveredCard(rows), dropTarget) { AlsoSelected = alsoSelected };
            foreach (var row in rows)
                row.ShowMarks(marks);
            foreach (var area in areas)
                area.ShowMarks(marks);
            outline.Show(dropTarget, dropTarget == null ? null : panel.ShownPlaceOf(dropTarget));
        }

        private static TreeNode? HoveredCard(IEnumerable<RowListTree> rows) =>
            rows.FirstOrDefault(row => row.IsPointerOver && row.Place is { CanToggle: true })?.DataContext as TreeNode;

        // An area that stays in the window keeps its control. A card that comes into the window takes a spare control first.
        private FrameListTreeCard AreaOf(Dictionary<ListTreeRowKey, FrameListTreeCard> kept, ListTreeRow row, ListTreePlace place)
        {
            if (!kept.Remove(place.Key, out var area))
                area = spareAreas.Take() ?? AddArea();

            area.ZIndex = ListTreeLayers.CardArea + place.Depth;
            area.Place = place;
            area.DataContext = row.Node;
            return area;
        }

        private FrameListTreeCard AddArea()
        {
            var area = new FrameListTreeCard();
            panel.Children.Add(area);
            return area;
        }
    }
}
