# Audit of the tests, 2026-11-24

Status 2026-11-25: `ValueInteraction` is gone since commit `bf3d27778`, and the Core suite runs in parallel since `07faee6ad`, so the point on the sequential run below is closed.

Branch `refactor/tree-structure` at commit 6ec81b72c. Three refactorer agents read every test file, one per
project. The main agent ran the three suites, checked the main findings against the code and wrote this summary.
Parts A, B and C are the three detailed audits. Nothing was changed.

## Summary

| Project | Test files | Lines | Methods | Cases run | Run time | Source lines |
|---|---|---|---|---|---|---|
| CMiX.Core.Tests | 150 | 18,960 | 850 | 1,075 | 1 min 36 s | 17,914 |
| CMiX.UI.Tests | 109 | 13,945 | 695 | 833 | 6 s | 10,925 |
| CMiX.Studio.Avalonia.Tests | 223 | 33,185 | 1,458 | 1,742 | 2 min 9 s | 17,435 |
| Total | 482 | 66,090 | 3,003 | 3,650 | 3 min 51 s | 46,274 |

All 3,650 cases pass. Two are skipped (`ProjectFileCompatibilityTests`, no sample folder set).
The test code is 1.4 times the size of the code it tests.

### Where the size comes from

- Small files. 54 files have one test, 56 have two. The median file has 4 tests. One subject is spread over
  many files: 41 `ListTree*`, 21 `ProjectTree*`, 20 filter files in Core with the same four tests each,
  15 modifier files, 6 `TextBoxEditable*`, 6 `Popup*`, 5 `ContextMenu*`.
- Pasted setup. 62 private window helpers in the UI tests, 445 `TestServiceProviderFactory.Create()` calls in
  Core, 17 private `Click`, 14 `CenterOf`, 12 `Press`, 16 `Settle` and 21 temp folder copies in Studio.
- Tests that prove what the compiler proves, pin a number with no rule behind it, or test the framework:
  about 330 in Core, about 32 in UI, about 60 in Studio.
- Dead code kept alive by its tests: the drag look of the old tree in `TreeNode` (five properties, no view
  binds them), `TreeDropService.Locate` and `ProjectTree.Locate` (no caller outside the tests), `RowStripes`,
  the `CursorRow` attached property and its theme, `ToggleButtonPlayPause`, `ToggleButtonUpdate` (Gallery only),
  the `OpacityDrag*` tokens. Checked by grep in the main session.

### Where the time comes from

| Suite | Slow part | Share |
|---|---|---|
| Core, 96 s | 6 sync and Redis classes: `SyncPeerRedisOutageTests` 39 s, `SyncPeerTests` 18 s, `RedisSyncStoreIntegrationTests` 17 s, `SyncPeerRedisIntegrationTests` 8 s, `SnapshotCompactorTests` 7 s, `SyncPeerStartCheckTests` 3 s | 97 percent |
| Studio, 129 s | `PanelInspectorBudgetTests` 14 s (one test), `PanelInspectorTests` 11 s, `ListTreeBigProjectTimingTests` 7 s, `OpenProjectTests` 5 s, `ResetMenuTests` 4 s | 32 percent |
| UI, 6 s | Nothing. The slowest test takes 0.4 s | |

Core runs with parallel tests off for the whole assembly (`AssemblyInfo.cs`, because of the ambient
`ValueInteraction` scope). 67 `Task.Delay` or `Thread.Sleep` calls in 10 files add up in series.
25 of them wait 300 ms to assert that nothing happened.

### Rule breaks

- 882 of 3,003 test names break `Method_Scenario_Result`. The three `TestNameTests` pin 433, 249 and 227.
  The real numbers today are 410, 222 and 225, so the pins can go down now.
- The word Delete is in the test code: `DeleteLastItemTests`, `DeleteUndoTests`, `DeleteSelectionRuleTests`
  and 9 method names in Core. `.claude/CLAUDE.md` bans it in code.
- Four Core files carry narrative comments with commit hashes.
- `MotionTests.cs` in UI has one test with a fully qualified attribute. It is fine, the count script missed it.
- Helpers exist twice or three times: `FakeCredentialStore`, `TestServiceProviderFactory`, `TestAppBuilder`,
  `TestNameTests`, `MessageRecorder` (three copies in Studio), two private `ContrastRatio` in UI.
- Two counters live in production code only for a test: `RowCounts.KindCountsComputations` and
  `PanelInspector.EditorViewsBuilt`.

### Expected result of the cleanup

| Project | Files | Lines | Methods |
|---|---|---|---|
| Core | 150 to about 85 | 18,960 to about 13,100 | 850 to about 520 |
| UI | 109 to about 70 | 13,945 to about 11,000 | 695 to about 640 |
| Studio | 223 to about 150 | 33,185 to about 29,500 | 1,458 to about 1,350 |
| Total | 482 to about 305 | 66,090 to about 53,600 | 3,003 to about 2,510 |

The coverage goes up, not down: the generic theories (every filter, every control with a model, every reset)
cover the types that have no hand written test today.

## Decisions for the owner

The cleanup needs these answers first. Each one removes production code or a pinned guard.

1. Remove the drag look of the old tree: `TreeNode.IsDropCandidate`, `IsDropNeighbor`, `IsDimmed`,
   `IsDropHint`, `IsDragActive`, the matching fields of `DragLook` and `TreeDragState.MarkRow`,
   `TreeDropService.Locate`, `ProjectTree.Locate`. About 150 production lines and 650 test lines.
2. Remove `RowStripes`, the `CursorRow` attached property with its theme and the `CursorFrame*` tokens, the
   six `Tree*` geometry tokens, the `OpacityDrag*` tokens, the themes `ToggleButtonPlayPause` and
   `ToggleButtonUpdate`. Or say which one the new tree will use.
3. Fix the 14 known bad bindings that `BindingIntegrityTests` pins since 2026-08, and pin 0.
4. The six "old JSON key still loads" tests in Core: keep or remove. Old files do not matter in this phase.
5. `ExpandedByDefaultTests` pins editor state that lives in Core: keep, or move the state.
6. The VL pins (`ISpreadableModifier`, `IComposable`, `IBlendable`, `IMaskable`) could join
   `ControlRepositoryVlMembersTests`. That name is fixed, so this needs a plan.
7. Remove the two timing tests that cannot be stable, `ListTreeBigProjectTimingTests` and the Stopwatch
   asserts of `PanelInspectorBudgetTests`, or keep them behind a trait that the normal run skips.

## Plan

Phases in order. Each step is one commit, the build and the three suites pass at every commit.
Complexity: 1 goes to a Sonnet subagent, 2 to 4 to an Opus subagent, 5 stays with the main agent.
The owner can run `/simplify` and `/code-review` on each commit.

### Phase 0: pins and quick removals, no decision needed

| Step | What | Complexity |
|---|---|---|
| 0.1 | Lower the three `PinnedBadNameCount` to 410, 222 and 225 | 1 |
| 0.2 | Remove the Core tests that prove a declaration: `AddressedOutputTests`, the 12 `IsAlsoAnIModifier`, `IModulatorTests` | 1 |
| 0.3 | Remove the UI pins and duplicates of Part B section 2 that need no decision (about 15 tests) | 1 |
| 0.4 | Remove the Studio duplicates of Part C section 2 that need no decision (shared test names, duplicate kind counts) | 2 |
| 0.5 | Fix the assertion that cannot fail in `CaptureLostTests.cs` line 117 | 1 |

### Phase 1: shared helpers

| Step | What | Complexity |
|---|---|---|
| 1.1 | UI: `Support/TestWindow`, `Tokens.Color/Brush/Theme`, `PointerInput.Click/Move`, `KeyInput`, `Parts.Find<T>`; replace the 62 window helpers and 93 raw `FindResource` casts | 3 |
| 1.2 | Studio: `Support/Screen` (Click, RightClick, Press, Hover, CenterOf, Settle) and `Support/TempFolder`; replace the private copies | 3 |
| 1.3 | Studio: `ListTreeMainScreen.Show(layers, fill)`, `ListTreeTestScreen.ShowWithSearch`, `ShowTwoViews` for the 15 files that build the tree window by hand | 3 |
| 1.4 | Core: move `ModelWithOneComposition`, `SnapshotOf`, `ModelOf`, a `StoreGate`, `UnreliableSyncStore`, `LingeringSyncStore`, `RedisPeerHarness` into `SyncTestHelpers` | 3 |
| 1.5 | Studio: one `MessageRecorder`, `UndoCounts` for every undo count | 2 |
| 1.6 | Share `FakeCredentialStore`, `TestServiceProviderFactory` base and `TestNameTests` between the projects by linked files, as `PointerInput` is today | 2 |

### Phase 2: generic theories in Core

| Step | What | Complexity |
|---|---|---|
| 2.1 | One theory over every `TextureFilterBase` subclass replaces the 20 filter files | 3 |
| 2.2 | Widen `FilterAndModifierRoundTripTests` to every `IControl` with a model; `ValueWalker` walks `ModulatorID` and `BoundOutputName`; remove the 30 hand written round trips | 4 |
| 2.3 | One generic Reset theory with `ValueWalker`; remove `ModulatorDefaultsTests` and the default pins | 3 |
| 2.4 | `SyncStoreContractTests` runs against `InMemorySyncStore` and `RedisSyncStore` | 3 |

### Phase 3: merges by subject

| Step | What | Complexity |
|---|---|---|
| 3.1 | Core: 15 modifier files plus `ControlFactoryModifierCreationTests` and `ModifierVLDiscoveryInterfaceTests` into `ModifierTests` | 3 |
| 3.2 | Core: list files by verb: `RemoveItemTests`, `ResetItemTests`, `DuplicateItemTests`, `MoveItemTests`, `SelectionTests`, `SharedItemTests`, `CompositionIDTests`. The word Delete leaves the test code | 3 |
| 3.3 | Core: `ModulatableValueTests` with a theory over float, int and bool; `ModulatorTests` with an output and category table | 3 |
| 3.4 | Core: split `SyncPeerTests` into six files that follow the partial files of `SyncPeer` | 2 |
| 3.5 | UI: six `TextBoxEditable*` into `TextBoxEditableTests` (the model for the others) | 2 |
| 3.6 | UI: `TogglePopup*` and `PopupEditModeTests` into `TogglePopupTests`; the `ButtonPopup` tests into `ButtonPopupTests` | 2 |
| 3.7 | UI: menu look files into `ContextMenuLookTests` and `MenuLookTests`; `ColorMix.Contrast` replaces the two `ContrastRatio` copies | 3 |
| 3.8 | UI: split `CaptureLostTests` into `CaptureLostTests`, `DragCursorReturnTests`, `DragSpeedTests`; fakes to Support | 3 |
| 3.9 | Studio: tooltips into `TreeTooltipsTests`, badges into `BadgesKindCountTests`, toggles into `TogglesRowTests` | 3 |
| 3.10 | Studio: `ProjectTree*` means the model only; the screen tests move to `ListTreeProjectTests`, `ListTreeKeysTests` and a new `TreeKeyCommandsTests` | 4 |
| 3.11 | Studio: small `ListTree*` files by subject, 19 files become 9; split `ListTreeMoveTests` into single and block | 3 |
| 3.12 | Studio: split `PanelInspectorTests` into header, selection and tab; fold `PanelInspectorBudgetTests` in | 3 |
| 3.13 | Studio: the 19 model tests of `ToolbarProjectTreeTests` move to `ProjectTreeAddGroupsTests` | 2 |
| 3.14 | Studio: `SelectionEchoTests` as one theory over four list kinds | 2 |
| 3.15 | Studio: `MainWindow*` 7 to 4, `Panel*` 10 to 7, modulator views 12 to 6, `Settings*` and `Unsaved*` pairs | 3 |

### Phase 4: time and stability

| Step | What | Complexity |
|---|---|---|
| 4.1 | Core: replace the 25 "wait 300 ms, assert nothing" delays with a wait for the positive condition plus one settle tick; fix the three wall clock asserts | 4 |
| 4.2 | Studio: replace `Task.Delay` in `MainWindowClosingTests` and `UnsavedChangesTests` with a pump; `PathDropServiceTests` as plain `[Fact]` | 1 |
| 4.3 | UI: clipboard seam on `ValueMenu` so `ValueMenuCopyPasteTests` needs no delay and no real clipboard | 2 |
| 4.4 | Remove the two test only counters from production, or mark them | 2 |

### Phase 5: after the owner decisions

| Step | What | Complexity |
|---|---|---|
| 5.1 | Remove the drag look of the old tree and its tests (decision 1) | 3 |
| 5.2 | Remove the dead UI parts and their tests (decision 2) | 2 |
| 5.3 | Fix the 14 bindings, pin 0 (decision 3) | 3 |
| 5.4 | Timing tests (decision 7) | 1 |
| 5.5 | Rename the remaining bad test names, one file per commit, pins to 0 | 2 |

### Coverage gaps worth a test, from the three parts

- Core: every `IControl` round trip (closed by 2.2), `SetAlpha` filter (closed by 2.1).
- UI: see Part B section 5.
- Studio: `PrefabFileDropBehavior`, `PanelSyncManagement`, two views during a drag or a file drop, the undo
  stack after `RevertCapture`, three converters, the Escape order, `ListTreeEndRoom`.


# Part A: CMiX.Core.Tests


Date: 2026-11-24. Branch: refactor/tree-structure. Read only, no change made.

### 1. Overview

#### Counts

| What | Count |
|---|---|
| Files (.cs) | 159: 150 test classes, 9 helper files |
| Lines | 18,960 |
| Test methods | 850: 784 `[Fact]`, 47 `[Theory]`, 19 `[SkippableFact]` |
| Test methods that break `Method_Scenario_Result` | 410 (the pin in `TestNameTests` is 433) |
| Calls of `TestServiceProviderFactory.Create()` | 445, each one builds the full object graph |
| Files with `Task.Delay` or `Thread.Sleep` | 10, with 67 calls in all (33 in `SyncPeerTests.cs`, 13 in `SnapshotCompactorTests.cs`) |
| Files with more than one top level type | 4: `SyncTestHelpers.cs` (6), `RedisSyncStoreIntegrationTests.cs` (2), `SyncPeerRedisOutageTests.cs` (2), `ValueWalker.cs` (2) |

#### How the suite is organized

- One flat folder, one namespace `CMiX.Core.Tests`, no sub folder. A reader finds a subject only by the file name.
- Parallel runs are off for the whole assembly (`CMiX.Core.Tests/AssemblyInfo.cs`), because `ValueInteraction` is a static. Every test runs in sequence, so every `Task.Delay` adds to the run time.
- Almost every test builds a real object graph with `TestServiceProviderFactory.Create()` and then a control with `ControlFactory.Create(typeof(X))` or `GetRequiredService<X>()`. There are no mocks. The test doubles are the sync store (`InMemorySyncStore`, `WrappingSyncStore`, `HangingSyncStore`, `ReconnectingSyncStore`), `RecordingSyncTarget`, `RecordingMessageSender` and `FakeCredentialStore`.
- The files fall into seven groups:

| Group | Files | Lines | Tests |
|---|---|---|---|
| One file per filter (Ascii, Blur, ... TriColor) | 20 | 1,856 | 89 |
| One file per modifier, plus modifier creation and VL interface | 17 | 1,718 | 73 |
| Modulators, modulatable values, Redis addresses | 27 | 2,590 | 141 |
| Lists, selection, undo, duplicate, move, shared items | 29 | 3,164 | 183 |
| Repository, factory, values, project, file, processors, assets | 44 | 4,560 | 232 |
| Sync peer and stores | 11 | 4,291 | 132 |
| Helpers | 9 | 1,000 | 0 |

#### What the tests cover well

- The sync protocol. `SyncPeerTests.cs`, `SyncPeerStartCheckTests.cs`, `SnapshotCompactorTests.cs`, `SyncPeerStartupTests.cs` and `EngineHostTests.cs` hold 101 tests on an in-memory store. They cover join, push, pull, recovery after a gap, restart during a join, an append or a tick, compaction, the dispatcher thread rule and the order of entries. `WrappingSyncStore` has gates before and after each store call, so the tests reach real races.
- The list semantics. `ManagerDataTests.cs` (22 tests) pins the selection rules on insert, remove and move. The remove, duplicate, move and selection files pin the undo step, the wire message and the peer side of each list action.
- Save and load. `FilterAndModifierRoundTripTests.cs` walks every value of every filter and modifier with `ValueWalker`. A value that `ToModel` or `FromModel` forgets fails there.
- The engine processors. `RenderListProcessorTests.cs` (22), `UpdateOrderProcessorTests.cs`, `VisibilityProcessorTests.cs`, `VisibleItemsProcessorTests.cs` and `AssetListProcessorTests.cs` cover the lists that the patch reads, including the hidden state.
- The two names the VL patches and the shader depend on: `ControlRepositoryVlMembersTests.cs`, `ModifierVLDiscoveryInterfaceTests.cs`, `BlendModeShaderTests.cs`.

#### What the tests do not cover

- A generic round trip exists only for filters and modifiers. Modulators, texture sources, entities, cameras, lights, the material and the settings objects have no generic round trip. This is why 30 hand written round trip tests exist (see section 2).
- The modifiers `OrbitModifier`, `TargetModifier`, `ZoomModifier`, `RandomModifier`, `SplitModifier`, `BillboardModifier` and the filter `SetAlpha` have no test of their own. Only the generic round trip and `EveryAddableModifier_CanBeCreatedViaControlFactory` touch them.
- The modulators `LFOModulator`, `RandomModulator`, `BeatLFOModulator` and `BeatRandomModulator` have only default tests.
- The commands of `PrefabManager` for the XAML: `MoveItemUpCommand`, `MoveItemDownCommand`, `ClearSelectionCommand` and `MoveItemToCommand` appear in no test. `AddItemCommand`, `RemoveItemCommand` and `RemoveEverywhereCommand` appear once each.
- `OutgoingQueue`, `StreamFollower`, `Presence` and `MessageCollectionManagerHandler` have no unit test. They are tested through `SyncPeer` and `PrefabManager.Receive` only.
- The VL names of `ManagerData` (`Items`, `ID`, `SelectedIndex`), `PrefabManager` (`SelectedItem`, `ManagerData`) and `ControlRepository.GetControl` are not pinned. Only `ControlRepository.Controls` is.

### 2. Remove candidates

The table names the tests, not only the files. A file with "all" loses every test. The line count is the count of lines that go. The covering test is the test that already proves the same thing.

