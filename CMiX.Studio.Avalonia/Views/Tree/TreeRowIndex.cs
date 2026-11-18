// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // The tree and each item tell when they prepare or clear an item, so no walk of all the rows is needed.
    internal sealed class TreeRowIndex
    {
        private readonly TreeView treeView;
        private readonly Dictionary<TreeNode, TreeViewItem> items = new();
        private readonly Dictionary<TreeViewItem, TreeNode> nodes = new();

        public TreeRowIndex(TreeView treeView)
        {
            this.treeView = treeView;
            treeView.ContainerPrepared += OnContainerPrepared;
            treeView.ContainerClearing += OnContainerClearing;
        }

        internal int FallbackWalks { get; private set; }

        internal int Count => items.Count;

        // A missing entry or an item that left the tree causes a walk.
        public TreeViewItem? FindItem(TreeNode node)
        {
            if (items.TryGetValue(node, out var known))
            {
                if (ReferenceEquals(known.DataContext, node) && treeView.IsVisualAncestorOf(known))
                    return known;

                Remove(known);
            }

            FallbackWalks++;
            var found = treeView.GetVisualDescendants().OfType<TreeViewItem>().FirstOrDefault(item => ReferenceEquals(item.DataContext, node));
            if (found != null)
                Add(found, node);

            return found;
        }

        // The dictionary has no row order, so this reads the containers of each level.
        public IEnumerable<TreeViewItem> InRowOrder(Func<TreeViewItem, bool> skip) => InRowOrder(treeView, skip);

        private static IEnumerable<TreeViewItem> InRowOrder(ItemsControl parent, Func<TreeViewItem, bool> skip)
        {
            foreach (var item in parent.GetRealizedContainers().OfType<TreeViewItem>())
            {
                if (skip(item))
                    continue;

                yield return item;
                foreach (var child in InRowOrder(item, skip))
                    yield return child;
            }
        }

        internal void Forget(TreeViewItem item)
        {
            item.ContainerPrepared -= OnContainerPrepared;
            item.ContainerClearing -= OnContainerClearing;
            Remove(item);
            foreach (var child in item.GetRealizedContainers().OfType<TreeViewItem>())
                Forget(child);
        }

        // A nested item does not tell the tree, so the index listens to each item too.
        private void OnContainerPrepared(object? sender, ContainerPreparedEventArgs e)
        {
            if (e.Container is not TreeViewItem item)
                return;

            item.ContainerPrepared -= OnContainerPrepared;
            item.ContainerClearing -= OnContainerClearing;
            item.ContainerPrepared += OnContainerPrepared;
            item.ContainerClearing += OnContainerClearing;
            if (item.DataContext is TreeNode node)
                Add(item, node);
        }

        private void OnContainerClearing(object? sender, ContainerClearingEventArgs e)
        {
            if (e.Container is TreeViewItem item)
                Forget(item);
        }

        private void Add(TreeViewItem item, TreeNode node)
        {
            Remove(item);
            if (items.TryGetValue(node, out var old))
                nodes.Remove(old);

            items[node] = item;
            nodes[item] = node;
        }

        private void Remove(TreeViewItem item)
        {
            if (!nodes.Remove(item, out var node))
                return;

            if (items.TryGetValue(node, out var current) && ReferenceEquals(current, item))
                items.Remove(node);
        }
    }
}
