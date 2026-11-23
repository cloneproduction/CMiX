// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using CMiX.Studio.Avalonia.ViewModels.Tree;

namespace CMiX.Studio.Avalonia.Views
{
    // The requests of the tree that the new tree answers while it is the active view, as TreeScreenRequests answers them for the old tree.
    internal sealed class ListTreeScreenRequests
    {
        private readonly PanelListTreeRows rows;
        private readonly ListTreeMenus menus;
        private readonly Func<bool> isActiveView;
        private readonly TreeViewLifetime lifetime;

        private ProjectTree? tree;

        public ListTreeScreenRequests(PanelListTreeRows rows, ListTreeMenus menus, Func<bool> isActiveView, TreeViewLifetime lifetime)
        {
            this.rows = rows;
            this.menus = menus;
            this.isActiveView = isActiveView;
            this.lifetime = lifetime;
        }

        public void Watch(ProjectTree watched)
        {
            tree = watched;
            watched.TreeFocusRequested += OnTreeFocusRequested;
            watched.CollapseOthersRequested += OnCollapseOthersRequested;
            watched.RenameRequested += OnRenameRequested;
            watched.AddRequested += OnAddRequested;
            watched.MenuRequested += OnMenuRequested;
        }

        public void Unwatch()
        {
            if (tree == null)
                return;

            tree.TreeFocusRequested -= OnTreeFocusRequested;
            tree.CollapseOthersRequested -= OnCollapseOthersRequested;
            tree.RenameRequested -= OnRenameRequested;
            tree.AddRequested -= OnAddRequested;
            tree.MenuRequested -= OnMenuRequested;
            tree = null;
        }

        private void AnswerWhenActive(Action<ProjectTree> answer)
        {
            if (isActiveView() && tree is { } active)
                answer(active);
        }

        private void OnAddRequested(object? sender, EventArgs e) => AnswerWhenActive(menus.OpenAddAtTarget);

        private void OnMenuRequested(object? sender, EventArgs e) => AnswerWhenActive(menus.OpenMenuAtTarget);

        private void OnTreeFocusRequested(object? sender, EventArgs e) => AnswerWhenActive(FocusTree);

        private void FocusTree(ProjectTree active)
        {
            var node = active.TargetNode ?? active.VisibleNodes().FirstOrDefault();
            if (node != null && rows.RowOf(node) is { } row && row.Focus(NavigationMethod.Tab))
                return;

            rows.Children.OfType<RowListTree>().FirstOrDefault()?.Focus(NavigationMethod.Tab);
        }

        private void OnRenameRequested(object? sender, TreeNode node) => AnswerWhenActive(_ => rows.RowOf(node)?.StartRename());

        private void OnCollapseOthersRequested(object? sender, EventArgs e) => AnswerWhenActive(CloseOtherRows);

        private void CloseOtherRows(ProjectTree active)
        {
            if (active.TargetNode is not { } target)
                return;

            active.CloseOtherRows();
            lifetime.Post(() => rows.RowOf(target)?.BringIntoView(), DispatcherPriority.Loaded);
        }
    }
}
