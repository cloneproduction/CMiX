// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // The row controls of the tree by the key of their row. Only the rows in the row window of the panel have a control.
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
            var free = new Stack<RowListTree>(TakeLeavingRows(shown));
            foreach (var row in rows)
            {
                var place = shown.PlaceOf(row.Key)!;
                var control = rowsByKey.GetValueOrDefault(row.Key);
                if (control == null && !Window.Overlaps(place.Y, place.RowHeight))
                    continue;

                var start = before.PlaceOf(row.Key) ?? place;
                ShowRow(control ?? ReuseRow(free, row.Key, start) ?? AddRow(row.Key, start), row, place);
            }

            foreach (var unused in free)
                RemoveRow(unused);
        }

        private bool IsKept(RowListTree control, ListTreePlace place) =>
            Window.Overlaps(place.Y, place.RowHeight) || Window.Overlaps(ListTreeRowSlide.ShownTopOf(control), place.RowHeight) || control.IsKeyboardFocusWithin;

        // A control whose row is gone or leaves the window loses its key and can take a row that comes into the window.
        private List<RowListTree> TakeLeavingRows(ListTreeLayout shown)
        {
            var leaving = rowsByKey.Where(entry => !shownRows.ContainsKey(entry.Key) || !IsKept(entry.Value, shown.PlaceOf(entry.Key)!)).ToList();
            foreach (var entry in leaving)
                rowsByKey.Remove(entry.Key);
            return leaving.Select(entry => entry.Value).ToList();
        }

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

        // A reused row starts like a new row: at its start top, with no slide and no flash.
        private RowListTree? ReuseRow(Stack<RowListTree> free, ListTreeRowKey key, ListTreePlace start)
        {
            if (!free.TryPop(out var control))
                return null;

            ListTreeRowSlide.Snap(control);
            control.StopFlash();
            PanelListTreeRows.SetTop(control, start.Y);
            rowsByKey[key] = control;
            return control;
        }

        private void RemoveRow(RowListTree control)
        {
            surfaces.Unwatch(control);
            panel.Children.Remove(control);
        }
    }
}
