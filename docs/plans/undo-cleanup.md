# Plan: undo cleanup

Status: phases 1 to 4 built on 2026-11-25, 20 commits from `5546ae63a` to `e2a0aa784`. Phase 5, the review, is open. Two steps changed on the way: 2.5 got a fix of its own first (`e3b48d67c`, the session start clears 17 steps that `OutputMappingManager` writes), and 4.2 added no clear test because `RemoveItemTests.Clear_OfTheUndoEntryOfARemove_TearsTheLayerDown` pins it.

The plan executes the proposals of `docs/undo-audit-2026-11-25.md`. The undo keeps its design: one stack, one gate, every step undoes itself through the code of a normal edit. The plan removes the second way of doing each thing, makes the manager required, and gives the stack a public count.

Branch: `refactor/tree-structure`. One commit per step. The build and the three suites pass at every commit.

## Decisions taken as assumptions

The owner can change any of them before phase 1 starts.

| Id | Decision | Default |
|---|---|---|
| D1 | Two beat clicks within 500 ms are one undo step (F8). | Keep. Write the rule into `ARCHITECTURE.md`. |
| D2 | One selection step or two (F13). | Keep two. Not in this plan. |
| D3 | The home of `IItemList` (F17). | Stays in `CMiX.Core/Undo`. Not in this plan. |
| D4 | Where `PrefabFileDropBehavior` gets the undo manager after P4. | `PrefabManagerBase.UndoManager` stays public, get only. |
| D5 | The long comments of the undo folder (F18). | Trimmed only in the files a step touches, to one line each. |

## How a step runs

- Complexity 1 goes to a Sonnet subagent, 2 to 4 to an Opus subagent, 5 to the main agent. The subagent is `cmix-step-executor` with the model of the complexity.
- Every brief carries the rules of `.claude/CLAUDE.md` and of the agent, the step text, the files, the commit message and the test filter.
- A subagent that finds a problem that changes another step stops and asks.
- The main agent checks every commit before the next step: the message, no trailer, only the files of the step, no em or en dash, comments in Simplified Technical English, `git diff --stat` matches the step.
- Build once per phase. The test filter of a step names the classes it touches. At the end of a phase: the Core suite without Redis and the Studio suite.
- The Studio runs from the owner's machine. A build of the Studio project while the Studio runs fails on the locked exe. Build the test projects, not the Studio exe, or build in a scratch copy.

Commands:

```powershell
dotnet build CMiX.Core.Tests\CMiX.Core.Tests.csproj
```

```powershell
dotnet test CMiX.Core.Tests\CMiX.Core.Tests.csproj --filter "Category!=Redis" --blame-hang --blame-hang-timeout 3m --blame-hang-dump-type none
```

```powershell
dotnet test CMiX.Studio.Avalonia.Tests\CMiX.Studio.Avalonia.Tests.csproj --blame-hang --blame-hang-timeout 3m --blame-hang-dump-type none
```

## Phase 1: names and headers

No behavior changes. Each step is a whole word replace, a build and the suites.

### Step 1.1 Rename `ValueChangedCommand` to `ValueChangedUndoCommand`

Complexity 1. Finding F5.

- `git grep -nw ValueChangedCommand` in this repository and in `..\dl_performance_mode`. The VL patches do not name it; confirm.
- Rename the file and the class. Replace whole words in `.cs` and `.md`: `CMiX.Core/Undo`, `ValueSync.cs`, `MasterBeat.cs`, `BeatTimedModulatorBase.cs`, `UndoManager.cs`, `ValueSyncTests.cs` (the test name `Changing_ThenChanged_PushesOneValueChangedCommandAndSendsOneMessage` gets the new name), `ARCHITECTURE.md`, `docs/undo-audit-2026-11-25.md`.
- Commit: `Renames ValueChangedCommand to ValueChangedUndoCommand`.
- Tests: `UndoManagerTests`, `ValueSyncTests`.

### Step 1.2 Rename `CompositeCommand` to `CompositeUndoCommand`

Complexity 1. Finding F5. Same procedure. Files: `CMiX.Core/Undo/CompositeCommand.cs`, `UndoManager.cs`, `ARCHITECTURE.md`, the audit. Commit: `Renames CompositeCommand to CompositeUndoCommand`. Tests: `UndoManagerTests`.