| File | Tests to remove | Lines | Reason | Covering test | Risk |
|---|---|---|---|---|---|
| 19 filter files: `AsciiTests.cs`, `BlurTests.cs`, `DitherTests.cs`, `EchoTests.cs`, `EdgeTests.cs`, `FeedbackTests.cs`, `HSCBTests.cs`, `HalftoneTests.cs`, `InvertTests.cs`, `KaleidoscopeTests.cs`, `KuwaharaTests.cs`, `LEDPanelTests.cs`, `PixelateTests.cs`, `PulseTests.cs`, `ShadowTests.cs`, `ShiftRGBTests.cs`, `ThresholdTests.cs`, `TransformTextureTests.cs`, `TriColorTests.cs`, plus the first 4 tests of `DisplaceTests.cs` | 80 (4 per file: `ModulatorManager_CanAddRandomModulator`, `ReorderingModulators_DoesNotUnassignTheMovedOnesBindings`, `DeletingAssignedModulator_UnassignsIt...`, `X_ToModel_FromModel_RoundTrips...`) | about 1,650 | The four tests are one template pasted 20 times. They test `PrefabManager.AddItem` and `ModulatorHost` (one class in `CMiX.Core/Modulation/ModulatorHost.cs`), not the filter. The value round trip is generic already. Only the live `BoundModulator` after a load is not generic. | `TextureSourceModulationTests.Host_UnbindsAValue_WhenItsModulatorIsRemoved` and `Host_KeepsTheBindingOfAMovedModulator` for the host. `FilterAndModifierRoundTripTests.EveryValue_SurvivesSaveAndLoad` for the values. `ControlFactoryModifierCreationTests.Modifier_FromModel_ReResolvesBoundModulatorFromModulatorID` for the bound modulator on a modifier. Replace the 80 tests with one theory over every `TextureFilterBase` subclass (21 types, `SetAlpha` included) that binds, saves, loads and checks `BoundModulator`. | Low |
| `PixelateTests.cs` | `XAndY_DefaultToPointFive` | 9 | Pins the default 0.5 that `PixelateModel.X` and `Y` hold (`CMiX.Core/Texturing/Filters/PixelateModel.cs` lines 15 and 16). The factory captures it. | `DefaultResetTests.ConstructorDefault_StillWorks` shows the mechanism on Blur. | Low |
| `PulseTests.cs` | `Create_NewPulse_HasTheDefaultValues` | 12 | Pins five model defaults. The `Bindables.Count == 3` assert is the only structure check. | Same. | Low |
| 12 modifier files: `CharWriter`, `CircularSpread`, `Color`, `Flip`, `Grid`, `Linear`, `RenderRandomEntity`, `RenderSequenceEntity`, `Rotation`, `TexCoord`, `Translate`, `Visibility` (`...ModifierTests.cs`) | 12 `X_IsAlsoAnIModifier` | about 100 | `Modifier` is declared `: IModifier` (`CMiX.Core/Modulation/Modifier.cs` line 13). The compiler proves it. `Assert.IsAssignableFrom` on a resolved instance tests the language. | None needed. | Low |
| Same 12 files plus `AlphaModifierTests.cs`, `ComposableVisibilityModifierTests.cs` | 14 `X_IsDiscoverableOn...` and `IsOfferedForOwner` | about 110 | Each test reads the `ModifierPanelAttribute` of one type. `ListRules.CanHold` reads the same attribute. One table theory holds all 20 modifiers in 20 lines. | `ListRulesTests.CanHold_AnOwnerAndAnItemType_GivesTheRule` has 14 rows already. Extend that table. | Low |
| `RenderRandomEntityModifierTests.cs`, `RenderSequenceEntityModifierTests.cs`, `RotationModifierTests.cs`, `ScaleModifierTests.cs`, `TranslateModifierTests.cs`, `VisibilityModifierTests.cs` | 6 `ModulatorManager_CanAddRandomModulator` | 50 | Tests `PrefabManager.AddItem`. | Every list test, for example `ManagerDataTests.Insert_PutsTheItemAtTheIndexAndSelectsIt`. | Low |
| `ScaleModifierTests.cs` | `TwoScaleModifiers_HaveDistinctModulatorManagers` | 9 | Tests that the container registers the control as transient. That is a DI framework fact. | `CMiXServiceCollectionExtensionsTests` is the place for registrations. | Low |
| `CircularSpread`, `Color`, `Grid`, `Linear`, `Rotation`, `Scale`, `TexCoord`, `Translate` (`...ModifierTests.cs`) | 8 `X_ToModel_FromModel_RoundTripsModifierModeSelector`, 8 `X_ToModel_FromModel_RoundTripsBindableValuesAndBinding`, `Grid_ToModel_FromModel_RoundTripsCount`, `FlipModifier_ToModel_FromModel_RoundTripsDirectionXYZ`, `AlphaModifierTests.RoundTrip_KeepsValue` | about 330 | `ValueWalker` reaches `ModifierModeSelector.Mode`, `Count`, `DirectionXYZ` and every bindable value, so the generic test covers the values. The `ModulatorID` part is covered at the value level. | `FilterAndModifierRoundTripTests.EveryValue_SurvivesSaveAndLoad`, `ModulatableValueFloatTests.Modulatable_ToModel_FromModel_RoundTripsValueAndBinding`. | Low |
| `ControlFactoryModifierCreationTests.cs` | `FreshCircularSpread_BindablesMatchOldCircularSpreadDefaults`, `FreshRenderRandomEntityModifier_BindableMatchesOldDefault`, `FreshRenderSequenceEntityModifier_BindableMatchesOldDefault`, `FreshGrid_NonBindableFieldsMatchOldGridDefaults`, `FreshColorModifier_ColorModeMatchesOldRandomHSVDefault` | 55 | Regression guards for the port of August 2026 ("match old defaults"). The defaults live in the models now. Two of them exist a second time. | `GridModifierTests.CountXYZ_ConstructAtDefaultOfOne_AndResetReturnsToItAfterAChange`, `ColorModifierTests.AllFourBindables_ConstructAtTheirOwnDefault_AndResetReturnsToItAfterAChange`. | Low |
| `ModulatorDefaultsTests.cs` | all 6 | 168 | Each test pins the model defaults of one modulator and shows that `Reset` goes back. The mechanism is one line in `ControlFactory` and `GenericValue.SetDefaults`. The values live in the models. | `DefaultResetTests.CreateFromType_ResetRestoresTheModelDefault`, `LoadedModel_ResetRestoresTheDefault_NotTheSavedValue`, `GenericValueTests.SetDefaults_CopiesTheValueToOriginalValue`. Replace with one theory over every control type that uses `ValueWalker`: change every value, call `Reset`, compare with a fresh control. | Medium |
| `FFTModulatorTests.cs`, `LISAModulatorTests.cs`, `RedisFloatModulatorTests.cs`, `GradientMeshTests.cs`, `LiquishTests.cs`, `SequencePlayerTests.cs` | `Create_ANewModulator_HasEveryRangeAtZeroToOne`, `ResetAll_ARange_GoesBackToZeroToOne`, `Create_ANewModulator_HasTheDefaultRanges`, `Range_ANewModulator_StartsAtZeroToOne`, `ResetAll_TheRange_GoesBackToZeroToOne`, `Create_NewGradientMesh_HasTheDefaultValues`, `Create_NewLiquish_HasTheDefaultValues`, `NewSequencePlayer_HasTheDefaultValues` (keep the `IsMomentary` and `IAssetTextureSource` asserts) | about 110 | Default pins. `ResetAll` of a `FloatRange` is tested once already. | `CompositeResetAllTests.FloatRange_ResetAllCommand_ResetsBothValuesToTheirDefault`, and the reset theory above. | Medium |
| `IModulatorTests.cs` | `RandomModulator_IsResolvableAsIModulator`, `TrackingModulator_Outputs_FilterByOutputType_ExcludesMismatchedOutputs`, `TrackingModulator_ToModel_FromModel_RoundTripsCountAndX`, `FFTModulator_ToModel_FromModel_RoundTripsTrackIndexAndBassRange` | 55 | The first tests the compiler. The second tests LINQ `OfType`. The two round trips exist again in the files of the modulators. Move the two output tests (`TrackingModulator_IsResolvableAsIModulator_AndHasNamedOutputs`, `FFTModulator_..._HasNamedFloatOutputs`) to `TrackingModulatorTests.cs` and `FFTModulatorTests.cs`, then drop the file. | `TrackingModulatorTests.FromModel_OfToModel_KeepsEveryValue`, `FFTModulatorTests.FromModel_OfToModel_KeepsEveryValue`. | Low |
| `AddressedOutputTests.cs` | all 6 | 56 | Tests that `AddressedOutput<T>` derives from `ModulatorOutput<T>` and stores two constructor arguments. That is the class declaration. | `RedisModulatorTests.Outputs_WithoutASelection_IsOneOutputNamedValue` checks the type, the name and the address of the output on a real modulator. | Low |
| `LISAModulatorTests.cs` | `Category_OfTheModulator_IsExternal` | 7 | The category table holds the same row. | `ModulatorCategoryTests.Category_OfAModulatorType_IsTheExpectedCategory`, row `LISAModulator`. | Low |
| `LISAModulatorTests.cs`, `IntegratorModulatorTests.cs`, `TrackingModulatorTests.cs`, `GradientMeshTests.cs`, `LiquishTests.cs` | 5 `Serialize_ThenDeserialize_KeepsTheModel` | 90 | Serialize a record and read it back with System.Text.Json. The CMiX part is the `$type` converter, which `ProjectSerializerTests` covers. One theory over every model type would be a real guard. The five copies are not. | `ProjectSerializerTests.Save_Then_Load_RoundTripsMinimalProject`, `ModulatableValueUpgradeJsonConverterTests.Read_NestedValue_GivesTheSameModelAsSerialized`. | Medium |
| `TrackingModulatorTests.cs` | `Serialize_ATrackingModulatorModel_KeepsTheKeyOrder` | 10 | Pins the order of the JSON keys. The file format does not depend on the order. No reason in the code or the commit. | None needed. | Low |
| `RedisModulatorTests.cs` | `ToModel_OfEachRedisModulator_SerializesTheSameKeysAsBefore` | 14 | Guard for the refactor of 2026-11-06 that shared the base class (commit e5529c658). It pins the key order. Keep only the `$type` assert if you want one. | `FromModel_OfToModel_KeepsTheIDAndTheSelection` in the same file. | Medium |
| `BeatStepModulatorTests.cs` | `ItStartsWithNoStep`, `AddingSteps_ListsThemInOrder`, `DeletingAStep_TakesItOutOfTheList` | 33 | Test `PrefabManager` through a modulator. | `ManagerDataTests`, `DeleteSelectionRuleTests`. Keep `TheSteps_SurviveSavingAndLoading`. | Low |
| `RedisAddressRegistryTests.cs` | `ThereIsOneRegistryForTheWholeProject`, `ItStartsEmpty`, `AddedAddresses_AreListedInOrder` | 25 | DI singleton check, and two list facts. Keep `TheListHasTheFixedID` (a fixed id on the wire). | `ManagerDataTests`, `RedisAddressDefaultNameTests.Add_AnAddress_NamesItAddressWithANumber`. | Low |
| `RedisBoolModulatorTests.cs`, `RedisIntModulatorTests.cs` | 5 `CanBind_...` | 60 | The type rule is tested on `ModulatableValue<T>`. These add that `Playing` is a bool and `SeekTarget` is an int. Fold into one theory row each. | `RedisModulatorTests.CanBind_AValueOfTheOutputType_IsTrueAndOfAnotherTypeIsFalse`. | Low |
| `SequencePlayerTests.cs` or `TextureSourceModulationTests.cs` | one of the two `DoSeek_PressAndReleaseThenUndo_LeavesDoSeekFalse` | 15 | The same test on `VideoPlayer` and on `SequencePlayer`. The rule is a property of `GenericValue.IsMomentary`. | `GenericValueTests.Value_OfAMomentaryValue_WritesNoUndoStep`. Keep one as a theory over the two players. | Low |
| `ModulatableValueUpgradeJsonConverterTests.cs` | `Options_Converters_HoldOneUpgradeConverter` | 5 | Pins a registration. The eleven other tests fail when the converter is absent. | The same file. | Low |
| `CollectionManagerTests.cs` | `Move_OutOfRangeOldIndex_IsNoOp`, `Move_OutOfRangeNewIndex_IsNoOp`, `Move_NegativeIndices_IsNoOp` | 36 | They call `manager.ManagerData.Move`, so they test `ManagerData`. The header comment names a guard on `CollectionManager.MoveItem`, which commit 001fde16b removed. | `ManagerDataTests.Move_WithAnIndexOutsideTheList_IsRefused` (4 rows). | Low |
| `DeleteLastItemTests.cs` | `RemoveItem_OfAMiddleLayer_UndoRestoresItAtTheSameIndex` | 18 | Undo of a remove in the middle is tested twice more. Keep the last composition test. | `DeleteSelectionRuleTests.UndoingTheDeleteOfTheSelectedItem_SelectsTheRestoredItem`, `AddItemUndoCommandTests.UndoThenRedo_OfAnItemInTheMiddle_KeepsItsIndex`. | Low |
| `DuplicateItemCommandTests.cs` | both | 40 | The command only calls `DuplicateItem`. Both behaviours exist in the method tests. | `DuplicateItemTests.DuplicateItem_InTheMiddle_PutsTheCopyRightAfterTheSourceAndSelectsIt`, `DuplicateItem_OfNothing_DoesNothing`. Keep one line in the merged file that the command runs the method. | Low |
| `DuplicateItemTests.cs` | `DuplicateItem_InTheMiddle_PutsTheCopyRightAfterTheSourceAndSelectsIt`, `DuplicateItem_UndoRemovesTheCopyAndRedoPutsItBackAtTheSameIndex`, `DuplicateItem_SendsOneAddMessageWithTheIndexOfTheCopy`, `DuplicateItem_AnotherPeerShowsTheSameOrder` | 75 | The same four exist as theories over five kinds in `DuplicateEntityTests.cs`. Add `Layer` and `Composition` to the `Kinds()` of that theory and the layer copies go. | `DuplicateEntityTests.DuplicateItem_OfEveryKind_*`. | Low |
| `SwapListUndoTests.cs` | `AddFromTheRepository_ThenUndo_TakesTheItemOutAgain` | 9 | The first half of `Redo_AfterAddAndUndo_PutsTheItemBack` in the same file. | Same file. | Low |
| `SharedFullscreenQuadTests.cs` | `AFullscreenQuad_IsInModelEntitiesButNotInEntities` | 12 | The typed lists of the repository are tested in two other files. | `ControlRepositoryTests.ModelEntities_HoldsEntitiesAndTextsInTheOrderTheyWereAdded`, `ControlRepositoryListTests.AControl_IsInNoOtherList`. | Low |
| `ProjectStateHashTests.cs` | `SameProject_HashesConsistently` | 7 | A subset of the entity test. | `Compute_AProjectWithAnEntity_HashesTheSameTwice` in the same file. | Low |
| `CMiXServiceCollectionExtensionsTests.cs` | `AddCMiXCore_ReturnsTheCollection`, `ISyncTarget_ResolvesToProjectSyncTarget` | 14 | The first tests a fluent return. The second is a registration that 445 `Create()` calls and `EngineHostTests` exercise. | `EngineHostTests.Start_GivesTheProviderAndTheQueue`. | Low |
| `MaterialModelCompatibilityTests.cs` | `ASavedMaterial_HasNoExplodeFields`, `ASavedMaterial_HasNoModifiers` | 16 | The model has no such property, so the serializer cannot write it. The compiler proves it. | None needed. | Low |
| `MaterialModelCompatibilityTests.cs`, `PrefabServiceModelTests.cs`, `ProjectSerializerTests.cs` | `AProjectFileWithTheOldModifiersOfAMaterial_StillLoads`, `AProjectFileWithTheOldExplodeFieldsOfAMaterial_StillLoads`, `AProjectFileWithTheOldFlags_StillLoads`, `ASavedPrefabService_HasNoSelectedFlagAndNoUpdateFlag`, `Load_FileWithTheRemovedOutputVisibility_StillLoads`, `Load_FileWithTheRemovedProjectModel_StillLoads` | 60 | `ProjectSerializer.Options` sets no `UnmappedMemberHandling`, so System.Text.Json skips an unknown key by default. The tests test that default. The owner also said that old save files do not matter during this phase (memory `feedback_saved_projects_dont_matter`). | `ProjectFileCompatibilityTests` opens real files when `CMIX_SAMPLE_PROJECTS` is set. | Medium |
| `UseCompositionResolutionTests.cs` | `Source_ExposesTheParameterThroughTheTextureSourceInterface` | 8 | Pins that the property is declared on `TextureSourceBase` (`DeclaringType`). An implementation detail with no behaviour. | `Source_KeepsTheParameterThroughSaveAndLoad` in the same file proves the behaviour. | Low |
| `ExpandedByDefaultTests.cs` | all 3 | 46 | Pins six `IsExpanded` defaults. These are editor state in Core (ARCHITECTURE.md, section 5). If they move to the Studio, the tests go with them. | None. The owner decides. | Medium |
| `StreamPositionTests.cs` | `Equality_OfTwoParsedEqualValues_IsTrue`, `Zero_IsLessThanFirstEntry` | 14 | Record equality is a language fact. `Zero < (1,0)` is one row of the compare tests. | `CompareTo_DifferentMilliseconds_OrdersByMilliseconds`. | Low |
| `FilteredControlsTests.cs` | `TheList_CannotBeChangedFromOutside` | 7 | Tests that `ReadOnlyCollection.Add` throws. | None needed. | Low |
| `InMemorySyncStoreTests.cs` | all 15 | 222 | Tests the test double, not CMiX code. The double must stay faithful to `RedisSyncStore`, so the right form is one contract theory that runs against both stores (see section 4). | `RedisSyncStoreIntegrationTests` has the same seven tests against Redis. | Medium |
| `WindowsCredentialStoreTests.cs` | `TryRead_UnknownName_ReturnsFalse` | 10 | Reads the real Windows Credential Manager for a random name. Machine dependent I/O for a "not found" path that the empty name test shows too. | `TryRead_EmptyName_ReturnsFalseAndEmptyValues`. | Low |
| `EngineHostTests.cs` or `SyncPeerStartupTests.cs` | `Start_AppliesNothingUntilTheQueueDrains`, `Dispose_StopsThePeerFast_AndClosesTheQueue`, `Start_SetsTheReadBatch` or their three twins `StartAsEngine_WithAQueue_*` | 50 | `EngineHost.Start` calls `SyncPeerStartup.StartAsEngine`. The same three facts are tested twice, once per layer. | The twin in the other file. Keep the `EngineHost` three, because the patch calls `EngineHost`. | Medium |
| `TestNameTests.cs` | none, but lower `PinnedBadNameCount` from 433 to 410 | 0 | 23 names were fixed since the pin was set. The pin only says "not more than". | Same file. | Low |

Sum of the table: about 330 tests and about 3,700 lines go. About 60 of those tests come back as rows of 6 new theories (filters, modifier owners, defaults with reset, every model survives JSON, store contract, Redis `CanBind`).

### 3. Merge candidates

