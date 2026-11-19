// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Studio.Avalonia.ViewModels.Tree
{
    // The commands of the tree. The keys of the main window have the condition of every tree key (CanRunTreeKey); the toolbar does not.
    public sealed partial class ProjectTree
    {
        [RelayCommand(CanExecute = nameof(CanAddItem))]
        private void AddItem(TreeAdd? add) => add?.Execute?.Invoke();

        private bool CanAddItem(TreeAdd? add) => add == null || add.Execute != null;

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

        [RelayCommand(CanExecute = nameof(CanRunTreeKey))]
        private void AddAtTarget() => AddRequested?.Invoke(this, EventArgs.Empty);

        [RelayCommand(CanExecute = nameof(CanRunTreeKey))]
        private void OpenMenuAtTarget() => MenuRequested?.Invoke(this, EventArgs.Empty);

        // The view runs CloseOtherRows itself, so it can keep the target row still on the screen.
        [RelayCommand(CanExecute = nameof(CanCollapseOthers))]
        private void CollapseOthers() => CollapseOthersRequested?.Invoke(this, EventArgs.Empty);

        private bool CanCollapseOthers() => CanRunTreeKey() && TargetNode != null;

        [RelayCommand(CanExecute = nameof(CanGrabTarget))]
        private void GrabTarget()
        {
            if (TargetNode is { } node && CanDrag(node))
                GrabRequested?.Invoke(this, node);
        }

        private bool CanGrabTarget() => CanRunTreeKey() && TargetNode is { } node && CanDrag(node);

        [RelayCommand(CanExecute = nameof(CanRevealSelected))]
        public void RevealSelected() => TryRevealSelected();

        private bool CanRevealSelected() => CanRunTreeKey() && selectedNode != null;

        [RelayCommand(CanExecute = nameof(CanInspectCursor))]
        private void InspectCursor() => Inspect(CursorNode);

        private bool CanInspectCursor() => CursorNode != null && !IsDragging && CanRunTreeKey();

        // Enter during a drag or a grab places the row, so it selects nothing then.
        [RelayCommand(CanExecute = nameof(CanOpenSelected))]
        private void OpenSelected() => Select(CursorNode, SelectionOrigin.Keyboard);

        private bool CanOpenSelected() => CursorNode != null && !IsDragging;

        [RelayCommand(CanExecute = nameof(CanToggleExpandTarget))]
        private void ToggleExpandTarget()
        {
            if (IsDragging)
                Views.ToggleDragRow();
            else if (TargetNode is { Children.Count: > 0 } node)
                node.IsExpanded = !node.IsExpanded;
        }

        private bool CanToggleExpandTarget() => IsDragging ? !Views.IsTextBoxFocused() : CanRunTreeKey() && TargetNode is { Children.Count: > 0 };

        [RelayCommand(CanExecute = nameof(CanExpandBranch))]
        public void ExpandBranch()
        {
            if (IsDragging || TargetNode is not { Children.Count: > 0 } target)
                return;

            openState.ExpandBranch(target);
        }

        private bool CanExpandBranch() => CanRunTreeKey() && TargetNode is { Children.Count: > 0 };

        [RelayCommand(CanExecute = nameof(CanRunTreeKey))]
        public void CloseLevel()
        {
            if (IsDragging)
                return;

            openState.CloseLevel(Roots, TargetNode);
        }

        [RelayCommand(CanExecute = nameof(CanRunTreeKey))]
        public void OpenLevel()
        {
            if (IsDragging)
                return;

            openState.OpenLevel(Roots);
        }

        [RelayCommand(CanExecute = nameof(CanRunTreeKey))]
        public void CollapseAll()
        {
            if (IsDragging)
                return;

            openState.CollapseAll(Roots, TargetNode);
        }

        [RelayCommand(CanExecute = nameof(CanToggleHideTarget))]
        public void ToggleHideTarget() => TryToggleHideTarget();

        private bool CanToggleHideTarget() => CanRunTreeKey() && TargetNode?.HiddenValue != null;

        [RelayCommand(CanExecute = nameof(CanRunTreeKey))]
        public void ShowAll() => TryShowAll();

        [RelayCommand(CanExecute = nameof(CanRunTreeKey))]
        public void FocusSearch()
        {
            if (!IsDragging)
                SearchFocusRequested?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand(CanExecute = nameof(CanClearSearch))]
        private void ClearSearch()
        {
            ClearSearchText();
            FocusTree();
        }

        private bool CanClearSearch() => IsSearchActive && !IsDragging;

        // A key binding checks the command when the key is pressed, so this needs no change event.
        // A text box does not take Ctrl+R or the key down of Shift+D, so a focused text box blocks the keys.
        private bool CanRunTreeKey() => !IsDragging && Views.AllowsTreeKey();

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
