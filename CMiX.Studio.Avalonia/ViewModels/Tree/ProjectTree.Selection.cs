// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    // The rows that Ctrl and Shift clicks of the tree add to the selected node.
    public sealed partial class ProjectTree
    {
        private IReadOnlyList<TreeNode> selectedNodes = Array.Empty<TreeNode>();
        private TreeNode? selectionAnchor;
        private bool isSelectingWithSet;

        // The selected node is always in the list. Every other change of SelectedNode leaves only the selected node.
        public IReadOnlyList<TreeNode> SelectedNodes => selectedNodes;

        // A Ctrl click: the row joins the selection and becomes the selected node.
        public void AddToSelection(TreeNode node)
        {
            var kept = selectedNodes.Where(other => !ReferenceEquals(other, node)).ToArray();
            SelectWithSetByClick(node, kept.Append(node).ToArray());
        }

        // A Ctrl click on a selected row. The last row that joined before it becomes the selected node. The only selected row stays.
        public void RemoveFromSelection(TreeNode node)
        {
            var kept = selectedNodes.Where(other => !ReferenceEquals(other, node)).ToArray();
            if (!selectedNodes.Contains(node) || kept.Length == 0)
                return;

            SelectWithSetByClick(ReferenceEquals(node, selectedNode) ? kept[^1] : selectedNode!, kept);
            selectionAnchor = node;
        }

        // A Ctrl click on a row.
        public void AddOrRemoveFromSelection(TreeNode node)
        {
            if (selectedNodes.Contains(node))
                RemoveFromSelection(node);
            else
                AddToSelection(node);
        }

        // A plain press on such a row waits for the release, so a drag can take every selected row.
        public bool IsOneOfSeveralSelected(TreeNode node) => selectedNodes.Count > 1 && selectedNodes.Contains(node);

        // The drop selects the moved items. A blocked drop leaves the sources selected, and the result is empty.
        public IReadOnlyList<TreeNode> SelectedNodesOf(IReadOnlyList<object?> items) =>
            selectedNode is { } selected && items.Contains(selected.Item, ReferenceEqualityComparer.Instance)
                ? selectedNodes.Where(node => items.Contains(node.Item, ReferenceEqualityComparer.Instance)).ToArray()
                : Array.Empty<TreeNode>();

        // A Shift click: the shown rows from the anchor to the row. With no anchor in the shown rows it is a plain click.
        public void SelectRange(TreeNode node, IReadOnlyList<TreeNode> shownRows)
        {
            var anchor = selectionAnchor ?? selectedNode;
            var from = anchor == null ? -1 : TreeWalk.IndexOf(shownRows, anchor);
            var to = TreeWalk.IndexOf(shownRows, node);
            if (from < 0 || to < 0)
            {
                SelectByClick(node);
                return;
            }

            SelectWithSetByClick(node, shownRows.Skip(Math.Min(from, to)).Take(Math.Abs(to - from) + 1).ToArray());
            selectionAnchor = anchor;
        }

        // After the drop of a block, the moved rows stay selected.
        private void SelectBlock(TreeNode primary, IReadOnlyList<TreeNode> nodes) => SelectWithSet(primary, nodes, SelectionOrigin.Program);

        private void SelectWithSetByClick(TreeNode node, IReadOnlyList<TreeNode> nodes)
        {
            SelectWithSet(node, nodes, SelectionOrigin.Pointer);
            CursorNode = node;
        }

        // The set is shown once: the reset to the selected node only does not run inside the select.
        private void SelectWithSet(TreeNode node, IReadOnlyList<TreeNode> nodes, SelectionOrigin origin)
        {
            isSelectingWithSet = true;
            try
            {
                Select(node, origin);
            }
            finally
            {
                isSelectingWithSet = false;
            }

            ShowSelectedNodes(nodes);
        }

        // The selected node becomes the anchor of the next Shift click, and the only selected row unless a select with a set runs.
        private void ResetSelectedNodes()
        {
            selectionAnchor = selectedNode;
            if (!isSelectingWithSet)
                ShowSelectedNodes(selectedNode == null ? Array.Empty<TreeNode>() : new[] { selectedNode });
        }

        private void ForgetSelectedUnder(TreeNode removed)
        {
            if (selectedNodes.Any(node => node.IsOrIsUnder(removed)))
                ShowSelectedNodes(selectedNodes.Where(node => !node.IsOrIsUnder(removed)).ToArray());
            if (selectionAnchor != null && selectionAnchor.IsOrIsUnder(removed))
                selectionAnchor = null;
        }

        private void ShowSelectedNodes(IReadOnlyList<TreeNode> nodes)
        {
            selectedNodes = nodes;
            OnPropertyChanged(nameof(SelectedNodes));
        }
    }
}