| Target file | Source files | Lines now | Lines after | What the merged file covers |
|---|---|---|---|---|
| `TextureFilterTests.cs` | the 19 template filter files and the first 4 tests of `DisplaceTests.cs` | 1,780 | 90, plus `DisplaceTests.cs` at 75 | One `MemberData` over every `TextureFilterBase` subclass: a modulator binds, a load gives the live `BoundModulator`, a remove unbinds, a move keeps the binding. `DisplaceTests.cs` keeps its five loop tests. |
| `ModifierTests.cs` | 15 `...ModifierTests.cs`, `ControlFactoryModifierCreationTests.cs`, `ModifierVLDiscoveryInterfaceTests.cs` | 1,718 | 300 | A table of modifier types with owners and the `ISpreadableModifier` VL pin. The bindable order of the spreadable modifiers in one theory. Every addable modifier creates, has `Count` 1, disposes, re-resolves its bound modulator after a load, and undoes an assignment in one step. The Gate binding and the name of `ComposableVisibilityModifier`. The old float file of `VisibilityModifier`. |
| `RemoveItemTests.cs` | `DeleteLastItemTests.cs`, `DeleteSelectionRuleTests.cs`, `DeleteUndoTests.cs` (the remove tests), `AddItemUndoCommandTests.cs` | 400 | 220 | Remove of a selected, unselected, middle and last item: selection, wire index, undo, redo, a remove from a peer, `RemoveEverywhere`. The name follows the verb rule of `.claude/CLAUDE.md`: `Delete` does not appear in code. The three `Delete*` class names and 9 method names (`DeletingAnItem...`, `DeleteThenUndo...`) break that rule today. |
| `ResetItemTests.cs` | `PrefabManagerResetGuardTests.cs`, the three `ResetItem_*` tests of `DeleteUndoTests.cs` | 110 | 90 | `ResetItem` swaps a fresh instance, undo restores, drop of the undo entry disposes, null and foreign controls do nothing. |
| `DuplicateItemTests.cs` | `DuplicateEntityTests.cs`, `DuplicateItemTests.cs`, `DuplicateItemCommandTests.cs` | 439 | 240 | One `Kinds()` with `Composition`, `Layer`, `Entity`, `TextEntity`, `FullscreenQuad`, `LightEntity`, `Camera`. The layer only facts (name counts up, foreign list, no undo step for the name, composition keeps the output mapping) stay as facts. |
| `MoveItemTests.cs` | `MoveItemToTests.cs`, `MoveItemToManagerTests.cs`, `LayerSharedItemsTests.cs` | 443 | 330 | `MoveItemTo` in a list, `MoveItemToManager` between compositions, and the shared item rule that blocks a move. |
| `SelectionTests.cs` | `ReceivedSelectionTests.cs`, `SelectionIndexContractTests.cs` | 190 | 150 | A select message from a peer, the index on the wire and in the file. |
| `CollectionSyncTests.cs` | `CollectionSyncTests.cs`, `SelectionMessageWireTests.cs`, `MessageCollectionManagerHandlerTests.cs` | 188 | 150 | Every message the sender makes, its MessagePack round trip, and how a peer applies an add. |
| `SharedItemTests.cs` | `SharedControlTests.cs`, `SharedFullscreenQuadTests.cs`, `SwapListUndoTests.cs`, `FullscreenQuadTests.cs` | 310 | 200 | One control in two lists: the repository count, the peer side, the load, the undo of the swap list, the dispose of a quad with two texture slots. |
| `CompositionIDTests.cs` | `CompositionIDTests.cs`, `EntityMaterialCompositionIDTests.cs`, `ModulatorCompositionIDTests.cs`, `TextureSourceModulationTests.EverySource_GivesItsCompositionIDToItsModulators` | 260 | 170 | How the composition id reaches every control under a composition. `EveryRegisteredControlThatHasACompositionIDReceivesIt` is the generic form. |
| `ControlRepositoryTests.cs` | `ControlRepositoryTests.cs`, `ControlRepositoryListTests.cs`, `ControlListMatchTests.cs`, the regression test of `FullscreenQuadTests.cs` | 437 | 330 | Index by id, the typed lists, no class in two lists, the held collection managers. `ControlRepositoryVlMembersTests.cs` stays as it is: `.claude/CLAUDE.md` names it. |
| `UniqueNameTests.cs` | `UniqueNameTests.cs`, `ControlFactoryTests.cs`, `ControlRepositoryTests.NameControl_StaysUnique_AfterAControlWasRenamed`, `ProjectLoadTests.SaveThenLoad_KeepsNewNamesUniqueAgainstTheLoadedOnes` | 150 | 90 | The Blender style names: new, numbered, typed, renamed, loaded twice. |
| `ModulatableValueTests.cs` | `ModulatableValueFloatTests.cs`, `ModulatableValueIntegerTests.cs`, `ModulatableBoolTests.cs`, `ModulatorOutputSelectionTests.cs`, `IsModulatedTests.cs` | 390 | 200 | One theory over `float`, `int` and `bool`: unbound at start, set and clear, round trip with the output name, `CanBind` by type, `ResetCommand` while modulated, `IsModulated` and its event. |
| `ModulatorTests.cs` | `IModulatorTests.cs`, `FFTModulatorTests.cs`, `TrackingModulatorTests.cs`, `LISAModulatorTests.cs`, `IntegratorModulatorTests.cs`, `VisitorDataModulatorTests.cs`, `ModulatorDefaultsTests.cs`, `ModulatorCategoryTests.cs`, `BeatStepModulatorTests.cs`, `BeatStepsTests.cs`, `AddressedOutputTests.cs` | 926 | 300 | A table of modulator types with their category and output names and types. The category table check that no type is missing. The round trip of every modulator through one helper. The `BeatSteps` id, the step list save, the `Reset` click of the integrator, a filter binds to an integrator or LISA output. |
| `RedisModulatorTests.cs` | `RedisModulatorTests.cs`, `RedisBoolModulatorTests.cs`, `RedisFloatModulatorTests.cs`, `RedisIntModulatorTests.cs` | 329 | 230 | The shared behaviour of the three Redis modulators, plus the range of the float one and `CanBind` on `Playing` and `SeekTarget`. |
| `RedisAddressTests.cs` | `RedisAddressRegistryTests.cs`, `RedisAddressDefaultNameTests.cs`, `RedisAddressRemoveTests.cs` | 316 | 260 | The one registry with its fixed id, the default name, the remove and its undo. |
| `ProjectSnapshotTests.cs` | `ProjectApplySnapshotTests.cs`, `ProjectSyncTargetTests.cs`, the five `ApplyingASnapshot_*` and `ANewProject_HasNoAddress` tests of `ProjectRedisAddressTests.cs` | 290 | 220 | `ApplySnapshot` on a project: order, selection, master beat, name, addresses before compositions, no message, no undo step, blocked messenger stays blocked, a throw restores the state. |
| `ProjectFileTests.cs` | `ProjectLoadTests.cs`, `ProjectSerializerTests.cs`, `ProjectFileCompatibilityTests.cs`, `MaterialModelCompatibilityTests.cs`, `PrefabServiceModelTests.cs`, the two file tests of `ProjectRedisAddressTests.cs` | 700 | 450 | Save and load through a file: ids, names, output slot, asset paths relative and absolute, corrupt file, old `$type`, the sample projects. One `SaveThenLoadInto(target)` helper replaces the 15 line setup that `ProjectLoadTests` repeats five times. |
| `GenericValueTests.cs` | `GenericValueTests.cs`, `SuppressSendingTests.cs` | 202 | 170 | One class, one file: defaults, momentary, suppress sending. One `CountingSender`. |
| `ResetTests.cs` | `DefaultResetTests.cs`, `CompositeResetAllTests.cs` | 240 | 150 | The reset of a value, of a composite value and of a loaded control. The five `CompositeResetAll` tests become one theory. |
| `ComposableInterfaceTests.cs` | `BlendableInterfaceTests.cs`, `ComposableInterfaceTests.cs`, `MaskableInterfaceTests.cs` | 159 | 90 | The three VL pins: `IComposable`, `IBlendable`, `IMaskable`, and that the interfaces return the own settings. |
| `EngineHostTests.cs` | `EngineHostTests.cs`, `SyncPeerStartupTests.cs` | 257 | 150 | The start of an engine with and without a queue, of a studio, the read batch, dispose. |
| `TextureSourceTests.cs` | `TextureSourceModulationTests.cs`, `GradientMeshTests.cs`, `LiquishTests.cs`, `SequencePlayerTests.cs`, `UseCompositionResolutionTests.cs` | 513 | 250 | Every source has a host, keeps its modulators, gives its composition id, keeps `UseCompositionResolution`; the bindables of each source; the players and `DoSeek`; the old `DoSeek` button file. |
| `AssetTests.cs` | `AssetManagerTests.cs`, `AssetTypesTests.cs`, `ImageSequenceAssetTests.cs` | 170 | 120 | The file walks, `ContainsImages`, the image sequence from a path. One `TempDirectoryFixture` instead of two hand made temp folders. |
| `SyncStoreContractTests.cs` | `InMemorySyncStoreTests.cs`, the seven store tests of `RedisSyncStoreIntegrationTests.cs` | 420 | 220 | One theory per store behaviour that runs against `InMemorySyncStore` and, when a server answers, `RedisSyncStore`. The Redis only tests (tail key, client name with a space, cancelled connect, unreachable host, wrong port) stay in `RedisSyncStoreIntegrationTests.cs`. |

Estimated reduction of the merges: about 2,300 lines beyond the removals of section 2. Both together: about 5,800 lines (30 percent of 18,960) and about 75 files fewer (159 to about 85).

### 4. Refactoring opportunities in the test code

#### Duplicated setup

- `ModelWithOneComposition()` is defined in 6 files and called 81 times (`SyncPeerTests.cs`, `SyncPeerStartCheckTests.cs`, `SnapshotCompactorTests.cs`, `SyncPeerRedisIntegrationTests.cs`, `SyncPeerRedisOutageTests.cs`, `ProjectSyncTargetTests.cs`). Move it to `SyncTestHelpers.cs` next to `WriteProjectSnapshotAsync`.
- `new Snapshot(VL.Serialization.MessagePack.MessagePackSerialization.Serialize(model), tail, "other", DateTime.UtcNow)` with the full namespace inline appears 25 times in `SyncPeerTests.cs`. `SyncPeerStartCheckTests.SnapshotOf(model, id)` exists already. Move `SnapshotOf` and `ModelOf(Snapshot)` (3 copies) to `SyncTestHelpers.cs`.
- The gate pattern `reached` and `release` as two `TaskCompletionSource<bool>` with `store.BeforeX = async () => { reached.TrySetResult(true); await release.Task; }` appears 15 times in `SyncPeerTests.cs` and 6 times in `SnapshotCompactorTests.cs`. A small `StoreGate` class with `Reached`, `Release()` and `Hold()` cuts about 150 lines and gives the pattern a name.
- `CreateActiveDouble(provider)` is in `UndoManagerTests.cs`, `SuppressSendingTests.cs` and `ValueInteractionTests.cs`. `CountingSender` is in `GenericValueTests.cs` and `SuppressSendingTests.cs`. `ValueInteractionTests.RecordingSender` is a third variant. One `RecordingMessageSender` exists already in its own file. Extend it with a `Count` of `MessageValueChanged` and use it everywhere.
- `GetRequiredService<ControlFactory>().Create(typeof(X))` appears 71 times in 39 files, often with a cast on the same line. The memory note `project_defaults_live_in_models` says a test that checks a default must create through the factory. A helper `TestControls.Create<T>()` makes the rule visible and short.
- `AddAddress(text)` is in 5 files (38 calls) with two different shapes (on a registry, on a project).
- `SyncPeerRedisIntegrationTests.cs` and `SyncPeerRedisOutageTests.cs` carry the same six helpers (`NewPeer`, `StartStudioAsync`, `StartEngineAsync`, `StartWriterAsync`, `ObserverStoreAsync`, `ModelWithOneComposition`) plus `PeerSnapshot` and `PeerIds` (two copies of the same retry loop). A `RedisPeerHarness` in `SyncTestHelpers.cs` or a shared base class removes about 120 lines.
- `CreateFactory()` is defined in 6 files. `Create()` that returns one typed control is defined in 14 files.

#### Helpers that should exist

- A generic round trip for every `IControl` with a model, not only filters and modifiers. `FilterAndModifierRoundTripTests.Types()` filters on `TextureFilterBase` and `Modifier`. Widen it to every non abstract class that implements `IControl` and has a `...Model` type, with a skip list for the few that need a container (`Project`, `MasterBeat`). `ValueWalker` already walks any CMiX type. This one change makes about 30 hand written round trips redundant (the modulators, the sources, `MaterialSettingsTests`, the players).
- A generic reset test with `ValueWalker`: create, change every value, call `Reset` on every value, compare with a fresh control. It replaces `ModulatorDefaultsTests`, the fresh default tests and the `ConstructAtDefault` tests.
- A `[Theory]` over every model type that serializes with `ProjectSerializer.Options` and reads back equal. It replaces the five `Serialize_ThenDeserialize_KeepsTheModel` copies and guards the `$type` converter for every type.
- `ValueWalker` collects only `Value`. It skips `ModulatorID` and `BoundOutputName` of a `ModulatableValue`. Add them as two more walked values, and the binding round trips of the modifiers become generic too.

#### Helpers that nothing uses, or that are used once

- `ControlRepositoryVlMembersTests.Create()` returns a tuple of four, and the one test reads one member.
- `WrappingSyncStore.CallThreads` is used by two tests. Its `ConcurrentBag<int>` grows with every store call in every test that uses the wrapper. Fine for now, but a counter per thread would be lighter.
- `HeldCollectionManagers.Count` is used in two files. Fine, but it belongs in `SyncTestHelpers.cs` style: one helpers file for the repository.
- `ValueWalker.SkippedSuffixes` holds `"Data"`, which skips `ManagerData` and so every item of a list. This is on purpose for the filters. The widened round trip (above) must walk the list items of a control or test them one by one.

#### Big files to split by subject

- `SyncPeerTests.cs` (1,865 lines, 56 tests) mirrors the nine partial files of `SyncPeer` (ARCHITECTURE.md, section 2). Split it the same way: `SyncPeerJoinTests.cs` (join, start as engine or studio, `SnapshotApplied` events: 12 tests), `SyncPeerRecoveryTests.cs` (`Recovery_*`, `TickJoin_*`, `AfterALongPause...`, `AnIdleTick...`, `Reconnect_DuringAFollowerRecovery...`: 9), `SyncPeerRestartTests.cs` (`RestartDuring*`, `Start_TwiceBefore...`, `Pull_AfterARestart...`, `PresenceTick_*`, `AfterARestart_AnEventOfTheOldStore...`, `JoinAndStop_AtTheSameTime...`: 12), `SyncPeerFollowerTests.cs` (order, duplicates, corrupt entries, batches: 10), `SyncPeerStopTests.cs` (`Start_AndStop_ReturnAtOnce...`, `Stop_ResetsThePositions...`, `Stopped_Completes...`, `StoreCalls_NeverRunOnTheDispatcherThread`, `LostConnection...`, `OnConnectionChanged...`: 6), and the two writer tests (`Push_WhileEditsAreQueued...`, `Outgoing_KeepsOrder`) into `SyncPeerSendTests.cs`. Move `UnreliableSyncStore` and `LingeringSyncStore` (private classes at the bottom, 85 lines) to `SyncTestHelpers.cs`, where the other test doubles live.
- `SyncTestHelpers.cs` (374 lines) holds six types. `.claude/CLAUDE.md` names it as the home of the test doubles, so the location is right, but one type per file in a `Sync` sub folder would follow the rule "a file has the name of its type".
- `SnapshotCompactorTests.cs` (491 lines, 17 tests) is one subject. Fine as it is after the gate helper.

#### Slow or fragile patterns

- `Task.Delay` as a negative assertion: "wait 300 ms, then assert that nothing more happened" appears about 25 times (`SyncPeerTests.cs`, `SnapshotCompactorTests.cs`, `SyncPeerStartCheckTests.cs`, `EngineHostTests.cs`, `SyncPeerStartupTests.cs`). With parallel runs off, these waits add up to about 12 seconds of pure sleep in the fast suite. They are also the first tests to fail on a loaded machine. Where possible, wait for the positive condition with `WaitUntilAsync` and then settle for one tick, or expose a counter on the loop (`PendingMessages` style) and wait for it.
- Repeat loops to catch a race: `SyncPeerStartCheckTests.ConnectionEventsDuringTheStartCheck_PushOnce` runs 10 times with a 300 ms wait each (3 seconds minimum). `SyncPeerTests.JoinAndStop_AtTheSameTime_ThrowNothing` runs 20 times. They are honest about what they do, but they are the slowest unit tests.
- Timing asserts: `Start_AndStop_ReturnAtOnce_WhenTheStoreNeverConnects` and `Stop_ResetsThePositionsThroughTheDispatcher` assert `< 50 ms`. `InMemorySyncStoreTests.ReadBlockingAsync_ReturnsAtOnce_WhenEntriesExist` asserts `< 1000 ms`. `RedisSyncStoreIntegrationTests.ConnectAsync_ToAnUnreachableHost_ReturnsControlAtOnce` asserts `< 200 ms` and `< 2500 ms`.
- Real network without a skip: `RedisSyncStoreIntegrationTests.ConnectAsync_ToAnUnreachableHost_ReturnsControlAtOnce` connects to `10.255.255.1` and `ConnectAsync_ToAWrongPort_ShowsTheReason` to port 6399. Both are `[Fact]`, not `[SkippableFact]`. They run in the `Redis` category, so the fast filter skips them, but a run of the Redis category without a server still waits for them.
- Real time in the outage tests: `SyncPeerRedisOutageTests.cs` has `Task.Delay(5000)`, `Task.Delay(15000)` and `Task.Delay(3000)`. The three tests take over a minute together. They skip when Memurai is absent, so the cost is only on the owner machine.
- Real files: `BlendModeShaderTests.FindShader` walks up from `AppContext.BaseDirectory` to find `CMiX.Engine/shaders/CMiXBlendOperations.sdsl`. It fails when the test runs from a copy of the binaries. `AssetManagerTests.cs` and `AssetTypesTests.cs` make their own temp folder with `Path.Combine(Path.GetTempPath(), ...)` instead of `TempDirectoryFixture`.
- Real Windows state: `WindowsCredentialStoreTests.cs` reads the Credential Manager of the machine.
- Thread.Sleep: only in `SyncPeerRedisIntegrationTests.PeerSnapshot` (5 ms retry) and `PrivateMemurai.WaitForPort`. Both are in test doubles and are fine.

#### Assertions that check too much or too little

- `UndoManagerTests.TrimStack_KeepsOnlyMaxStepsAndDropsOldestEntriesFirst` hard codes `maxSteps = 128`. If `UndoManager` exposes the limit, read it.
- `SyncPeerRedisOutageTests.Peers_SurviveAnOutage_ThatLosesTheData` writes nine lines of diagnostics and asserts only `IsConnected`, a snapshot exists and the late engine has the same hash. It does not assert what happened to the pending messages. The comment in the test says why: the data is lost, so the result is open. Fine, but it reads as a probe, not a test.
- `ModulatorCategoryTests.Category_EveryConcreteModulatorType_HasAnEntryInTheTable` and `ControlListMatchTests.NoControlClass_BelongsToMoreThanOneTypedList` are good scans. `ControlListMatchTests` also asserts `controlClasses.Count > 50` so a broken scan fails. Copy that guard to the other scans (`FilterAndModifierRoundTripTests.Types()`, `UseCompositionResolutionTests.SourceTypeNames()`, `ControlFactoryModifierCreationTests.EveryAddableModifier...`). An empty `MemberData` passes silently.
- No test has zero assertions. `EngineHostTests.Dispose_BeforeStart_DoesNothing` and `Dispose_Twice_DoesNothing` assert only by not throwing. Wrap them in `Record.Exception` so the intent is visible.
- `ComposableVisibilityModifierTests.IsNamedVisibilityInTheListsAndOnTheNewItem` asserts two strings in one test (`"Visibility"` and `"Visibility Modifier"`). Two facts, two tests, or one theory.

#### Names that break `Method_Scenario_Result`

410 of 850 names break the rule. The 20 filter files account for 80, the 15 modifier files for 85. After the merges of section 3, most of them vanish, because the theories get new names. The remaining heavy files are `SyncPeerTests.cs` (26), `FilteredControlsTests.cs` (12), `SnapshotCompactorTests.cs` (8), `ControlRepositoryTests.cs` (8), `ProjectRedisAddressTests.cs` (7), `InMemorySyncStoreTests.cs` (7). Examples of the patterns to fix:

- Two parts only: `Outgoing_KeepsOrder`, `Snapshot_RoundTrips`, `Fresh_ValueIsOne`, `ItStartsWithNoStep`.
- No underscore: `KeepsItsValueAndBindingThroughTheModel`, `IsOfferedForOwner`, `ItStartsEmpty`.
- Four or more parts: `Push_Then_Undo_RestoresPriorState`, `Modulatable_ToModel_FromModel_RoundTripsValueAndBinding`, `Peers_SurviveAnOutage_AndCatchUp`.
- A verb that the rule bans: `DeletingAnItemThatIsNotSelected_...`, `DeleteThenUndo_...` (9 names). `.claude/CLAUDE.md`: do not use Delete in the code.
- A label instead of a method: `Regression_MergedPushAfterUndo_ClearsRedoStack` in `UndoManagerTests.cs`.

Lower `TestNameTests.PinnedBadNameCount` after each cleanup step, so the number cannot grow back.

#### Class name and file name

- Every test class has the name of its file. The helper files hold extra types: `RedisFixture` lives in `RedisSyncStoreIntegrationTests.cs`, `PrivateMemurai` in `SyncPeerRedisOutageTests.cs`, `WalkedValue` in `ValueWalker.cs`, and `RecordingSyncTarget`, `HangingSyncStore`, `WrappingSyncStore`, `ReconnectingSyncStore`, `SingleThreadDispatcher` in `SyncTestHelpers.cs`. The rule "a file has the name of its type" wants one file each. `RedisFixture` and `PrivateMemurai` are shared by two test classes, so they should move out of a test file in any case.

#### Comments

- Four files carry narrative comments with commit hashes or audit labels: `CollectionManagerTests.cs` ("commit 59a147eb", and the guard it names is gone), `UndoManagerTests.cs` ("commit 56258859"), `ControlFactoryTests.cs` ("D5 of the post parity audit", "post 0b39de2a"), `DeleteUndoTests.cs` (a four line header on ownership). `.claude/CLAUDE.md` asks for brief comments without history. Trim them when the files are touched.

### 5. Coverage gaps worth a test

1. A generic round trip for every `IControl` with a model (modulators, sources, entities, cameras, lights, material, settings). Today a forgotten `ToModel` line in `LFOModulator` or `Camera` fails no test.
2. A generic `Reset` test over every control with `ValueWalker`: a value whose default the factory did not capture goes to zero on Reset, and no test sees it except for the few types with a hand written default test.
3. The VL names that `.claude/CLAUDE.md` lists: `ManagerData.Items`, `ManagerData.ID`, `ManagerData.SelectedIndex`, `PrefabManager.SelectedItem`, `PrefabManager.ManagerData`, `ControlRepository.GetControl`. Only `ControlRepository.Controls` is pinned. One reflection test with six rows.
4. `MoveItemUp` and `MoveItemDown` live in `ManagerReorderService` and in `ProjectTree.Commands`, not in `PrefabManager`. `ReorderThroughManagerTests` covers the service: one undo step, one `MessageMoveItem`, stop at the edges.
5. `PrefabManager.ClearSelectionCommand` and `ClearSelectionUndoCommand`: the sender side is tested (`CollectionSyncTests.SendClearSelection...`), the command and its undo step are not.
6. `OutgoingQueue` on its own: order, the count that `PendingMessages` reads, and the drop on a restart. Today only `SyncPeerTests` reaches it through the peer.
7. The modulators `LFOModulator`, `RandomModulator`, `BeatLFOModulator`, `BeatRandomModulator`: outputs and their types, in the output table of the merged `ModulatorTests.cs`.
8. `OrbitModifier`, `TargetModifier`, `ZoomModifier`, `RandomModifier`, `SplitModifier`, `BillboardModifier`, `SetAlpha`: a row each in the owner table and the generic round trip. No other test is needed.
9. `MasterBeat` behaviour beyond its defaults: `Periods`, the period index, a tap. `DefaultResetTests.MasterBeat_StartsWithTheModelDefaults_AndResetRestoresThem` is the only test.
10. `ProjectSyncTarget.Apply` with a value message: it posts on `WeakReferenceMessenger.Default`. One direct test next to the snapshot tests, so the sync group does not depend on `SyncPeerTests.ValueChange_FromAnotherPeer_ReachesControlInRealProject` alone.