### Step 1.3 Add the license header to three files

Complexity 1. Finding F18. `AddItemUndoCommand.cs`, `PrefabManager.cs`, `PrefabManagerBase.cs` get the two SPDX lines that every other file of the chain has. Commit: `Adds the license header to three files of the undo chain`. Tests: none, build only.

### Step 1.4 `ResetItem` calls `Run`

Complexity 1. Finding F16. In `PrefabManager.ResetItem`, `command.Execute(); UndoManager?.Push(command);` becomes `Run(command)`. Commit: `Makes ResetItem run its step through Run`. Tests: `ResetItemTests`.

End of phase 1: Core suite without Redis, Studio suite.

## Phase 2: the manager and the gate

### Step 2.1 Remove the three re-sets of the undo manager

Complexity 1. Finding F4. `Project` line 26 (with its arrow comment), `ProjectSession.SetupList`, `RedisAddressRegistry` constructor: the line `x.UndoManager = undoManager` goes. The constructor of `PrefabManagerBase` already sets it. Commit: `Removes the re-sets of the undo manager on the lists`. Tests: `ProjectSnapshotTests`, `RedisAddressTests`, `OpenProjectTests`.

### Step 2.2 The undo manager is a required member of the classes that push

Complexity 3. Finding F4.

- `ReceivableControl.UndoManager` goes.
- `PrefabManagerBase` gets `public UndoManager UndoManager { get; }` set in the constructor (D4). `MasterBeat` and `BeatTimedModulatorBase` get a private readonly field set in the constructor.
- `IntegratorModulator`, `LFOModulator` and `RandomModulator` set the property and never read it: the assignment and the constructor parameter go. Check the DI and the tests that construct them.
- Every `UndoManager?.` in `PrefabManager` (13), `MasterBeat` (3), `BeatTimedModulatorBase` (3), `RedisAddressRegistry` (2) and `PrefabFileDropBehavior` (2) becomes `UndoManager.`.
- `ARCHITECTURE.md` line 228 (the debt item) and step 9 of its plan are updated.
- Commit: `Makes the undo manager a required member of the classes that push`.
- Tests: Core suite without Redis, `EntityDropBehaviorTests`, `TextureDropBehaviorTests`, `ModifierTests`.

### Step 2.3 One gate in `Push`

Complexity 2. Finding F1.

- `UndoManager.Push` returns when `IsApplying`.
- `ValueSync.Record` loses its check. The three checks in `PrefabManager` (`DuplicateItem`, `ResetItem`, `MoveItemToManager`) stay: they stop the action, not the push.
- New test `UndoManagerTests.Push_WhileApplying_RecordsNothing`: a command whose `Undo` pushes another command; after `Undo`, `CanUndo` is false and `CanRedo` is true.
- Commit: `Moves the IsApplying check of a push into UndoManager`.
- Tests: `UndoManagerTests`, `ValueSyncTests`.

### Step 2.4 The stack reports its size and its limit

Complexity 2. Findings F9, F11, F12.

- `UndoManager`: `public const int MaxSteps = 128;`, `public int StepCount`, `public int RedoStepCount`. `IsSuppressed` goes.
- `ProjectSnapshotTests` lines 191 and 235 assert `StepCount` after a push instead of `IsSuppressed`.
- `UndoManagerTests.TrimStack_...` reads `UndoManager.MaxSteps`.
- `Support/UndoCounts`: `StepsOf` and `RedoStepsOf` read the counts. `LastStepOf` keeps the reflection until a caller shows a need; count its callers, and if none needs the step object, it goes.
- Commit: `Gives UndoManager a public count and limit`.
- Tests: `UndoManagerTests`, `ProjectSnapshotTests`, every Studio test that uses `UndoCounts` (7 files).

### Step 2.5 The Edit menu follows the stack

Complexity 2. Finding F11.

