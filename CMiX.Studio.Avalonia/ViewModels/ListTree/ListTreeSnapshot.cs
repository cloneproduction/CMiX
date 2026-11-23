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

        public static ListTreeSnapshot Of(IEnumerable<TreeNode> roots) => new(RowsOf(roots, 0, null));

        public IReadOnlyList<ListTreeRow> VisibleRows()
        {
            var rows = new List<ListTreeRow>();
            AddVisibleRows(Roots, rows);
            return rows;
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

        private static IReadOnlyList<ListTreeRow> RowsOf(IEnumerable<TreeNode> nodes, int depth, ListTreeRowKey? parent) =>
            nodes.Where(IsShown).Select(node => RowOf(node, depth, ListTreeRowKey.Of(node, parent))).ToArray();

        private static ListTreeRow RowOf(TreeNode node, int depth, ListTreeRowKey key) =>
            new(key, node, depth, IsCard(node), node.IsExpanded, RowsOf(node.Children, depth + 1, key));

        private static bool IsShown(TreeNode node) => node.Group == null || node.Children.Count > 0;

        private static bool IsCard(TreeNode node) => node.ChildManager != null || node.Children.Count > 0;
    }
}