### 6. Top 10 actions by value for effort

1. Replace the 20 per filter files with one theory over every `TextureFilterBase` subclass. Removes 80 tests and about 1,650 lines in one step, adds `SetAlpha` to the coverage. Risk low. Effort: one file, half a day.
2. Widen `FilterAndModifierRoundTripTests` to every `IControl` with a model, and let `ValueWalker` also walk `ModulatorID` and `BoundOutputName`. Then remove the 30 hand written round trips (modulators, sources, players, `MaterialSettingsTests`, the modifier binding round trips). Closes gap 1. Effort: one day.
3. Add one generic Reset theory with `ValueWalker` and remove `ModulatorDefaultsTests.cs`, the five "fresh matches old default" tests of `ControlFactoryModifierCreationTests.cs`, and the default pins in `Pixelate`, `Pulse`, `FFT`, `LISA`, `RedisFloat`, `GradientMesh`, `Liquish`, `SequencePlayer`, `Color`, `Scale`, `Grid`. Closes gap 2. Removes about 25 tests and 350 lines.
4. Merge the 15 modifier files, `ControlFactoryModifierCreationTests.cs` and `ModifierVLDiscoveryInterfaceTests.cs` into `ModifierTests.cs` with an owner table and an output table. Removes the 12 `IsAlsoAnIModifier`, the 14 attribute tests and the 6 `CanAddRandomModulator`. About 1,400 lines go, 60 bad names go.
5. Merge the list files by verb: `RemoveItemTests.cs`, `ResetItemTests.cs`, `DuplicateItemTests.cs`, `MoveItemTests.cs`, `SelectionTests.cs`, `SharedItemTests.cs`, `CompositionIDTests.cs`. One search for the verb then finds the test file, as `.claude/CLAUDE.md` wants. This also removes the word `Delete` from the test code. About 700 lines go.
6. Move the shared sync helpers into `SyncTestHelpers.cs`: `ModelWithOneComposition`, `SnapshotOf`, `ModelOf`, a `StoreGate`, `UnreliableSyncStore`, `LingeringSyncStore`, and a `RedisPeerHarness` for the two Redis integration files. About 450 lines of duplication go, and `SyncPeerTests.cs` loses its two private store classes.
7. Split `SyncPeerTests.cs` into six files that follow the nine partial files of `SyncPeer` (join, recovery, restart, follower, stop, send). No test changes. The owner can then read one subject at a time.
8. Turn `InMemorySyncStoreTests.cs` and the seven store tests of `RedisSyncStoreIntegrationTests.cs` into one `SyncStoreContractTests.cs` theory that runs against both stores. The double stays faithful by construction. About 200 lines go.
9. Merge the modulatable value files into `ModulatableValueTests.cs` with a theory over `float`, `int` and `bool`, and the modulator files into `ModulatorTests.cs` with an output and category table. Delete `AddressedOutputTests.cs` and `IModulatorTests.cs`. About 800 lines go, and gaps 7 and 8 close with table rows.
10. Replace the 25 "wait then assert nothing happened" `Task.Delay` calls with a wait for the positive condition plus one settle tick, fix the three asserts that measure wall clock time, and lower `TestNameTests.PinnedBadNameCount` to the real number after each step. The fast suite loses about 12 seconds of sleep and the flaky tail.

Low value, do not start with them: the single small files that are already clean (`DerivedIdTests.cs`, `MainloopQueueTests.cs`, `ListRulesTests.cs`, `StreamPositionTests.cs`, `SyncOptionsTests.cs`, `ManagerDataTests.cs`), the processor files, `UndoManagerTests.cs` and `ValueInteractionTests.cs`.

Open points for the owner, not decided here:

- `ExpandedByDefaultTests.cs` pins editor state that lives in Core. Keep until the state moves, or drop now?
- The six "old key still loads" tests: keep as documentation of the file format, or drop because old files do not matter in this phase?
- `ControlRepositoryVlMembersTests.cs` keeps its name because `.claude/CLAUDE.md` names it. The other VL pins (`ISpreadableModifier`, `IComposable`, `IBlendable`, `IMaskable`) could join it in one `VlContractTests.cs`. That is a rename that needs a plan.

# Part B: CMiX.UI.Tests


Date: 2026-11-24. Branch: refactor/tree-structure. Read only. Every test file was read in full. Every theme key, token and control that a test names was checked against CMiX.UI with grep. The users of each control were counted in CMiX.Studio.Avalonia and CMiX.UI.Gallery.

### 1. Overview

#### Counts

| What | Count |
|---|---|
| Files in CMiX.UI.Tests | 118 .cs: 109 test files, TestAppBuilder.cs, 8 files in Support |
| Lines | 13,945 |
| Test methods | 695: 589 [AvaloniaFact], 62 [AvaloniaTheory], 39 [Fact], 5 [Theory] |
| Theory rows | 174 [InlineData] plus 10 [MemberData] theories |
| Test names that break Method_Scenario_Result | 222 of 695. The pin in TestNameTests.cs is 249 |
| Files where every test name breaks the rule | 17 |
| Files with one test | 10 |
| Files over 300 lines | 5: CaptureLostTests 906, DragCursorWrapTests 433, ListGroupedSearchTests 432, ListBoxEditableTests 384, TabCyclingTests 318 |
| Private `Show` or `Host` helpers that make a window | 62, with 133 `new Window {` in total |
| Raw `Application.Current!.FindResource` casts | 93 in 40 files, next to 65 `Tokens.Get` calls |

#### How the suite is organized

- One flat folder, one namespace `CMiX.UI.Tests`. No sub folders by subject.
- `TestAppBuilder.cs` holds the test application: SimpleTheme, the icon styles, CMiXTheme and `OutsideClick.Install()`. A control that needs more fails in `ThemeTests`.
- `Support/` holds `PointerInput` (press, release, click on a control), `ManualTimeProvider` (a clock for the notification), `Tokens` (reads a token and the list sizes), `CursorSpy` and `NoRealCursor` (take the real cursor out of the drag tests), `FakeStorageItem` (a file drag), `StringValue` and `NumberValue` (models for the editors). `PointerInput.cs` and `ManualTimeProvider.cs` are linked into CMiX.Studio.Avalonia.Tests (its csproj, lines 26 and 27).
- The files follow three shapes. A file per control (`ButtonIconTests`, `TextBoxSearchTests`). A file per behavior or rule (`OutsideClickTests`, `TabCyclingTests`, `CaptureLostTests`). A file per look (`*LookTests`, `StateTokensTests`, `RowInsetTests`). The three shapes overlap: one control is tested in up to six files (TextBoxEditable), and one look is tested in up to four files (the shortcut text).
- Almost every file came in one commit for one feature or one fix (see the git history in the tables below). Nobody merged them after the feature landed. That is the root of the size.
- The guard tests that `docs/DEVELOPMENT.md` names and that live here: `ThemeTests`, `LibraryBoundaryTests`, `ValueResetMenuTests`, `ScaleTokensTests`, `ScrollStabilityTests`, `ToggleButtonPowerTests`, `ComboBoxCaptionedSelectionTests`, `InteractionHostTests`. They stay. The other guards that the document names (`SizeLiteralTests`, `ResourceReferenceTests`, `PopupThemeGuardTests`, `PopupWiringGuardTests`, `ToolTipSourceTests`, `IconKeyTests`, `ResetMenuTests`, `CollectionPanelContractTests`) live in CMiX.Studio.Avalonia.Tests and are out of scope.

#### What the tests cover well

- The pointer behavior of the value editors: drag, capture lost, the cursor that comes back, the wrap at the edge, Shift and Ctrl speeds, edit mode, Tab between editors (`CaptureLostTests`, `DragCursorWrapTests`, `TabCyclingTests`, `PopupEditModeTests`, `InteractionHostTests`).
- The popup family: open and close with a toggle, close on pointer away in all three shapes, a press outside, the placement rule and its use for tooltips, popups and menus, the card and its tip (`TogglePopup*`, `ClosePopupBehaviorTests`, `CloseFlyoutOnPointerAwayBehaviorTests`, `ContextMenuPointerAwayTests`, `OutsideClickTests`, `Popup*`, `ToolTipDefaultsTests`).
- The value menu: title, Copy, Paste, Reset, Reset All, the row under the pointer, the side content that takes no menu (`ResetMenuTitleTests`, `ValueMenuCopyPasteTests`, `ValueResetMenuTests`, `VectorResetMenuTests`, `MenuAnchorTests`, `SideContentContextMenuTests`, `SelectorColorMenuTests`).
- The lists: `ListBoxEditable`, `ListBoxRepository`, `ListGroupedSearch`, `ListPanelItem`, `PanelItem`, the scroll step and the resize strip.
- The design rules as measured values: the state tokens on buttons, toggles, drag values and rows; the two icon opacities; the on state text; the row inset; the scale of the sizes (`StateTokensTests`, `IconOpacityTests`, `OpacityTokenUseTests`, `OnStateLookTests`, `RowInsetTests`, `ScaleTokensTests`, `GeometryTokenTests`).
- The pure rules: `PopupPlacement`, `ColorMix`, `KeyGestureTextConverter`, `TipContent`, `PopupTip` geometry.

#### What the tests do not cover

- `TextBoxIP`, `EditableValue`, `SelectOnFocus`, `NumbersOnly`, `NumericInput`, `NumericTextHelper`: no test.
- `AxisSelector2`, `AxisSelector3`, `Integer2`, `Integer3`, `Vector2`, `Vector3`: only the Reset menu is tested (`VectorResetMenuTests`), not the bindings or the layout.
- 13 converters in `CMiX.UI/Converters`: only `KeyGestureTextConverter` has a test.
- `StackPanelPositioned` and `UniformGridPositioned`: no test. `DockPanelPositioned` has one.
- `ListBoxReorder` and `IReorderService` drag reorder: no test in this project.
- The color math: `ColorSpaceHelper`, `ObservableColor`, `ColorState`, `ColorToHexConverter`: no unit test. The wheel is tested only through the pointer.
- `RunCommandAfterClose`, `PopupOwner`, `MouseDown`, `RoundedClip`, `DragHandle` (only through `PanelItem`): no direct test.
- `Icons.Register`, `ThemeResource`, `AppIcon` with an unknown key: no test here (`IconKeyTests` is a Studio test).
- `ColorPickerFull` and `ColorPickerWheel` have no Studio user (grep: 0 in Studio and Gallery). `SelectorColor` uses them. The tests cover the wheel drag but not the full picker.

### 2. Remove candidates

Tests that `docs/DEVELOPMENT.md` names are not in this table. "Tests" is the number of test methods. For a part of a file the table names the methods.

| File (relative to the repository) | Tests | Lines | Reason | Risk |
|---|---|---|---|---|
| `CMiX.UI.Tests/RowStripesTests.cs` | 9 | 133 | Tests a control that no code uses. `RowStripes` has no user in CMiX.Studio.Avalonia, CMiX.UI.Gallery or the CMiX.UI themes (grep `RowStripes`: only `CMiX.UI/Controls/Parts/RowStripes.cs`). History: d3279004d removed the stripes behind the tree, 0abebfcc3 restored the control, nothing used it after that. Remove the test with the control, or ask the owner if the control has a future. | low |
| `CMiX.UI.Tests/CursorRowTests.cs` | 5 (12 cases) | 118 | Tests an attached property that no view uses. `CursorRow`, `CursorPseudoClass` and the class `cursor` have no user in the Studio (grep: 0). The old tree view that set it is gone (5326a5e66). The theme `CursorRow.axaml` and the `CursorFrame` border in `ListBox.axaml` stay in CMiX.UI with no setter. Ask: the new tree may want a cursor row. | medium |
| `CMiX.UI.Tests/CursorAndOpacityTokensTests.cs` | 5 (9 cases) | 51 | Pins token values with no design reason: `OpacityDisabled` 0.5, `OpacityDragSource` 0.4, `OpacityDragGhost` 0.8, `WarningMutedBrush` #FFB4685C, the two cursor thicknesses. `OpacityDragSource` and `OpacityDragGhost` have no user anywhere (grep: only `Geometry.axaml`). The cursor tokens have no user outside the dead `CursorRow` theme. The tokens with a reason are tested where they are used: `OpacityIconIdle` and `OpacityIconAction` in `IconOpacityTests` and `OpacityTokenUseTests`, `OpacityDisabled` in `OpacityTokenUseTests`. | low |
| `CMiX.UI.Tests/GeometryTokenTests.cs`, methods `TreeIndentMargin_Token_IsTheTreeIndentAtTheLeft` and `TreeSpaces_Tokens_AreSpacingTokens` | 2 of 6 | about 20 | Test tokens of the removed tree view. `TreeIndent`, `TreeIndentMargin`, `TreeMargin`, `TreeRowContentMargin`, `TreeHeaderMargin`, `TreeToolbarPadding` have no user outside `Geometry.axaml` (grep: 0 in Studio and CMiX.UI styles). The new tree has its own `ListTree*` tokens, tested in `ListTreeTokensTests`. Keep the four scroll bar and splitter tests. | low |
| `CMiX.UI.Tests/PopupTipTests.cs`, method `Tokens_OfTheCard_AreTheDarkNeutralColors` | 1 of 10 | 9 | Pins the color #18181B of `PopupBackgroundBrush` and asserts that the removed token `PopupCardBorderBrush` is unset (the token left in 120a500b2). A guard for a token that is gone. The frames that read the token are tested in `DropdownFrameTests.ThePanel_HasTheAddPopupColor`, `PopupCardTests.Card_InAWindow_HasTheLookOfTheTokens`, `ToolTipLookTests.Show_ATooltip_HasTheLookOfTheCard` and `ContextMenuRowLookTests.Open_AMenu_ShowsItsRowsOnTheCard`. | low |
| `CMiX.UI.Tests/StateTokensTests.cs`, method `PopupBackground_IsOneToken_ThatTheFramesRead` | 1 of 13 | 5 | Pins the same #FF18181B a second time. Same cover as the row above. | low |
| `CMiX.UI.Tests/ShortcutTextLookTests.cs`, theory `ContrastRatio_TextOnThePopupCard_HasItsValue` | 1 (3 cases) | 10 | Pins the contrast ratios 4.835, 4.499 and 2.827 to three decimals. The design rule is the AA floor of 4.5, and `ContrastRatio_ShortcutTextOnThePopupCard_ReachesWcagAA` in the same file tests that. The `Grey800` and `Grey700` rows pin colors that the shortcut does not use. | low |
| `CMiX.UI.Tests/MenuMinWidthTests.cs`, method `TheToken_IsTwoHundred` | 1 of 3 | 5 | Pins `PopupMinWidth` to 200. `AShortMenu_HasTheLeastWidth` in the same file and `TogglePopupWidthTests.ThePopupContent_IsAtLeastTheMinimumWidth` already measure 200 on a real popup. Both of those should read the token, not the number. | low |
| `TheDistance_IsSixtyFourByDefault` in `CMiX.UI.Tests/ClosePopupBehaviorTests.cs`, `CMiX.UI.Tests/CloseFlyoutOnPointerAwayBehaviorTests.cs` and `CMiX.UI.Tests/ContextMenuPointerAwayTests.cs` | 3 | 15 | The same default, 64, pinned three times. The three shapes share `PointerAwayCloser` (DEVELOPMENT.md, "Close when the pointer goes away"). Keep one, on the class that owns the number. | low |
| `CMiX.UI.Tests/ListItemIconTests.cs`, method `TheItem_HasOnlyTheVisibilityToggle` | 1 of 4 | 7 | A regression guard for a toggle that is gone. The update toggle was hidden in a819b4a16 and its flag removed in ae2a9fd62. `ListItemIcon.axaml` has one toggle (line 22). The guard protects code that does not exist. | low |
| `CMiX.UI.Tests/ExpanderAnimationTests.cs` | 3 theories (9 cases) | 81 | Duplicates `PanelCollapseTests` through the three expander themes. `AClosedExpander_HidesItsContent` is `PanelCollapseTests.AClosedPanel_HasNoHeight` and `AnOpenPanel_HasTheHeightOfItsContent`. `Opening_IsAnAnimation` is `PanelCollapseTests.Opening_ShowsTheContentAtOnce_AndTheHeightGrowsWithAnAnimation`. `AfterTheAnimation_TheExpanderIsOpenOrClosed` is `PanelCollapseTests.AfterTheClosing_TheContentIsHidden`. What is not covered elsewhere: that each expander theme has one `PanelCollapse`. One theory of three lines keeps that. | medium |
| `CMiX.UI.Tests/FontSizeTests.cs`, method `ChangingTheToken_ChangesTheText` | 1 of 2 | 18 | Tests that a `DynamicResource` follows a change of the resource. That is Avalonia, not CMiX code. The first test of the file (the text reads `FontSizeMD`) is the CMiX rule. | low |
| `CMiX.UI.Tests/ExpanderDefaultTests.cs`, method `Show_AValueOrABinding_CanCloseAnExpander` | 1 of 3 | 9 | Tests that a local value and a binding beat a default value. That is Avalonia. The other two tests (open by default) are the CMiX rule from 106af92f0. | low |
| `CMiX.UI.Tests/TextBoxEditableEmptyTextTests.cs`, method `Enter_WithARealText_CommitsIt` | 1 of 6 | 9 | Duplicate of `TextBoxEditableCancelTests.Enter_DuringAnEdit_CommitsTheText`. | low |
| `CMiX.UI.Tests/TogglePopupTests.cs`, method `APointerFarAway_ClosesThePopup_AndUnchecksTheToggle` | 1 of 6 | 13 | Duplicate of `TogglePopupCloseTests.ByDefault_ThePopupClosesWhenThePointerGoesAway`, except for the toggle check, which `OpeningAndClosingThePopup_ChecksAndUnchecksTheToggle` in the same file already covers. | low |
| `CMiX.UI.Tests/DropdownFrameTests.cs`, method `ThePanelOfAComboBox_StartsAtTheLeftEdgeOfTheComboBox_AndIsAsWide` | 1 of 10 | 14 | Same setup and same width assertion as `TheComboBoxDropdown_HasTheSameFrame_AndItsPanelIsAsWideAsTheComboBox` in the same file. Keep the left edge assertion in the one that stays. | low |
| `CMiX.UI.Tests/MainMenuRowLookTests.cs`, method `Down_InAContextMenu_MakesTheShortcutLighterAndReadable` | 1 of 5 | 14 | A context menu test in a main menu file. The fill part is `ContextMenuKeyboardRowTests.Down_InAnOpenMenu_GivesTheRowTheHoverFill`. The shortcut color part is new and belongs in the context menu look file. Move, do not drop. | low |
| `CMiX.UI.Tests/ValueMenuCopyPasteTests.cs`, method `TheMenu_HasCopyAndPaste_AfterTheTitle` | 1 of 11 | 7 | The row list is a sub set of `ResetMenuTitleTests.WithResetAll_TheMenuEndsWithResetAndResetAll`. | low |
| `CMiX.UI.Tests/ToggleButtonPowerTests.cs`, rows for `ToggleButtonUpdate` and `ToggleButtonPlayPause` | 0 files, 5 theory rows and 1 fact | about 20 | Not a removal of the guard. A note: the themes `ToggleButtonUpdate` (user: the Gallery only) and `ToggleButtonPlayPause` (no user, grep: only `ToggleButton.axaml`) are dead in the Studio. If the owner removes the themes, the rows go with them. | medium |

Sum of the table: about 32 test methods and about 560 lines, of which 302 lines are the three whole files (`RowStripesTests`, `CursorRowTests`, `CursorAndOpacityTokensTests`).

Checked and kept, with a note:

