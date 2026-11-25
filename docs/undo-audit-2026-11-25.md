# Undo and redo audit, 2026-11-25

Status: `docs/plans/undo-cleanup.md` executed proposals P1 to P8 on 2026-11-25. P9 (F13, F15 is documented, F17, the long comments) stays open. The root cause of the startup steps, `OutputMappingManager` writes values in its constructor, is open too.

An audit of every piece of code that records, groups, applies or drops an undo step. The undo works. This document says where the code is hard to read, where it does the same thing in two ways, and what a simplification can look like. Nothing in this document is a change. Each proposal waits for a decision of the owner.

The audit reads the branch `refactor/tree-structure` at commit `267467903`. The split of `IValueSync` into `Changing`, `Changed` and `Send`, and the `TimeProvider` of `ValueSync`, are in that state.

Every finding was checked a second time against the code. Section 6 lists what the second check changed.

## 1. The map

| Part | Files | Lines |
|---|---|---|
| The stack | `CMiX.Core/Undo/UndoManager.cs` | 192 |
| The step contract | `CMiX.Core/Undo/IUndoCommand.cs` | 11 |
| The list contract | `CMiX.Core/Undo/IItemList.cs` | 41 |
| The list steps | `AddItemUndoCommand`, `RemoveItemUndoCommand`, `ReplaceItemUndoCommand`, `MoveItemUndoCommand`, `SelectItemUndoCommand`, `ClearSelectionUndoCommand` | 230 |
| The value step | `ValueChangedCommand` | 34 |
| The group | `CompositeCommand` | 35 |
| The value source | `ValueSync` (through `IValueSync`), `ValueInteraction`, `IInteractiveValue` | 122 |
| The list source | `PrefabManager` (`Run`, `SelectAt`, `ClearSelection`, `ResetItem`, four captures) | 337 |
| The list that sends | `CollectionSync` implements `IItemList` | 96 |
| The beat sources | `MasterBeat.WithUndo`, `BeatTimedModulatorBase.WithUndo` | 2 x 16 |
| The group callers | `ModulatableValue.SetModulator`, `RedisAddressRegistry.InOneUndoStep`, `PrefabFileDropBehavior`, `TreeEdit.ShowAll`, `TreeFileDropRules.Drop`, `ProjectTree.SelectChain`, `ProjectTree.DropAsync`, `ProjectTree.DropBlockAsync`, `MainWindow.InstallToggleDrag` | 9 sites |
| The suspend callers | `ProjectSyncTarget.ApplySnapshot`, `MasterBeat.WithUndo`, `BeatTimedModulatorBase.WithUndo` | 3 sites |
| The clear callers | `ProjectSession.ClearAll`, `ProjectSyncTarget.ApplySnapshot` | 2 sites |
| The user entry | `MainViewModel.UndoCommand`, `RedoCommand`, `MainWindow.axaml` key bindings, `MenuMain.axaml`, `KeyBindingTextGuard` | |
| The notices | `IStatusNotice.ShowWithUndo`, `Notification.UndoGesture`, `TreeKeyHints` | |

Ownership of a removed control: `RemoveItemUndoCommand`, `AddItemUndoCommand` and `ReplaceItemUndoCommand` hold the control while they sit on a stack. `UndoManager.DropCommand` disposes the command when the stack drops it, and `IItemList.Release` calls `OwnedControl.DisposeIfOrphaned`, which checks the repository first. This is correct and it is tested (`Dispose_...` tests of the three commands, `RemoveItemTests`, `SharedItemTests`, `ResetItemTests`).

Tests: `UndoManagerTests` has 11 tests. The six step classes have 21 tests on `FakeItemList`. 19 Core test files and 22 Studio test files call `Undo`, `Redo` or count the steps. The Studio tests count the steps through reflection into the private fields (`Support/UndoCounts`).

## 2. What is correct and stays

- One stack, one gate. Every edit goes through the same code when the user does it and when the undo applies it, so the engine sees the undo as a normal edit. `IsApplying` stops the second recording. This is the right design for a synced project.
- The ownership rule of a removed control is in one place (`OwnedControl`) and the three owning commands follow it.
- The capture nests, and `RevertCapture` gives a block drop the "all or nothing" it needs.
- `IsApplying` and the suspend depth come back in a `finally`. The tests pin this.
- The drag records one step at the end (`ValueInteraction`, `IInteractiveValue`). The 500 ms merge window stays for writes outside a drag.

