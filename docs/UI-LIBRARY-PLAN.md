# Plan: extract CMiX.UI from CMiX.Studio.Avalonia

Audited at CMiX commit `f85be64` (submodule `dependencies/submodules/CMiX`, branch master).

Status: phases 0 to 4 are done. The layout of `CMiX.UI/Controls` changed after the plan: the controls
are now in folders by the type of data they edit. The current layout is in `docs/DEVELOPMENT.md`,
section "The UI library". The target layout below is the plan as it was written.

## Context

`CMiX.Studio.Avalonia` holds about 9.7k lines of C# and 10.6k lines of AXAML (157 views, 36 theme files). Generic UI (controls, theme, behaviors, converters) and domain views live in the same project. The goal is a separate `CMiX.UI` library that does not reference `CMiX.Core`, plus a light reorganization of Studio's folders and namespaces done at the same time. No visual or behavior change.

Non-goals: new controls, renaming the `CMiX*` control classes, rewriting the flat `Views` namespace of domain views.

## Facts the plan rests on

- The view models are the `CMiX.Core` models. Studio's `ViewModels/` has 3 files. Generic controls bind to them by property name (`Value`, `Caption`, `ResetCommand`, `IsModulated`). Bindings are not compiled (`AvaloniaUseCompiledBindingsByDefault=false`), so this is no compile dependency.
- Generic-area code that references Core:
  - `Views/Controls/DragValue.cs`, `CMiXSlider.cs`: static `CMiX.Core.BaseControls.ValueInteraction` (`BeginScope()` at `DragValue.cs:89`, `CMiXSlider.cs:164`).
  - Modulation family: `ModulatorAssignableUserControl`, `ModulatorAssignButton`, `ModulatorOutputChoice`, `ModulatablePanel`, `ModulatableModifierPanel`, `ModifierPanel`, converters `ModulatorStateToBrush` and `MultiPeriodToBeatAnimation`, `Animations/`.
  - Domain widgets: `Outliner`, `PrefabListItem`, `TextureDropBehavior`, `PeerSyncStateConverter`, `TypeNameToDisplayLabelConverter`, `Resources/EnumProviders`, `Services/ManagerReorderService`, `Services/StudioSettings`.