- `UndoManager`: `public event Action Changed;` raised after push, merge, undo, redo, clear and trim.
- `MainViewModel`: `UndoCommand = new RelayCommand(undoManager.Undo, () => undoManager.CanUndo)`, the same for redo; on `Changed` both commands call `NotifyCanExecuteChanged`.
- New tests: `UndoManagerTests.Changed_OnEveryChangeOfTheStack_FiresOnce` and `MenuMainBindingTests.UndoItem_WithAnEmptyStack_IsDisabled`.
- Commit: `Disables Undo and Redo in the menu when the stack is empty`.
- Tests: `UndoManagerTests`, `MenuMainBindingTests`, `MainWindowShortcutTests`.

### Step 2.6 One style for the selection after an undo of an add or a remove

Complexity 2. Finding F14.

- `AddItemUndoCommand.Undo` and `RemoveItemUndoCommand.Undo`: after the list change, `var index = _list.IndexOf(previous); if (index >= 0) _list.SelectAt(index);`. `AddItemUndoCommand` drops `_previousIndex`. Nothing selected before gives no `SelectAt` call and no message.
- `RemoveItemUndoCommandTests.Undo_WithNothingSelectedBefore_SelectsNothing` expects `Remove`, `Insert 0` and no `SelectAt`. The restored item stays selected, as `Insert` leaves it: check the Studio tests of a remove and undo (`PanelInspectorSelectionTests`, `TreeKeyCommandsTests`) and tell the owner if one expects nothing selected.
- Commit: `Restores the selection the same way after an undo of an add and of a remove`.
- Tests: `CMiX.Core.Tests/Undo`, `RemoveItemTests`, `PanelInspectorSelectionTests`, `TreeKeyCommandsTests`.

End of phase 2: Core suite without Redis, Studio suite.

## Phase 3: scopes

### Step 3.1 A capture scope and a suspend scope

Complexity 3. Findings F3, F10.

- `UndoManager` gets `public CaptureScope Capture()` and `public SuspendScope Suspend()`, two readonly structs that implement `IDisposable`, as `ReceivableControl.SuppressionScope`. `CaptureScope.Dispose` calls `EndCapture`; `CaptureScope.Revert()` calls `RevertCapture` and marks the scope so that `Dispose` does nothing more. `SuspendScope.Dispose` calls `ResumeUndo`.
- `BeginCapture`, `EndCapture`, `RevertCapture`, `SuppressUndo` and `ResumeUndo` stay public: `MainViewModel` needs the begin and the end as two commands for the toggle drag of the window.
- The capture state becomes one `List<IUndoCommand> _captured` and one `Stack<int> _captureStarts`. `_captureDepth` goes (`_captureStarts.Count`). The dead null check in `EndCapture` goes. The fields move above the methods that use them.
- New tests: `UndoManagerTests.Capture_ADisposedScope_FlushesOneStep`, `Capture_RevertThenDispose_LeavesTheStackEmpty`, `Suspend_Nested_ResumesOnlyAtTheOuterDispose`.
- Commit: `Adds a capture scope and a suspend scope to UndoManager`.
- Tests: `UndoManagerTests`.

### Step 3.2 The Core callers use the scopes

Complexity 2. Finding F3. `ModulatableValue.SetModulator`, `RedisAddressRegistry` (`InOneUndoStep` goes, the two callers use `using`), `PrefabManager.AddItem(type, afterAdd)`, `RemoveEverywhere`, `MoveItemToManager`, `ProjectSyncTarget.ApplySnapshot` (the suspend). Commit: `Uses the undo scopes in Core`. Tests: `ModulatableValueTests`, `RedisAddressTests`, `RemoveItemTests`, `MoveItemTests`, `ProjectSnapshotTests`, `ProjectFileCompatibilityTests`.

### Step 3.3 The Studio callers use the scopes

Complexity 2. Finding F3. `TreeEdit.ShowAll`, `TreeFileDropRules.Drop`, `ProjectTree.SelectChain`, `ProjectTree.DropAsync`, `ProjectTree.DropBlockAsync` (with `capture.Revert()`), `PrefabFileDropBehavior.OnPathsDropped`. Commit: `Uses the undo scopes in the Studio`. Tests: `TreeKeyCommandsTests`, `ListTreeFileDropTests`, `ListTreeMoveBlockTests`, `ListTreeRowDragTests`, `ProjectTreeSelectionAfterDropTests`, `EntityDropBehaviorTests`, `TextureDropBehaviorTests`.

