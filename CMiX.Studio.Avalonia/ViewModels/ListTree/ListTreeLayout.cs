// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    public sealed class ListTreeLayout
    {
        private readonly IReadOnlyDictionary<ListTreeRowKey, ListTreePlace> placeByKey;

        private ListTreeLayout(IReadOnlyList<ListTreePlace> places, IReadOnlyDictionary<ListTreeRowKey, ListTreePlace> placeByKey, double totalHeight)
        {
            Places = places;
            this.placeByKey = placeByKey;
            TotalHeight = totalHeight;
        }

        public IReadOnlyList<ListTreePlace> Places { get; }

        public double TotalHeight { get; }

        // Null when the row is under a closed card or not in the snapshot.
        public ListTreePlace? PlaceOf(ListTreeRowKey key) => placeByKey.GetValueOrDefault(key);

        public static ListTreeLayout Of(ListTreeSnapshot snapshot, ListTreeSizes sizes)
        {
            var places = new List<ListTreePlace>();
            AddPlaces(snapshot.Roots, sizes, places);
            var placeByKey = places.ToDictionary(place => place.Key);
            return new ListTreeLayout(places, placeByKey, TotalHeightOf(places.Count, sizes));
        }

        private static void AddPlaces(IEnumerable<ListTreeRow> rows, ListTreeSizes sizes, List<ListTreePlace> places)
        {
            foreach (var row in rows)
            {
                places.Add(PlaceOf(row, places.Count * sizes.Pitch, sizes));
                if (row.IsExpanded)
                    AddPlaces(row.Children, sizes, places);
            }
        }

        private static ListTreePlace PlaceOf(ListTreeRow row, double y, ListTreeSizes sizes) =>
            new(row.Key, y, sizes.RowHeight, TotalHeightOf(VisibleCountOf(row), sizes), row.Depth, row.Depth * sizes.Indent, row.Children.Count > 0);

        private static int VisibleCountOf(ListTreeRow row) =>
            1 + (row.IsExpanded ? row.Children.Sum(VisibleCountOf) : 0);

        private static double TotalHeightOf(int rowCount, ListTreeSizes sizes) =>
            rowCount == 0 ? 0 : rowCount * sizes.Pitch - sizes.Gap;
    }
}
