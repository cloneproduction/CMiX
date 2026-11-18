// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    public sealed partial class ProjectTree
    {
        private TreeNode? selectedNode;
        private bool selecting;

        // The row that the commands of the tree act on: the row of the arrow keys, else the selected row. Null when there is none.
        // Rename, Remove, Duplicate, Move, Expand, CollapseOthers, the menu, the Add panel and the toolbar use it.
        public TreeNode? TargetNode => CursorNode ?? selectedNode;

        // The row that the arrow keys move to. It does not select the chain: no undo step and no wire message.
        // Enter (OpenSelectedCommand) or a click selects the chain. A change of SelectedNode moves it to the selected node.
        // A click on a row always moves it to that row (see SelectByClick).
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OpenSelectedCommand))]
        private TreeNode? cursorNode;

        partial void OnCursorNodeChanged(TreeNode? oldValue, TreeNode? newValue)
        {
            if (oldValue != null)
                oldValue.IsCursor = false;
            if (newValue != null)
                newValue.IsCursor = true;
            TreeNode.ClearCursorBranch(oldValue);
            TreeNode.MarkCursorBranch(newValue);
            OnTargetChanged();
        }

        // The nodes that show a row, from the top: a child shows when all its parents are expanded.
        public IReadOnlyList<TreeNode> VisibleNodes() => TreeWalk.VisibleRows(Roots);

        // Moves the cursor by a number of visible rows. It stops at the first and the last row.
        public void MoveCursor(int rows)
        {
            var nodes = VisibleNodes();
            if (nodes.Count == 0)
                return;

            var index = CursorNode == null ? -1 : TreeWalk.IndexOf(nodes, CursorNode);
            var next = index < 0 ? (rows >= 0 ? 0 : nodes.Count - 1) : Math.Clamp(index + rows, 0, nodes.Count - 1);
            CursorNode = nodes[next];
        }

        public void MoveCursorToFirst()
        {
            var nodes = VisibleNodes();
            if (nodes.Count > 0)
                CursorNode = nodes[0];
        }

        public void MoveCursorToLast()
        {
            var nodes = VisibleNodes();
            if (nodes.Count > 0)
                CursorNode = nodes[^1];
        }

        // Left: an expanded row collapses, else the cursor goes to the parent row.
        public void MoveCursorOut()
        {
            if (CursorNode is not { } node)
                return;

            if (node.IsExpanded && node.Children.Count > 0)
                node.IsExpanded = false;
            else if (node.Parent is { } parent && !ReferenceEquals(parent, root))
                CursorNode = parent;
        }

        // Right: a collapsed row expands, else the cursor goes to the first child row.
        public void MoveCursorIn()
        {
            if (CursorNode is not { Children.Count: > 0 } node)
                return;

            if (!node.IsExpanded)
                node.IsExpanded = true;
            else
                CursorNode = node.Children[0];
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.PropertyName == nameof(SelectedNode))
            {
                // The row of the arrow keys goes to the selected row. When it does not change, the target can still change: the
                // selection goes away while no row of the arrow keys is set.
                var before = CursorNode;
                CursorNode = selectedNode;
                if (ReferenceEquals(before, CursorNode))
                    OnTargetChanged();
            }
        }

        // The target node changed: its add choices and the commands follow.
        private void OnTargetChanged()
        {
            var hadAdds = HasAdds;
            targetAdds = adds.ChoicesFor(TargetNode);
            OnPropertyChanged(nameof(TargetAdds));
            if (hadAdds != HasAdds)
                OnPropertyChanged(nameof(HasAdds));

            OnPropertyChanged(nameof(TargetNode));
            RefreshCommands();
        }

        // The binding of the tree view sets this. The origin is Pointer.
        public TreeNode? SelectedNode
        {
            get => selectedNode;
            set => Select(value, SelectionOrigin.Pointer);
        }

        // The node the inspector shows. Only Tree.Inspect (Space) and a middle click on a row set it: a selection (a click, Enter) is the row
        // the lists and the engine act on, and never moves the inspector. A moved item keeps the inspector (its new node), a removed one passes it up.
        public TreeNode? InspectedNode
        {
            get => inspectedNode;
            private set => SetProperty(ref inspectedNode, value);
        }

        private TreeNode? inspectedNode;

        // Sends a node to the inspector.
        public void Inspect(TreeNode? node) => InspectedNode = node;

        // What made the last selection. The view brings a row into view only for some origins.
        public SelectionOrigin Origin { get; private set; } = SelectionOrigin.Program;

        // Selects a node and its chain in the lists. The origin is set before the change event.
        public void Select(TreeNode? node, SelectionOrigin origin)
        {
            Origin = origin;
            if (!SetProperty(ref selectedNode, node, nameof(SelectedNode)) || node == null || selecting)
                return;

            SelectChain(node);
        }

        // A key acts on the cursor. A click acts on the row under the pointer and moves the cursor there, also when the row is
        // already selected. A click on the chevron or on a toggle of a row does not come here.
        public void SelectByClick(TreeNode node)
        {
            Select(node, SelectionOrigin.Pointer);
            CursorNode = node;
        }

        // Selects the item of every node from the composition down to the node.
        private void SelectChain(TreeNode node)
        {
            var chain = new List<TreeNode>();
            for (var current = node; current != null; current = current.Parent)
                chain.Insert(0, current);

            selecting = true;
            undoManager.BeginCapture();
            try
            {
                foreach (var step in chain)
                    if (step.Owner != null && step.Item != null)
                        step.Owner.SelectedItem = step.Item;
            }
            finally
            {
                undoManager.EndCapture();
                selecting = false;
            }
        }

        // A list changed its selection. The tree follows without writing back to the lists.
        internal void SelectFromModel(TreeNode node)
        {
            if (selecting || ReferenceEquals(selectedNode, node))
                return;

            // A local list and a peer both come here. The tree cannot tell them apart, so the origin is Program.
            Origin = SelectionOrigin.Program;
            SetProperty(ref selectedNode, node, nameof(SelectedNode));
        }

        // The root and the Compositions row, which is the one row under it, are the top of the tree. A selection does not move there.
        private bool IsTop(TreeNode? node) => node == null || ReferenceEquals(node, root) || ReferenceEquals(node.Parent, root);

        // A node leaves the tree. A selection inside it moves to the parent.
        internal void NodeRemoved(TreeNode node)
        {
            if (inspectedNode != null && inspectedNode.IsOrIsUnder(node))
                Inspect(IsTop(node.Parent) ? null : node.Parent);
            if (CursorNode != null && CursorNode.IsOrIsUnder(node))
                CursorNode = selectedNode != null && !selectedNode.IsOrIsUnder(node) ? selectedNode : null;

            if (selectedNode == null || !selectedNode.IsOrIsUnder(node))
                return;

            Origin = SelectionOrigin.Program;
            SetProperty(ref selectedNode, IsTop(node.Parent) ? null : node.Parent, nameof(SelectedNode));
        }
    }
}
