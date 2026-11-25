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
        private readonly ListTreeSpareControls<RowListTree> spareRows = new();

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
            KeepSpare(TakeLeavingRows(shown));
            foreach (var row in rows)
            {
                var place = shown.PlaceOf(row.Key)!;
                var control = rowsByKey.GetValueOrDefault(row.Key);
                if (control == null && !Window.Overlaps(place.Y, place.RowHeight))
                    continue;

                ShowRow(control ?? TakeRow(row.Key, before.PlaceOf(row.Key) ?? place), row, place);
            }
        }

        // A control with the focus goes, so the focus keeper sees the lost focus. Every other control waits hidden for a row.
        private void KeepSpare(List<RowListTree> leaving)
        {
            foreach (var focused in leaving.Where(control => control.IsKeyboardFocusWithin).ToList())
            {
                leaving.Remove(focused);
                RemoveRow(focused);
            }

            foreach (var control in leaving)
            {
                ListTreeRowSlide.Snap(control);
                control.StopFlash();
            }

            spareRows.KeepAll(leaving);
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

            var control = TakeRow(key, place);
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

        // A spare control, else a new control. It starts at the place its row had in the last layout, or at its place when the row is new in the tree.
        private RowListTree TakeRow(ListTreeRowKey key, ListTreePlace start)
        {
            var control = spareRows.Take() ?? AddRow();
            PanelListTreeRows.SetTop(control, start.Y);
            rowsByKey[key] = control;
            return control;
        }

        private RowListTree AddRow()
        {
            var control = new RowListTree { ZIndex = ListTreeLayers.Row };
            surfaces.Watch(control);
            panel.Children.Add(control);
            return control;
        }

        private void RemoveRow(RowListTree control)
        {
            surfaces.Unwatch(control);
            panel.Children.Remove(control);
        }
    }
}