### Step 3.4 One `RecordAsOneStep` for the beat and the beat modulators

Complexity 2. Finding F2.

- `UndoManager.RecordAsOneStep(IControl control, Action action)`: captures `control.ToModel()`, runs the action in a suspend scope, pushes one `ValueChangedUndoCommand`. The step is the same object as today, so the `Periods` table of the beat and the merge of two clicks (D1) do not change.
- `MasterBeat.WithUndo` and `BeatTimedModulatorBase.WithUndo` go. Their callers call `UndoManager.RecordAsOneStep(this, () => ...)`.
- New test: `UndoManagerTests.RecordAsOneStep_TwoValueWrites_PushesOneStepOfTheControl`.
- Commit: `Moves WithUndo of the beat classes into UndoManager`.
- Tests: `UndoManagerTests`, `ModifierTests`, the modulator tests (`BeatLFO`, `BeatRandom`, `BeatStep`), `MasterBeat` tests if any.

End of phase 3: Core suite without Redis, Studio suite.

## Phase 4: the merge, the tests and the documents

### Step 4.1 The merge uses the time provider and a plain write

Complexity 3. Findings F6, F7.

- `UndoManager` takes `TimeProvider` in its constructor (the DI registers one since the `ValueSync` change; check `AddCMiXCore`).
- `ValueChangedUndoCommand` loses `Timestamp` and `WithBefore`. `Before` becomes a settable property, or a method `MergeBefore(IControlModel before)` with no return.
- `UndoManager` keeps `_lastPushTicks` and stamps it on every push. The merge check reads the last node and the ticks.
- `UndoManagerTests.MergeWindow_...` drives a `FakeTimeProvider` (the one `ValueSyncTests` uses): two writes inside the window merge, two writes outside it do not.
- Commit: `Drives the merge window of UndoManager from the time provider`.
- Tests: `UndoManagerTests`, `ValueSyncTests`, `ValueInteractionTests`.

### Step 4.2 Three tests for the gaps

Complexity 2. Finding F19.

- `CompositeUndoCommandTests.Dispose_TwoOwningSteps_ReleasesBothItems` on `FakeItemList`.
- `UndoManagerTests.Suspend_Nested_...` exists since 3.1; add `Clear_WithOwningSteps_ReleasesTheirItems` if no test pins it.
- `ListTreeMoveBlockTests.DropBlockAsync_ABlockWithARowThatTheListRefuses_LeavesTheStackEmpty` with `UndoCounts.StepsOf`.
- Commit: `Adds tests for the dispose of a group, the clear and the reverted block drop`.

### Step 4.3 The documents

Complexity 1. Findings F8, F15, F19 and the audit itself.

- `ARCHITECTURE.md`, section "Undo and redo": the scopes, `RecordAsOneStep`, the rule of the merge (D1), the rule of the first selection (F15), the public count. Line 228 and step 9 of its plan were updated in 2.2; check.
- `docs/tests-audit-2026-11-24.md`: the line on `PrefabManager.MoveItemUpCommand` names `ManagerReorderService` and the test that covers it.
- `docs/undo-audit-2026-11-25.md`: a line at the top says that the plan executed it, with the date.
- Commit: `Describes the undo scopes and the rules of the merge and the first selection`.

End of phase 4: the Core suite twice with details captured, the Studio suite once, the UI suite once.

## Phase 5: the review

Complexity 5, main agent. The owner runs `/code-review` on the branch, or asks for a review agent on `CMiX.Core/Undo`, `ValueSync.cs`, `PrefabManager.cs` and the nine scope sites. The review goes after the code has one shape, so after phase 4.

## Out of the plan

- D2: one selection step. If the owner wants it, it is one step of complexity 2 after 2.6.
- D3: the home of `IItemList`. One step of complexity 1 if the owner wants it, after 1.2.
- The long comments of F18 in files that no step touches.
- The split of `PrefabManager` into the list and its XAML commands (`ARCHITECTURE.md`, step 9). This plan removes the optional manager, which that step needs first.
