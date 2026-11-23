// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Animation;
using CMiX.Studio.Avalonia.ViewModels.ListTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.UI.Themes;

namespace CMiX.Studio.Avalonia.Views
{
    // The row controls of the new tree by the key of their row. A row control lives as long as the key of its row.
    internal sealed class ListTreeRowControls
    {
        private readonly PanelListTreeRows panel;
        private readonly ListTreeSurfaces surfaces;
        private readonly Dictionary<ListTreeRowKey, RowListTree> rowsByKey = new();

        public ListTreeRowControls(PanelListTreeRows panel, ListTreeSurfaces surfaces)
        {
            this.panel = panel;
            this.surfaces = surfaces;
        }

        public RowListTree? RowOf(ListTreeRowKey key) => rowsByKey.GetValueOrDefault(key);

        public RowListTree? RowOf(TreeNode node) => rowsByKey.Values.FirstOrDefault(row => ReferenceEquals(row.DataContext, node));

        public void Show(IReadOnlyList<ListTreeRow> rows, ListTreeLayout layout)
        {
            foreach (var row in rows)
            {
                var place = layout.PlaceOf(row.Key)!;
                var control = ControlOf(row.Key, place);
                Slide(control);
                ShowRow(control, row, place, ListTreeRowLight.Rest);
            }

            RemoveRowsNotIn(layout);
        }

        // The dragged row has no transition: it snaps into its landing place.
        public void ShowDrag(ListTreeDragLook look)
        {
            foreach (var row in look.Rows)
            {
                var place = look.Layout.PlaceOf(row.Key)!;
                var control = ControlOf(row.Key, place);
                var light = look.LightOf(row.Key);
                if (light == ListTreeRowLight.Dragged)
                    Snap(control);
                else
                    Slide(control);
                ShowRow(control, row, place, light);
            }

            RemoveRowsNotIn(look.Layout);
        }

        private void RemoveRowsNotIn(ListTreeLayout layout)
        {
            foreach (var key in rowsByKey.Keys.Where(key => layout.PlaceOf(key) == null).ToArray())
                RemoveRow(key);
        }

        private RowListTree ControlOf(ListTreeRowKey key, ListTreePlace place) => rowsByKey.GetValueOrDefault(key) ?? AddRow(key, place);

        private static void ShowRow(RowListTree control, ListTreeRow row, ListTreePlace place, ListTreeRowLight light)
        {
            control.DataContext = row.Node;
            control.IconKey = ListTreeIcons.KeyOf(row);
            control.ItemCount = row.Node?.Children.Count ?? 0;
            control.Place = place;
            control.ZIndex = light == ListTreeRowLight.Dragged ? ListTreeLayers.DraggedRow : ListTreeLayers.Row;
            control.ShowLight(light);
            PanelListTreeRows.SetTop(control, place.Y);
        }

        // A new row shows at its place at once. Only later moves slide.
        private RowListTree AddRow(ListTreeRowKey key, ListTreePlace place)
        {
            var control = new RowListTree { ZIndex = ListTreeLayers.Row };
            PanelListTreeRows.SetTop(control, place.Y);
            Slide(control);
            surfaces.Watch(control);
            rowsByKey[key] = control;
            panel.Children.Add(control);
            return control;
        }

        private void Slide(RowListTree control)
        {
            if (control.Transitions == null && panel.MoveDuration > TimeSpan.Zero)
                control.Transitions = new Transitions
                {
                    new DoubleTransition { Property = PanelListTreeRows.TopProperty, Duration = panel.MoveDuration, Easing = Motion.Easing }
                };
        }

        private static void Snap(RowListTree control) => control.Transitions = null;

        private void RemoveRow(ListTreeRowKey key)
        {
            surfaces.Unwatch(rowsByKey[key]);
            panel.Children.Remove(rowsByKey[key]);
            rowsByKey.Remove(key);
        }
    }
}
