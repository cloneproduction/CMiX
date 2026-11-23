// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using Avalonia.Controls;
using CMiX.Studio.Avalonia.ViewModels.ListTree;

namespace CMiX.Studio.Avalonia.Views
{
    // The card areas of the new tree. Each rebuild makes them again in tree order, so an inner area draws over its outer area.
    internal sealed class ListTreeCardAreas
    {
        private readonly Panel panel;
        private readonly List<FrameListTreeCard> areas = new();

        public ListTreeCardAreas(Panel panel)
        {
            this.panel = panel;
        }

        public IReadOnlyList<FrameListTreeCard> Areas => areas;

        public void Show(IReadOnlyList<ListTreeRow> rows, ListTreeLayout layout)
        {
            panel.Children.RemoveAll(areas);
            areas.Clear();
            foreach (var row in rows)
                if (layout.PlaceOf(row.Key) is { CanToggle: true } place)
                    Add(row, place);
        }

        public void ShowMarks(ListTreeMarks marks)
        {
            foreach (var area in areas)
                area.ShowMarks(marks);
        }

        private void Add(ListTreeRow row, ListTreePlace place)
        {
            var area = new FrameListTreeCard { Place = place, DataContext = row.Node, ZIndex = ListTreeLayers.CardArea };
            areas.Add(area);
            panel.Children.Add(area);
        }
    }
}