- `CMiX.UI.Tests/ColorPickerSlidersTests.cs` (1 test, 54 lines): the class `hiddenMode` still exists in `ColorPickerSliders.axaml` (lines 37, 43, 50), so the guard still guards live code. The comment is history (names commit fc493d82, says "used to be") and breaks the comment rule. The test belongs in a ColorPicker test file. See section 3.
- `CMiX.UI.Tests/ListTreeTokensTests.cs`, theory `Brush_EachKey_HasTheColourOfTheSpec`: pins 12 hex values. It is one day old (2026-11-23) and tracks the spec in `docs/tree-port-plan.md` for the tree that is under construction. Keep for now. Six of its rows repeat what `Surface_EachOpaqueKey_IsWhiteOverTheTreeBackground` derives. Revisit when the tree lands.
- `CMiX.UI.Tests/HoverTargetTests.cs`: `Slider_HoveringTheBody_LightsTheTrack` sets `:pointerover` by hand because the headless pointer does not reach the body. It tests the selector, not the hit test. It still guards the rule of bc4941949 (the hover of the slider is the body, not the side content). Keep.
- `CMiX.UI.Tests/ScrollStabilityTests.cs`, `ASingleScrollViewer_CanTurnTheScrollOnFocusBackOn`: tests Avalonia, but on purpose: it proves that the test next to it can fail. DEVELOPMENT.md names the file. Keep.

### 3. Merge candidates

The estimate of saved lines comes from the shared parts: one `Show` helper for one window, one set of part finders, one set of usings and one class header. About 25 to 35 percent of a merged group.

| Group (files, tests, lines) | Target file | What the merged file covers | Saved lines |
|---|---|---|---|
| `TextBoxEditableCancelTests` 67, `TextBoxEditableContextMenuTests` 116, `TextBoxEditableDoubleTapTests` 120, `TextBoxEditableEmptyTextTests` 108, `TextBoxEditableLayoutTests` 46, `TextBoxEditableSuffixTests` 85 (26 tests, 542 lines) | `TextBoxEditableTests.cs` | One `Show(text, suffix)` that returns the box, the `textDisplay` block and the `inputValue` box. Sections: start of the edit (double tap, double click), end of the edit (Enter, Escape, lost focus, detach, empty text), the menu of the row in edit mode, the layout (text does not move, suffix follows the text). The six files each have their own Show and their own part finders today. | about 150 |
| `ContextMenuTests` 164, `ContextMenuPointerAwayTests` 145 (15 tests, 309 lines) | `ContextMenuTests.cs` | The behavior of the Reset menu: opens on a context request, not in edit mode, not from the popup, closes on pointer away, the companion, the distance, the switch. Both files make a DragValue with a ResetCommand and a `RequestMenu` helper. | about 80 |
| `ContextMenuLookTests` 123, `ContextMenuRowLookTests` 159, `ContextMenuSubmenuLookTests` 56, `ContextMenuKeyboardRowTests` 95, plus `MainMenuRowLookTests.Down_InAContextMenu_MakesTheShortcutLighterAndReadable`, `ShortcutTextLookTests.Open_TheShortcutOfAContextMenuRow_HasTheShortcutLook` and `RowInsetTests.Show_ARowOfAMenu_StartsItsContentAtEight` (20 tests, about 470 lines) | `ContextMenuLookTests.cs` | The look of a menu with `ContextMenuDefault`: the card, the separator row, the centre on the click, the rows (shortcut, arrow, title, danger, disabled), the keyboard row fill and its shortcut color, the submenu panel and row theme, the inset. One `Open(params MenuItem[])` helper. Four files each build the same menu on a 200 by 30 border in a 500 by 400 window today. | about 170 |
| `MainMenuDropdownLookTests` 39, `MainMenuRowLookTests` 163 (without the context menu test), `ShortcutTextLookTests` (the two main menu tests and `Open_TheMainMenuDropdownAndItsSubmenu`) (9 tests, about 300 lines) | `MenuLookTests.cs` | The main menu with `MenuDefault` and `MenuItemTop`: the dropdown is a PopupFrame with `MenuItemContext` rows on the card, hover and arrow keys give the row fill, the shortcut is readable on the fill, the open top item keeps the accent. Three files each build a File menu with an Open row today. Use `ColorMix.Contrast` in place of the two private `ContrastRatio` copies (`MainMenuRowLookTests` lines 79 to 93, `ShortcutTextLookTests` lines 19 to 33). | about 110 |
| `MenuFlyoutLookTests` 115 stays as is, it is its own control. `ShortcutTextLookTests` keeps `Show_TheShortcutOfATooltip`, `Show_AKeyCap` and the AA test, and shrinks to about 90 lines. | | | about 40 |
| `TogglePopupTests` 114, `TogglePopupCloseTests` 165, `TogglePopupWidthTests` 106, `PopupEditModeTests` 177 (29 tests, 562 lines) | `TogglePopupTests.cs` | The attached property `TogglePopup`: open with the toggle, the placement target and the one `ClosePopupBehavior`, close on pointer away and its settings, a press outside, the width (minimum, matches the toggle), the edit mode of a value in the popup. Four files each build a toggle plus popup in a 1200 by 800 window with `Middle` and `Away` helpers today. The `ListPanelItem` and `ButtonPopup` theories move to the next row. | about 160 |
| `ButtonPopupTests` 50, `TogglePopupCloseTests.TheButtonPopup_PassesTheSettingToItsPopup`, `TogglePopupWidthTests.TheButtonPopup_IsAsWideAsItsToggle`, `DropdownFrameTests.ThePopupOfAButtonPopup_HoldsADropdownFrame` and the four DropdownFrame tests that use `ShowButtonPopup` (9 tests, about 140 lines) | `ButtonPopupTests.cs` | Everything about `ButtonPopup`: the caption, the content in the logical tree, open with the toggle, the frame, the offsets, the width, the pointer away setting. | about 50 |
| `DropdownFrameTests` (the combo box tests) 178, `PopupFrameTests` 72, `MenuMinWidthTests` 48 (13 tests after the moves, about 250 lines) | `PopupFrameTests.cs` | The three frames: `PopupFrame` template and shadow, `DropdownFrame` for a combo box, `FlyoutPresenterPanel`, the least width of a menu, the centre on the pointer. | about 70 |
| `PopupCardTests` 144, `PopupTipTests` 112 (17 tests, 256 lines) | `PopupCardTests.cs` | The card and its tip: look from the tokens, the room of the shadow, the tip on each edge, the size and outline of the tip, the tip ignores the pointer. The pure `PopupTip` theories stay `[Theory]`. | about 50 |
| `ToolTipDefaultsTests` 192, `ToolTipLookTests` 99, `TipContentTests` 34 (21 tests, 325 lines) | `ToolTipTests.cs` | The tooltip: the placement (custom by default, kept when set, follows the pointer, flips, the side), the card look, the lines of a `TipContent`, the record itself. `ShortcutTextLookTests.Show_TheShortcutOfATooltip` joins. | about 70 |
| `ExpanderAnimationTests` 81, `ExpanderDefaultTests` 46, `ExpanderDepthTests` 90, `ExpanderIndentedCardTests` 122 (19 tests, 339 lines) | `ExpanderIndentedTests.cs` | The expander card: open by default, rounded card with the margin token, the level colors, a background set by the caller wins, the indentation, one `PanelCollapse` and one `Chevron` per theme. After the removal of the animation duplicates (section 2). | about 130 |
| `ScrollStepTests` 136, `ListScrollStepTests` 222 (19 tests, 358 lines) | `ScrollStepTests.cs` | The attached property `ScrollStep` on a combo box, a plain list and a `ListBoxCMiX`, the wheel to the panel around, the resize strip. The `Wheel` helper is written twice today (`ScrollStepTests` lines 37 to 49, `ListScrollStepTests` lines 32 to 44). | about 60 |
| `ResetMenuTitleTests` 66, `VectorResetMenuTests` 116 into `ValueResetMenuTests` 102 (14 tests, 284 lines) | `ValueResetMenuTests.cs` (the name stays, DEVELOPMENT.md names it) | The Reset menu of every editor: when it exists, the title, Reset All, the vectors and the range. | about 50 |
| `ValueMenuCopyPasteTests` 160, `SelectorColorMenuTests` 85 (16 tests, 245 lines) | `ValueMenuCopyPasteTests.cs` | Copy and Paste through `IValueText` on a drag value, an integer, a slider, a toggle, a combo box and the color selector. | about 40 |
| `IconOpacityTests` 85, `OpacityTokenUseTests` 91, `TabItemIconLookTests` 52, `TabControlSurfaceTests` 25 (17 tests, 253 lines) | `OpacityTokensTests.cs` and `TabItemTests.cs` | The opacity tokens in use: action icons, idle icons, disabled controls. The icon tabs: lit when selected or under the pointer, dim at rest, the transparent content area. Today the idle opacity of `ToggleButtonIcon` is tested in `OpacityTokenUseTests`, `ToggleButtonPowerTests` and `TabItemIconLookTests` with the literal 0.6. | about 80 |
| `OnStateLookTests` 131, `ListRowLookTests` 101, `RowInsetTests` 59 (list and combo rows), `TextBoxFocusLookTests` 46, `PanelFocusStyleTests` 48 (24 tests, about 370 lines) | `StateTokensTests.cs` (fill states, already 218 lines: take only the on state) and `RowLookTests.cs` (list row, combo row: fill states, inset, text color) and `FocusLookTests.cs` (text box border, panel focus line) | The look of a row in every list, the on state on every control, the focus marks. Five files each force `:pointerover` with `IPseudoClasses` today. | about 90 |
| `DragValueIntegerTests` 84, `DragValueStepTests` 29 (6 tests, 113 lines) | `DragValueTests.cs` | The defaults and the steps of `DragValue` and `DragValueInteger`, the two way binding, Reset All. | about 25 |
| `ComboBoxPathLookTests` 26 into `ComboBoxPathTests` 119 | `ComboBoxPathTests.cs` | One control, one file. | about 20 |
| `ColorPickerSlidersTests` 54 and the wheel sections of `CaptureLostTests` | `ColorPickerTests.cs` | The sliders keep their height across modes, the wheel drag, the cursor on the indicator, the hue at the centre. See the split of `CaptureLostTests` in section 4. | part of the split |
| `MotionTests` 18, `PanelCollapseTests` 151, `ChevronTests` 155 | `PanelCollapseTests.cs` takes `Duration_APanelAndAChevron_AreTheDurationOfMotion` | One test file less. | about 15 |
| `LogicalPanelContentTests` 41 and `ScrollViewerOverlayKeepOffsetTests` 26 | keep as files, rename the second to `ScrollViewerBehaviorTests.cs` (the type under test) | One test files are fine when the type is small and alone. | 0 |

Sum of the merges: about 1,400 lines and about 40 files less (109 to about 70). With the removals of section 2: about 13,900 to about 11,500 lines.

### 4. Refactoring opportunities in the test code

#### Duplicated setup

- A window: 62 private `Show` or `Host` helpers and 133 `new Window {` across the files. Each one sets a width and a height, calls `Show()` and `Dispatcher.UIThread.RunJobs()`. Five of them also clear the clips for the headless hit test (`CaptureLostTests` 24 to 34, `ComboBoxPathTests` 33 to 45, `DragCursorWrapTests` 105 to 114, `InteractionHostTests` 55 to 64, `StateTokensTests` 129 to 131). Helper to write: `Support/TestWindow.cs` with `Show(Control content, double width = 400, double height = 300)` and `ShowWithoutClips(...)`.
- A token: 93 raw `(ISolidColorBrush)Application.Current!.FindResource(key)!` casts in 40 files, next to 65 `Tokens.Get<T>(key)` calls. Each file also writes its own `Brush(key)`, `Token(key)`, `Theme(key)` or `Color(key)` wrapper (`ChevronTests` 25 and 94, `ContextMenuKeyboardRowTests` 44, `CursorRowTests` 18 and 20, `ExpanderDepthTests` 15, `GridSplitterThemeTests` 20 to 25, `HoverTargetTests` 45, `ListRowLookTests` 17, `OnStateLookTests` 17, `StateTokensTests` 19, `TextBoxFocusLookTests` 15, `ToggleButtonCaptionedTests` 29, `ToggleButtonPowerTests` 33 and more). Helpers to write on `Tokens`: `Color(key)`, `Brush(key)`, `Theme(key)`, `Thickness(key)`, `Size(key)`. `Tokens.Get` already fails with a clear message when the key is missing, the raw casts fail with a null reference.
- A click at a point: 16 files write `window.MouseMove(p); window.MouseDown(p, Left); window.MouseUp(p, Left); RunJobs()`. `PointerInput.Click` exists but takes a control. Add `PointerInput.Click(Window, Point, MouseButton)` and `PointerInput.Move(Window, Point)`.
- A pointer with a real `Pointer` object for a capture test: `CaptureLostTests` 36 to 58 and `DragCursorWrapTests` 97 to 102 and 140 to 143 both write `Press`, `Move` and `Release` that raise `PointerPressedEventArgs` on a target. Move them to `PointerInput`.
- A key press on a focused element: `ComboBoxSearchTests` 83, `ContextMenuKeyboardRowTests` 31, `ListGroupedSearchTests` 60, `MainMenuRowLookTests` 51 each write `RaiseEvent(new KeyEventArgs { RoutedEvent = KeyDownEvent, Key = key })`. Helper: `KeyInput.Press(Control target, Key key, int times = 1)`.
- A context request: six files write `source.RaiseEvent(new ContextRequestedEventArgs { Source = source })` (`ContextMenuPointerAwayTests` 29, `ContextMenuTests` 45, `MenuAnchorTests` 38, `SideContentContextMenuTests` 36, `TextBoxEditableContextMenuTests` 35, `VectorResetMenuTests` 41). Helper: `ContextMenuInput.Request(Control source)`.
- A forced pseudo class: five files write `((IPseudoClasses)control.Classes).Set(":pointerover", true)` in a loop over the visual descendants (`HoverTargetTests` 49, `ListRowLookTests` 34, `OnStateLookTests` 31, `StateTokensTests` 41, `TabItemIconLookTests` 45). Helper: `PseudoClasses.Force(Control root, string pseudoClass)`.
- A template part by name: `Part<T>(root, name)` is written in `ContextMenuRowLookTests` 52, `HoverTargetTests` 40, `OpacityTokenUseTests` 23, and inline as `GetVisualDescendants().OfType<Border>().First(b => b.Name == "...")` in about 30 files. Helper: `Parts.Find<T>(Visual root, string name)`.
- The WCAG contrast: `MainMenuRowLookTests` 79 to 93 and `ShortcutTextLookTests` 19 to 33 each hold a private `ContrastRatio`. `CMiX.UI.Mathematics.ColorMix.Contrast` is the same formula, tested in `ColorMixTests`, and `OnStateLookTests` and `ToolTipLookTests` already use it. Delete the two copies.
- The wheel: `ScrollStepTests` 37 to 49 and `ListScrollStepTests` 32 to 44 (see the merge).
- The open color selector: `CaptureLostTests.ShowSelectorWithOpenPopup` 371 to 395, `ClosePopupBehaviorTests.ShowWideSelectorWithOpenPopup` 23 to 42 and `ContextMenuTests.ShowSelectorColorWithOpenPopup` 130 to 141 build the same thing three ways.
- The open context menu on a border: `ContextMenuKeyboardRowTests.Open`, `ContextMenuRowLookTests.Open`, `ContextMenuLookTests.ContextMenu_WithADashItem`, `MainMenuRowLookTests.OpenContextMenu`, `ShortcutTextLookTests.Open_TheShortcutOfAContextMenuRow`, `OutsideClickTests.Click_OutsideAnOpenContextMenu` (six copies).
- The File menu: `MainMenuDropdownLookTests` 19 to 31, `MainMenuRowLookTests` 18 to 35, `ShortcutTextLookTests` 62 to 75, 94 to 104 and 146 to 158 (five copies).

#### Helpers that should exist

- `Support/TestWindow.cs`, `Support/KeyInput.cs`, `Support/ContextMenuInput.cs`, `Support/PseudoClasses.cs`, `Support/Parts.cs` as above.
- `Support/FakeMouse.cs` and `Support/FakeScreen.cs`: today private classes in `DragCursorWrapTests` (lines 26 to 103, 78 lines). `CaptureLostTests.MouseTakenByAnotherWindow` (lines 202 to 208) is the same kind of seam on `DragEditHelper`. One file `Support/DragEditSeams.cs` can hold the three.
- `Tokens.Color`, `Tokens.Brush`, `Tokens.Theme` as above.

#### Helpers that nothing uses or that one file uses

- Nothing is dead in `Support`. `NoRealCursor` runs as a module initializer. `CursorSpy` has 3 users, `FakeStorageItem` 2, `StringValue` 5, `NumberValue` 2, `ManualTimeProvider` 1 (and the Studio tests).
- `Tokens.ListMargin`, `Tokens.ListSpacing`, `Tokens.RowPitch` and `Tokens.ListHeight` have one user each, `ListScrollStepTests`. `Tokens.ListVisibleItems` has two. They can stay: they describe the list sizes in one place. `ListBoxEditableTests.RoomForRows` (302 to 306) and the inline sum at 288 to 290 do the same sum for the editable list and belong next to them.

#### Big files to split by subject

- `CMiX.UI.Tests/CaptureLostTests.cs`, 906 lines, 31 tests. It is six subjects with comment rulers: the slider and drag value capture (70 to 197), the mouse taken by another window (199 to 289), the wheel (291 to 339), the resize strip (341 to 367), the popup that holds a drag (369 to 476), the cursor that goes back on the slider and the wheel (478 to 818), Shift and Ctrl speed (820 to 905). Split: `CaptureLostTests.cs` (capture lost and mouse taken, about 300 lines), `DragCursorReturnTests.cs` (the cursor goes back, slider and wheel, about 350 lines), `DragSpeedTests.cs` (Shift and Ctrl on the wheel and the slider, about 100 lines, close to `DragCursorWrapTests` which tests the same `DragEditHelper.FineSpeed`). The popup section (five tests) belongs to `ClosePopupBehaviorTests`. The resize strip test belongs to `ListScrollStepTests`, which already tests the strip.
- `CMiX.UI.Tests/DragCursorWrapTests.cs`, 433 lines: after `FakeMouse` and `FakeScreen` move to `Support`, about 340 lines. One subject, keep as one file.
- `CMiX.UI.Tests/ListGroupedSearchTests.cs`, 432 lines, 28 tests: one control, three subjects (the groups and their state, the rows that are not enabled, the search and the keys). Keep one file with the three sections, or split the search and keys (13 tests) into `ListGroupedSearchKeysTests.cs`. Two `Show` helpers (30 to 49 and 160 to 177) can become one with a parameter.
- `CMiX.UI.Tests/ListBoxEditableTests.cs`, 384 lines, 17 tests: the buttons (6), the rows and the sizes (9), the drag handle (2). Keep as one file. The `RoomForRows` sum appears twice (288 to 290 and 302 to 306).
- `CMiX.UI.Tests/TabCyclingTests.cs`, 318 lines, 17 tests: one subject, `EditCycle`. Keep as one file, rename to `EditCycleTests.cs` (the file has the name of its type).

#### Slow or fragile patterns

- `CMiX.UI.Tests/ValueMenuCopyPasteTests.cs` 34 to 39: `Click` awaits `Task.Delay(50)` and `CopyOnOneValue_AndPasteOnAnother_MovesTheValue` goes through the real clipboard of the headless platform. It is the one `async Task` test and the one `Task.Delay` in the suite. The ten other tests of the file use `IValueText` directly. Either test the clipboard round trip through a seam on `ValueMenu`, or drop the delay and run the dispatcher.
- `AvaloniaHeadlessPlatform.ForceRenderTimerTick()` in `KeyHintStripTests` 107, `PanelNotificationTests` 139, `TextBoxEditableDoubleTapTests` 111, `ToggleDragBehaviorTests` 63: needed for hit testing after a layout change. Put it in `TestWindow.Show`, once.
- `CMiX.UI.Tests/TabCyclingTests.cs` 48 to 52: `Pump()` runs the dispatcher ten times with no comment why. If one run is not enough, the reason belongs in a comment or in the control.
- `CMiX.UI.Tests/DragCursorWrapTests.cs`: 12 cases that each raise 300 to 800 pointer events (`for (var i = 0; i < 400; i++) mouse.Move(20, 0)`). About 7,000 pointer events per run. It works, it is the slow file of the suite. A step of 40 halves it.
- `FontSize = 12.5` on the window in `HoverTargetTests` 26, `MenuAnchorTests` 31 and 111, `MenuMinWidthTests` 21, `SideContentContextMenuTests` 31, `TabCyclingTests` 26: a magic number with no comment. `FontSizeTests` shows the theme already gives `FontSizeMD` to a window. Remove the six and see if a test moves.
- `CMiX.UI.Tests/CaptureLostTests.cs` 388 to 389 and `ClosePopupBehaviorTests` 39: the popup opens with a click at 5 percent of the width of the swatch. A change of the swatch layout breaks it in a way that is hard to read.
- `CMiX.UI.Tests/ListGroupedSearchTests.cs` 255: `Click(window, list.RowVisuals[6])` depends on the row index of the Filter header. A named lookup by text would say what it clicks.