## 3. Findings

Each finding has a number. A proposal in section 4 refers to the findings it answers.

### F1. The stack has three gates, and the callers check one of them by hand

`UndoManager` has three ways to stop a recording: `SuppressUndo` (a depth), a capture (a list), and `IsApplying` (a flag). `Push` honors the first two. It does not honor `IsApplying`. So every source checks `IsApplying` itself:

- `ValueSync.Record`: `if (!_undoManager.IsApplying)`.
- `PrefabManager.DuplicateItem`, `ResetItem`, `MoveItemToManager`: `if (UndoManager?.IsApplying == true) return;`.
- `MasterBeat.WithUndo` and `BeatTimedModulatorBase.WithUndo`: no check. None is needed today: an undo of a beat step calls `FromModel`, which writes the values through `ValueSync`, and `ValueSync` has the check. `WithUndo` itself runs only from a user action.

A reader has to know which gate a path uses and why a path needs none. One gate in `Push` makes every source the same.

### F2. `WithUndo` exists twice and is a third way to group

`MasterBeat.WithUndo` and `BeatTimedModulatorBase.WithUndo` are the same 16 lines. They suspend the stack, run the action, and push one `ValueChangedCommand` of the whole control. This is a group of value steps, as a capture is, but made with the suspend gate instead of the capture. The suspend gate is global: while the action runs, nothing else in the application records.

A capture does not replace it. `MasterBeatModel` holds the `Periods` table, and `Tap` and the `BPM` setter regenerate the table. One `ValueChangedCommand` of the whole beat restores the table through `FromModel`. A `CompositeCommand` of the inner value steps restores `Index`, `Period` and `BeatIndex` and leaves the table as it is, so the modulators that read `MasterBeat.Periods` would see a stale table after an undo. The two copies can still become one method, with the same step on the stack.

### F3. The capture needs a `try` and a `finally` at every site

Nine sites write `BeginCapture(); try { ... } finally { EndCapture(); }`. `RedisAddressRegistry.InOneUndoStep` is a local helper for the same five lines. `MainViewModel` exposes `BeginUndoCaptureCommand` and `EndUndoCaptureCommand` so that the window can do the same from a behavior. `ReceivableControl.SuppressSending` already shows the shape that fits: a `using` scope.

### F4. `UndoManager` is optional in 23 places, and never null

`ReceivableControl.UndoManager` is a settable property on every control. The DI constructor of `PrefabManagerBase` sets it. Then `Project`, `ProjectSession.SetupList` and `RedisAddressRegistry` set it again to the same singleton. No test constructs a `PrefabManager` without it. Still `PrefabManager` (13), `MasterBeat` (3), `BeatTimedModulatorBase` (3), `RedisAddressRegistry` (2) and `PrefabFileDropBehavior` (2) write `UndoManager?.`. `ARCHITECTURE.md` lists this as debt (step 9 of its plan).

`GenericValue` inherits the property and never reads it. The property belongs to the few classes that push.

### F5. The names of two steps do not follow the rule

`.claude/CLAUDE.md`: an undo step is `...UndoCommand`. `ValueChangedCommand` and `CompositeCommand` are undo steps. `ARCHITECTURE.md` line 326 already proposes `ValueChangedUndoCommand` and `CompositeUndoCommand`. A search for `UndoCommand` today misses the two most common steps on the stack. The VL patches (`CMiX.Engine/*.vl`, `CMiX.Extensions.vl`, `dl_pm.vl`) name neither class, nor `UndoManager` nor `IItemList`.

### F6. `ValueChangedCommand.WithBefore` changes the object and returns it

The name reads as a copy. `UndoManager.Push` writes `lastNode.Value = vc.WithBefore(lastVc.Before)`, which assigns the same object back. A method `MergeBefore(IControlModel before)` with no return, or a settable `Before`, says what happens.

### F7. The merge uses `DateTime.Now`

The merge window compares `DateTime.Now` of two steps. `ValueSync` takes a `TimeProvider` since commit `267467903`. The merge is the second clock of the undo and it stays on the wall clock, so no test can drive it. The test `MergeWindow_TwoPushesOnSameControl_CollapseToOneUndoStep` passes because two pushes take less than 500 ms on any machine.

### F8. The merge also merges clicks

