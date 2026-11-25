# Plan: undo follow ups

Status: built on 2026-11-25, eleven commits from `9fa05f8cb` to `35325912f`. Step 2.3 also made `PrefabManager.SelectAt` and `ClearSelection` record through `Run`, because the base takes the selection before in `Execute`. The three defaults D1 to D3 stand.

Three items that the undo review of 2026-11-25 left open, plus two stale document lines. The plan follows `docs/plans/undo-cleanup.md`: one commit per step, the build and the suites pass at every commit, complexity 1 goes to a Sonnet subagent, 2 to 4 to an Opus subagent.

Branch: `refactor/tree-structure`. Another session writes in this checkout; every commit stages by path, and the file list of each step goes to that session before the step starts.

## The three items

### Item 1. The output mapping writes its defaults as edits

`OutputMappingManager` builds nine `OutputMapping` slots with `ControlFactory.Create`. `Create` ends with `ActivateAll`, so every value of a slot is active when it returns. The constructor then calls `ResetSlot(i)`, which writes `Name`, `Resolution.X`, `Resolution.Y` and `TexcoordSemantic` through the active setters. Each write is an edit: `ValueSync` records an undo step and sends a message. That is 17 steps and 36 messages before a project exists. `ProjectSession.Start` clears the steps today; the messages still go out, and a host without a session keeps the steps.

The cause is the order: the ids and the values come after the activation. The other fixed control of the project, `MasterBeat`, shows the right order: it calls `FromModel(new MasterBeatModel())` while its values are inactive, and activates after.

The fix: a slot is created from a model that already carries its fixed ids and its default values. `Create(model)` calls `FromModel` before `ActivateAll`, so the values land without an edit. One method gives that model, and `Reset` uses the same method, so the defaults have one source.

Before:

```csharp
for (int i = 0; i < SlotCount; i++)
    items.Add((OutputMapping)controlFactory.Create(typeof(OutputMapping)));
Items = items;

for (int i = 0; i < SlotCount; i++)
    ResetSlot(i);
```

After:

```csharp
for (int i = 0; i < SlotCount; i++)
    items.Add((OutputMapping)controlFactory.Create(SlotModel(i)));
Items = items;
```

with

```csharp
// The model of a new slot: the fixed ids of the slot and the values of a new project.
private static OutputMappingModel SlotModel(int i)
{
    var ids = ManagerIDs.OutputMappingSlotFields[i];
    return new OutputMappingModel
    {
        ID = ManagerIDs.OutputMappingSlots[i],
        Name = new GenericValueModel<string>($"Output {i + 1}") { ID = ids.Name },
        Resolution = new Integer2Model(1920, 1080) { ID = ids.Resolution, X = ..., Y = ... },
        TexcoordSemantic = new GenericValueModel<TexcoordSemantic>((TexcoordSemantic)i) { ID = ids.TexcoordSemantic }
    };
}
```

`OutputMapping.FromModel` leaves the slot id untouched today, so the constructor still sets `mapping.ID` once after `Create`, or `FromModel` takes the id from the model (check the comment in the file: it protects a load from an older file, so the load path keeps `AssignFixedIds`). `Reset` becomes `Items[i].FromModel(SlotModel(i))`: a user action, so its writes send and record, and `ProjectSession.ClearAll` clears the stack after it as today. `ResetSlot` goes. `AssignFixedIds` stays for the load path only.

Check the shape of `Integer2Model` and `GenericValueModel<T>` first: both need an `ID` that `init` can set. If a model has no settable id, the step adds it.

### Item 2. Ctrl+Z returns to the exact state, selection included

Six steps change a list: add, remove, replace, move, select, clear selection. Each one restores the selection on `Undo` in its own way:

| Step | Selection on Undo today |
|---|---|
| `AddItemUndoCommand` | Selects the item selected before, if it is still in the list. Nothing selected before: the neighbour that `Remove` selects stays. |
| `RemoveItemUndoCommand` | Selects the item selected before, if it is still in the list. Nothing selected before: the restored item stays selected. |
| `ReplaceItemUndoCommand` | Nothing. The incoming item is selected because `Insert` selects. |
| `MoveItemUndoCommand` | Nothing. `ManagerData.Move` keeps the selection. |
| `SelectItemUndoCommand` | Selects the previous index. |
| `ClearSelectionUndoCommand` | Selects the previous index. |

And no step restores the selection on `Redo`: the redo gives the selection that the list change happens to give.

The rule the owner wants: an undo returns the list to the exact state before the action, and a redo returns it to the exact state after the action. The selection is part of that state. The rule belongs in one place, not in six.

The design: one base class for the list steps. It takes the selection before the change and after the change, and it restores them. A step says only what it changes and how it changes it back.

```csharp
// A step that changes a list. Undo and Redo restore the selection of the list exactly.
public abstract class ItemListUndoCommand : IUndoCommand
{
    private IControl _selectedBefore;
    private IControl _selectedAfter;
    private bool _executedOnce;

    protected ItemListUndoCommand(IItemList list) => List = list;

    protected IItemList List { get; }

    public void Execute()
    {
        if (_executedOnce)
        {
            Change();
            RestoreSelection(_selectedAfter);
            return;
        }

        _selectedBefore = List.SelectedItem;
        Change();
        _selectedAfter = List.SelectedItem;
        _executedOnce = true;
    }

    public void Undo()
    {
        ChangeBack();
        RestoreSelection(_selectedBefore);
    }

    // The list change of the step.
    protected abstract void Change();

    // The inverse list change.
    protected abstract void ChangeBack();

    // Selects the item, or nothing, only when the list does not show it already. So a restore sends at most one message.
    private void RestoreSelection(IControl item)
    {
        if (ReferenceEquals(List.SelectedItem, item)) return;
        if (item == null) { List.ClearSelection(); return; }
        var index = List.IndexOf(item);
        if (index >= 0) List.SelectAt(index);
    }
}
```

The six steps on the base:

| Step | `Change` | `ChangeBack` | What goes |
|---|---|---|---|
| `AddItemUndoCommand` | `Insert(item, index)` | `Remove(item)` | `_previousItem`, the selection code |
| `RemoveItemUndoCommand` | `_index = Remove(item)` | `Insert(item, _index)` | `_selectedBefore`, the selection code |
| `ReplaceItemUndoCommand` | `Replace(previous, new)` | `Replace(new, previous)` | nothing, the comment shrinks to two lines |
| `MoveItemUndoCommand` | `Move(old, new)` | `Move(new, old)` | nothing |
| `SelectItemUndoCommand` | `SelectAt(newIndex)` | nothing | `_previousIndex`, the two guards |
| `ClearSelectionUndoCommand` | `ClearSelection()` | nothing | `_previousIndex` |

The two selection steps keep their names: the verb chain of `.claude/CLAUDE.md` (`ClearSelection`, `ClearSelectionUndoCommand`, `MessageClearSelection`, `SendClearSelection`) stays searchable. Each is seven lines on the base. The question of the audit (F13, one step or two) closes with "two".

Before, in `RemoveItemUndoCommand`:

```csharp
public void Undo()
{
    _list.Insert(_item, _index);
    if (ReferenceEquals(_selectedBefore, _item)) return;
    var index = _list.IndexOf(_selectedBefore);
    if (index >= 0) _list.SelectAt(index);
}
```

After:

```csharp
protected override void ChangeBack() => List.Insert(_item, _index);
```

The wire: `RestoreSelection` sends one `MessageSelectItem` or one `MessageClearSelection` only when the selection differs from the one the list change gave. The engine follows the message as it does today. The ownership of a removed item (`IDisposable`, `Release`) does not change.

The rule of the first selection stays as it is (`PrefabManager.SelectAt` records no step when nothing was selected before). It is a separate decision, D1 below.

### Item 3. Two test names

