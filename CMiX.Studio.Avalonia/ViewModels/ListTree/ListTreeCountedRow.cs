// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // A row of the live snapshot with the counts of its visible rows, as ListTreeLayout gives them.
    public sealed class ListTreeCountedRow
    {
        private readonly int[] countsBefore;

        private ListTreeCountedRow(ListTreeRow row, int index, IReadOnlyList<ListTreeCountedRow> children, int[] countsBefore)
        {
            Row = row;
            Index = index;
            Children = children;
            this.countsBefore = countsBefore;
        }

        public ListTreeRow Row { get; }

        public object Model => Row.Key.Model;

        // The index of the row in the children of its parent.
        public int Index { get; }

        public IReadOnlyList<ListTreeCountedRow> Children { get; }

        // The row and the rows that show under it, when the row shows.
        public int Count => 1 + (Row.IsExpanded ? countsBefore[^1] : 0);

        // The rows that the children before the index show.
        public int CountBefore(int index) => countsBefore[index];

        public static ListTreeCountedRow Of(ListTreeRow row, int index)
        {
            var children = row.Children.Select(Of).ToArray();
            return new ListTreeCountedRow(row, index, children, CountsBeforeOf(children));
        }

        private static int[] CountsBeforeOf(IReadOnlyList<ListTreeCountedRow> children)
        {
            var counts = new int[children.Count + 1];
            for (var index = 0; index < children.Count; index++)
                counts[index + 1] = counts[index] + children[index].Count;

            return counts;
        }
    }
}
