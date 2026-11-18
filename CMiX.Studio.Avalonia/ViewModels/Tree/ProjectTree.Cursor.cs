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

        public TreeNode? TargetNode => CursorNode ?? selectedNode;

        // It does not select the chain: no undo step and no wire message.
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

        public IReadOnlyList<TreeNode> VisibleNodes() => TreeWalk.VisibleRows(Roots);

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

        public void MoveCursorOut()
        {
            if (CursorNode is not { } node)
                return;

            if (node.IsExpanded && node.Children.Count > 0)
                node.IsExpanded = false;
            else if (node.Parent is { } parent && !ReferenceEquals(parent, root))
                CursorNode = parent;
        }

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
                // The target can change also when the cursor does not: the selection goes away with no cursor.
                var before = CursorNode;
                CursorNode = selectedNode;
                if (ReferenceEquals(before, CursorNode))
                    OnTargetChanged();
            }
        }

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

        public TreeNode? SelectedNode
        {
            get => selectedNode;
            set => Select(value, SelectionOrigin.Pointer);
        }

        // Only Tree.Inspect and a middle click set it: a selection never moves the inspector.
        public TreeNode? InspectedNode
        {
            get => inspectedNode;
            private set => SetProperty(ref inspectedNode, value);
        }

        private TreeNode? inspectedNode;

        public void Inspect(TreeNode? node) => InspectedNode = node;

        public SelectionOrigin Origin { get; private set; } = SelectionOrigin.Program;

        // The origin is set before the change event.
        public void Select(TreeNode? node, SelectionOrigin origin)
        {
            Origin = origin;
            if (!SetProperty(ref selectedNode, node, nameof(SelectedNode)) || node == null || selecting)
                return;

            SelectChain(node);
        }

        // A click moves the cursor also when the row is already selected.
        public void SelectByClick(TreeNode node)
        {
            Select(node, SelectionOrigin.Pointer);
            CursorNode = node;
        }

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

        internal void SelectFromModel(TreeNode node)
        {
            if (selecting || ReferenceEquals(selectedNode, node))
                return;

            // The tree cannot tell a local list from a peer, so the origin is Program.
            Origin = SelectionOrigin.Program;
            SetProperty(ref selectedNode, node, nameof(SelectedNode));
        }

        private bool IsTop(TreeNode? node) => node == null || ReferenceEquals(node, root) || ReferenceEquals(node.Parent, root);

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