`MergeWindow_TwoPushesOnSameControl_CollapseToOneUndoStep` and `MergeWindow_TwoPushesOutsideTheWindow_StayTwoSteps` in `UndoManagerTests` start with a concept, not a method. They become `Push_TwoValueWritesOfOneControlInsideTheWindow_MergeToOneStep` and `Push_TwoValueWritesOutsideTheWindow_StayTwoSteps`.

## Decisions taken as assumptions

| Id | Decision | Default |
|---|---|---|
| D1 | Is the first selection of a list (from nothing to an item) an undo step? | No, as today. With a yes, `PrefabManager.SelectAt` drops its `previousItem == null` guard in step 2.4, and `ARCHITECTURE.md` changes its line. |
| D2 | Does `Redo` restore the selection after the action exactly? | Yes. The base keeps the selection after the first `Execute` and restores it on every later `Execute`. |
| D3 | Does a restore that changes nothing send a message? | No. `RestoreSelection` returns when the list already shows the selection. |

## Phase 1: housekeeping

### Step 1.1 Status lines in two documents

Complexity 1. `docs/undo-audit-2026-11-25.md`: one line in the status block: since commit `bf3d27778` a drag belongs to the value (`IDraggable`, `ValueSync.BeginInteraction`); `ValueInteraction` and `IInteractiveValue` in the map and in section 2 are the names of 2026-11-25. `docs/tests-audit-2026-11-24.md`: one line at the top: `ValueInteraction` is gone since `bf3d27778` and the Core suite runs in parallel since `07faee6ad`, so the point of line 193 is closed. Commit: `Marks the removed drag types in the undo and the tests audit`. No build.

### Step 1.2 Rename the two merge tests

Complexity 1. Item 3. `UndoManagerTests.cs` only. Commit: `Names the merge tests after Push`. Tests: `UndoManagerTests`.

## Phase 2: exact selection (item 2)

### Step 2.1 The base class

Complexity 3. New file `CMiX.Core/Undo/ItemListUndoCommand.cs` as above, with the SPDX header. New test file `CMiX.Core.Tests/Undo/ItemListUndoCommandTests.cs` on `FakeItemList` with a small private step that inserts an item: `Undo_WithNothingSelectedBefore_SelectsNothing`, `Undo_WithAnotherItemSelectedBefore_SelectsItAgain`, `Execute_ASecondTime_RestoresTheSelectionAfterTheFirstExecute`, `Undo_WhenTheListAlreadyShowsTheSelection_SendsNoSelectCall` (the `Calls` list of the fake has no `SelectAt`). No step uses the base yet. Commit: `Adds ItemListUndoCommand, the list step that restores the selection`.

### Step 2.2 Add, remove and replace on the base

Complexity 3. `AddItemUndoCommand`, `RemoveItemUndoCommand`, `ReplaceItemUndoCommand`. The comment of `ReplaceItemUndoCommand` shrinks to two lines (the ownership of the two items; the swap travels as a remove and an add). The tests of the three commands change where the calls differ: `RemoveItemUndoCommandTests.Undo_WithNothingSelectedBefore_KeepsTheRestoredItemSelected` becomes `..._SelectsNothingAgain` and expects `Remove`, `Insert 0`, `ClearSelection`; `AddItemUndoCommandTests.Undo_WithNothingSelectedBefore_SelectsNothingNew` expects a `ClearSelection` call when `Remove` moved the selection to a neighbour. Each file gets one redo test: `Execute_AfterUndo_RestoresTheSelectionOfTheFirstExecute`. Commit: `Restores the selection exactly after an undo or a redo of an add, a remove and a replace`. Tests: `CMiX.Core.Tests/Undo`, `RemoveItemTests`, `ResetItemTests`, `DuplicateItemTests`, `SharedItemTests`, `MoveItemTests`; Studio: `PanelInspectorSelectionTests`, `TreeKeyCommandsTests`, `RemoveNotificationTests`, `ProjectTreeTests`, `RepositoryRowMenuTests`, `SlotRowMenuTests`, `SelectionEchoTests`, `ListTreeBlockDragTests`, `ListTreeMoveBlockTests`, `ListTreeRowDragTests`. A Studio test that expects the old selection after an undo is a decision of the owner, not a test change: the step stops and reports it.

