// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    // The commands of the tree. The keys of the main window have the condition of every tree key (CanRunTreeKey); the toolbar does not.
    public sealed partial class ProjectTree
    {
        // ---------------------------------------------------------------- toolbar, on the target node

        // With no add, the command tells only whether the target node has something to add.
        [RelayCommand(CanExecute = nameof(CanAddItem))]
        private void AddItem(TreeAdd? add) => add?.Execute?.Invoke();

        private bool CanAddItem(TreeAdd? add) => add == null ? HasAdds : add.Execute != null;

        [RelayCommand(CanExecute = nameof(HasTargetPlace))]
        private void DuplicateItem()
        {
            if (PlaceOfTarget() is { } place)
                edit.Duplicate(place);
        }

        [RelayCommand(CanExecute = nameof(CanMoveItemUp))]
        private void MoveItemUp()
        {
            if (PlaceOfTarget() is { } place)
                edit.MoveUp(place);
        }

        private bool CanMoveItemUp() => PlaceOfTarget() is { } place && edit.CanMoveUp(place);

        [RelayCommand(CanExecute = nameof(CanMoveItemDown))]
        private void MoveItemDown()
        {
            if (PlaceOfTarget() is { } place)
                edit.MoveDown(place);
        }

        private bool CanMoveItemDown() => PlaceOfTarget() is { } place && edit.CanMoveDown(place);

        [RelayCommand(CanExecute = nameof(HasTargetPlace))]
        private void RemoveItem()
        {
            if (PlaceOfTarget() is { } place)
                edit.Remove(place, TargetNode!.Header);
        }

        private bool HasTargetPlace() => PlaceOfTarget() != null;

        [RelayCommand(CanExecute = nameof(CanRenameSelected))]
        private void RenameSelected()
        {
            if (TargetNode is { Item: not null } node)
                RenameRequested?.Invoke(this, node);
        }

        private bool CanRenameSelected() => TargetNode?.Item != null;

        // ---------------------------------------------------------------- the same edits by key (Tree.Rename, Remove, Duplicate, MoveUp, MoveDown)

        [RelayCommand(CanExecute = nameof(CanRenameSelectedByKey))]
        private void RenameSelectedByKey() => RenameSelected();

        private bool CanRenameSelectedByKey() => CanRunTreeKey() && CanRenameSelected();

        [RelayCommand(CanExecute = nameof(CanRemoveItemByKey))]
        private void RemoveItemByKey() => RemoveItem();

        private bool CanRemoveItemByKey() => CanRunTreeKey() && HasTargetPlace();

        [RelayCommand(CanExecute = nameof(CanDuplicateItemByKey))]
        private void DuplicateItemByKey() => DuplicateItem();

        private bool CanDuplicateItemByKey() => CanRunTreeKey() && HasTargetPlace();

        [RelayCommand(CanExecute = nameof(CanMoveItemUpByKey))]
        private void MoveItemUpByKey() => MoveItemUp();

        private bool CanMoveItemUpByKey() => CanRunTreeKey() && CanMoveItemUp();

        [RelayCommand(CanExecute = nameof(CanMoveItemDownByKey))]
        private void MoveItemDownByKey() => MoveItemDown();

        private bool CanMoveItemDownByKey() => CanRunTreeKey() && CanMoveItemDown();

        // ---------------------------------------------------------------- requests to the active view

        // Tree.Add: the view opens the Add panel on the target row, or offers a composition with no target.
        [RelayCommand(CanExecute = nameof(CanRunTreeKey))]
        private void AddAtTarget() => AddRequested?.Invoke(this, EventArgs.Empty);

        // Tree.Menu: the view opens the menu of the target row, or the menu of the project with no target.
        [RelayCommand(CanExecute = nameof(CanRunTreeKey))]
        private void OpenMenuAtTarget() => MenuRequested?.Invoke(this, EventArgs.Empty);

        // Tree.CollapseOthers: the view runs CloseOtherRows itself, so it can keep the target row still on the screen.
        [RelayCommand(CanExecute = nameof(CanCollapseOthers))]
        private void CollapseOthers() => CollapseOthersRequested?.Invoke(this, EventArgs.Empty);

        private bool CanCollapseOthers() => CanRunTreeKey() && TargetNode != null;

        // Tree.Grab: the row follows the pointer and the arrow keys until a click or Enter puts it down (see TreeGrab).
        [RelayCommand(CanExecute = nameof(CanGrabTarget))]
        private void GrabTarget()
        {
            if (TargetNode is { } node && CanDrag(node))
                GrabRequested?.Invoke(this, node);
        }

        private bool CanGrabTarget() => CanRunTreeKey() && TargetNode is { } node && CanDrag(node);

        // Tree.Reveal: the selected row, the row of the Inspector, comes into the middle of the view.
        [RelayCommand(CanExecute = nameof(CanRevealSelected))]
        public void RevealSelected() => TryRevealSelected();

        private bool CanRevealSelected() => CanRunTreeKey() && selectedNode != null;

        // ---------------------------------------------------------------- cursor row

        // Tree.Inspect sends the cursor row to the inspector. A selection (a click, Enter) never does.
        [RelayCommand(CanExecute = nameof(CanInspectCursor))]
        private void InspectCursor() => Inspect(CursorNode);

        private bool CanInspectCursor() => CursorNode != null && !IsDragging && CanRunTreeKey();

        // Tree.Open: Enter during a drag or a grab places the row (see TreeGrab), so it selects nothing then.
        [RelayCommand(CanExecute = nameof(CanOpenSelected))]
        private void OpenSelected() => Select(CursorNode, SelectionOrigin.Keyboard);

        private bool CanOpenSelected() => CursorNode != null && !IsDragging;

        // ---------------------------------------------------------------- open and close

        // Tree.Expand: during a drag of a row, it acts on the row under the pointer instead (see ToggleDragRow).
        [RelayCommand(CanExecute = nameof(CanToggleExpandTarget))]
        private void ToggleExpandTarget()
        {
            if (IsDragging)
                ToggleDragRow();
            else if (TargetNode is { Children.Count: > 0 } node)
                node.IsExpanded = !node.IsExpanded;
        }

        private bool CanToggleExpandTarget() => IsDragging ? !IsTextBoxFocused() : CanRunTreeKey() && TargetNode is { Children.Count: > 0 };

        // Tree.ExpandBranch
        [RelayCommand(CanExecute = nameof(CanExpandBranch))]
        public void ExpandBranch()
        {
            if (IsDragging || TargetNode is not { Children.Count: > 0 } target)
                return;

            openState.ExpandBranch(target);
        }

        private bool CanExpandBranch() => CanRunTreeKey() && TargetNode is { Children.Count: > 0 };

        // Tree.CloseLevel
        [RelayCommand(CanExecute = nameof(CanRunTreeKey))]
        public void CloseLevel()
        {
            if (IsDragging)
                return;

            openState.CloseLevel(Roots, TargetNode);
        }

        // Tree.OpenLevel
        [RelayCommand(CanExecute = nameof(CanRunTreeKey))]
        public void OpenLevel()
        {
            if (IsDragging)
                return;

            openState.OpenLevel(Roots);
        }

        // Tree.CollapseAll
        [RelayCommand(CanExecute = nameof(CanRunTreeKey))]
        public void CollapseAll()
        {
            if (IsDragging)
                return;

            openState.CollapseAll(Roots, TargetNode);
        }

        // ---------------------------------------------------------------- hide and show

        // Tree.Hide: for a group row it is the hide toggle of the whole list.
        [RelayCommand(CanExecute = nameof(CanToggleHideTarget))]
        public void ToggleHideTarget() => TryToggleHideTarget();

        private bool CanToggleHideTarget() => CanRunTreeKey() && TargetNode?.HiddenValue != null;

        // Tree.ShowAll
        [RelayCommand(CanExecute = nameof(CanRunTreeKey))]
        public void ShowAll() => TryShowAll();

        // ---------------------------------------------------------------- search

        // Tree.Search: the search box of the active view takes the focus.
        [RelayCommand(CanExecute = nameof(CanRunTreeKey))]
        public void FocusSearch()
        {
            if (!IsDragging)
                SearchFocusRequested?.Invoke(this, EventArgs.Empty);
        }

        // Tree.ClearSearch runs from anywhere, also from a text box, and gives the focus to the cursor row.
        [RelayCommand(CanExecute = nameof(CanClearSearch))]
        private void ClearSearch()
        {
            ClearSearchText();
            FocusTree();
        }

        private bool CanClearSearch() => IsSearchActive && !IsDragging;

        // The one condition of every tree key of the main window. The focus comes first: a key runs while the tree has the focus, with
        // the pointer anywhere. It also runs while the pointer is over the tree and no other panel has the focus. It does not run while
        // a text box has the focus: Ctrl+R and the key down of Shift+D do not change the text, so the text box does not take them. It
        // does not run during a drag of a row. A key binding checks the command when the key is pressed, so the condition needs no
        // change event.
        private bool CanRunTreeKey() => !IsDragging && views.AllowsTreeKey();

        private void RefreshCommands()
        {
            AddItemCommand.NotifyCanExecuteChanged();
            DuplicateItemCommand.NotifyCanExecuteChanged();
            MoveItemUpCommand.NotifyCanExecuteChanged();
            MoveItemDownCommand.NotifyCanExecuteChanged();
            RemoveItemCommand.NotifyCanExecuteChanged();
            RenameSelectedCommand.NotifyCanExecuteChanged();
        }
    }
}