The merge window keys on `ControlID`. `MasterBeat.WithUndo` pushes a `ValueChangedCommand` with the id of the beat. Two `Multiply` clicks within 500 ms become one undo step. Two `Tap` clicks become one. A single captured step (`EndCapture` with one command calls `Push`) can merge into the step before the capture. This may be wanted for a tap tempo. It is not written anywhere. A decision of the owner: keep and document, or merge only steps that come from `ValueSync`.

### F9. `IsSuppressed` has no caller in the application

Only two asserts in `ProjectSnapshotTests` read it. It is public API for a test.

### F10. The capture state is three fields where one is enough

`_captureList`, `_captureDepth` and `_captureStarts` describe one thing. `_captureDepth` is always `_captureStarts.Count`. `EndCapture` checks `_captureList == null` after the depth check, which cannot be true. The three fields are declared between `Push` and `BeginCapture`, after the methods that use them.

### F11. The tests read the private stacks by reflection

`Support/UndoCounts` reads `_undoStack` and `_redoStack` by name. The tests audit of 2026-11-24 already asks for a public count. A count also serves the menu: `UndoCommand` and `RedoCommand` are `RelayCommand` with no `CanExecute`, so the Edit menu never greys out Undo or Redo. A `Changed` event on the stack lets the two commands raise `CanExecuteChanged`.

### F12. `MaxSteps` is private and a test repeats the number

`TrimStack_OverMaxSteps_DropsTheOldestEntriesFirst` writes `const int maxSteps = 128`. A public constant removes the copy.

### F13. Two selection steps are one step with a special index

`SelectItemUndoCommand(list, previousIndex, newIndex)` and `ClearSelectionUndoCommand(list, previousIndex)` differ only in that the second has `newIndex = -1`. `IItemList.SelectAt(-1)` already clears. One class with the guard `index < 0 ? ClearSelection() : SelectAt(index)` serves both. Against it: the naming rule asks for one chain per verb (`ClearSelection` has its `UndoCommand`, its message and its sender), and `MessageClearSelection` stays on the wire either way. The owner decides which rule wins.

### F14. `AddItemUndoCommand.Undo` and `RemoveItemUndoCommand.Undo` restore the selection in two styles

`AddItemUndoCommand` keeps `_previousIndex` and checks `IndexOf(_previousItem) >= 0` before it selects the stored index. `RemoveItemUndoCommand` selects `IndexOf(_selectedBefore)`, which is `-1` and clears when nothing was selected, so the restored item is first selected by `Insert` and then unselected by `SelectAt(-1)`: two messages for one undo. The tests pin both behaviors. One style for the two, with the `IndexOf` at undo time, reads better and sends one message.

### F15. `PrefabManager.SelectAt` records no step when nothing was selected before

A selection from nothing to an item is not an undo step. A clear from an item to nothing is. `SelectItemUndoCommand.Undo` has the guard for it. This looks intended (the first click in an empty inspector is not an edit). It is not written down.

### F16. `ResetItem` repeats `Run`

`ResetItem` writes `command.Execute(); UndoManager?.Push(command);`, which is `Run(command)`.

### F17. `IItemList` lives in the `Undo` folder

`IItemList` is the list that sends every change. `CollectionSync` in `Prefabs/Managers` implements it. The undo steps use it, and so does `PrefabManager`. It is the contract of a synced list, not a part of the undo. Low priority. No VL patch names it, so a move is safe for the patches.

### F18. Comments and headers

- `ReplaceItemUndoCommand` has an 11 line comment, `UndoManager` has 14 comment lines, `RemoveItemUndoCommand` and `AddItemUndoCommand` carry a paragraph each. `.claude/CLAUDE.md` asks for brief comments. The memory of the owner says: trim only when the file is touched.
- `Project.cs` line 26: `// ← set here`. An arrow and a note with no content.
- `AddItemUndoCommand.cs`, `PrefabManager.cs` and `PrefabManagerBase.cs` have no SPDX header. Every other file of the chain has one.

### F19. Test gaps

