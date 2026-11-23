// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // The rows of one live snapshot by key, by node and the first node of each model, made once for a drag.
    public sealed class ListTreeLiveRows
    {
        private readonly IReadOnlyDictionary<object, TreeNode> nodesByModel;
        private readonly IReadOnlyDictionary<TreeNode, ListTreeRow> rowsByNode;

        private ListTreeLiveRows(IReadOnlyDictionary<ListTreeRowKey, ListTreeRow> byKey,
            IReadOnlyDictionary<object, TreeNode> nodesByModel, IReadOnlyDictionary<TreeNode, ListTreeRow> rowsByNode)
        {
            ByKey = byKey;
            this.nodesByModel = nodesByModel;
            this.rowsByNode = rowsByNode;
        }

        public IReadOnlyDictionary<ListTreeRowKey, ListTreeRow> ByKey { get; }

        public static ListTreeLiveRows Of(ListTreeSnapshot snapshot)
        {
            var byKey = new Dictionary<ListTreeRowKey, ListTreeRow>();
            var nodesByModel = new Dictionary<object, TreeNode>(ReferenceEqualityComparer.Instance);
            var rowsByNode = new Dictionary<TreeNode, ListTreeRow>();
            foreach (var row in snapshot.AllRows())
            {
                byKey.Add(row.Key, row);
                if (row.Node is { } node)
                {
                    nodesByModel.TryAdd(row.Key.Model, node);
                    rowsByNode.Add(node, row);
                }
            }

            return new ListTreeLiveRows(byKey, nodesByModel, rowsByNode);
        }

        public TreeNode? NodeOf(object model) => nodesByModel.GetValueOrDefault(model);

        public ListTreeRow? RowOf(TreeNode node) => rowsByNode.GetValueOrDefault(node);
    }
}