### Step 2.3 Move and the two selection steps on the base

Complexity 2. `MoveItemUndoCommand`, `SelectItemUndoCommand`, `ClearSelectionUndoCommand`. The guards of `SelectItemUndoCommand` go; `RestoreSelection` has the only guard. Tests of the three files follow. Commit: `Puts the move and the selection steps on ItemListUndoCommand`. Tests: `CMiX.Core.Tests/Undo`, `SelectionTests`, `MoveItemTests`; Studio: `SelectionEchoTests`, `ReorderThroughManagerTests`.

### Step 2.4 The documents

Complexity 1. `ARCHITECTURE.md`, section "Undo and redo": one line: a list step restores the selection exactly on undo and on redo (`ItemListUndoCommand`), and sends a selection message only when the selection differs. The line on the first selection stays (D1). `docs/undo-audit-2026-11-25.md` status block: F13 and F14 closed by this plan. Commit: `Describes the selection rule of the list steps`.

## Phase 3: the output mapping defaults (item 1)

### Step 3.1 The slot model

Complexity 3. `OutputMappingManager`: `SlotModel(i)` as above; the constructor creates each slot from it; `Reset` applies it with `FromModel`; `ResetSlot` goes; `AssignFixedIds` stays for `FromModel` of the manager (the load path). Check `OutputMappingModel`, `Integer2Model` and `GenericValueModel<T>` for a settable `ID`; add `init` where it is missing, in this step, and say so. Tests: the existing output mapping tests (`git grep -l OutputMapping -- '*Tests/*.cs'`), plus `OutputMappingManagerTests.Create_ANewManager_RecordsNoUndoStepAndSendsNothing` (resolve the manager from a provider with a `RecordingMessageSender` registered before, assert `StepCount` is 0 and `Sent` is empty) and `Reset_AfterAChange_PutsTheDefaultsBack`. Commit: `Builds the output mapping slots from a model so the defaults are no edit`.

### Step 3.2 Remove the clear at the session start

Complexity 2. `ProjectSession.Start` loses `_undoManager.Clear()` and its comment. `ProjectSessionTests.Create_ANewSession_LeavesTheUndoStackEmpty` must still pass: it is the proof that the cause is gone. If the stack is not empty, the step reports where the steps come from and stops. `ARCHITECTURE.md` loses the sentence "The session start clears the stack too, because the construction of the project writes default values." Commit: `Removes the clear of the undo stack at the session start`. Tests: `ProjectSessionTests`, `MenuMainBindingTests`, `OpenProjectTests`.

## End of the plan

The Core suite twice with details captured, the Studio suite once, the UI suite once. Then the status lines of this plan and of the audit.

## Order and file lists for the other session

| Step | Files |
|---|---|
| 1.1 | `docs/undo-audit-2026-11-25.md`, `docs/tests-audit-2026-11-24.md` |
| 1.2 | `CMiX.Core.Tests/UndoManagerTests.cs` |
| 2.1 | `CMiX.Core/Undo/ItemListUndoCommand.cs` (new), `CMiX.Core.Tests/Undo/ItemListUndoCommandTests.cs` (new) |
| 2.2 | `CMiX.Core/Undo/AddItemUndoCommand.cs`, `RemoveItemUndoCommand.cs`, `ReplaceItemUndoCommand.cs`, their tests |
| 2.3 | `CMiX.Core/Undo/MoveItemUndoCommand.cs`, `SelectItemUndoCommand.cs`, `ClearSelectionUndoCommand.cs`, their tests |
| 2.4 | `ARCHITECTURE.md`, `docs/undo-audit-2026-11-25.md` |
| 3.1 | `CMiX.Core/Rendering/OutputMappingManager.cs`, maybe `OutputMappingModel.cs`, `Integer2Model.cs`, `GenericValueModel.cs`; a new test file |
| 3.2 | `CMiX.Studio.Avalonia/Session/ProjectSession.cs`, `ARCHITECTURE.md` |
