// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // The row controls of the new tree by the key of their row. Only the rows in the row window of the panel have a control.
    internal sealed class ListTreeRowControls
    {
        private readonly PanelListTreeRows panel;
        private readonly ListTreeSurfaces surfaces;
        private readonly Dictionary<ListTreeRowKey, RowListTree> rowsByKey = new();
        private readonly Dictionary<ListTreeRowKey, ListTreeRow> shownRows = new();
        private readonly Dictionary<TreeNode, ListTreeRowKey> keyByNode = new(ReferenceEqualityComparer.Instance);

        private ListTreeLayout layout = ListTreeArrangement.Empty.Layout;
        private ListTreeDragLook? dragLook;

        public ListTreeRowControls(PanelListTreeRows panel, ListTreeSurfaces surfaces)
        {
            this.panel = panel;
            this.surfaces = surfaces;
        }

        public ListTreeRowWindow Window { get; private set; } = ListTreeRowWindow.All;

        public IEnumerable<RowListTree> RealizedRows => rowsByKey.Values;

        public RowListTree? RealizedRowOf(ListTreeRowKey key) => rowsByKey.GetValueOrDefault(key);

        // A shown row outside the window gets its control now, so a reveal, a focus or a rename finds it.
        public RowListTree? RowOf(ListTreeRowKey key) => RealizedRowOf(key) ?? RealizeOutsideWindow(key);

        public RowListTree? RowOf(TreeNode node) => keyByNode.TryGetValue(node, out var key) ? RowOf(key) : null;

        public ListTreePlace? PlaceOf(TreeNode node) => keyByNode.TryGetValue(node, out var key) ? layout.PlaceOf(key) : null;

        public void Show(IReadOnlyList<ListTreeRow> rows, ListTreeLayout shown)
        {
            dragLook = null;
            ShowRows(rows, shown);
        }

        public void ShowDrag(ListTreeDragLook look)
        {
            dragLook = look;
            ShowRows(look.Rows, look.Layout);
        }

        // A row that leaves the window slides out before its control goes. A row that comes in slides from its last place.
        private void ShowRows(IReadOnlyList<ListTreeRow> rows, ListTreeLayout shown)
        {
            var before = layout;
            layout = shown;
            Window = panel.RowWindow;
            Remember(rows);
            var kept = new HashSet<ListTreeRowKey>();
            foreach (var row in rows)
            {
                var place = shown.PlaceOf(row.Key)!;
                var control = rowsByKey.GetValueOrDefault(row.Key);
                if (control != null ? !IsKept(control, place) : !Window.Overlaps(place.Y, place.RowHeight))
                    continue;

                ShowRow(control ?? AddRow(row.Key, before.PlaceOf(row.Key) ?? place), row, place);
                kept.Add(row.Key);
            }

            foreach (var key in rowsByKey.Keys.Where(key => !kept.Contains(key)).ToArray())
                RemoveRow(key);
        }

        private bool IsKept(RowListTree control, ListTreePlace place) =>
            Window.Overlaps(place.Y, place.RowHeight) || Window.Overlaps(ListTreeRowSlide.ShownTopOf(control), place.RowHeight) || control.IsKeyboardFocusWithin;

        private void Remember(IReadOnlyList<ListTreeRow> rows)
        {
            shownRows.Clear();
            keyByNode.Clear();
            foreach (var row in rows)
            {
                shownRows[row.Key] = row;
                if (row.Node != null)
                    keyByNode.TryAdd(row.Node, row.Key);
            }
        }

        private RowListTree? RealizeOutsideWindow(ListTreeRowKey key)
        {
            if (!shownRows.TryGetValue(key, out var row) || layout.PlaceOf(key) is not { } place)
                return null;

            var control = AddRow(key, place);
            ShowRow(control, row, place);
            panel.PlaceRowNow(control);
            surfaces.ShowMarks();
            return control;
        }

        // The dragged row snaps into its landing place. Another row slides from where it shows, unless its top stays.
        private void ShowRow(RowListTree control, ListTreeRow row, ListTreePlace place)
        {
            var light = dragLook?.LightOf(row.Key) ?? ListTreeRowLight.Rest;
            var shownTop = ListTreeRowSlide.ShownTopOf(control);
            var moves = PanelListTreeRows.GetTop(control) != place.Y;

            control.DataContext = row.Node;
            control.IconKey = ListTreeIcons.KeyOf(row);
            control.ItemCount = row.Node?.Children.Count ?? 0;
            control.Place = place;
            control.ZIndex = light == ListTreeRowLight.Dragged ? ListTreeLayers.DraggedRow : ListTreeLayers.Row;
            control.ShowLight(light);
            PanelListTreeRows.SetTop(control, place.Y);
            if (light == ListTreeRowLight.Dragged)
                ListTreeRowSlide.Snap(control);
            else if (moves)
                ListTreeRowSlide.From(control, shownTop, panel.MoveDuration);
        }

        // A new row starts at the place it had in the last layout, or at its place when it is new in the tree.
        private RowListTree AddRow(ListTreeRowKey key, ListTreePlace start)
        {
            var control = new RowListTree { ZIndex = ListTreeLayers.Row };
            PanelListTreeRows.SetTop(control, start.Y);
            surfaces.Watch(control);
            rowsByKey[key] = control;
            panel.Children.Add(control);
            return control;
        }

        private void RemoveRow(ListTreeRowKey key)
        {
            surfaces.Unwatch(rowsByKey[key]);
            panel.Children.Remove(rowsByKey[key]);
            rowsByKey.Remove(key);
        }
    }
}
