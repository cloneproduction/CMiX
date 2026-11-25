// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.ViewModels.ListTree
{
    // Raises Changed when a row comes, goes, moves, a card opens or closes or a search mark changes, and HiddenChanged when a row is hidden or shown. Call Watch again after a change.
    public sealed class ListTreeNodeWatch
    {
        private readonly List<TreeNode> watchedNodes = new();
        private readonly List<INotifyCollectionChanged> watchedLists = new();

        public event EventHandler? Changed;

        // The rows under the row get a new IsUnderHiddenParent at the same time.
        public event EventHandler? HiddenChanged;

        public void Watch(IEnumerable<TreeNode> roots)
        {
            Unwatch();
            WatchList(roots);
        }

        public void Unwatch()
        {
            foreach (var node in watchedNodes)
                node.PropertyChanged -= OnNodeChanged;
            foreach (var list in watchedLists)
                list.CollectionChanged -= OnListChanged;

            watchedNodes.Clear();
            watchedLists.Clear();
        }

        private void WatchList(IEnumerable<TreeNode> nodes)
        {
            if (nodes is INotifyCollectionChanged list)
            {
                list.CollectionChanged += OnListChanged;
                watchedLists.Add(list);
            }

            foreach (var node in nodes)
            {
                node.PropertyChanged += OnNodeChanged;
                watchedNodes.Add(node);
                WatchList(node.Children);
            }
        }

        private void OnNodeChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(TreeNode.IsExpanded) or nameof(TreeNode.IsSearchMatch) or nameof(TreeNode.HasSearchMatchBelow))
                Changed?.Invoke(this, EventArgs.Empty);
            else if (e.PropertyName == nameof(TreeNode.IsHidden))
                HiddenChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnListChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
            Changed?.Invoke(this, EventArgs.Empty);
    }
}