- `CompositeCommand.Dispose` has no direct test. `SharedItemTests` and `RemoveItemTests` cover it through `RemoveEverywhere`.
- `SuppressUndo` nesting has no test. `ProjectFileCompatibilityTests` calls it, but nothing asserts a nested resume.
- The block drop that the list refuses: `ListTreeMoveBlockTests.DropBlockAsync_ABlockWithARowThatTheListRefuses_MovesNoRow` asserts the two lists and nothing else. No test asserts an empty stack after `RevertCapture` on a real tree. `RevertCapture` is tested only in `UndoManagerTests` (the tests audit, item 5 of its test gaps).
- `MoveItemUp` and `MoveItemDown` live in `ManagerReorderService` and in `ProjectTree.Commands`, not in `PrefabManager`. `ReorderThroughManagerTests` tests the service: one step, one message, stop at the edges. The tests audit names `PrefabManager.MoveItemUpCommand` and calls it untested. That line is out of date.

## 4. Proposals

Each proposal is one commit or one short series. The complexity follows the plan rule of `.claude/CLAUDE.md`.

### P1. One gate in `Push` (F1)

`Push` returns when `IsApplying`. The three hand checks in `PrefabManager` stay where they stop the action itself (`DuplicateItem`, `ResetItem`, `MoveItemToManager` must not run during an apply). The check in `ValueSync.Record` goes.

Before:

```csharp
private void Record(IControlModel before, IControlModel after)
{
    if (!_undoManager.IsApplying)
        _undoManager.Push(new ValueChangedCommand(_value, before, after));
}
```

After:

```csharp
private void Record(IControlModel before, IControlModel after) =>
    _undoManager.Push(new ValueChangedCommand(_value, before, after));
```

and in `UndoManager`:

```csharp
public void Push(IUndoCommand command)
{
    if (IsApplying || _groupDepth > 0) return;
```

Complexity 2. Tests: `UndoManagerTests`, `ValueSyncTests`.

### P2. Scopes for the capture and the suspend (F3, F10)

Two `using` scopes, as `SuppressSending` does. The scope ends the capture or the suspend in its `Dispose`. `RevertCapture` becomes a method of the capture scope.

Before (nine sites):

```csharp
undoManager.BeginCapture();
try
{
    foreach (var value in hidden)
        value!.Value = false;
}
finally
{
    undoManager.EndCapture();
}
```

After:

```csharp
using (undoManager.Capture())
{
    foreach (var value in hidden)
        value!.Value = false;
}
```

The block drop:

```csharp
using var capture = undoManager.Capture();
move = drop.MoveBlock(root, nodes, target, position);
if (move.Refusal != null && move.MovedNodes.Count > 0)
    capture.Revert();
```

`MainViewModel.BeginUndoCaptureCommand` and `EndUndoCaptureCommand` stay for the toggle drag of the window, because a stroke spans two events. `RedisAddressRegistry.InOneUndoStep` goes. The capture state becomes one stack of lists, or one list and one stack of start indexes. Complexity 3. Tests: `UndoManagerTests`, `ToggleDragTests`, `ListTreeBlockDragTests`, `TreeKeyCommandsTests`, `RedisAddressTests`.

### P3. One `WithUndo` for the beat and the beat modulators (F2)

The step stays one `ValueChangedCommand` of the whole control, because `MasterBeat` restores its `Periods` table through `FromModel` (see F2). Only the copy goes: the method moves to `UndoManager`, with the suspend scope of P2.

Before (twice):

```csharp
private void WithUndo(Action action)
{
    var before = ToModel();
    UndoManager?.SuppressUndo();
    try { action(); }
    finally { UndoManager?.ResumeUndo(); }
    UndoManager?.Push(new ValueChangedCommand(this, before, ToModel()));
}
```

After, once in `UndoManager`:

```csharp
// Runs the action as one step of the control. The value steps inside it are not recorded.
public void RecordAsOneStep(IControl control, Action action)
{
    var before = control.ToModel();
    using (Suspend())
        action();
    Push(new ValueChangedCommand(control, before, control.ToModel()));
}
```

and in the two classes: `public void Tap() => UndoManager.RecordAsOneStep(this, () => ...)`. The merge of two clicks (F8) does not change. Complexity 2. Tests: the beat tests of `ModifierTests` and the modulator tests.

### P4. `UndoManager` is required (F4)

`PrefabManager`, `MasterBeat` and the modulators take it in the constructor today. The property on `ReceivableControl` goes, or becomes `protected` and `init`. The three re-sets in `Project`, `ProjectSession` and `RedisAddressRegistry` go. The 23 `UndoManager?.` become `UndoManager.`. `PrefabFileDropBehavior` reads `PrefabManager.UndoManager`; it keeps a public getter or takes the manager from the view model.