#### Assertions that check too much or too little

- `CMiX.UI.Tests/CaptureLostTests.cs` line 117: `Assert.NotEqual(StandardCursorType.None, slider.Cursor?.GetType() == typeof(Cursor) ? StandardCursorType.Arrow : StandardCursorType.Arrow)`. Both branches give Arrow. The assertion can never fail. Replace with the `CursorName` check of line 798, or delete.
- `CMiX.UI.Tests/OpacityTokenUseTests.cs` 36 to 37: asserts the token and then the literal 0.5. The second line pins the token value. Same in `TabItem_Disabled` 50, `QuietButtonIcon_AtRest` 71, `ToggleButtonIcon_AtRest` 80, `TextBlock_Disabled` 88 (literals 0.5 and 0.6 in a file named "token use").
- `CMiX.UI.Tests/ListBoxEditableTests.cs` 241 to 243: asserts the token color, then `Colors.Black`, then an opacity range. The token assertion is the rule, the rest pins the value.
- `CMiX.UI.Tests/ToggleButtonPowerTests.cs` 73 and 115, `TabItemIconLookTests` 37 and `OpacityTokenUseTests` 71 and 80 assert the literal 0.6 where `Tokens.Get<double>("OpacityIconIdle")` is the rule.
- `CMiX.UI.Tests/ChevronTests.cs` 66 to 73 `TheTurn_IsAnAnimation_NotAJump`: asserts only `NotEqual(0, Angle)`. A broken animation that stays at -90 passes.
- `CMiX.UI.Tests/ContextMenuRowLookTests.cs` 78 and `ListGroupedSearchTests` 187 and 198: the disabled opacities 0.5 and 0.4 as literals, next to the token `OpacityDisabled`.
- `CMiX.UI.Tests/ListBoxEditableTests.cs` 170 to 173: `CornerRadius(2, 2, 0, 0)` as a literal where `CornerRadiusAll` is the token.
- No test without an assertion was found.

#### Names that break Method_Scenario_Result

- 222 of 695 names. `TestNameTests.PinnedBadNameCount` is 249: lower it to 222 now, it only goes down.
- 17 files where every name breaks the rule: `ButtonCaptionedTests` (6), `CaptionContractTests` (2), `ColorPickerSlidersTests` (1), `ComboBoxPathLookTests` (1), `DockPanelPositionedTests` (5), `DragCursorWrapTests` (7), `DragValueStepTests` (2), `InteractionHostTests` (3), `LibraryBoundaryTests` (3), `MenuMinWidthTests` (3), `ScaleTokensTests` (4), `TabControlSurfaceTests` (1), `ThemeTests` (2), `ToggleButtonCaptionedTests` (10), `TogglePopupCloseTests` (5), `TogglePopupWidthTests` (6), `VectorResetMenuTests` (3).
- Big files with many bad names: `TabCyclingTests` 12 of 17, `ListBoxEditableTests` 10 of 17, `ListScrollStepTests` 9 of 11, `CaptureLostTests` 7 of 26, `PanelCollapseTests` 7 of 10, `PanelItemTests` 7 of 10.
- Names that carry history, which the comment rule forbids: `ToggleButtonCaptionedTests.TheBoxLooksLikeTheOldCheckBox_WhenOffAndWhenOn` and `TheBoxAndTheRowHaveTheOldSize`, `PanelItemTests.TheHeaderButtons_AreInTheOldOrder_EyeThenCloseThenHandle`, `TextBoxSearchTests.Show_WithoutShowsClear_HasNoClearButtonAndTheOldPadding`.
- `ListTreeTokensTests` writes `Colour` in two names where the rest of the code writes `Color`.
- Comments that are history: `ColorPickerSlidersTests` 11 to 14 (names a commit, "used to be"), `TestNameTests` 13 ("The tests from before 2026-11-01").

#### Test classes whose file name differs from the type under test

- `TestAppBuilder.cs` holds two classes, `TestApp` and `TestAppBuilder`. Split `TestApp.cs` out, or rename the file to the class that the assembly attribute names.
- `TabCyclingTests.cs` tests `EditCycle`: rename to `EditCycleTests.cs`.
- `ScrollViewerOverlayKeepOffsetTests.cs` tests `ScrollViewerBehavior` through a theme key: rename to `ScrollViewerBehaviorTests.cs`.
- `CaptureLostTests`, `DragCursorWrapTests`, `HoverTargetTests`, `OnStateLookTests`, `RowInsetTests`, `StateTokensTests`, `SideContentContextMenuTests`, `MenuMinWidthTests`, `FontSizeTests`, `CursorAndOpacityTokensTests`, `GeometryTokenTests`, `ScaleTokensTests`, `OpacityTokenUseTests`, `IconOpacityTests`, `TabCyclingTests`, `ScrollStabilityTests`, `RowStripesTests`: named by a rule, not by a type. The rule "a file has the name of its type" cannot hold for a rule test. A decision is needed: allow `...RuleTests` names for the files that test a rule across controls, and keep type names for the rest. The merges in section 3 reduce the rule files to about eight.
- Class names match file names in every test file (checked with grep).

#### [Fact] where [AvaloniaFact] is needed, or the reverse

- No wrong one was found. `CaptionContractTests`, `LibraryBoundaryTests`, `TestNameTests`, `ColorMixTests`, `KeyGestureTextConverterTests`, `PopupPlacementTests`, `TipContentTests` and the static theories of `PopupTipTests` are pure and use `[Fact]` or `[Theory]`. Everything that makes a control or reads a resource uses the Avalonia attributes.
- `MotionTests` writes the attribute with its full namespace, `[Avalonia.Headless.XUnit.AvaloniaFact]`, the one file that does. It is also the one test that could be a plain `[Fact]`: it compares two `TimeSpan` values of two constructors. It stays `[AvaloniaFact]` only because a control constructor is safer under the platform.
- `PopupTipTests` mixes `[Theory]` for the pure geometry and `[AvaloniaFact]` for `Measure` and the token. Correct, and a reason to keep the pure part together when it merges into `PopupCardTests`.

### 5. Coverage gaps worth a test

1. `CMiX.UI/Controls/String/TextBoxIP.axaml.cs`: no test. A typed address, an invalid one, the binding to `Value`.
2. `CMiX.UI/Controls/Number/NumericInput.cs`, `NumbersOnly.cs`, `NumericTextHelper.cs`: no test of the text that a number editor accepts (a comma in a culture with a comma, a minus, a clamp to the range). `TabCyclingTests` and `PopupEditModeTests` type whole numbers only.
3. `CMiX.UI/Controls/String/EditableValue.axaml.cs` and `SelectOnFocus.cs`: no test.
4. `CMiX.UI/Converters`: 12 of 13 converters have no test. `DoubleRemapConverter`, `DoubleToStringConverter`, `EnumMatchConverter` and `ControlPositionToCornerRadiusConverter` carry logic that a `[Theory]` tests in a few lines each.
5. `CMiX.UI/AttachedProperties/ListBoxReorder.cs` with `IReorderService`: the drag handle shows (tested) but no test moves a row through `CanDrop` and `Dropped`.
6. `CMiX.UI/Panels/StackPanelPositioned.cs` and `UniformGridPositioned.cs`: the corner positions of their children, as `DockPanelPositionedTests` does for the dock panel.
7. `CMiX.UI/Controls/ColorPicker/ColorSpaceHelper.cs`, `ObservableColor.cs`, `ColorState.cs`: the RGB to HSV round trip and the hex text, as pure `[Theory]` tests. The wheel tests use `HSV_H` and `HSV_S` but nothing tests the conversion itself.
8. `CMiX.UI/Controls/Vector/Integer2`, `Integer3`, `Vector2`, `Vector3`, `AxisSelector2`, `AxisSelector3`: a binding test per control (X, Y, Z to the model both ways). `VectorResetMenuTests` covers the menu only.
9. `CMiX.UI/AttachedProperties/RunCommandAfterClose.cs` and `PopupOwner.cs`: no test. Both sit in the popup flow that has the most tests otherwise.
10. `CMiX.UI/Themes/Icons.cs` and `CMiX.UI/Controls/AppIcon.axaml.cs`: what an `AppIcon` shows for a key that is not registered. `IconKeyTests` in the Studio scans the AXAML, nothing tests the library behavior.

### 6. Top 10 actions by value for effort

1. Write the shared helpers in `CMiX.UI.Tests/Support`: `TestWindow.Show` with the clip clearing and the render tick, `Tokens.Color`, `Tokens.Brush`, `Tokens.Theme`, `PointerInput.Click(Window, Point)` and `PointerInput.Move`, the real pointer `Press`, `Move`, `Release` from `CaptureLostTests` and `DragCursorWrapTests`, `KeyInput.Press`, `ContextMenuInput.Request`, `PseudoClasses.Force`, `Parts.Find<T>`. Then replace the 62 private `Show` helpers and the 93 raw `FindResource` casts. About 400 lines less, every later merge gets easier. Effort medium, value high.
2. Ask the owner about the three dead features, then remove their tests: `RowStripesTests` (with `RowStripes`), `CursorRowTests` and the cursor part of `CursorAndOpacityTokensTests` (with `CursorRow` and its theme if the new tree does not want them), the two tree token tests in `GeometryTokenTests` (with the six `Tree*` tokens in `Geometry.axaml`), the `OpacityDrag*` pins (with the two tokens). About 20 tests and 300 lines. Effort low, value high, because every one of them guards code that nothing runs.
3. Merge the six `TextBoxEditable*` files into `TextBoxEditableTests.cs`. One control, one `Show`, about 150 lines less and one duplicate test less. Effort low. A model for the other merges.
4. Merge the four `TogglePopup*` and `PopupEditMode` files into `TogglePopupTests.cs`, and collect the `ButtonPopup` tests from four files into `ButtonPopupTests.cs`. About 200 lines less, one duplicate less. Effort low.
5. Merge the menu look files: four `ContextMenu*Look` and keyboard files plus the context menu tests of `MainMenuRowLookTests`, `ShortcutTextLookTests` and `RowInsetTests` into `ContextMenuLookTests.cs`; the main menu tests into `MenuLookTests.cs`. Delete the two `ContrastRatio` copies for `ColorMix.Contrast`, delete the ratio pins. About 300 lines less. Effort medium.
6. Split `CaptureLostTests.cs` (906 lines) into `CaptureLostTests`, `DragCursorReturnTests` and `DragSpeedTests`, move the five popup tests to `ClosePopupBehaviorTests` and the strip test to the scroll step file, move `FakeMouse`, `FakeScreen` and `MouseTakenByAnotherWindow` to `Support`, and fix the assertion at line 117 that cannot fail. Effort medium, value high for anyone who reads the drag code.
7. Remove the pure pins and the duplicates of section 2 that need no decision: the three `SixtyFour` tests to one, the two `#18181B` pins, the `PopupCardBorderBrush` guard, the `PopupMinWidth` pin, the `ListItemIcon` update toggle guard, `Enter_WithARealText_CommitsIt`, `APointerFarAway_ClosesThePopup_AndUnchecksTheToggle`, the combo box duplicate in `DropdownFrameTests`, the `ExpanderAnimation` theories, `ChangingTheToken_ChangesTheText`, `Show_AValueOrABinding_CanCloseAnExpander`. About 15 tests, 150 lines. Effort low, risk low.
8. Replace the literals 0.5, 0.6, 0.4, `Colors.Black` and `CornerRadius(2, ...)` in the look tests with the tokens (`OpacityTokenUseTests`, `ToggleButtonPowerTests`, `TabItemIconLookTests`, `ContextMenuRowLookTests`, `ListGroupedSearchTests`, `ListBoxEditableTests`), so a look test measures the rule and not the number. Effort low, value medium: the UI kit rule "a token changes in one place" then holds in the tests too.
9. Rename: lower `TestNameTests.PinnedBadNameCount` from 249 to 222 at once; rename the 17 files whose every name breaks the rule (about 70 names) and the four names that say "Old"; rename `TabCyclingTests.cs` to `EditCycleTests.cs` and `ScrollViewerOverlayKeepOffsetTests.cs` to `ScrollViewerBehaviorTests.cs`; split `TestApp` out of `TestAppBuilder.cs`. Each merge of items 3 to 6 is the moment to rename the tests it moves. Effort low per file.
10. Take the `Task.Delay(50)` and the real clipboard out of `ValueMenuCopyPasteTests.CopyOnOneValue_AndPasteOnAnother_MovesTheValue` through a clipboard seam on `ValueMenu`, remove the six `FontSize = 12.5` magic values from the windows, and put a comment or a fix on `TabCyclingTests.Pump`. Effort low, value: the one timing dependent test in the suite goes away.

Expected end state after the ten actions: about 70 files in place of 109, about 11,000 lines in place of 13,945, about 640 tests in place of 695, no test on code that nothing runs, no bad name in a file that was touched, one set of helpers for a window, a token, a click, a key and a menu request.

# Part C: CMiX.Studio.Avalonia.Tests


Date: 2026-11-24. Branch: `refactor/tree-structure` at `6ec81b72c`. Read only. No build, no test run.

### 1. Overview

#### Counts

| What | Count |
|---|---|
| `.cs` files | 245 (223 test files, 22 helpers) |
| Lines | 33,185 |
| Test methods | 1,458: 905 `[AvaloniaFact]`, 60 `[AvaloniaTheory]`, 465 `[Fact]`, 27 `[Theory]`, 1 `[WindowsFact]` |
| Source under test | CMiX.Studio.Avalonia, about 17,000 lines. The tree alone: `ViewModels/Tree` 3,500, `ViewModels/ListTree` 2,062, `Behaviors/ListTree` 2,434, `Views/ListTree` 1,152 |
| Files named in the docs as guards (do not remove) | `ResetMenuTests`, `ValueBindingTests`, `CommandBindingTests`, `RepositoryBindingTests`, `CollectionPanelContractTests`, `SectionEntityModifiersTests`, `ResourceReferenceTests`, `IconKeyTests`, `SizeLiteralTests`, `PopupThemeGuardTests`, `PopupWiringGuardTests`, `ToolTipSourceTests`, `ProjectTreeSearchExitTests` (DEVELOPMENT.md line 496). `docs/tree-port-plan.md` says `ProjectTreeGroupsTests` stays as it is (5.1) and names `ProjectTreeAddGroupsTests` (5.2). |

#### How the suite is organized

- One flat folder. The file name is the subject, with the prefix of the view or the view model (`ListTree*`, `ProjectTree*`, `Tree*`, `Panel*`, `MainWindow*`). `Integration/` holds the File menu flows against a saved file. `Support/` holds the tree fixtures.
- Three levels of test:
  1. Pure model tests with `[Fact]` on `ProjectTree`, `TreeDropService` and the pure parts of `ViewModels/ListTree` (snapshot, layout, slots, candidates, move). They build a project with `TreeFixtures.CreateScene`.
  2. View tests with `[AvaloniaFact]` on one control in a plain `Window` (`ListTreeTestScreen.Show`, or a hand made `new Window { Content = ... }`, 103 files).
  3. Main window tests with `TestServiceProviderFactory.ShowMainWindow()` (41 files): keys of the window, the side panels, the inspector, the File menu.
- Nine AXAML scanner tests read the source tree (`SourceTree`) and guard names that the compiler does not check.
- The old tree view and its tests are gone. Commits `b14449824` (34 files) and `0f05bc5c1` removed them and `9c45475f4` removed the view. Every `ProjectTree*` and `Tree*` test that still exists runs against the shared `ProjectTree` model or against `ListTreeProject`. I checked every view, view model, behavior and theme key that a test names: all exist. One exception is in section 2: the drag look flags of `TreeNode` exist, but no view binds them.

#### What the tests cover well

- The new list tree: every pure part has its own test file, and the screen behaviors (drag, grab, keys, rename, focus, menus, tooltips, file drop, auto scroll, screen requests) each have one. `ListTreeMoveTests` compares the pure move with the real drop for every slot of every source.
- `ProjectTree` as a model: build, follow the lists, selection chain, search, cursor, open state, add groups, undo steps.
- The lists of the tabs: selection echo (undo step and message), row menus, duplicate, reorder, drop of files.
- The guards of the AXAML: resources, element names, icon keys, size literals, popup themes, tooltip placement, command bindings.
- Settings: the shortcut map and its file, the Studio settings file, the sync settings view model, all against temp paths.

#### What the tests do not cover

- `PrefabFileDropBehavior`, `PanelSyncManagement`, the converters `PeerSyncStateConverter`, `MultiPeriodToBeatAnimationConverter`, `KindIconBrushConverter`, the views `Easing` and `Quantize`, `MainWindowController` (one test file touches it), the behaviors `ListTreeScrollAnchor`, `ListTreeEndRoom`, `ListTreeLastPointer`, `ListTreePressFocus` (only through other tests).
- Two views of the tree at the same time during a drag or a file drop.
- The undo stack after a block drop that the list refuses (`RevertCapture`): `ListTreeMoveTests.DropBlockAsync_ABlockWithARowThatTheListRefuses_MovesNoRow` checks the rows, not the stack.

### 2. Remove candidates

The risk is the risk that a real regression goes unseen after the removal. "Production dead code" means the test is the only caller of the code it tests. Such a removal is one commit together with the production code, after the owner agrees.

