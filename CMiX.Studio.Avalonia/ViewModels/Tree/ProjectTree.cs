// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CMiX.Core;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    public sealed partial class ProjectTree : ObservableObject, IDisposable, TreeNodeHost
    {
        private readonly UndoManager undoManager;
        private readonly TreeDropService dropService = new();
        private readonly PrefabManager compositions;
        private readonly Project project;
        private readonly TreeNode root;
        private readonly TreeAddCatalogue adds;
        private readonly TreeFileDropRules fileDrop;
        private readonly TreeEdit edit;
        private readonly TreeDragState dragState;
        private readonly TreeDrop drop;
        private readonly TreeSearch search;
        private readonly TreeOpenState openState;
        private IReadOnlyList<TreeAdd> targetAdds;

        public ProjectTree(Project project, UndoManager undoManager, IMoveBlockedNotice notice,
            ShortcutMap shortcuts, PathDropService pathDrop, IToastService toasts, InterfaceOptions interfaceOptions)
        {
            this.undoManager = undoManager;
            Shortcuts = shortcuts;
            Interface = interfaceOptions;
            this.project = project;
            compositions = project.CompositionManager;
            Views = new TreeViewSet();
            adds = new TreeAddCatalogue(compositions);
            fileDrop = new TreeFileDropRules(pathDrop, compositions, undoManager);
            edit = new TreeEdit(undoManager, toasts);
            dragState = new TreeDragState(dropService);
            drop = new TreeDrop(dropService, notice);
            search = new TreeSearch(Views);
            openState = new TreeOpenState(Views, search);
            // The one row under the root is the group of the compositions.
            root = new TreeNode(this, null, null, null, "Project", null);
            root.AddGroups(project);
            targetAdds = adds.ChoicesFor(null);
        }

        // These keys change only the open state: no undo step and no wire message.

        public IReadOnlyList<TreeNode> CloseOtherRows()
        {
            if (IsDragging || TargetNode is not { } target)
                return new List<TreeNode>();

            return openState.CollapseOthers(Roots, target);
        }

        // A view that goes takes back only its own hooks, so the other view keeps working.
        public void AttachView(TreeViewHooks view) => Views.Attach(view);

        public void DetachView(TreeViewHooks view) => Views.Detach(view);

        public bool IsActiveView(TreeViewHooks view) => Views.IsActiveView(view);

        public TreeViewSet Views { get; }

        public ShortcutMap Shortcuts { get; }

        public InterfaceOptions Interface { get; }

        public ObservableCollection<TreeNode> Roots => root.Children;

        public IReadOnlyList<TreeAddGroup> GetAddGroups(TreeNode? node) => adds.GroupsFor(node);

        public IReadOnlyList<TreeAdd> GetAddChoices(TreeNode? node) => adds.ChoicesFor(node);

        public IReadOnlyList<TreeAdd> TargetAdds => targetAdds;

        private TargetPlace? PlaceOfTarget()
        {
            if (TargetNode is not { Owner: { } list, Item: { } item })
                return null;

            var index = list.ManagerData.Items.IndexOf(item);
            return index < 0 ? null : new TargetPlace(list, item, index);
        }

        void TreeNodeHost.ListChanged()
        {
            RefreshCommands();
            RefreshSearch();
        }

        bool TreeNodeHost.IsLoading => project.IsLoading;

        void TreeNodeHost.RefreshSearch() => RefreshSearch();

        public bool IsPathTarget(TreeNode node) => fileDrop.IsTarget(node);

        public bool AcceptsPath(string path) => fileDrop.Accepts(path);

        public bool CanDropPaths(IEnumerable<string> paths) => fileDrop.CanDrop(paths);

        public void DropPaths(TreeNode? hovered, IReadOnlyList<string> paths) => fileDrop.Drop(hovered, paths);

        public void Dispose() => root.Dispose();
    }
}
