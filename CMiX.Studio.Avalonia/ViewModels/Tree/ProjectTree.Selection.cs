// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    // The rows that Ctrl and Shift clicks of the new tree add to the selected node. The old view shows only SelectedNode.
    public sealed partial class ProjectTree
    {
        private IReadOnlyList<TreeNode> selectedNodes = Array.Empty<TreeNode>();
        private TreeNode? selectionAnchor;

        // The selected node is always in the list. Every other change of SelectedNode leaves only the selected node.
        public IReadOnlyList<TreeNode> SelectedNodes => selectedNodes;

        // A Ctrl click: the row joins the selection and becomes the selected node.
        public void AddToSelection(TreeNode node)
        {
            var kept = selectedNodes.Where(other => !ReferenceEquals(other, node)).ToArray();
            SelectByClick(node);
            ShowSelectedNodes(kept.Append(node).ToArray());
        }

        // A Ctrl click on a selected row. The last row that joined before it becomes the selected node.
        public void RemoveFromSelection(TreeNode node)
        {
            if (!selectedNodes.Contains(node))
                return;

            var kept = selectedNodes.Where(other => !ReferenceEquals(other, node)).ToArray();
            if (kept.Length == 0)
            {
                Select(null, SelectionOrigin.Pointer);
                CursorNode = node;
                selectionAnchor = node;
                return;
            }

            SelectByClick(ReferenceEquals(node, selectedNode) ? kept[^1] : selectedNode!);
            ShowSelectedNodes(kept);
            selectionAnchor = node;
        }

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

            SelectByClick(node);
            ShowSelectedNodes(shownRows.Skip(Math.Min(from, to)).Take(Math.Abs(to - from) + 1).ToArray());
            selectionAnchor = anchor;
        }

        // After the drop of a block, the moved rows stay selected.
        private void SelectBlock(TreeNode primary, IReadOnlyList<TreeNode> nodes)
        {
            Select(primary, SelectionOrigin.Program);
            ShowSelectedNodes(nodes);
        }

        // The selected node becomes the only selected row, and the anchor of the next Shift click.
        private void ResetSelectedNodes()
        {
            selectionAnchor = selectedNode;
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