| File | Tests | Lines | Reason | Covered by | Risk |
|---|---|---|---|---|---|
| `CMiX.Studio.Avalonia.Tests/ProjectTreeDropTargetsTests.cs` | 20 | 388 | Tests `TreeNode.IsDropCandidate`, `IsDropNeighbor`, `IsDimmed`, `IsDropHint`, `IsDragActive` and the `PropertyChanged` of `ShowDragLook`. These were the look of the old tree view. `Views/ListTree/RowListTree.axaml` binds none of them (it binds `DropBlockReason`, `IsCursor`, `ShowsBadges`, `Header`, `Item`). Nothing in `Views/` or `Behaviors/` reads them (`grep -rw` finds only `TreeNode.cs` and `TreeDragState.cs`). The new tree computes its own light and dim in `ListTreeBrightRows`. Production dead code: the flags, `DragLook.IsCandidate`, `IsNeighbor`, `HasTargetBelow` and most of `TreeDragState.MarkRow`. `DropBlockReason` and `IsDropBlocked` stay alive. | `ListTreeBrightRowsTests` (6), `ListTreeCandidatesTests` (8), `PanelListTreeRowsDragTests` (12). The block reasons: `TreeDropServiceTests.BlockReason_*` (4), `ProjectTreeTooltipTests.BlockedRow_DuringADrag_ShowsTheReason`. | medium: remove the flags first, then the file. Ask the owner. |
| `CMiX.Studio.Avalonia.Tests/ProjectTreeBudgetTests.cs` | 2 | 79 | `BeginDragAndEndDrag_OnALargeTree_StayInThePropertyChangedBudget` counts the changes of the dead flags above. `KindCounts_AfterALeafIsAdded_RecomputesEachAncestorOnce` is the same test as `ProjectTreeKindCountsTests.KindCounts_AfterAChildIsAdded_RecomputesOnce` (same three counters, same add). | `ProjectTreeKindCountsTests` | low |
| `CMiX.Studio.Avalonia.Tests/TreeDropServiceTests.cs`, the 11 `Locate_*` tests only | 11 (of 54) | about 150 | `TreeDropService.Locate(source, hovered, fraction)` was the hit test of the old drop bar (top half, bottom half of a row). The only callers are this test (21 calls) and the pass through `ProjectTree.Locate`, which nothing calls. Production dead code. The new tree uses `TreeDropSlots` and `ListTreeSlots`. | `TreeDropSlotsTests` (13), `ListTreeSlotsTests` (12) | medium: remove `Locate` and `ProjectTree.Locate` first |
| `CMiX.Studio.Avalonia.Tests/ProjectTreeDragTests.cs` | 1 | 26 | Added 2026-11-23 with the drag. Asserts that `Tree.DropService` is the same object twice and that `Tree.CanDrop` equals `DropService.CanDrop`. A pass through test. | `ProjectTreeGroupsTests.CanDrop_AnItem_IsTrueOnlyForAListOfItsKind`, `TreeDropServiceTests.CanDrop_*` | low |
| `CMiX.Studio.Avalonia.Tests/ProjectTreeKeyboardTests.cs`, `Press_ATreeViewKey_OpensOrClosesNoRow` | 1 theory, 4 cases | about 25 | Presses the NumPad keys that an Avalonia `TreeView` uses to open and close. The control is no `TreeView` since `9c45475f4`, so the test checks that a key that nothing handles does nothing. It tests Avalonia, not CMiX. | none needed | low |
| `CMiX.Studio.Avalonia.Tests/ListTreeBigProjectTimingTests.cs` | 1 theory, 4 cases | 97 | Measures `ListTreeCandidates.Of` and `ListTreeBrightRows.Of` with a `Stopwatch` against 200 ms. It was the measure of step 4.3 of the port plan. The report in the output was the value; the budget cannot be stable on a loaded machine. | `ListTreeCandidatesTests`, `ListTreeBrightRowsTests` for the behaviour. Keep the big scene builder in `Support/ListTreeScenes` if a count based budget test is wanted. | low |
| `CMiX.Studio.Avalonia.Tests/PanelInspectorBudgetTests.cs`, the time part | 1 | 81 | `Assert.True(results[i].Milliseconds < steps[i].Bound)` with bounds of 400 to 1500 ms. The views count part (`EditorViewsBuilt <= 1`) is stable and worth keeping. | Keep the views count in `PanelInspectorTests`. | low |
| `CMiX.Studio.Avalonia.Tests/ToolbarProjectTreeTests.cs`, `Show_TheToolbar_KeepsTheLookOfItsButtons` | 1 | 20 | Pins 24 by 24, corner radius 2, icon opacity 0.9. The comment says "the look of the buttons before the change to ButtonIcon". A pin of old values with no design reason. | `IconKeyTests.Check_EveryIconSizeAndSlotInTheAxaml_IsIconSizeXS` for the icon size | low |
| `CMiX.Studio.Avalonia.Tests/ProjectTreeTests.cs`, `Build_ALayerNode_HasEntitiesFiltersModifiersThenCamerasAndLights` | 1 | 15 | The same test name and the same assertion exist in `ProjectTreeGroupsTests` (line 41). Keep the one in `ProjectTreeTests` (it also checks the five `ChildManager`), remove the one in `ProjectTreeGroupsTests`, or the reverse. | the other copy | low |
| `CMiX.Studio.Avalonia.Tests/ListTreeKeysTests.cs`, `Press_EscapeDuringASearch_ClearsTheSearch` | 1 | 12 | `ProjectTreeSearchExitTests.Escape_InTreeAfterDownFromSearch_ClearsSearchAndDim` and `Escape_InTreeWithNoSearch_ChangesNothing` cover the same key with more asserts, and that file is a documented guard. | `ProjectTreeSearchExitTests` | low |
| `CMiX.Studio.Avalonia.Tests/ListTreeMenusTests.cs`, `Show_TheNewTree_HasTheToolbarOverTheRowsAndTheIdleHintsUnderThem` (the hint asserts) and `W_TheMenuOpens_ShowsEscCloseThenTheIdleHintsAgain` | 1 and a half | about 30 | `TreeKeyHintStripTests.Show_TheTree_ShowsTheIdleHintsUnderTheRows` and `OpenMenu_TheMenuOfARow_ShowsEscCloseThenTheIdleHintsAgain` assert the same words and keys. Keep the toolbar position assert. | `TreeKeyHintStripTests` | low |
| `CMiX.Studio.Avalonia.Tests/ProjectTreeTooltipTests.cs`, the placement asserts in `Show_TheMaskToggle_OpensItsTooltipBelowIt`, `VisibilityToggle_Show_HasTheVisibleTip`, `Show_TheCountOfAGroup_NamesItsKindInItsTooltip`, `BlockedRow_DuringADrag_ShowsTheReason` | 1 test and 6 asserts | about 20 | `PlacementMode.Custom` and `PopupSide.Bottom` are the defaults of `ToolTipDefaults` in CMiX.UI. `ToolTipSourceTests` guards that no view sets a placement. The asserts test CMiX.UI again. | `ToolTipSourceTests`, CMiX.UI tests | low |
| `CMiX.Studio.Avalonia.Tests/ComposableItemTogglesTests.cs` | 2 | 48 | The file keeps the name of the removed view `ComposableManagerItem` (renamed `RowComposable` in `48d28d43c`). `TogglesComposableTests.Show_ALayerRow_HasAVisibilityAndAMaskToggleWithNoFill` shows the two toggles. The order assert (mask left of visibility) is the one thing that is new. Move that one assert into `TogglesComposableTests`, then remove the file. | `TogglesComposableTests` | low |
| `CMiX.Studio.Avalonia.Tests/BindingIntegrityTests.cs` | 1 | 61 | Pins that `MainWindow` produces at most 14 "Could not find a matching property accessor" warnings, measured 2026-08-15. The 14 are known bad bindings against `PrefabManager` that nobody fixed. A new wrong binding in a view that the window does not show at start is not seen. The scanner guards (`ElementNameReferenceTests`, `RepositoryBindingTests`, `SidePanelBindingTests`, `ValueBindingTests`, `CommandBindingTests`) and `ComposableSettingsPanelTests` (which also uses `RecordingLogSink`) do the same job with a clear target. | the scanner guards | medium: the better move is to fix the 14 warnings and pin 0. Ask the owner. |
| `CMiX.Studio.Avalonia.Tests/Support/HeadlessScaling.cs` | 0 | 28 | No test uses it (0 references). It writes a private field of the Avalonia headless window by reflection. | none needed | low |
| `CMiX.Studio.Avalonia.Tests/Support/TreeDrag.cs`, method `Wheel` | 0 | 6 | No caller. `ListTreeAutoScrollTests.Wheel_DuringADrag_ScrollsTheTreeAndKeepsTheDrag` turns the wheel itself. | none needed | low |

Not a remove candidate, checked and kept:

- `ManagerReorderServiceTests` (comment talks about a "post parity audit", code is alive), `BeatAnimationsTests` (the clock rule is current), `ListTemplateRegressionTests` (the `DataTemplates` of `ListRepository` can still lose a type; only the comment with commit hashes should go), `CtrlClickSelectionTests` and `MoveSelectionTests` (guards of CMiX repairs of Avalonia list box behaviour, issue 16279), `NameBoxPixelTests` (real renderer, one design reason: the same ink in both modes), `ComposableRowLookTests` and `MainWindowSurfaceTests` (compare to tokens or to each other, no literal color), `TreeDropSlotsTests` (`TreeDropSlots` is alive: `ListTreeSlots` builds on it, and `ListTreeSlotsTests.Of_AnOpenTree_GivesEverySlotOfTreeDropSlots` compares the two).

### 3. Merge candidates

Rule used: one file per subject, named after the control or the class when there is one (`.claude/CLAUDE.md`: a file has the name of its type). A target over about 450 lines is split by subject, not by date.

#### 3.1 The 41 `ListTree*` files (about 6,500 lines) and the 3 `PanelListTree*` files (592 lines)

Pure parts (`ViewModels/ListTree`), today 19 files, target 9:

| Target file | Takes | Covers | Lines now, target |
|---|---|---|---|
| `ListTreeSnapshotTests` | `ListTreeSnapshotTests`, `ListTreeMarksTests`, `ListTreeNodeWatchTests` | the snapshot, the row keys, the hidden marks, the watch of the nodes | 287, about 240 |
| `ListTreeLayoutTests` | `ListTreeLayoutTests`, `ListTreeArrangementTests`, `ListTreeRowWindowTests` | places, pitch, indent, card areas, the shown arrangement, the row window | 229, about 190 |
| `ListTreeSlotsTests` | `ListTreeSlotsTests` | the legal slots | 223, same |
| `ListTreeCandidatesTests` | `ListTreeCandidatesTests`, `ListTreeNearCardTests` | the candidates and the near card | 218, about 190 |
| `ListTreePickTests` | `ListTreePickTests`, `ListTreePressTests`, `ListTreeRowPressRuleTests` | the press, the press rule, the nearest pick, the hysteresis | 271, about 230 |
| `ListTreeDragTests` | `ListTreeDragTests`, `ListTreeDragNodesTests`, `ListTreeDragNoteTests`, `ListTreeGrabStepsTests` | the start of a drag, the nodes of a block, the note, the grab steps | 284, about 220 |
| `ListTreeMoveTests` | the single source part of `ListTreeMoveTests` | the pure move against the real drop | 516, about 300 |
| `ListTreeMoveBlockTests` | the block part of `ListTreeMoveTests` (the `Blocks` table, `DropBlockAsync_*`, `SlotsOf_ABlock*`) | the block move and its refusals | about 220 |
| `ListTreeBrightRowsTests` | `ListTreeBrightRowsTests`, `ListTreeMenuKindTests` goes to `ListTreeMenusTests` | the bright and dimmed rows | 131, same |

Screen parts (`Views/ListTree`, `Behaviors/ListTree`), today 22 `ListTree*` plus 3 `PanelListTree*` plus the `ProjectTree*` and `Tree*` screen files that run against `ListTreeProject`, target 16:

| Target file | Takes | Covers | Lines now, target |
|---|---|---|---|
| `ListTreeProjectTests` | `ListTreeProjectTests`, `ProjectTreeActiveViewTests`, the `[AvaloniaFact]` of `TreeViewHooksTests` | the view in the side panels, attach and detach, the active view of two views, the requests that go to the active view | 337 plus, about 330 |
| `ListTreeLookTests` | the look tests of `ListTreeHoverAndSelectionTests` (`Show_*`, `Hover_*`, `Arrange_TheCardAreas*`), `ListTreeHiddenLookTests` | rest, hover, selected surfaces and borders, the hidden grey, the card areas | 420, about 380 |
| `ListTreeSelectionTests` | the click tests of `ListTreeHoverAndSelectionTests` (`Click_*`), `ListTreeMultiSelectionTests` | click, icon click, toggle click, Ctrl and Shift, the set from outside and from a peer | 290, about 280 |
| `ListTreeKeysTests` | `ListTreeKeysTests`, `ListTreeWindowKeysTests`, `ProjectTreeKeyboardTests` without the NumPad theory | cursor keys, End, Right opens the Add panel, Tab and Shift+Tab, the window keys through `CanRunTreeKey`, the paused bindings during a rename | 465, about 380 |
| `TreeKeyCommandsTests` | `ProjectTreeHideKeysTests`, the `[AvaloniaFact]` of `ProjectTreeLevelKeysTests`, the tree key tests of `MainWindowShortcutTests` (`Press_ShiftA*`, `Press_W*`, `Press_E*`, `KeyBindings_OfTheWindow_HoldThe*CommandOfTheTree`, `Set_AnotherGestureForTreeAdd*`) | the `Tree.*` commands from the main window: Hide, ShowAll, the level keys, Add, Menu, Expand, with their bindings and their text box gate | 284 plus 160 plus 250, split in two files of about 350 if needed |
| `ListTreeRowDragTests` | `ListTreeRowDragTests`, `ListTreeRowDragDropTests` | start, pick, cancel, drop, the flash, the open state after the drop | 411, same |
| `ListTreeRowDragOpenTests` | as is | the near card and the rest timer | 188 |
| `ListTreeBlockDragTests` | as is | the block drag | 195 |
| `ListTreeGrabTests` | as is | the keyboard grab | 291 |
| `ListTreeAutoScrollTests` | as is | the auto scroll | 168 |
| `PanelListTreeRowsTests` | `PanelListTreeRowsTests`, `PanelListTreeRowsWindowTests` | arrange, icons, names, the realized rows of a big tree | 354, about 320 |
| `PanelListTreeRowsDragTests` | as is | the panel during a drag | 238 |
| `ListTreeMenusTests` | `ListTreeMenusTests`, `ListTreeMenuKindTests`, without the hint asserts | the menus by key, by context key, by right click, the Add row | 281, about 240 |
| `ListTreeRenameTests`, `ListTreeFocusTests`, `ListTreeFileDropTests`, `ListTreeScreenRequestsTests`, `ListTreeViewStateInputTests`, `ListTreeRebuildQueueTests` | as is | | |
| `TreeTooltipsTests` | `TreeTooltipsTests`, `ListTreeTooltipsTests`, the texts of `ProjectTreeTooltipTests` (`Show_TheMaskToggle_HasItsTooltip*`, `VisibilityToggle_Show_HasTheVisibleTip`, `Show_ACountSquare*`, `Show_TheCountOfAGroup*`, `BlockedRow_DuringADrag*`) | the class `TreeTooltips`, the real pointer, the texts of every tip of a row | 143 plus 115 plus 170, about 330. The pure `Describe_ACount` theory goes to `ProjectTreeKindCountsTests`, `MaskOwner_EachKind` to `ProjectTreeTests`. |
| `BadgesKindCountTests` | `ProjectTreeKindBadgesTests`, the `Badges_*` and `TogglesAndBadges_*` of `ListTreeTogglesAndBadgesTests`, `PanelListTreeRowsTests.IsExpanded_AClosedCard_HidesItsRowsAndShowsItsKindBadges` | the count squares: when, how many, color, white count, plus two, the missing sign | 264 plus 80, about 300 |
| `TogglesRowTests` | the `Toggles_*` and `Click_*` of `ListTreeTogglesAndBadgesTests`, `TogglesComposableTests`, the order assert of `ComposableItemTogglesTests` | the toggles of a row: which, checked, dimmed, the click writes the model | 140 plus 140, about 240 |

Estimated reduction of the tree screen and pure files: about 1,100 lines, mostly setup, `Dispose`, `Show`, `Settle`, `Press`, `Click` helpers written again in each file (see section 4).

#### 3.2 The 21 `ProjectTree*` files (about 4,750 lines), target 9 model files

| Target file | Takes | Covers | Lines now, target |
|---|---|---|---|
| `ProjectTreeTests` | `ProjectTreeTests`, `TreeNodeTests`, `ProjectTreeTooltipTests.MaskOwner_EachKind`, without the duplicate `Build_ALayerNode` | build, follow the lists, selection chain, undo step, cursor, dispose | 398, about 370 |
| `ProjectTreeGroupsTests` | stays as it is (plan 5.1). Its `Drop_*`, `CanDrop_*`, `CanDrag_*` (lines 280 to 350) are `TreeDropService` tests and could move to `TreeDropServiceTests` when the owner lifts the rule. | the groups under an item | 424 |
| `ProjectTreeAddGroupsTests` | `ProjectTreeAddGroupsTests`, `ProjectTreeAddsTests.TargetAdds_*`, the `[Fact]` `GetAdds_*`, `TargetAdds_*`, `CanExecute_*` of `ToolbarProjectTreeTests` | `GetAddGroups`, `GetAddChoices`, `TargetAdds`, the enablement of the five commands | 261 plus 60 plus 200, about 450 |
| `ProjectTreeSelectionTests` | `ProjectTreeSelectionTests`, `ProjectTreeSelectionAfterDropTests`, `ProjectTreeSelectionSetTests` | the origin, the selection after a drop, the set | 380, about 330 |
| `ProjectTreeSearchTests` | `ProjectTreeSearchTests`, `ProjectTreeSearchOpenTests` | the match, the dim, the rows that the search opens and closes | 473, keep two files if over 450 |
| `ProjectTreeOpenStateTests` | `ProjectTreeCursorCloseTests`, the `[Fact]` of `ProjectTreeLevelKeysTests` | a close over the cursor, ExpandBranch, CloseLevel, OpenLevel, CollapseAll | 128 plus 160, about 260 |
| `ProjectTreeKindCountsTests` | `ProjectTreeKindCountsTests`, `ProjectTreeTooltipTests.Describe_ACount` | the counts by kind and their text | 130 plus 25 |
| `TreeCategoryTests` | `TreeCategoryTests`, `ProjectTreeCategoryTests` | the kind of a type, of a group, of a node, the depths, the color keys | 172, about 150 |
| `TreeDropServiceTests` | without `Locate_*`, split in `TreeDropServiceTests` (CanDrop, Drop, PlaceOf) and `TreeDropBlockReasonTests` (BlockReason, DescribeBlock, Accepts) | the rules of a drop | 713, about 300 plus 200 |

Gone from the `ProjectTree*` group: `ProjectTreeDropTargetsTests`, `ProjectTreeBudgetTests`, `ProjectTreeDragTests` (section 2); `ProjectTreeActiveViewTests`, `ProjectTreeKeyboardTests`, `ProjectTreeHideKeysTests`, `ProjectTreeLevelKeysTests` (screen), `ProjectTreeKindBadgesTests`, `ProjectTreeTooltipTests` move to the screen files of 3.1. `ProjectTreeSearchExitTests` keeps its name (documented guard). Estimated reduction: about 700 lines.

#### 3.3 `MainWindow*` (7 files, 1,175 lines)

| Target | Takes | Lines now, target |
|---|---|---|
| `MainWindowLayoutTests` | `MainWindowTreeTests`, `MainWindowSplitterTests`, `MainWindowSurfaceTests`, `MainWindowOpenTests` | 403, about 330 |
| `MainWindowShortcutTests` | the undo, redo, panel, Ctrl+O and Alt tests; the tree key tests go to `TreeKeyCommandsTests` | 410, about 160 |
| `MainWindowClosingTests` | as is, with `UnsavedChangesTests.NewProjectCommand_*` and `OpenProjectCommand_*` (the three prompt tests) | 111 plus 40 |
| `ToggleDragTests` | `MainWindowToggleDragTests` (the subject is the behavior `ToggleDrag` across the tree and the lists) | 251 |

#### 3.4 `Panel*` (10 files, 2,137 lines)

| Target | Takes | Lines now, target |
|---|---|---|
| `PanelAddTypesTests` | `PanelAddTypesHiddenTests`, `PanelAddTypesLookTests`, `HiddenTypesTests`; `AddableTypesTests` stays pure | 323, about 250 (the 23 top level helper classes become nested or one generic fake) |
| `PanelInspectorTests` split in three | `PanelInspectorHeaderTests` (title bar, badge, toggles, name: 10 tests), `PanelInspectorSelectionTests` (what each selection shows, remove, undo: 20 tests), `PanelInspectorTabTests` (the tab stays, middle click, scroll, the views count of `PanelInspectorBudgetTests`: 8 tests) | 785 plus 81, about 3 by 280 |
| `PanelFocusCycleTests`, `PanelFocusLineTests` | as is | 389, 147 |
| `PanelListTreeRows*` | see 3.1 | |
| `PanelOutputMappingsTests`, `PanelRedisAddressesTests` | as is; `TheTabSettings_ShowsTheOutputMappings*` and `SettingsShortcutTests.Show_TheTabSettings_HasTheShortcutsExpanderClosed` and `TreeKeyHintStripTests.Show_TheTabSettings_HasTheKeyHintsToggle*` are three tests of `TabSettings` in three files: one `TabSettingsTests` | |
| `PanelChoiceTests` | into `SidePanelTests` | 22 |
| `PanelEnableToggleTests` | into `ExpanderLookTests` | 39 |

#### 3.5 `Modulator*` and the modulator views (12 files, 1,500 lines)

| Target | Takes | Lines now, target |
|---|---|---|
| `ButtonModulatorAssignTests` | `ModulatorAssignButtonColorTests`, `ModulatorAssignToolTipTests`, `FlyoutPointerAwayTests.TheModulatorAssignMenu_*` | 201 plus 30, about 190 |
| `ModulatorOutputChoiceTests` | `ModulatorOutputChoiceTests`, `ModulatorAssignMenuTests` (the popup shows the rows of the choice) | 390, about 330 |
| `ModulatorHoverBehaviorTests` | as is; `ModifierManagerTests.ThePointerOverAPanel_HoversItsModulator` is the same case and goes | 65 |
| `ModulatorRangeViewsTests` | `LISAModulatorViewTests`, `TrackingModulatorViewTests`, `VisitorDataModulatorViewTests`, `FFTModulatorViewTests` as one theory over the view type (the three files share the test names `Show_AModulator_HasOneRangeValuePerAxis`, `Show_AFreshModulator_ShowsTheDefaults`, `SetValue_OnARangeValue_ChangesTheRange`) | 328, about 150 |
| `RedisModulatorViewTests` | `RedisModulatorViewTests`, `RedisFloatModulatorViewTests` | 212, about 180 |
| `IntegratorModulatorViewTests` | as is (the Reset button is special) | 106 |

#### 3.6 `Settings*`, `Shortcut*`, `StudioSettings*`, the sync status (10 files)