- No Core dependency: `ColorSliderBehavior`, the whole color picker, `PopupFrame`, `NumericInput`, `Captioned*`, `Integer*Value`, `Vector2`, `VectorXYZ`, `IPBox`, the text inputs, panels, attached properties, `ListBoxReorderDragDrop`, `LogicalPanelContent`, `MathUtils`.
- `Views/ViewModelToViewTemplate.cs` finds a view by `CMiX.Studio.Avalonia.Views.{ModelTypeName}`. Domain views must stay in that flat namespace until the locator is indexed (phase 1b). 111 of 232 `.cs` files do not match their folder namespace.
- Tests boot the real `App` (`Tests/TestAppBuilder.cs`) to load the theme. The headless runtime does not run `OnFrameworkInitializationCompleted`, so the interaction hook and theme entry point must be set in `App.Initialize`.
- Rewrite scope: 149 `.axaml` files use `clr-namespace:...Views.Controls`, 21 use `AttachedProperties`, 8 use `Converters`. 19 of 31 test files touch these namespaces; 12 of those use no Core type.
- Placement rule: anything with no Core dependency and no domain meaning goes to `CMiX.UI`, down to the most basic styles and controls. Only code that needs a Core type or names a domain concept stays in Studio.
- Theme: `Themes/Styles/Modifier.axaml` holds two generic themes, `ModifierCloseButtonStyle` (a close button built on `AppIcon`) and `DragHandlerButton` (a drag-handle button). Neither uses Core; they only have domain-looking names or sit next to `ModifierPanel`. Both move to the library, under neutral names (`CloseIconButton`, `DragHandleButton`; Studio's `ModifierPanel.axaml` is the only user). Studio-only styles are just `EntityModifiersSection` (references `ModifierManager` and `AutoSelectionPanel`) and the sync indicators in `StatusIndicators`. `Windows/MainWindow.axaml` defines `PopupFlyoutStyle`, a generic popup theme documented in `docs/DEVELOPMENT.md`. `Tab.axaml` keys `TabControlLayerPanel`/`TabControlVerticalPanel` need a check. `Themes/Basic/Icons.cs` mixes generic keys (Copy, Reset, chevrons) with domain keys (NewLayer, PostFX, MeshLight).
- Material.Icons is used by `Themes/Basic/Geometry.axaml` and the Button, ComboBox, DragValue and Expander styles, so `CMiX.UI` takes that package.
- Build/CI: `CMiX.sln`, `build/Build.cs` and `.fallout/build.schema.json` reference only the Studio project. `CMiX.Core.Tests` mentions Studio in a comment only. `D:\GitHub\CMiX_UI` is a stale separate clone with uncommitted changes: leave it alone.

## Target layout

```
CMiX.UI/        (net8.0; Avalonia, Avalonia.Xaml.Behaviors, Material.Icons.Avalonia)
  CMiXTheme.cs            single theme entry point (Styles)
  Themes/Tokens/          was Themes/Basic: Colors, Brushes, Geometry, Typography, Converters
  Themes/Styles/          generic styles incl. PopupFlyoutStyle
  Themes/Icons.cs         generic icon keys only
  Controls/               DragValue, CMiXSlider, CMiXToggleButton, CMiXListBox, CMiXPopup, PopupFrame,
                          NumericInput/Text, LabeledContent, Captioned*, Integer*Value, Vector*, RangeValue,
                          AxisSelector2D/3D, IPBox, ColorSelector, AppIcon, DefaultResetMenu, ...
  Controls/ColorPicker/   Controls/TextBox/   Panels/ (Positioned*, IndentedExpander)
  Behaviors/              ClosePopup, ScrollViewer, ColorSlider, ListBoxReorderDragDrop
  AttachedProperties/     all 5 + LogicalPanelContent
  Converters/             generic ones only
  Interaction/            InteractionHost (replaces ValueInteraction use)
  Mathematics/MathUtils.cs
```

Studio after phase 2:

```
Views/Modulation/   Modulatable*, ModulatorAssign*, ModulatorOutputChoice, ModulatablePanel,
                    ModulatableModifierPanel, the 5 modulator views
Views/Modifiers/    ModifierManager, ModifierPanel, ModifierEntry, + the 20 loose *Modifier views in subfolders
Views/Controls/     Outliner, PrefabListItem only
Themes/             EntityModifiersSection, StatusIndicators (sync), domain icon keys
```

Rule from phase 4: `CMiX.UI` never references `CMiX.Core`.

## Phases

Each phase is one or more commits. Solution builds and all tests pass after every commit.

### Phase 0: branch and baseline
Very first action, before any other change: create the branch.
1. In `dependencies/submodules/CMiX`, run `git switch -c feature/ui-library` from the clean master. Do not touch `master` after that, and do not bump the submodule pointer in `dl_performance_mode` until the end.
2. Write this plan to `docs/UI-LIBRARY-PLAN.md` on the branch and commit it (first commit of the branch).
3. `dotnet build CMiX.sln` and both test projects; record the baseline and any failing tests.
4. Baseline visual capture, done by you (see "Visual checks by you"): screenshots of every screen in the checklist, taken on `master` before the first code change. Claude can also capture them through the app if you prefer.

### Phase 1: decouple in place (no moves)
- **1a. Interaction seam.** Add `InteractionHost` with `static Func<IDisposable>? BeginScope`. `DragValue` and `CMiXSlider` call it and keep an `IDisposable?`. In `App.Initialize` set it to `() => ValueInteraction.BeginScope()`. Update the ambient-static comment in `App.axaml.cs`. Tests: `DragValueStepTests` and Core `ValueInteractionTests` stay green; add a test that a drag opens and ends a scope through the hook.
- **1b. Locator.** `ViewModelToViewTemplate` builds a name-to-type index of `CMiX.Studio.Avalonia.Views` and child namespaces on first use. Keep "view is named like its model". Test with a view in a sub-namespace.
- **1c.** Rename `Views/Controls/Panels/IndentedExpander.axaml.cs` to `IndentedExpander.cs`.
- **1d. Placement audit of every file outside `Views/` domain screens.** Apply the placement rule (no Core type, no domain meaning means library) to each file in `Themes/`, `Views/Controls/`, `Behaviors/`, `AttachedProperties/`, `Converters/`, `Services/`, `Animations/`, `Resources/` and write the result as a table in `docs/UI-LIBRARY-PLAN.md` (file, destination, reason). Already decided: `Modifier.axaml` moves to the library (both themes, renamed `CloseIconButton` and `DragHandleButton`), `PopupFlyoutStyle` moves. To decide: `Tab.axaml` keys `TabControlLayerPanel`/`TabControlVerticalPanel`, `Window.axaml`, and the rest of the mixed files (`Button`, `Expander`, `Slider`, `ColorWheel`); the sync indicators and `Outliner`/`PrefabListItem` stay in Studio. Review the table with you before phase 3.

### Phase 2: reorganize folders inside Studio (no namespace change)
`git mv` only, plus `DependentUpon` entries in the `.csproj`.
- 2a. Create `Views/Modulation` and move the modulation controls, `ModifierPanel`, `ModulatableModifierPanel` and the 5 modulator views.
- 2b. Move the 20 loose `*Modifier` views and `TransformSRT` into `Views/Modifiers` subfolders; move `ModifierEntry.cs` next to `ModifierManager`.
- 2c. Document in `docs/DEVELOPMENT.md`: domain views are in the flat namespace `CMiX.Studio.Avalonia.Views`, a view is named like its model, folders are free.
- Left as is: `ViewModels/` and the duplicate `MainMenu`/`MainWindowController` names.

### Phase 3: create CMiX.UI and move the generic parts
- 3a. Add `CMiX.UI/CMiX.UI.csproj` (same package versions as Studio), add to `CMiX.sln`, add `ProjectReference` from Studio. SPDX header on every file. Decide the xmlns approach: a single `[XmlnsDefinition]` URI vs per-namespace `clr-namespace:...;assembly=CMiX.UI`.
- 3b. `git mv` in separate commits: (1) tokens and generic styles with `CMiXTheme` (App.axaml then includes `<ui:CMiXTheme />`), (2) helpers: attached properties, behaviors except `TextureDropBehavior`, generic converters, `MathUtils`, `LogicalPanelContent`, `ListBoxReorderDragDrop`, (3) controls, color picker, text inputs, panels, (4) `InteractionHost`, (5) generic icon keys; Studio registers its domain keys into the icon map at startup.
- 3c. One scripted commit for the namespace and URI rewrite and nothing else: `...Views.Controls` to `CMiX.UI.Controls` (sub-namespaces matching folders), `...AttachedProperties`/`Behaviors`/`Converters`/`Themes` to `CMiX.UI.*`, `x:Class`, xmlns in about 180 `.axaml` files, `using` lines, `avares://CMiX.Studio.Avalonia/Themes/...` URIs, `{x:Type}` keys. Keep the script in `build/` so it can be re-run on rebase.
- 3d. Studio keeps only `EntityModifiersSection` and `StatusIndicators` styles and includes them from its own `Themes/Generic.axaml`. `Modifier.axaml` moves to the library; update the two keys in `ModifierPanel.axaml`.
- Exit: `dotnet build CMiX.UI` works alone; Studio starts; screens match the Phase 0 screenshots.

### Phase 4: tests, guards, docs
- 4a. `CMiX.UI.Tests` with its own headless `Application` including only `CMiXTheme`. Move the Core-free tests (`ColorSlidersPicker`, `ContextMenuLook`, `DragValueStep`, `MenuFlyoutLook`, `PopupFrame`, `PopupThemeGuard`, `PointerInput`, and the Reset and Toggle tests with a stand-in value model).
- 4b. `PopupThemeGuardTests` and `ResetMenuTests` scan the AXAML of both projects.
- 4c. Guard: fail when `CMiX.UI.csproj` references `CMiX.Core`, or a `CMiX.UI` type references a Core type.
- 4d. Update `docs/DEVELOPMENT.md` (layout, theme entry point, guard ownership, how to add a library control).
- 4e. Check `build/Build.cs`, `.fallout/build.schema.json` and the choco package still ship `CMiX.UI.dll`.

### Phase 5: merge and bump
Rebase on master; if it moved, re-run the 3c script rather than resolve by hand. Merge into CMiX master, then bump the submodule pointer in `dl_performance_mode` in its own commit. The vvvv engine references only `CMiX.Core`, so it is unaffected.

## Verification

- Every phase: `dotnet build CMiX.sln`, `dotnet test CMiX.Core.Tests`, `dotnet test CMiX.Studio.Avalonia.Tests` (plus `CMiX.UI.Tests` from phase 4).
- Visual and interaction checks by you, at the checkpoints below.
- After 3c: a search for `CMiX.Studio.Avalonia.Views.Controls` and `avares://CMiX.Studio.Avalonia/Themes` in moved areas must return nothing.

## Visual checks by you

Claude cannot judge the look of the UI reliably, so you check it. Claude stops after each checkpoint and waits for your OK before the next phase.

**When**
- Checkpoint 0: on `master`, before any change, to take the reference screenshots.
- Checkpoint A: end of phase 1 (hook and locator).
- Checkpoint B: end of phase 2 (folder moves).
- Checkpoint C: after the theme move in 3b(1), after the helpers and controls moves in 3b(2-3), and after the namespace rewrite 3c. The theme and the rewrite are the two risky steps for the look.
- Checkpoint D: end of phase 4, final.

**How**: run the Studio from the branch (`dotnet run --project CMiX.Studio.Avalonia`), open the same project each time, and compare with the reference screenshots side by side. Same window size, same project, same selection.

**Checklist**
1. Main window: all three main tabs (Composition, Entities, Layers), the main menu, window frame and drop shadow.
2. Value editors: float drag value, integer value, Vector2, VectorXYZ, range value, slider. Check fonts, heights, spacing, hover, pressed and read-only (modulated) states.
3. Drag a value and a slider: the value follows the mouse, the cursor hides while dragging, one undo step per drag, and the engine receives the final value.
4. Reset: right-click each editor type and use Reset and Reset All. Toggles, combos with captions, enum selectors.
5. Text inputs: editable text box, numbers-only box, IP box, select-on-focus.
6. Color: color selector button, color picker popup, wheel, sliders, hex input.
7. Menus and popups: context menus, menu flyouts (modulator assign), the panel flyout (300 wide), tooltips, shadows and rounded translucent panels.
8. Lists and panels: expanders with indentation, layer list, manager lists with drag reorder, outliner selection, tabs (layer tab panel and vertical tab panel).
9. Modulation: assign a modulator to a value, check the colored state brush and the beat animation; open each of the 5 modulator views.
10. Domain screens: one filter, one light, one mesh, one modifier, a texture source, the sync panel with its status indicators, the asset manager.
11. Icons: toolbar and button icons, chevrons, the add/close buttons.
12. Light and dark: the app is dark only (`RequestedThemeVariant="Dark"`), so only the dark look is compared.

**Report back**: for each differing screen, a screenshot and the screen name. A visible difference at checkpoints A, B and C is a bug in the refactor, since the plan changes no visuals.

## Risks

| Risk | Mitigation |
|---|---|
| A missed xmlns or `avares://` URI fails only at runtime | tests that load each view; the post-3c search |
| Interaction hook set too late, drags stop coalescing undo | set in `App.Initialize`; test in 1a |
| Locator cannot find moved views | index in 1b with a nested-namespace test |
| Merge conflicts with other branches touching `Views` | phases 2 and 3c are mechanical and scripted; keep the branch short |
| A resource key stays in Studio while a library control needs it | 1d audit; a test that instantiates every library control with only `CMiXTheme` |
| `internal` members used across the old single assembly | build errors reveal them; make public only when needed |
| Package version drift between projects | same pinned versions or one `Directory.Packages.props` |

## Decisions needed before phase 3

1. Namespace root `CMiX.UI`, and single `XmlnsDefinition` URI vs per-namespace xmlns (plan assumes the single URI).
2. Keep the `CMiX*` prefix on `CMiXSlider`, `CMiXListBox`, `CMiXPopup`, `CMiXToggleButton` for now (plan assumes yes).
3. Modulation controls stay in Studio under `Views/Modulation` (plan assumes yes).
(Settled: the branch `feature/ui-library` is created first, and the plan is committed on it as the first commit.)

## Placement audit (step 1d)

Rule: no Core type and no domain meaning means `CMiX.UI`. Result of the audit at the end of phase 1. "Lib" is `CMiX.UI`, "Studio" stays in `CMiX.Studio.Avalonia`. Items marked **decide** need your answer before phase 3.

### Themes

| File | Destination | Note |
|---|---|---|
| `Basic/Colors`, `Brushes`, `Geometry`, `Typography` | Lib | Tokens. Scan the brush names for domain names in 3b. |
| `Basic/Converters.axaml` | Split | Lib: ColorToBrush, HexToBrush, ColorToHex, ColorToNoAlpha, DoubleToString, NullToVisibility, CountToVisibility, FontFamilyToString, CurrentStep, PositionToCornerRadius. Studio: ModulatorStateToBrush, MultiPeriodToBeatAnimation, PeerSyncState, SyncStatusIsConnecting. |
| `Basic/Icons.cs` | Split | Lib: generic keys. Studio: NewLayer, DeleteLayer, PostFX, MeshLight, Scheduler and other domain keys. |
| `Generic.axaml` | Split | Lib gets `CMiXTheme`. Studio keeps a small `Generic.axaml` for its own styles. |
| `Styles/Border`, `Button`, `ComboBox`, `DragValue`, `GridSplitter`, `IndentedExpanderDepth`, `Label`, `LabeledContent`, `NumericInput`, `RadioButton`, `ScrollViewer`, `TextBlock`, `ToolTip` | Lib | Generic. |
| `Styles/Expander` | Lib | Includes `IndentedExpander` and `ItemControlExpander`. |
| `Styles/Sliders/Slider`, `Sliders/ColorWheel` | Lib | Color picker and slider styles. |
| `Styles/Menu/ContextMenu`, `Menu`, `PopupFrame` | Lib | Popup look. |
| `Styles/Windows/MainWindow.axaml` | Lib | Holds only `PopupFlyoutStyle`. Rename the file to `Menu/PopupFlyout.axaml`. |
| `Styles/Windows/Window.axaml` | Lib | Holds only `WindowDropShadow`. |
| `Styles/Modifier.axaml` | Lib | Decided. Keys become `CloseIconButton` and `DragHandleButton`. |
| `Styles/Tab.axaml` | Lib | Decided here. The keys `TabControlLayerPanel` and `TabControlVerticalPanel` have domain names. Keep the keys in phase 3, rename later. **decide** |
| `Styles/TextBox` | Split | Lib: the default style, `TextBoxNumbersOnly`. Studio: `TypeWriterStyle`. |
| `Styles/ListBox` | Split | Lib: `CMiXListBox`, `ListBoxItemStyle`, `ListBoxResizeBorderStyle`, `ListBoxItemAlternationStyle`. Studio: `ComposableListBoxStyle`, `ComposableListBoxItemStyle`, `ComposableListBoxItemBorderStyle`. |
| `Styles/ToggleButton` | Lib | `ToggleTransformModifierMenu` has a domain name. Keep in the lib, rename later. **decide** |
| `Styles/StatusIndicators` | Studio | Two sync state ellipses. They could be a generic status dot later. |
| `Styles/EntityModifiersSection` | Studio | Uses `ModifierManager` and `AutoSelectionPanel`. |

### Controls (`Views/Controls`)

| Files | Destination | Note |
|---|---|---|
| `DragValue`, `CMiXSlider`, `CMiXToggleButton`, `CMiXListBox`, `CMiXPopup`, `PopupFrame`, `NumericInput`, `NumericText`, `LabeledContent`, `DefaultResetMenu`, `ContextMenuGuard`, `DragEditHelper`, `ControlPosition`, `CaptionedUserControl` | Lib | `DragValue` and `CMiXSlider` use `InteractionHost` since step 1a. |
| `CaptionedButton`, `CaptionedComboBox`, `CaptionedToggleButton`, `IntegerValue`, `Integer2Value`, `Integer3Value`, `Vector2`, `VectorXYZ`, `RangeValue`, `AxisSelector2D`, `AxisSelector3D`, `IPBox`, `ColorSelector`, `AppIcon` | Lib | They bind to a model by property name only. |
| `ColorPicker/*`, `TextBox/*` | Lib | No Core type. |
| `Panels/PositionedStackPanel`, `PositionedUniformGrid`, `IndentedExpander` | Lib | |
| `ModulatorAssignableUserControl`, `ModulatorAssignButton`, `ModulatorOutputChoice`, `ModulatablePanel`, `ModulatableFloatSlider`, `ModulatableFloatValue`, `ModulatableIntegerValue`, `ModulatableVectorXY`, `ModulatableVectorXYZ` | Studio | Use `PrefabManager`, `IModulator`. Move to `Views/Modulation` in phase 2. |
| `Panels/ModifierPanel`, `Panels/ModulatableModifierPanel` | Studio | Move to `Views/Modifiers` or `Views/Modulation` in phase 2. |
| `Outliner`, `PrefabListItem` | Studio | Use `PrefabManagerBase` and prefabs. |

### Other folders

| Item | Destination | Note |
|---|---|---|
| `AttachedProperties/*` (5 files) | Lib | `LogicalPanelContent` from `Views/` joins them. |
| `Behaviors/ClosePopupBehavior`, `ScrollViewerBehavior`, `ColorSliderBehavior` | Lib | `ColorSliderBehavior` has no Core type. |
| `Behaviors/TextureDropBehavior` | Studio | Uses assets and texture sources. |
| `Converters`: ColorToBrush, ColorToHex, ColorToNoAlpha, HexToBrush, CountToVisibility, CurrentStep, DoubleToString, EnumMatch, FontFamilyToString, NullToVisibility, ControlPositionToCornerRadius, RemapDouble | Lib | No Core type. |
| `Converters`: ModulatorStateToBrush, MultiPeriodToBeatAnimation, PeerSyncState, SyncStatusIsConnecting, TypeNameToDisplayLabel | Studio | Core types or sync domain. |
| `Mathematics/MathUtils` | Lib | Used by `CMiXSlider`. |
| `Services/ListBoxReorderDragDrop` | Lib | Only Avalonia types. |
| `Services/EmptyViewLocator`, `MainWindowDialogManager` | Studio | Glue for the dialog package. |
| `Services/ManagerReorderService`, `StudioSettings` | Studio | Core types. |
| `Animations/*` | Studio | Beat animation, uses Core. |
| `Resources/EnumProviders` | Studio | Core enums. |
| `Interaction/InteractionHost` | Lib | Added in step 1a. |
| `Views/ViewModelToViewTemplate`, `ViewsAssemblyMarker`, `ModifierEntry` | Studio | |
