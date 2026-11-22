# Views audit, 2026-11-20

Scope: CMiX.Studio.Avalonia/Views (266 views in 15 folders and 16 subfolders), CMiX.UI/Behaviors, CMiX.UI/AttachedProperties, CMiX.Studio.Avalonia/Behaviors.
Method: 27 read only scans by the agents in .claude/agents (Avalonia, MVVM, UI kit per folder group, one reuse scan, one naming audit). Nothing changed. Two claims were checked by hand and dropped: the child namespaces Controls, Managers, Menus, Modulation and Windows are allowed by docs/DEVELOPMENT.md, and the commands bound in Managers exist on PrefabManager in Core.
The themes are ranked by what they cost a programmer who comes new to the code.

## 1. Attached properties and behaviors have no rule

Done on 2026-11-20, 20 commits from 606cc11c to dd1cad86: the rule is in docs/DEVELOPMENT.md, the pointer away family has one set of names and one owner of the press outside, the dead state and ExpanderIndentation are gone, every class is in its folder, CursorRow sets a pseudo class, and the theme ScrollViewerOverlayKeepOffset applies ScrollViewerBehavior in place of 11 copies. ModulatorHoverBehavior has a user and stays. The findings below are the state before.

- One job, close on pointer away, in three shapes over the same PointerAwayCloser: ClosePopupBehavior, CloseFlyoutOnPointerAwayBehavior, ContextMenuPointerAway, TogglePopup, PopupButton. ClosesOnPointerAway and Distance are declared five times, the menu version calls them IsEnabled and Distance.
- Press outside has two owners: ClosePopupBehavior and the static OutsideClick, which reaches back into the behavior. Close logic sits in three files.
- MouseDown and DragHandle both track a pressed state. MouseDown.IsMouseDown has no reader. Slider.axaml sets MouseDown.IsEnabled and nothing reads it.
- ModulatorHoverBehavior has no user outside its test. The tree binds IsHovered to IsPointerOver instead.
- The folders mix kinds. Behaviors holds attached properties (ToggleDrag, RunCommandAfterClose, ListBoxReorder), a static helper (OutsideClick), an internal helper (PointerAwayCloser), a record (DropFiles) and an interface (IReorderService). AttachedProperties holds TogglePopup and ContextMenuPointerAway, which install behaviors. Ten more attached properties sit inside control and view files (SelectOnFocus, NumbersOnly, EditCycle, ComboBoxSearch, ToolTipDefaults, ToolTipCard, ListBoxEditable.RowNumber, TreeRowBrush, TreeDropMark, KindCountBadges).
- ScrollViewerBehavior is repeated in 11 views because a behavior cannot live in a ControlTheme; ScrollStep does the same kind of job from the themes.
- An attached property reaches a style three ways: CursorRow sets a class, PositionedControl and PopupOwner set pseudo classes, TreeDropMark and TreeRowBrush have their own form.
- ExpanderIndentation is one class for one margin binding.

Proposed rule, from what the code does best today:
1. An attached property (static class, Changed handler, state in a ConditionalWeakTable) when a theme or a template must set it. A Behavior<T> only when one view declares it in XAML, it holds state per instance, or it raises its own events.
2. Both kinds share one logic class, as PointerAwayCloser. The property or the behavior is a thin switch with the same property names in every shape.
3. Attached properties live in CMiX.UI/AttachedProperties, behaviors in CMiX.UI/Behaviors, Studio ones in CMiX.Studio.Avalonia/Behaviors, shared logic in a Support folder. A class with no XAML user goes. A state property with no reader is not registered.

## 2. Duplicate and near duplicate views

- Mesh/MeshCone and MeshCylinder: identical XAML but the class name. MeshSphere and MeshTorus share the Radius and Tessellation rows.
- Layer/LayerCameraTab, LayerLightTab and LayerEntityTab: the same tab shell, differing in the manager, the list, the add button and the detail view.
- Modulation/DragValueModulatable and IntegerValueModulatable: one DragValue plus the assign button, differing in IsInteger and the steps.
- Vector2Modulatable, Vector3Modulatable and the CMiX.UI Vector2, Vector3, Integer2, Integer3: the X, Y, Z row pattern five times, the X row without a ControlTitle, the others with one.
- Managers/RepositoryManager, RepositoryTab and PrefabSlotManager: the same four styled properties (RowMenu, SelectionPanel, EditingPanel, ItemsSource) and the same LogicalPanelContent.Track call three times. RepositoryManager and RepositoryTab are the same generic list host under two suffixes.
- Three drop paths: AssetManager hand writes AllowDrop, DragOver, Drop and the path read on five targets; FileDropBehavior already does it; TreeFileDropInput repeats the wiring with only the DropFiles record.
- The mask toggle row by hand in FullscreenQuad and ModelEntity, while ComposableToggles owns the mask toggle.
- The ContainerPrepared handler for PrefabRowMenu.AttachTo is written in PrefabSlotManager and RepositoryManager.
- ScrollViewer plus ScrollViewerOverlay plus ScrollViewerBehavior: the same shell in 11 files.

## 3. Logic in code behind