- Keep the three shapes apart: `ShortcutMapTests` (service), `ShortcutSettingsTests` (view model), `SettingsShortcutTests` (view). Note the two names `ShortcutSettings` and `SettingsShortcut` collide in a search; a rename of the view model to `SettingsShortcutViewModel` is a naming task for the owner, not a test task.
- `SettingsComposableTests` (60) and `ComposableSettingsPanelTests` (63) test the same view `SettingsComposable`: one file `SettingsComposableTests`.
- `BadgeSyncStatusTests` (77), `StatusIndicatorTests` (119), `SyncStatusConnectTextConverterTests` (64): one `BadgeSyncStatusTests` for the dots and their texts, about 220 lines. The converter test can stay a small pure file if the owner wants one file per type.
- `SettingsSyncTests` (view) and `SettingsSyncViewModelTests` (view model): keep both.
- `StudioSettingsTests` and `StudioSettingsFileTests`: keep both.

#### 3.7 `Unsaved*`

- `UnsavedChangesTests` (service and the two commands) and `UnsavedChangesPromptTests` (the dialog): keep both. Move the three `*Command_WithUnsavedChanges*` tests into `MainWindowClosingTests` so the prompt flow is in one place, or the reverse. One of the two, not both.

#### 3.8 Other pairs on one subject

- `SelectionEchoTests` (268) and `ListSelectionEchoTests` (197): the same five test names, two harnesses (`ListComposable` against `ListPrefabSlot`, `ListRepository`, `BeatStep`). One theory over four list kinds for the five shared tests; the composition specific tests (`Duplicate_*`, `Move_*`, `PeerReceivingTheMessages_*`) stay as facts. About 300 lines in one file.
- `RowComposableTests`: `ListRowDoubleClickTests` (66), `RowComposableMenuTests` (159), `ComposableRowLookTests` (72), `CtrlClickSelectionTests` (48), `MoveSelectionTests` (78), `RowRightClickTests` (123): all on `RowComposable` and `ListComposable`. Two files: `RowComposableTests` (look, double click, right click, menu) and `ListComposableTests` (Ctrl click, move, selection), about 450 in all.
- `AddPanelTests` (280) and `ToolbarProjectTreeTests` (`Click_Add*`, `AddHeader_*`, 12 tests): both open the Add panel and measure where it opens. Keep the placement in `AddPanelTests`, keep in `ToolbarProjectTreeTests` only that the button opens the panel of the target.

### 4. Refactoring opportunities in the test code

#### Duplicated setup

- The main window with "one composition with N layers, every row open": written by hand in `ProjectTreeSearchExitTests.Show`, `ProjectTreeHideKeysTests.Show`, `MainWindowToggleDragTests.Show`, `PanelFocusCycleTests`, `PanelFocusLineTests`, `PanelInspectorTests.Show`, `RemoveNotificationTests`, `TextBoxSearchTreeTests.Show_TheMainWindow_*`, and more (41 files call `ShowMainWindow`). `Support/ListTreeMainScreen.ShowWithThreeLayers` does exactly this and has 3 users. Give it a `layers` parameter and a `fill` action and use it everywhere.
- A `ListTreeProject` in a plain `Window`: `Support/ListTreeTestScreen.Show` exists (16 users), and 15 files still build `new ListTreeProject { DataContext = tree }` plus `Rows.MoveDuration = TimeSpan.Zero` plus `new Window` by hand (`ListTreeFocusTests`, `ListTreeKeysTests`, `ListTreeProjectTests`, `ListTreeRenameTests`, `ListTreeScreenRequestsTests`, `ListTreeTogglesAndBadgesTests`, `ListTreeTooltipsTests`, `ListTreeViewStateInputTests`, `PanelListTreeRowsTests`, `ProjectTreeActiveViewTests`, `ProjectTreeAddsTests`, `TextBoxSearchTreeTests`, `ToolbarProjectTreeTests`, `TreeKeyHintStripTests`, `TreeViewHooksTests`). `TextBoxSearchTreeTests` and `TreeKeyHintStripTests` both build the `DockPanel` with the search box over the tree: a `ListTreeTestScreen.ShowWithSearch`. `ProjectTreeActiveViewTests` and `ListTreeProjectTests` both build the two views in a `Grid`: a `ShowTwoViews`.
- 19 files declare a private `Screen` record or class, 17 a private `Click`, 14 a `CenterOf` or `MiddleOf`, 12 a `Press`, 16 a `Settle` wrapper, 8 call `ForceRenderTimerTick` by hand. `PointerInput.cs` is linked from CMiX.UI.Tests and only 5 files use it. One `Support/Screen.cs` (or an extension class on `Window`) with `Click`, `RightClick`, `DoubleClick`, `Hover`, `Press`, `CenterOf`, `Settle` ends most of this.
- Three copies of `MessageRecorder : IMessageSender` (`SelectionEchoTests`, `ListSelectionEchoTests`, `ProjectTreeHideKeysTests`).
- Five readers of the private `_undoStack` by reflection: `Support/UndoCounts` (1 user) and private copies in `ListSelectionEchoTests`, `SelectionEchoTests`, `RowRightClickTests`, `ProjectTreeHideKeysTests`. Use `UndoCounts` in all four. Better: ask the owner for a public `UndoManager.Count` (a change of a Core public API, ask first).
- 21 files make a temp folder or file with their own Guid name and their own `Dispose` or `finally` cleanup (`ComboBoxAssetPathTests`, `EntityDropBehaviorTests`, `TextureDropBehaviorTests`, `PathDropServiceTests`, `ListTreeFileDropTests`, `ListTreeAutoScrollTests`, `ShortcutMapTests`, `ShortcutSettingsTests`, `SettingsShortcutTests`, `SettingsSyncViewModelTests`, `StudioSettingsTests`, `StudioSettingsFileTests`, `TreeKeyHintStripTests`, `MainWindowClosingTests`, `UnsavedChangesTests`, `MenuMainBindingTests`, `VideoPlayerViewTests`, the four `Integration/*`). One `Support/TempFolder : IDisposable` with `File(name, text)`.
- Nine AXAML scanners each write `Comment.Replace(File.ReadAllText(file), "")` with their own `Comment` regex (`CollectionPanelContractTests`, `ElementNameReferenceTests`, `PopupThemeGuardTests`, `PopupWiringGuardTests`, `RepositoryBindingTests`, `ResourceReferenceTests`, `SidePanelBindingTests`, `SizeLiteralTests`, `ValueBindingTests`). One `SourceTree.TextWithoutComments(file)`.
- The scene "layer 1 of A has two entities with modifiers, a camera and a filter" is built three times: `Support/ListTreeScenes.Fill`, `TreeDropSlotsTests.Fill`, the private scene of `ProjectTreeDropTargetsTests`. `ProjectTreeSelectionTests`, `ProjectTreeCategoryTests`, `ProjectTreeBudgetTests`, `ProjectTreeAddsTests`, `ProjectTreeKindBadgesTests` build the project by hand instead of `TreeFixtures.CreateScene(layers, fill)`.
- `Support/TreeFixtures`, `Support/ListTreeScenes` and `Integration/ProjectFixtures` overlap on "make a project with these items": `TreeFixtures` makes a `Scene` from a provider, `ListTreeScenes` fills a `Scene`, `ProjectFixtures` shows a whole main window six times only to save a file. `ProjectFixtures` can build the `Project` from a provider (as `TreeFixtures` does) and save it, with no window. The three can be one `Support/ProjectFixtures` with `CreateScene`, the named fills, and `SaveProject(path, fill)`.

#### Helpers that should exist

- `Screen` (above), `TempFolder`, `MessageRecorder`, `ListTreeTestScreen.ShowWithSearch` and `ShowTwoViews`, `ListTreeMainScreen.Show(layers, fill)`, `SourceTree.TextWithoutComments`.
- A `TooltipText(control)` helper: `ProjectTreeTooltipTests.TextOfTooltip` reads the private `ToolTip.ToolTipProperty` by reflection, and `ListTreeTooltipsTests` and `TreeTooltipsTests` read it another way.

#### Helpers that nothing uses

- `Support/HeadlessScaling.cs` (0 users), `TreeDrag.Wheel` (0 users), `TreeFixtures.Scene.Neighbors` (1 user, in a remove candidate), `ProjectTreeTestExtensions` is fine (global, many users).

#### Big files to split or trim

| File | Lines | Action |
|---|---|---|
| `PanelInspectorTests.cs` | 785 | three files by subject (3.4) |
| `TreeDropServiceTests.cs` | 713 | remove `Locate_*`, split rules and block reasons (3.2) |
| `ToolbarProjectTreeTests.cs` | 703 | 19 `[Fact]` on the model to `ProjectTreeAddGroupsTests`, drop the look pin, keep 20 view tests (about 350) |
| `Integration/OpenProjectTests.cs` | 578 | `OpenProjectTests` (open, replace, undo stack, errors), `NewProjectTests` (take `NewProject_*` here and merge with `Integration/NewProjectTests`), `OutputMappingsRestoreTests` (the four output mapping tests) |
| `TextBoxSearchTreeTests.cs` | 528 | the seven drag tests (`BeginDrag_*`, `Enter_InTheFieldDuringADrag*`, `FocusSearch_DuringADrag*`, `EndDrag_*`, `Escape_InTheFieldDuringAPointerDrag*`, `Press_CtrlFOverTheTreeDuringADrag*`) to `TextBoxSearchTreeDragTests`; the three main window tests to `ProjectTreeSearchExitTests` |
| `ListTreeMoveTests.cs` | 516 | block part out (3.1) |
| `ListSearchableItemPickerTests.cs` | 501 | keep, or split keys and pointer |
| `ProjectTreeGroupsTests.cs` | 424 | owner rule: stays |
| `MainWindowShortcutTests.cs` | 410 | tree keys out (3.3) |

#### Slow or fragile patterns

- `Task.Delay(10)` in `MainWindowClosingTests.cs:27` and `Task.Delay(50)` in `UnsavedChangesTests.cs:67`: a wait for the dispatcher by sleeping. Replace with `Pump()` or a `TaskCompletionSource` on the event.
- `Stopwatch` budgets in `ListTreeBigProjectTimingTests` (200 ms) and `PanelInspectorBudgetTests` (400 to 1500 ms): machine dependent (section 2).
- Reflection into Avalonia internals: `ToolTip.ToolTipProperty` (`ProjectTreeTooltipTests`), `<RenderScaling>k__BackingField` (`HeadlessScaling`). Reflection into Core internals: `_undoStack` (5 places). Test only counters in production code: `RowCounts.KindCountsComputations` and `PanelInspector.EditorViewsBuilt` (both `internal`, both only for tests). Note them; the owner decides.
- `ListTreeTestScreen.Settle` calls `ForceRenderTimerTick(60)` to run the animations to their end on every settle. It is the one place, which is good. The 8 files that call `ForceRenderTimerTick` by hand should call `Settle` or `TreeDrag.Render`.
- Real files on disk in 21 files: needed for the file drop and the settings tests. Fine with one `TempFolder`.
- `ProjectFixtures` shows a full main window to write a file: six times slower than needed.

#### Assertions

- No test has zero assertions. The five with zero direct `Assert` calls (`ListTreeMenusTests.W_AfterDown_*`, `PanelFocusLineTests.FocusLine_FocusInTree*` and `FocusLine_FocusInOtherPanel*`, `NameBoxPositionTests` three tests, `SlotAddContentLookTests` five tests, `TreeDropSlotsTests.New_ACamera*`) assert through a private helper. Fine.
- Tests that check too much in one method: `ProjectTreeHideKeysTests.ToggleHideTarget_AndTheEyeClick_WriteTheSameUndoStepAndMessage` (12 asserts), `PanelInspectorTests.Select_AGroup_ShowsItsNameAndTheListPanelOfItsKind` (13), `UserControlLimitedModulatableTests.Limits_NotSet_KeepTheDefaultsOfEachEditor` (12), `ShortcutMapTests.Names_ByDefault_ListEveryNameWithAGesture` (23 asserts, one per name: a `Assert.Equal` on one expected list is one assert and a clearer message), `SettingsSyncTests.CredentialNameAndToggle_DisableUserAndPassword` (10), `SidePanelTests.Show_EachSide_HasAnIconButtonOfTheChoiceAtItsTop` (10).
- Tests that check too little: `ProjectTreeDragTests` (section 2), `ListTreeProjectTests.LeftPanel_ByDefault_ShowsOneListTreeProjectOfTheProjectTree` and `SidePanelTests.Show_ByDefault_PutsTheTreeLeftAndTheTabsRight` say the same thing.

#### Names

- `TestNameTests` pins 227 names that break `Method_Scenario_Result` (counted 2026-11-06). The rule says the number only goes down. The files with the most: `BeatAnimationsTests`, `MainWindowClosingTests` (four part names such as `WithChanges_Save_WritesTheFile_ThenClosesTheWindow`), `UnsavedChangesTests`, `UnsavedChangesPromptTests`, `ModulatorAssignMenuTests`, `ModulatorOutputChoiceTests`, `ModulatedReadOnlyTests`, `ListPrefabSlot*Tests`, `SelectionEchoTests`, `ListSelectionEchoTests`, `StatusIndicatorTests`, `PanelOutputMappingsTests`, `PanelRedisAddressesTests`, `Integration/*`, `MaskTextureViewTests`, `VideoPlayer*Tests`, `StudioSettingsTests`, `ManagerReorderServiceTests`, `ComboResetTests`, `ToggleResetTests`, `DuplicateButtonTests`, `DeleteButtonLookTests`, `ComposableRowLookTests`, `MaskToggleTests`, `TextureViewTests`, `SettingsSyncTests`, `BadgeSyncStatusTests`, `MainWindowSurfaceTests`, `ModifierManager*Tests`, `TabRepositoryTests`, `ComboBoxAssetPathTests`. A merge is the moment to rename them and lower the pin.
- Ten test names exist twice in two files: `Build_ALayerNode_HasEntitiesFiltersModifiersThenCamerasAndLights` (a true duplicate), `Menu_UsesTheSharedPopupTheme` (3 files, `PopupThemeGuardTests` covers the AXAML; these cover menus built in code, keep one theory), `Drop_ThreeFiles_IsOneUndoStep`, `View_ShowsTheAddModulatorList`, `Bind_TimeRow_IsReadOnlyWhileAModulatorIsBound`, `Show_TimeRow_HasAModulatorAssignButtonAtItsRight` (GradientMesh and Liquish: one theory), and the three modulator range names.
- File name differs from the first class: `PanelAddTypesHiddenTests.cs` (first class `HiddenMenuOwner`, 5 top level fakes) and `PanelAddTypesLookTests.cs` (18 top level fakes `ThreeAddableA` to `TenAddableJ`). Nest the fakes in the test class, or make one generic fake.
- `ComposableItemTogglesTests` carries a removed type name (section 2). `MainWindowToggleDragTests` tests `ToggleDrag`, `ListRowDoubleClickTests` tests `RowComposable`, `ListTemplateRegressionTests` tests `ListRepository` templates: the names say the symptom, not the type.

#### `[Fact]` and `[AvaloniaFact]`

- `PathDropServiceTests`: five `[AvaloniaFact]` on a service with no Avalonia type (`TextureTypeOf`, `AddTexture` through `ControlFactory`). `[Fact]` is enough and faster.
- Mixed classes where the pure and the screen tests sit together: `ToolbarProjectTreeTests` (19 `[Fact]`, 21 `[AvaloniaFact]`), `TreeViewHooksTests` (7 and 1), `MoveBlockedTests` (3 and 3: the notice service and the dialog view), `CategoryPaletteTests` (10 and 5), `ProjectTreeTooltipTests` (2 and 7), `TreeKeyHintStripTests` (2 and 9, the two `[Fact]` test `StudioSettingsFile`). Split by shape.
- No `[Fact]` test touches a `Window` or the `Dispatcher` without `[AvaloniaFact]`. Good.

### 5. Coverage gaps worth a test

1. `Behaviors/PrefabFileDropBehavior.cs`: no test names it. `EntityDropBehaviorTests` and `TextureDropBehaviorTests` cover the two other drop behaviors.
2. `Views/Messenger/PanelSyncManagement.axaml`: no test. `PanelSync` has `SettingsSyncTests` and `BadgeSyncStatusTests` only.
3. A drag of a row while two `ListTreeProject` views show the tree: the rows of the other view must stay at rest, and the drop must reach the one `ProjectTree`.
4. A file drop while two views show the tree: only the active view marks the target.
5. `ListTreeMoveTests.DropBlockAsync_ABlockWithARowThatTheListRefuses_MovesNoRow`: add the assert that the undo stack has no step after `RevertCapture` (`0c65f1d9f` added it; no test counts the steps).
6. `MainWindowController`: the window state commands (maximize, dock) have no test; `WindowDockTests` covers the pure `Compute` only.
7. `Converters/PeerSyncStateConverter.cs` and `MultiPeriodToBeatAnimationConverter.cs`: no test; a wrong enum case gives no error.
8. `KindIconBrushConverter.cs`: no test, though `RowListTree.axaml` uses it for the icon colour of every row.
9. The Escape chain of the new tree in one test: a grab, then a menu, then a rename, then the search, each takes Escape first (DEVELOPMENT.md line 496 describes the order; `ListTreeKeyInput` implements it; no test presses Escape with two of them open).
10. `ListTreeScrollAnchor` and `ListTreeEndRoom` directly: `ListTreeScreenRequestsTests.CollapseOthers_ATargetInView_StaysAtItsPlace` covers the anchor through one case only; the end room (the empty space under the rows for a drop) has no test.

### 6. Top 10 actions by value for effort

1. Remove the dead drag look of the old tree with its tests: `TreeNode.IsDropCandidate`, `IsDropNeighbor`, `IsDimmed`, `IsDropHint`, `IsDragActive`, the matching fields of `DragLook` and `TreeDragState.MarkRow`, `TreeDropService.Locate`, `ProjectTree.Locate`; then `ProjectTreeDropTargetsTests.cs`, `ProjectTreeBudgetTests.cs`, `ProjectTreeDragTests.cs` and the 11 `Locate_*` tests. About 650 test lines and about 150 production lines. Needs the owner's yes on the production part.
2. Add `Support/Screen.cs` (Click, RightClick, Press, Hover, CenterOf, Settle) and `Support/TempFolder.cs`, then delete the 17 `Click`, 14 `CenterOf`, 12 `Press`, 16 `Settle` and 21 temp folder copies. About 600 lines, no change of what is tested.
3. Give `ListTreeMainScreen.Show(layers, fill)` and `ListTreeTestScreen.ShowWithSearch` and `ShowTwoViews` to the 15 files that build the tree window by hand and the main window tests that build "one composition with three layers" by hand. About 300 lines.
4. Merge the three tooltip files into `TreeTooltipsTests`, the badge tests into `BadgesKindCountTests`, the toggle tests into `TogglesRowTests`, and remove the placement asserts. About 250 lines, and the subjects get the names of their controls.
5. Merge `ProjectTreeActiveViewTests` and the screen test of `TreeViewHooksTests` into `ListTreeProjectTests`; merge `ProjectTreeKeyboardTests` (without the NumPad theory) and `ListTreeWindowKeysTests` into `ListTreeKeysTests`; move the tree key tests of `MainWindowShortcutTests` with `ProjectTreeHideKeysTests` and the screen half of `ProjectTreeLevelKeysTests` into `TreeKeyCommandsTests`. The `ProjectTree*` prefix then means the model only.
6. Split `PanelInspectorTests` (785) into header, selection and tab files and fold the views count of `PanelInspectorBudgetTests` into the tab file; drop the time asserts there and in `ListTreeBigProjectTimingTests`.
7. Move the 19 model `[Fact]` of `ToolbarProjectTreeTests` and the `TargetAdds` test of `ProjectTreeAddsTests` into `ProjectTreeAddGroupsTests`, drop `Show_TheToolbar_KeepsTheLookOfItsButtons`, and keep in `ToolbarProjectTreeTests` only what the toolbar shows and does. About 350 lines leave the biggest view file.
8. One `SelectionEchoTests` with a theory over four list kinds, one `MessageRecorder` and `UndoCounts` for every undo count. About 200 lines, and one place for the echo contract.
9. Merge the small pure `ListTree*` files by subject (snapshot with marks and node watch, layout with arrangement and row window, pick with press and press rule, drag with drag nodes, drag note and grab steps, candidates with near card, menu kind into menus) and split `ListTreeMoveTests` into single and block. 19 files become 9, about 250 lines.
10. Rename the 227 pinned test names during the merges and lower `TestNameTests.PinnedBadNameCount` with each commit; nest the 23 top level fakes of `PanelAddTypesHiddenTests.cs` and `PanelAddTypesLookTests.cs`; replace the two `Task.Delay` with a pump; make `PathDropServiceTests` plain `[Fact]`.

Expected result of 1 to 9: about 223 test files become about 150, and 33,200 lines become about 29,500, with no loss of a case that a view, a view model or a behavior of today needs.