Before:

```csharp
compositionManager.UndoManager = undoManager;  // ← set here
```

After: the line goes. Complexity 2. Tests: the Core suite, `EntityDropBehaviorTests`, `TextureDropBehaviorTests`.

### P5. Rename the two steps (F5)

`ValueChangedCommand` becomes `ValueChangedUndoCommand`. `CompositeCommand` becomes `CompositeUndoCommand`. Whole word replace in `.cs`, `.md`. No VL patch names them (check with `git grep` in both repositories first, as the rename rule says). Complexity 1. One commit per rename.

### P6. Count, event and limit on the stack (F9, F11, F12)

```csharp
public const int MaxSteps = 128;
public int StepCount => _undoStack.Count;
public int RedoStepCount => _redoStack.Count;
public event Action Changed;
```

`Changed` fires after every push, undo, redo, clear and trim. `MainViewModel` makes `UndoCommand = new RelayCommand(undoManager.Undo, () => undoManager.CanUndo)` and raises `NotifyCanExecuteChanged` on the event. `UndoCounts` reads the counts and loses the reflection. `IsSuppressed` goes, and the two asserts read `StepCount` after a push instead. Complexity 2. Tests: `UndoManagerTests`, `MenuMainBindingTests`, every Studio test that uses `UndoCounts`.

### P7. The merge (F6, F7, F8)

`WithBefore` becomes a plain write. The timestamp comes from the `TimeProvider` that `ValueSync` already takes, so the test can drive the window. Which steps merge is a decision of the owner (F8). Complexity 2.

### P8. Small cleanups (F14, F16, F18)

One commit each: `ResetItem` calls `Run`; `AddItemUndoCommand.Undo` and `RemoveItemUndoCommand.Undo` select by `IndexOf` at undo time and send nothing when nothing was selected; the three SPDX headers; the arrow comment in `Project.cs`. Complexity 1.

### P9. Held for a decision

- F13, one selection step or two.
- F15, write the rule of the first selection in `ARCHITECTURE.md`.
- F17, the home of `IItemList`.
- F18, the long comments, when the files are touched.
- F19, the three test gaps, and the out of date line of the tests audit.

## 5. Order

1. P5 (renames, no behavior change) and P8 (small cleanups).
2. P4 (required manager), then P1 (one gate), then P6 (count, event, limit).
3. P2 (scopes), then P3 (one `WithUndo`).
4. P7 (the merge) after the decision on F8.

Each step builds and runs the Core suite and the Studio tests it names. A code review of the chain comes after step 3, when the code has one shape.

## 6. The second check

What the second check against the code confirmed:

- `Push` honors the suspend depth and the capture, not `IsApplying`. `ValueSync.Record` checks `IsApplying` itself. (F1)
- `EndCapture` with one captured step calls `Push`, so the step can merge into the step before the capture. Two `Tap` or two `Multiply` calls within 500 ms merge, because both push a step with the id of the beat. (F8)
- No `new PrefabManager(` and no `UndoManager = null` in any project or test. The manager always comes from the container. (F4)
- `RemoveItemUndoCommand.Undo` with nothing selected before calls `SelectAt(-1)`; the test `Undo_WithNothingSelectedBefore_SelectsNothing` pins the call. On `CollectionSync` that is one `MessageAddItem` and one `MessageSelectItem`. (F14)
- `UndoCommand` and `RedoCommand` are `RelayCommand` with no `CanExecute`. No test reads their `CanExecute`. (F11)
- `SyncPeer.ApplyDecoded` and `ApplySnapshotOnDispatcher` run on the dispatcher thread, so the stack has no second thread.

What the second check changed:

- F1: the beat classes do not work "by accident". Their undo path is `FromModel`, which `ValueSync` gates. The wording was wrong.
- F2 and P3: a capture cannot replace `WithUndo` on `MasterBeat`, because the `Periods` table is in the model and not in a value. The proposal now keeps one `ValueChangedCommand` per action and removes only the copy.
- F5 and F17: the VL patches name none of the undo types. The renames and the move need no patch change.
- F7 and P7: the `TimeProvider` is committed, not in progress.
- F19: the refused block test is `ListTreeMoveBlockTests.DropBlockAsync_ABlockWithARowThatTheListRefuses_MovesNoRow`. `MoveItemUp` lives in `ManagerReorderService`, not in `PrefabManager`.
