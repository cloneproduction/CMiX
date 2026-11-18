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
    // The compositions of the project with their layers, and the entities, cameras and lights of each layer.
    // The tree follows the lists, also when a peer changes them.
    // A selected node selects its whole chain in the lists, as one undo step.
    public sealed partial class ProjectTree : ObservableObject, IDisposable
    {
        private readonly UndoManager undoManager;
        private readonly TreeDropService dropService = new();
        private readonly PrefabManager compositions;
        private readonly Project project;
        private readonly TreeNode root;
        private readonly TreeViewSet views;
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
            // The one row under the root is the group of the compositions. The compositions are under it.
            root = new TreeNode(this, null, null, null, "Project", null);
            root.AddGroups(project);

            adds = new TreeAddCatalogue(compositions);
            fileDrop = new TreeFileDropRules(pathDrop, compositions, undoManager);
            targetAdds = adds.ChoicesFor(null);
            views = new TreeViewSet();
            edit = new TreeEdit(undoManager, toasts);
            dragState = new TreeDragState(dropService);
            drop = new TreeDrop(dropService, notice);
            search = new TreeSearch(CloseRowsInView);
            openState = new TreeOpenState(CloseRowsInView, search);
        }

        // ---------------------------------------------------------------- hide and show
        // These keys write the value of the eye toggle of a row (TreeNode.HiddenValue), as a click on the eye does. So the key and the
        // click write the same undo step (ValueChangedCommand) and send the same wire message (MessageValueChanged).

        // Toggles the hide value of the target row and shows a toast with the undo key. Nothing changes during a drag, with no
        // target, or on a row that has no visibility.
        public bool TryToggleHideTarget()
        {
            if (IsDragging || TargetNode is not { } node)
                return false;

            return edit.TryToggleHide(node);
        }

        // Shows every hidden row of the tree, also the rows under a closed row, as one undo step, and shows a toast with the undo
        // key. With no hidden row, or during a drag, nothing changes: no undo step and no toast.
        public bool TryShowAll()
        {
            if (IsDragging)
                return false;

            return edit.TryShowAll(Roots);
        }

        // ---------------------------------------------------------------- open and close by branch and by level
        // These keys change only the open state of the rows: no undo step and no wire message, as Tree.Expand. A row that closes over
        // the cursor takes the cursor (see OnRowExpandedChanged). Nothing changes during a drag.

        // Opens the parents of the target row and closes every other row. The target keeps its state. Nothing changes with no target
        // or during a drag. Returns the rows that it closed.
        public IReadOnlyList<TreeNode> CloseOtherRows()
        {
            if (IsDragging || TargetNode is not { } target)
                return new List<TreeNode>();

            return openState.CollapseOthers(Roots, target);
        }

        // Opens every closed parent of the selected row and moves the cursor to it. The view then scrolls the row to the middle. The
        // selection does not change: no undo step and no wire message. The opened parents leave the search, so the end of the search
        // does not close them. Nothing changes with no selection or during a drag.
        public bool TryRevealSelected()
        {
            if (IsDragging || selectedNode is not { } node)
                return false;

            openState.OpenParents(node);
            CursorNode = node;
            RevealRequested?.Invoke(this, node);
            return true;
        }

        // A view gives its hooks when it shows the tree, and takes them back when it goes. A view that goes takes back only its own
        // hooks, so the other view keeps working. The hooks of the tree (IsPointerOverTree, IsTreeFocused and the others) ask every view.
        public void AttachView(TreeViewHooks view) => views.Attach(view);

        public void DetachView(TreeViewHooks view) => views.Detach(view);

        // True when the view of these hooks is the active view (see TreeViewSet.ActiveView). A view answers a request (RenameRequested,
        // AddRequested, MenuRequested, CollapseOthersRequested, RevealRequested, TreeFocusRequested, SearchFocusRequested) only when this is true,
        // so with two views one request opens one menu.
        public bool IsActiveView(TreeViewHooks view) => views.IsActiveView(view);

        // The active view keeps the rows still on the screen while rows close: the row of the node when it is in view, else the first
        // row in view. The view runs the close and returns the rows that it closed. Without a view, the close only runs.
        public void CloseRowsInView(TreeNode? node, Func<IReadOnlyList<TreeNode>> close) => views.CloseRowsInView(node, close);

        // True when a control outside the panel of the tree has the focus. The focus on the window itself, or on no control, is not
        // the focus in another panel. With two views of the tree, the focus in the panel of one view is not in another panel.
        public bool IsFocusInOtherPanel() => views.IsFocusInOtherPanel();

        // True while the project loads. A row made then starts collapsed. A row made by the user, or by a peer later, starts expanded.
        internal bool IsLoading => project.IsLoading;

        // The gestures of the key bindings of the tree view (the names Tree.*).
        public ShortcutMap Shortcuts { get; }

        // The interface settings, for example the hint strip of the keys under the tree (InterfaceOptions.ShowKeyHints).
        public InterfaceOptions Interface { get; }

        // True when the pointer is over a view of the tree.
        public bool IsPointerOverTree() => views.IsPointerOverTree();

        // The view of the drag opens or closes the row under the pointer (see TreeRowDrag.ToggleHovered).
        public void ToggleDragRow() => views.ToggleDragRow();

        // True when a text box has the focus.
        public bool IsTextBoxFocused() => views.IsTextBoxFocused();

        // True when a view of the tree or one of its controls has the focus.
        public bool IsTreeFocused() => views.IsTreeFocused();

        // The view of the drag ends a drag of a row without a drop, for example on Escape in the search box (see TreeRowDrag.Cancel).
        public void CancelDrag() => views.CancelDrag();

        public ObservableCollection<TreeNode> Roots => root.Children;

        public IReadOnlyList<TreeAddGroup> GetAddGroups(TreeNode? node) => adds.GroupsFor(node);

        public IReadOnlyList<TreeAdd> GetAddChoices(TreeNode? node) => adds.ChoicesFor(node);

        // The add choices of the target node. The tree makes them again when the target changes.
        public IReadOnlyList<TreeAdd> TargetAdds => targetAdds;

        // True when the target node has something to add. It enables the Add button.
        public bool HasAdds => targetAdds.Count > 0;

        // The target node as an item of a list: the list, the item and its place in the list.
        // A group, or no node, is not an item.
        private TargetPlace? PlaceOfTarget()
        {
            if (TargetNode is not { Owner: { } list, Item: { } item })
                return null;

            var index = list.ManagerData.Items.IndexOf(item);
            return index < 0 ? null : new TargetPlace(list, item, index);
        }

        // A list of the tree changed: a place in a list, and so the commands, may have changed.
        internal void ListChanged()
        {
            RefreshCommands();
            RefreshSearch();
        }

        public bool IsPathTarget(TreeNode node) => fileDrop.IsTarget(node);

        public bool AcceptsPath(string path) => fileDrop.Accepts(path);

        public bool CanDropPaths(IEnumerable<string> paths) => fileDrop.CanDrop(paths);

        public void DropPaths(TreeNode? hovered, IReadOnlyList<string> paths) => fileDrop.Drop(hovered, paths);

        public void Dispose() => root.Dispose();
    }
}
