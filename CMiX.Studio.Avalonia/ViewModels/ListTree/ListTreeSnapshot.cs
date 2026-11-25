// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    public sealed class ListTreeSnapshot
    {
        public ListTreeSnapshot(IReadOnlyList<ListTreeRow> roots)
        {
            Roots = roots;
        }

        public IReadOnlyList<ListTreeRow> Roots { get; }

        public static ListTreeSnapshot Of(IEnumerable<TreeNode> roots) => Of(roots, ListTreeSearchFilter.None);

        public static ListTreeSnapshot Of(IEnumerable<TreeNode> roots, ListTreeSearchFilter filter) => new(RowsOf(roots, 0, null, filter));

        public IReadOnlyList<ListTreeRow> VisibleRows()
        {
            var rows = new List<ListTreeRow>();
            AddVisibleRows(Roots, rows);
            return rows;
        }

        public IReadOnlyList<TreeNode> VisibleNodes() => VisibleRows().Select(row => row.Node).OfType<TreeNode>().ToArray();

        // Also the rows under a closed card, in tree order.
        public IReadOnlyList<ListTreeRow> AllRows()
        {
            var rows = new List<ListTreeRow>();
            AddAllRows(Roots, rows);
            return rows;
        }

        // Null when the node has no row in the snapshot.
        public ListTreeRow? RowOf(TreeNode node) => AllRows().FirstOrDefault(row => ReferenceEquals(row.Node, node));

        private static void AddAllRows(IEnumerable<ListTreeRow> nodes, List<ListTreeRow> rows)
        {
            foreach (var row in nodes)
            {
                rows.Add(row);
                AddAllRows(row.Children, rows);
            }
        }

        private static void AddVisibleRows(IEnumerable<ListTreeRow> nodes, List<ListTreeRow> rows)
        {
            foreach (var row in nodes)
            {
                rows.Add(row);
                if (row.IsExpanded)
                    AddVisibleRows(row.Children, rows);
            }
        }

        private static IReadOnlyList<ListTreeRow> RowsOf(IEnumerable<TreeNode> nodes, int depth, ListTreeRowKey? parent, ListTreeSearchFilter filter) =>
            nodes.Where(filter.Shows).Select(node => RowOf(node, depth, ListTreeRowKey.Of(node, parent), filter)).Where(row => row.IsShown).ToArray();

        private static ListTreeRow RowOf(TreeNode node, int depth, ListTreeRowKey key, ListTreeSearchFilter filter) =>
            new(key, node, depth, IsCard(node), node.Group != null, filter.ShowsOpen(node), RowsOf(node.Children, depth + 1, key, filter));

        private static bool IsCard(TreeNode node) => node.ChildManager != null || node.Children.Count > 0;
    }
}
