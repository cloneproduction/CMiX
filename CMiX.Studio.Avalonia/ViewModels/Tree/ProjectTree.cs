// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CMiX.Core;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    // The compositions of the project with their layers, and the entities, cameras and lights of each layer.
    // The tree follows the lists, also when a peer changes them.
    // A selected node selects its whole chain in the lists, as one undo step.
    public sealed partial class ProjectTree : ObservableObject, IDisposable
    {
        private readonly UndoManager undoManager;
        private readonly IToastService? toasts;
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
        private TreeNode? selectedNode;
        private IReadOnlyList<TreeAdd> targetAdds;
        private bool selecting;

        public ProjectTree(Project project, UndoManager undoManager, IMoveBlockedNotice? notice = null,
            ShortcutMap? shortcuts = null, PathDropService? pathDrop = null, IToastService? toasts = null,
            InterfaceOptions? interfaceOptions = null)
        {
            this.undoManager = undoManager;
            this.toasts = toasts;
            Shortcuts = shortcuts ?? new ShortcutMap();
            Interface = interfaceOptions ?? new InterfaceOptions();
            this.project = project;
            compositions = project.CompositionManager;
            // The one row under the root is the group of the compositions. The compositions are under it.
            root = new TreeNode(this, null, null, null, "Project", null);
            root.AddGroups(project);

            adds = new TreeAddCatalogue(compositions);
            fileDrop = new TreeFileDropRules(pathDrop ?? new PathDropService(), compositions, undoManager);
            targetAdds = adds.ChoicesFor(null);
            views = new TreeViewSet();
            edit = new TreeEdit(undoManager, toasts);
            dragState = new TreeDragState(dropService);
            drop = new TreeDrop(dropService, notice);
            search = new TreeSearch(CloseRowsInView);
            openState = new TreeOpenState(CloseRowsInView, search);
            AddItemCommand = new RelayCommand<TreeAdd?>(add => add?.Execute?.Invoke(), add => add == null ? HasAdds : add.Execute != null);
            DuplicateItemCommand = new RelayCommand(
                () => { if (PlaceOfTarget() is { } place) edit.Duplicate(place); },
                () => PlaceOfTarget() != null);
            MoveItemUpCommand = new RelayCommand(
                () => { if (PlaceOfTarget() is { } place) edit.MoveUp(place); },
                () => PlaceOfTarget() is { } place && edit.CanMoveUp(place));
            MoveItemDownCommand = new RelayCommand(
                () => { if (PlaceOfTarget() is { } place) edit.MoveDown(place); },
                () => PlaceOfTarget() is { } place && edit.CanMoveDown(place));
            RemoveItemCommand = new RelayCommand(
                () => { if (PlaceOfTarget() is { } place) edit.Remove(place, TargetNode!.Header); },
                () => PlaceOfTarget() != null);
            AddAtTargetCommand = new RelayCommand(
                () => AddRequested?.Invoke(this, EventArgs.Empty),
                CanRunTreeKey);
            // Tree.Inspect sends the cursor row to the inspector. A selection (a click, Enter) never does: the inspector keeps what was sent.
            InspectCursorCommand = new RelayCommand(
                () => Inspect(cursorNode),
                () => cursorNode != null && !IsDragging && CanRunTreeKey());
            ToggleExpandTargetCommand = new RelayCommand(
                () =>
                {
                    if (IsDragging)
                        ToggleDragRow();
                    else if (TargetNode is { Children.Count: > 0 } node)
                        node.IsExpanded = !node.IsExpanded;
                },
                () => IsDragging ? !IsTextBoxFocused() : CanRunTreeKey() && TargetNode is { Children.Count: > 0 });
            OpenMenuAtTargetCommand = new RelayCommand(
                () => MenuRequested?.Invoke(this, EventArgs.Empty),
                CanRunTreeKey);
            FocusSearchCommand = new RelayCommand(
                FocusSearch,
                CanRunTreeKey);
            ClearSearchCommand = new RelayCommand(
                () =>
                {
                    ClearSearch();
                    FocusTree();
                },
                () => IsSearchActive && !IsDragging);
            RenameSelectedCommand = new RelayCommand(
                () => { if (TargetNode is { Item: not null } node) RenameRequested?.Invoke(this, node); },
                () => TargetNode?.Item != null);
            // Enter during a drag or a grab places the row (see TreeGrab), so it selects nothing then.
            OpenSelectedCommand = new RelayCommand(
                () => Select(cursorNode, SelectionOrigin.Keyboard),
                () => cursorNode != null && !IsDragging);
            RenameSelectedByKeyCommand = ByKey(RenameSelectedCommand);
            RemoveItemByKeyCommand = ByKey(RemoveItemCommand);
            DuplicateItemByKeyCommand = ByKey(DuplicateItemCommand);
            MoveItemUpByKeyCommand = ByKey(MoveItemUpCommand);
            MoveItemDownByKeyCommand = ByKey(MoveItemDownCommand);
            CollapseOthersCommand = new RelayCommand(
                () => CollapseOthersRequested?.Invoke(this, EventArgs.Empty),
                () => CanRunTreeKey() && TargetNode != null);
            ExpandBranchCommand = new RelayCommand(ExpandBranch, () => CanRunTreeKey() && TargetNode is { Children.Count: > 0 });
            CloseLevelCommand = new RelayCommand(CloseLevel, CanRunTreeKey);
            OpenLevelCommand = new RelayCommand(OpenLevel, CanRunTreeKey);
            CollapseAllCommand = new RelayCommand(CollapseAll, CanRunTreeKey);
            ToggleHideTargetCommand = new RelayCommand(() => ToggleHideTarget(), () => CanRunTreeKey() && TargetNode?.HiddenValue != null);
            ShowAllCommand = new RelayCommand(() => ShowAll(), CanRunTreeKey);
        }

        // ---------------------------------------------------------------- hide and show
        // These keys write the value of the eye toggle of a row (TreeNode.HiddenValue), as a click on the eye does. So the key and the
        // click write the same undo step (ValueChangedCommand) and send the same wire message (MessageValueChanged).

        // Hides or shows the target row (Tree.Hide). For a group row it is the hide toggle of the whole list.
        public IRelayCommand ToggleHideTargetCommand { get; }

        // Shows every hidden row of the tree as one undo step (Tree.ShowAll).
        public IRelayCommand ShowAllCommand { get; }

        // Toggles the hide value of the target row and shows a toast with the undo key. Nothing changes during a drag, with no
        // target, or on a row that has no visibility.
        public void ToggleHideTarget() => TryToggleHideTarget();

        public bool TryToggleHideTarget()
        {
            if (IsDragging || TargetNode is not { } node)
                return false;

            return edit.TryToggleHide(node);
        }

        // Shows every hidden row of the tree, also the rows under a closed row, as one undo step, and shows a toast with the undo
        // key. With no hidden row, or during a drag, nothing changes: no undo step and no toast.
        public void ShowAll() => TryShowAll();

        public bool TryShowAll()
        {
            if (IsDragging)
                return false;

            return edit.TryShowAll(Roots);
        }

        // ---------------------------------------------------------------- open and close by branch and by level
        // These keys change only the open state of the rows: no undo step and no wire message, as Tree.Expand. A row that closes over
        // the cursor takes the cursor (see OnRowExpandedChanged). Nothing changes during a drag.

        // Opens the target row and every row under it (Tree.ExpandBranch).
        public IRelayCommand ExpandBranchCommand { get; }

        // Closes the deepest open level of the rows that show (Tree.CloseLevel).
        public IRelayCommand CloseLevelCommand { get; }

        // Opens the shallowest closed level of the rows that show (Tree.OpenLevel).
        public IRelayCommand OpenLevelCommand { get; }

        // Closes every row of the tree (Tree.CollapseAll).
        public IRelayCommand CollapseAllCommand { get; }

        public void ExpandBranch()
        {
            if (IsDragging || TargetNode is not { Children.Count: > 0 } target)
                return;

            openState.ExpandBranch(target);
        }

        public void CloseLevel()
        {
            if (IsDragging)
                return;

            openState.CloseLevel(Roots, TargetNode);
        }

        public void OpenLevel()
        {
            if (IsDragging)
                return;

            openState.OpenLevel(Roots);
        }

        public void CollapseAll()
        {
            if (IsDragging)
                return;

            openState.CollapseAll(Roots, TargetNode);
        }

        // Closes every row of the tree but the path to the target row (Tree.CollapseOthers, see TargetNode).
        // The parents of that row open, and the row keeps its own state. The command has the conditions of the other tree keys. The view
        // runs CollapseOthers itself (see CollapseOthersRequested), so it can keep the row still on the screen while the rows close.
        public IRelayCommand CollapseOthersCommand { get; }

        // The active view handles this request (see IsActiveView): it runs CollapseOthers and keeps the target row in view.
        public event EventHandler? CollapseOthersRequested;

        // The row that the commands of the tree act on: the row of the arrow keys, else the selected row. Null when there is none.
        // Rename, Remove, Duplicate, Move, Expand, CollapseOthers, the menu, the Add panel and the toolbar use it.
        public TreeNode? TargetNode => cursorNode ?? selectedNode;

        // Opens the parents of the target row and closes every other row. The target keeps its state. Nothing changes with no target
        // or during a drag. Returns the rows that it closed.
        public IReadOnlyList<TreeNode> CollapseOthers()
        {
            if (IsDragging || TargetNode is not { } target)
                return new List<TreeNode>();

            return openState.CollapseOthers(Roots, target);
        }

        private IRelayCommand? revealSelectedCommand;

        // Reveals the selected row, the row of the Inspector (Tree.Reveal, see RevealSelected). The command has the condition of every
        // tree key (see CanRunTreeKey). With no selection it cannot run.
        public IRelayCommand RevealSelectedCommand => revealSelectedCommand ??= new RelayCommand(
            () => RevealSelected(),
            () => CanRunTreeKey() && selectedNode != null);

        // The active view handles this request (see IsActiveView): it scrolls the row of the node to the middle of the view.
        public event EventHandler<TreeNode>? RevealRequested;

        // Opens every closed parent of the selected row and moves the cursor to it. The view then scrolls the row to the middle. The
        // selection does not change: no undo step and no wire message. The opened parents leave the search, so the end of the search
        // does not close them. Nothing changes with no selection or during a drag.
        public void RevealSelected() => TryRevealSelected();

        public bool TryRevealSelected()
        {
            if (IsDragging || selectedNode is not { } node)
                return false;

            openState.OpenParents(node);
            CursorNode = node;
            RevealRequested?.Invoke(this, node);
            return true;
        }

        private IRelayCommand? grabTargetCommand;

        // Grabs the target row (Tree.Grab): the row follows the pointer and the arrow keys until a click or Enter puts it down (see
        // TreeGrab). The command has the condition of every tree key (see CanRunTreeKey). A row that cannot move cannot be grabbed.
        public IRelayCommand GrabTargetCommand => grabTargetCommand ??= new RelayCommand(
            () => { if (TargetNode is { } node && CanDrag(node)) GrabRequested?.Invoke(this, node); },
            () => CanRunTreeKey() && TargetNode is { } node && CanDrag(node));

        // The active view handles this request (see IsActiveView): it starts the grab of the node.
        public event EventHandler<TreeNode>? GrabRequested;

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

        // The keys of the main window for the target row (Tree.Rename, Tree.Remove, Tree.Duplicate, Tree.MoveUp, Tree.MoveDown). A key
        // runs its command on the condition of every tree key (see CanRunTreeKey). The buttons of the toolbar use the commands without
        // this condition.
        public IRelayCommand RenameSelectedByKeyCommand { get; }
        public IRelayCommand RemoveItemByKeyCommand { get; }
        public IRelayCommand DuplicateItemByKeyCommand { get; }
        public IRelayCommand MoveItemUpByKeyCommand { get; }
        public IRelayCommand MoveItemDownByKeyCommand { get; }

        // The one condition of every tree key of the main window. The focus comes first: a key runs while the tree has the focus, with
        // the pointer anywhere. It also runs while the pointer is over the tree and no other panel has the focus. It does not run while
        // a text box has the focus: Ctrl+R and the key down of Shift+D do not change the text, so the text box does not take them. It
        // does not run during a drag of a row. A key binding checks the command when the key is pressed, so the condition needs no
        // change event.
        private bool CanRunTreeKey() => !IsDragging && views.AllowsTreeKey();

        // True when a control outside the panel of the tree has the focus. The focus on the window itself, or on no control, is not
        // the focus in another panel. With two views of the tree, the focus in the panel of one view is not in another panel.
        public bool IsFocusInOtherPanel() => views.IsFocusInOtherPanel();

        private IRelayCommand ByKey(IRelayCommand command) =>
            new RelayCommand(() => command.Execute(null), () => CanRunTreeKey() && command.CanExecute(null));

        // True while the project loads. A row made then starts collapsed. A row made by the user, or by a peer later, starts expanded.
        internal bool IsLoading => project.IsLoading;

        // The gestures of the key bindings of the tree view (the names Tree.*).
        public ShortcutMap Shortcuts { get; }

        // The interface settings, for example the hint strip of the keys under the tree (InterfaceOptions.ShowKeyHints).
        public InterfaceOptions Interface { get; }

        // Opens the Add panel of the target row on that row (Tree.Add). With no target, the panel offers a composition. The panel
        // adds nothing at once. The command has the condition of every tree key (see CanRunTreeKey).
        public IRelayCommand AddAtTargetCommand { get; }

        public IRelayCommand InspectCursorCommand { get; }

        // True when the pointer is over a view of the tree.
        public bool IsPointerOverTree() => views.IsPointerOverTree();

        // The active view handles this request (see IsActiveView): it opens the Add panel.
        public event EventHandler? AddRequested;

        // Opens the menu of the target row under that row (Tree.Menu), or the menu of the project with no target. The command
        // has the condition of every tree key (see CanRunTreeKey).
        public IRelayCommand OpenMenuAtTargetCommand { get; }

        // Expands or collapses the target row (Tree.Expand). A row with no children does not
        // change. The command has the condition of every tree key (see CanRunTreeKey). During a drag of a row, it acts on the row under
        // the pointer instead (see ToggleDragRow), with the focus anywhere but in a text box.
        public IRelayCommand ToggleExpandTargetCommand { get; }

        // The view of the drag opens or closes the row under the pointer (see TreeRowDrag.ToggleHovered).
        public void ToggleDragRow() => views.ToggleDragRow();

        // True when a text box has the focus.
        public bool IsTextBoxFocused() => views.IsTextBoxFocused();

        // The active view handles this request (see IsActiveView): it opens the menu.
        public event EventHandler? MenuRequested;

        // True when a view of the tree or one of its controls has the focus.
        public bool IsTreeFocused() => views.IsTreeFocused();

        // Gives the focus back to the rows of the tree, for example after Escape in an empty search box. The active view handles it.
        public void FocusTree() => TreeFocusRequested?.Invoke(this, EventArgs.Empty);

        public event EventHandler? TreeFocusRequested;

        // The view of the drag ends a drag of a row without a drop, for example on Escape in the search box (see TreeRowDrag.Cancel).
        public void CancelDrag() => views.CancelDrag();

        // Starts the rename of the target row. The view sets the name box of the row to edit mode (see RenameRequested).
        public IRelayCommand RenameSelectedCommand { get; }

        // Selects the cursor node and its chain in the lists.
        public IRelayCommand OpenSelectedCommand { get; }

        // The active view handles this request (see IsActiveView): it starts the rename of the row of the node.
        public event EventHandler<TreeNode>? RenameRequested;

        private TreeNode? cursorNode;

        // The row that the arrow keys move to. It does not select the chain: no undo step and no wire message.
        // Enter (OpenSelectedCommand) or a click selects the chain. A change of SelectedNode moves it to the selected node.
        // A click on a row always moves it to that row (see SelectByClick).
        public TreeNode? CursorNode
        {
            get => cursorNode;
            set
            {
                var old = cursorNode;
                if (!SetProperty(ref cursorNode, value))
                    return;

                if (old != null)
                    old.IsCursor = false;
                if (value != null)
                    value.IsCursor = true;
                TreeNode.ClearCursorBranch(old);
                TreeNode.MarkCursorBranch(value);

                OpenSelectedCommand?.NotifyCanExecuteChanged();
                OnTargetChanged();
            }
        }

        // The nodes that show a row, from the top: a child shows when all its parents are expanded.
        public IReadOnlyList<TreeNode> VisibleNodes() => TreeWalk.VisibleRows(Roots);

        // Moves the cursor by a number of visible rows. It stops at the first and the last row.
        public void MoveCursor(int rows)
        {
            var nodes = VisibleNodes();
            if (nodes.Count == 0)
                return;

            var index = cursorNode == null ? -1 : TreeWalk.IndexOf(nodes, cursorNode);
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
            if (cursorNode is not { } node)
                return;

            if (node.IsExpanded && node.Children.Count > 0)
                node.IsExpanded = false;
            else if (node.Parent is { } parent && !ReferenceEquals(parent, root))
                CursorNode = parent;
        }

        // Right: a collapsed row expands, else the cursor goes to the first child row.
        public void MoveCursorIn()
        {
            if (cursorNode is not { Children.Count: > 0 } node)
                return;

            if (!node.IsExpanded)
                node.IsExpanded = true;
            else
                CursorNode = node.Children[0];
        }

        public ObservableCollection<TreeNode> Roots => root.Children;

        // The commands of the toolbar. They act on the target node, and they are disabled when it has nothing to do.
        // AddItemCommand runs the add that it gets. The toolbar picks the add (see TargetAdds). With no add, it tells only
        // whether the target node has something to add.
        public IRelayCommand<TreeAdd?> AddItemCommand { get; }
        public IRelayCommand DuplicateItemCommand { get; }
        public IRelayCommand MoveItemUpCommand { get; }
        public IRelayCommand MoveItemDownCommand { get; }
        public IRelayCommand RemoveItemCommand { get; }

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

        private void RefreshCommands()
        {
            AddItemCommand?.NotifyCanExecuteChanged();
            DuplicateItemCommand?.NotifyCanExecuteChanged();
            MoveItemUpCommand?.NotifyCanExecuteChanged();
            MoveItemDownCommand?.NotifyCanExecuteChanged();
            RemoveItemCommand?.NotifyCanExecuteChanged();
            RenameSelectedCommand?.NotifyCanExecuteChanged();
        }

        // A list of the tree changed: a place in a list, and so the commands, may have changed.
        internal void ListChanged()
        {
            RefreshCommands();
            RefreshSearch();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.PropertyName == nameof(SelectedNode))
            {
                // The row of the arrow keys goes to the selected row. When it does not change, the target can still change: the
                // selection goes away while no row of the arrow keys is set.
                var before = cursorNode;
                CursorNode = selectedNode;
                if (ReferenceEquals(before, cursorNode))
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

        public bool IsPathTarget(TreeNode node) => fileDrop.IsTarget(node);

        public bool AcceptsPath(string path) => fileDrop.Accepts(path);

        public bool CanDropPaths(IEnumerable<string> paths) => fileDrop.CanDrop(paths);

        public void DropPaths(TreeNode? hovered, IReadOnlyList<string> paths) => fileDrop.Drop(hovered, paths);

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
            if (cursorNode != null && cursorNode.IsOrIsUnder(node))
                CursorNode = selectedNode != null && !selectedNode.IsOrIsUnder(node) ? selectedNode : null;

            if (selectedNode == null || !selectedNode.IsOrIsUnder(node))
                return;

            Origin = SelectionOrigin.Program;
            SetProperty(ref selectedNode, IsTop(node.Parent) ? null : node.Parent, nameof(SelectedNode));
        }

        public void Dispose() => root.Dispose();
    }

    // What made a selection of the tree.
    public enum SelectionOrigin
    {
        Program,
        Pointer,
        Keyboard,
        Peer
    }
}