- Messenger/SyncSettings: settings load and save, validation, option building, peer Stop and Start, the WindowsCredentialStore created in the view, the connect button text from a string compare on Status, credential boxes enabled through event hooks, the same flyout close handler written twice.
- MainWindow: the closing flow with the unsaved question, the QuitRequested subscription kept in a field, three services in the constructor, the parameterless constructor wires four events, long comments on Alt and Tab. WindowDock.Compute returns a tuple.
- Tree: TreeSearchBox decides Escape, Enter and Down and passes a direction int; the watch and unwatch of ProjectTree is written in TreeSearchBox and ProjectTreeView; ProjectTreeView wires 17 helpers with a lambda cycle; ProjectTreeToolbar builds the Add header text in the Click handler.
- Managers/AutoSelectionPanel builds its grid and buttons in code and binds a command by a string; PrefabSlotManager keeps the swap popup state and reads a resource on attach.
- Modulation/ButtonModulatorAssign filters outputs and builds menu rows in code, hooks CollectionChanged in four places; SliderModulatable, DragValueModulatable and IntegerValueModulatable repeat the Minimum and Maximum property block; FloatRangeEditor sets a class in code.
- Assets/AssetManager.OnDrop walks the file system, starts a Task, marshals to the dispatcher and reads App.MessageNotice.
- Controls/Outliner reverts a refused selection in a handler.
- Component/ComposableManagerItem sets a drag handle pressed state in pointer handlers and attaches the row menu with a lambda into a named control; ComposableToggles mirrors the DataContext into a field.
- Texturing/Source/PlayerTransport adds and removes the loop row in code.
- Modifiers/ModifierManager carries three long explaining comments.
- Clean: all domain views of Texturing/Filter, Texturing/Source, Modifiers subfolders, Component/Entities, Beat, the three MainWindow tabs, SettingsTab, ShortcutSettingsView, Inspector, Panels, Menus, Dialogs.

## 4. Hard coded values where a token exists

- A margin or padding of 8, 4 or 1 is written as a literal in about 100 places across Component (55), Texturing (20), Beat, Managers, Messenger, Tree, Modifiers, Modulation, ShortcutSettingsView, MainWindow and MainMenu. The root cause: SpacingMD, SpacingSM and SpacingXXS are Double tokens, so a Margin cannot bind them. A set of Thickness tokens (for example PanelMargin, SectionMargin, RowPadding) makes the pass mechanical.
- Grey400Brush is set directly in eight files where a semantic brush would serve.
- Background Transparent by hand on two Beat buttons; a 2 px frame in the background color in MainWindow; BoxShadow literal in ProjectTreeView and TreeRowShadow; Opacity 0.6 on the stripes; FontSize 10 on the warning sign; MinWidth 240 and the window size with no token.
- Classes with no rule or no user: filled (10 uses, no style found by the kit scan; check DragValue.axaml:66), window (3), count (KindCountBadges, dead), bare (FloatRangeEditor, dead style).
- A wide selector :is(ContentControl) in PanelModulatable sets ClipToBounds on every nested ContentControl.

## 5. Names

- x:Name case is split: 71 percent camelCase (Texturing, Modifiers, Modulation), 29 percent PascalCase (Messenger, Dialogs, Tree, the root tabs). No PART_ prefix anywhere.
- The suffix of an editor view is Settings in Component, Panel in Managers and Messenger, Tab at the root and in Managers. SyncSettings, SyncPanel and SyncManagementPanel sit in one folder.
- RepositoryManager and RepositoryTab name one thing twice. Repository appears only there; the data type is ControlRepository.
- ComposableManagerItem and ComposableItemSettings order the same words two ways.
- Selection means the panel for choosing in SelectionPanel and EntitySelectionPanel, and the selected item in Select and ClearSelection.
- ButtonModulatorAssign uses a verb as a suffix. EntityModifiersSection is the only Section. CharWriterModifier is an empty view.
- docs/DEVELOPMENT.md lists the child namespaces the locator skips; Dialogs is missing from the list although MessageDialog lives there.

## 6. UI kit gaps

- About 75 views have no Part B entry: the 34 Texturing filter and source views, the 25 Modifier views, EntitySelectionPanel, LocalReflection, AmbientOcclusion, FullscreenQuad, ModelEntity, TextEntity, MaterialSettings, CameraSettings, LightSettings, Transform2D, SamplerState, MaskTexture, DiffuseTexture, SelectedTexture, OutputMappingsPanel, RedisAddressesPanel, RepositoryManager, AssetManager, SyncPanel, SyncManagementPanel, SyncSettings, MainWindowController, ToastHost.
- Out of date: 22.1 misses the Color and TrackingModulatorBase rows, 22.3 misses the swap popup and the filled class, 24.1 and 24.2 show plain buttons where the view uses ButtonIcon filled, 27.7 describes a multi line field that SplitModifier uses for one line.
- No entry for MenuFlyoutPresenterDefault, CloseFlyoutOnPointerAwayBehavior, UserControlCaptioned, the classes filled and window.
- Decision for the owner: does Part B list every domain view, or only the shared controls and the panels?

## Suggested order

1. Theme 1: write the rule into docs/DEVELOPMENT.md, then merge the three pointer away shapes, drop the dead state and the dead behavior, and move each class to its folder. No look changes.
2. Theme 4: add the Thickness tokens, then one mechanical pass per folder. The kit tokens update in the same commits.
3. Theme 2: the duplicate views, one pair per step, starting with the three Layer tabs and the two Mesh views.
4. Theme 3: the code behind, one view per step, SyncSettings and MainWindow first.
5. Theme 5: the names, after the shapes are settled, so a rename happens once.
6. Theme 6: the kit entries, folded into the step that touches each view, plus the owner decision on Part B.
