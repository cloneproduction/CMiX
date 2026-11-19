# CMiX architecture

This document is context for Claude and for the refactorer agent (`.claude/agents/Refactorer.md` in the parent repository).
It is the map of the code. Read it before you change code.

Date of the survey: 2026-11-13. Sizes are lines of C# without AXAML.

**How to use it.**

- Part 1 is facts about the code at the date of the survey. Check a fact against the code before you build on it. Where the text says "not sure", read the code.
- Part 2 is a proposal. The owner has not approved it. Do not start a step of Part 2 section 4 without his approval, and do not decide an open question of section 5: ask.
- The class rules (one job, required dependencies, the tripwires, the MVVM rules) are in the agent file. This document does not repeat them and does not change them.
- The working rules of the repository are in `.claude/CLAUDE.md`: the language, the commits, the pinned names, the tests. Read it too. `docs/DEVELOPMENT.md` describes the UI library, the tree and the sync in detail.
- Names that the VL patches, the saved files and the wire use do not change without a plan. See "Names that do not change without a plan" in `.claude/CLAUDE.md`.

**The five shapes.** These are the definitions of the agent file.

| Shape | Definition |
|---|---|
| Model | Domain data that owns its children. It knows nothing about UI, undo or messaging. |
| ViewModel | State and commands for one piece of UI. It never references a view, never does I/O, and has no Avalonia type. |
| View | XAML plus view-only behaviour (focus, pointer, drag). It holds no logic and no data. |
| Service | One capability behind an interface. It holds no UI state. |
| Message | A plain record sent between parts or to the engine. No behaviour. |

In Part 1, a class counts as a Service when it is one capability with no UI state, with or without an interface. Most Core services have no interface.

---

# Part 1: What is there now

## 1. Projects

| Project | Purpose | References | Size |
|---|---|---|---|
| `CMiX.Core` | The domain objects, their models, the lists, undo, the sync over Redis. The Studio and the engine both load it. | Packages only: CommunityToolkit.Mvvm, StackExchange.Redis, VL.Core, VL.Serialization.MessagePack, Scrutor | 402 files, 17,800 lines |
| `CMiX.UI` | Generic Avalonia controls, behaviors, converters and the theme. It knows no domain type. | Avalonia only. A test (`LibraryBoundaryTests`) keeps it free of Core. | 133 files, 10,800 lines, 68 AXAML |
| `CMiX.Studio.Avalonia` | The Studio application: the views of the domain objects, the project tree, the main window. | `CMiX.Core`, `CMiX.UI` | 258 files, 13,700 lines, 176 AXAML |
| `CMiX.Console` | An engine without a window. It joins the sync and applies the updates. | `CMiX.Core` | 2 files |
| `CMiX.UI.Gallery` | A window that shows each control of `CMiX.UI` in each state. | `CMiX.UI` | 3 files |
| `CMiX.Engine` | The vvvv patches (`*.vl`), the shaders, and the helper project `CMiX_ENGINEUtils`. The patch references `CMiX.Core` as a project. | `CMiX_ENGINEUtils` references VL.Core and VL.IO.Redis, not Core | 3 patches, 8 C# files |
| `CMiX.Core.Tests`, `CMiX.UI.Tests`, `CMiX.Studio.Avalonia.Tests` | The tests of the project with the same name. | That project | 18,700, 13,800 and 32,900 lines |
| `build` | The build script project. | Fallout.Common | not read |

The reference graph has no cycle: `Studio` to `Core` and `UI`. `Core` and `UI` do not know each other.

## 2. Shapes

The most important fact: **a domain object in Core is not one shape.**
`Entity`, `Layer`, `Blur` and about 120 more classes are each a pair:

- `Blur` is the live object. It is observable, it holds commands, it holds view state (`IsExpanded`), it converts itself to and from its model, and its values send and receive messages. The code calls it a **control** (`IControl`).
- `BlurModel` is a record with data only. This is a true Model.

The view `Blur.axaml` in the Studio binds straight to the Core `Blur`. There is no ViewModel class between them. The engine loads the same `Blur`.

### CMiX.Core

| Shape | Main classes |
|---|---|
| Model | All 125 `...Model` records: `ProjectModel`, `CompositionModel`, `LayerModel`, `EntityModel`, `GenericValueModel<T>`, `ModulatableValueModel<T>`, `PrefabManagerModel`, `ManagerDataModel`, each filter, source, modifier and modulator model. Also `SyncOptions`, `SyncTimings`, `StreamPosition`, `PeerInfo`. |
| Message | `MessageValueChanged`, `MessageAddItem`, `MessageRemoveItem`, `MessageMoveItem`, `MessageSelectItem`, `MessageClearSelection`, `MessageOnClick`, `MessageProjectSnapshot`, and the wrapper `MessageEnvelope` (all in `Networking/Messages/Messages.cs`). |
| Service | `UndoManager`, `ControlFactory`, `ControlRepository`, `ControlMessenger`, `MessageFactory`, `MessageCollectionManagerHandler`, `ControlActivationService`, `AssetRepository`, the five processors (`VisibilityProcessor`, `VisibleItemsProcessor`, `UpdateOrderProcessor`, `RenderListProcessor`, `AssetListProcessor`), `ProjectSyncTarget`, `RedisSyncStore`, `OutgoingQueue`, `StreamFollower`, `SnapshotCompactor`, `Presence`, `MainloopQueue`, `EngineHost`, `WindowsCredentialStore`. |
| ViewModel | None that is only a ViewModel. |
| View | None. |

Classes that are several shapes at once, or none:

| Class | What it is |
|---|---|
| Every control: `Project`, `Composition`, `Layer`, `Entity`, `TextEntity`, `FullscreenQuad`, `Camera`, `LightEntity`, `Texture`, the 22 filters, the 11 texture sources, the 20 modifiers, the 13 modulators, the settings objects | Domain state, ViewModel (commands, `IsExpanded`) and model converter in one class. |
| `GenericValue<T>`, `CMiXButton` | One value. Also the undo source, the message sender and the message receiver. |
| `ModulatableValue<T>` | Three `GenericValue` in one: the value, the id of the modulator, the name of the output. Also commands. |
| `PrefabManager` | A list of controls. ViewModel (7 commands), undo source, message sender and receiver, model converter. The VL patches use it by name. |
| `ManagerData`, `CollectionManager`, `CollectionSync` | The three inner parts of a list: the items and the selection, the add and remove with the repository, the sender. Not a shape of their own. |
| `SyncPeer` | Service (the sync state machine) and ViewModel (the sync panel binds its properties and its `PushCommand` and `PullCommand`). |
| `MasterBeat`, `AssetManager`, `OutputMappingManager`, `RedisAddressRegistry` | Singleton and control at once. |
| `PrefabService` | Not a service. It is the name and the visibility of a prefab. |
| `ValueInteraction`, `ControlCloner`, `ListRules`, `DerivedId`, `ManagerIDs`, `ProjectSerializer`, `ProjectStateHash`, `ProjectModelBuilder`, `SyncPeerStartup`, `ControlExtensions` | Static helpers. `ValueInteraction` holds state. |
| The undo steps (`AddItemUndoCommand`, `ValueChangedCommand`, `CompositeCommand`, ...) | None of the five shapes. They are small objects with `Execute` and `Undo`. |

### CMiX.Studio.Avalonia

| Shape | Main classes |
|---|---|
| View | Everything in `Views/`: one view for each Core control with the same name (`Blur`, `Layer`, `ModelEntity`, ...), `MainWindow`, `ProjectTreeView`, `Inspector`, `SyncPanel`, `AssetManager`, `MainMenu` (the view), the tree helpers (`TreeDragSession`, `TreeGrab`, `AddPanel`, `PrefabRowMenu`, ...), the converters and the behaviors. |
| ViewModel | `MainViewModel`, `MainMenuViewModel`, `ProjectFileViewModel`, `MainWindowController`, `ShortcutSettings`, `PanelChoice`, and the tree: `ProjectTree`, `TreeNode`, `TreeGroup`, `TreeAdd`, `TreeViewHooks`. |
| Service | `ShortcutMap`, `InterfaceOptions`, `StudioSettings`, `JsonSettingsFile`, `PathDropService`, `PanelFocusCycle`, `ToastService`, `UnsavedChanges`, `UnsavedChangesPrompt`, `MessageNotice`, `MoveBlockedNotice`, `MainWindowDialogManager`, `TreeDropService`. |
| Model, Message | None. |

Several at once, or none:

| Class | What it is |
|---|---|
| `ManagerReorderService` | Named a service. It is a small ViewModel (observable, 2 commands), one for each list. |
| `MainViewModel`, `MainWindowController` | ViewModels that also implement `IControl`. `ToModel` and `FromModel` throw. `MainMenuViewModel` dropped it on 2026-11-19. |
| `ViewModelToViewTemplate` | The view locator. It finds the view by the type name of the control. |
| `TreeViewHooks` | A set of callbacks, one per tree view. `TreeViewSet` in `ProjectTree` holds them and picks the active view. See `ViewModels/Tree/README.md`. |
| `AnimatedDouble`, `BeatAnimations`, `MasterBeatAnimationController` | Animation helpers. I did not read them in detail. |

### CMiX.UI

All classes are Views or view helpers: controls, panels, attached properties, behaviors, converters, the theme.
Three static entry points connect it to an application: `InteractionHost`, `Icons` and `OutsideClick`.
`docs/DEVELOPMENT.md` describes this library in detail and it matches the code I read.

## 3. Flows

### A value changes (the user drags a slider)

1. **View.** `SliderModulatable` binds to `ModulatableValue<float>.Value`. The CMiX.UI editor opens a drag scope through `InteractionHost`, which `App.Initialize` connects to `ValueInteraction.BeginScope`.
2. **Value.** `ModulatableValue.Value` passes the write to `GenericValue<T>.Value`. The setter does three things: it stores the value, it pushes a `ValueChangedCommand` to `UndoManager`, and it calls `SendValueChanged`. During a drag it sends at most 60 messages each second and makes one undo step for the whole drag.
3. **Message.** `MessageFactory` makes a `MessageValueChanged` with the id of the value and its model. `ControlMessenger.SendMessage` gives it to each registered sender. A blocked messenger drops it.
4. **Peer.** `SyncPeer.SendMessage` drops the message if the peer is not the writer. Else it wraps it in a `MessageEnvelope`, serializes it with MessagePack and puts it in `OutgoingQueue`.
5. **Redis.** One task appends the entry to the stream `{prefix}:updates` through `RedisSyncStore`, sets the tail key and publishes a wake signal. `SnapshotCompactor` writes a new snapshot later.
6. **Engine.** `StreamFollower` reads the new entries. `SyncPeer` posts each batch to its dispatcher. In vvvv that is `MainloopQueue`, which the patch drains once for each frame. In `CMiX.Console` the apply runs inline.
7. **Apply.** `ProjectSyncTarget.Apply` sends the message on `WeakReferenceMessenger.Default`. Every active control receives every message. `GenericValue.Receive` compares the id and, on a match, sets the value inside `SuppressSending()`, so it sends nothing and records no undo step.
8. **Render.** The patch reads the state through `EngineHost.ServiceProvider`: `Project`, `ControlRepository`, `PrefabManager`, `ManagerData` and the processors. I did not open the patches in vvvv. I only searched them for type names.

### A list changes (add, remove, move, select)

The view calls a command of `PrefabManager`, for example `AddItemCommand`. `PrefabManager` makes an undo step (`AddItemUndoCommand`), runs it and pushes it. The step changes the list through `CollectionManager` (which also updates `ControlRepository`) and sends `MessageAddItem` through `CollectionSync`. On the engine, `PrefabManager.Receive` compares the id of the list and gives the message to `MessageCollectionManagerHandler`.

### The way back

The engine does not write to the stream. Three things come back to the Studio:

- **Presence.** Each peer writes its peer key every 2 s. The Studio lists the peers and shows the lag of each.
- **Start check, Push, Pull.** `SyncPeer.CheckStartAsync` compares the stored state with the loaded project. The sync panel offers Push or Pull.
- **Engine values.** `DEVELOPMENT.md` says that the patch can write plain Redis keys. The Redis modulators and `RedisAddressRegistry` hold addresses for this. I did not find the place where the Studio reads such a key, and I am not sure there is one.

### Undo and redo

`UndoManager` is one singleton with two stacks and a limit of 128 steps. `MainViewModel.UndoCommand` calls it; the menu binds that command through the window since 2026-11-19.
A step undoes itself through the same code as a normal edit, so the undo also sends messages to the engine. `UndoManager.IsApplying` stops the edit from recording a new step.
Two value steps on the same control within 500 ms merge. `BeginCapture` and `EndCapture` group several steps into one `CompositeCommand`.
A removed control stays alive inside its step. It is disposed when the step leaves the stack.
A snapshot apply (join, pull, File Open) clears both stacks.

### Selection

There are two selections, and they are linked.

- **The selection of a list.** `ManagerData.SelectedItem`. Each `PrefabManager` has one. It is saved in the file (`ManagerDataModel.SelectedIndex`), it is sent (`MessageSelectItem`), and it is an undo step (`SelectItemUndoCommand`). The VL patches read `SelectedIndex` and `SelectedItem`.
- **The selection of the tree.** `ProjectTree.SelectedNode`. It exists only in the Studio. `ProjectTree.Select` calls `SelectChain`, which sets `SelectedItem` on each list from the node up to the root. So one click in the tree can send several select messages. A change that comes from a list goes back to the tree through `SelectFromModel`. `Inspector` binds to `SelectedNode.Item` and shows the view of that control.

The tree also has a **cursor row** for the keyboard (`TargetNode`). It is not the selection.

### Construction

`AddCMiXCore` registers every `IControl` class as transient and the services as singletons. `ControlFactory.Create` finds the control type from the model type by name (`BlurModel` gives `Blur`), builds it from the container, loads a default model, records the defaults for Reset, loads the real model, names it, and activates its values. A control that the container builds without the factory has no defaults and no name.

## 4. Vocabulary

| Term | Meaning in the code |
|---|---|
| Peer | One running process in the sync: the Studio, a vvvv engine or a Console. |
| Studio, engine | The one writer with the UI. A reader that renders. |
| Project | The root. It holds the compositions, the master beat, the output mappings and the Redis addresses. |
| Composition | One output picture. It holds layers, filters and modifiers. |
| Layer | One 3D scene inside a composition. It holds entities, cameras, lights, filters and modifiers. |
| Composable | A composition or a layer: something with compositing and mask settings (`IComposable`). |
| Entity | A mesh with a transform, a material, a texture and a color. |
| Control | Any live domain object with an id and a model (`IControl`). |
| Model | The data record of a control (`...Model`). |
| Prefab | A control with a name and a visibility that can sit in a list (`IPrefab`). |
| Manager | A list of controls with a selection (`PrefabManager`). |
| Repository | The index of all controls by id (`ControlRepository`). |
| Value | One editable number, flag, text or id (`GenericValue<T>`). |
| Modulatable value | A value that a modulator output can drive (`ModulatableValue<T>`). |
| Modulator | A source of changing numbers (LFO, beat, FFT, Redis key, tracking) with one or more outputs. |
| Modifier | An item in a `ModifierManager` that changes its owner (transform, color, visibility). It owns modulators. |
| Texture source | Something that makes a picture: an image, a video, a noise (`ITextureSource`). |
| Filter | An effect on a picture: blur, edge, kaleidoscope (`ITextureFilter`). |
| Asset | A file or a folder on disk that a control points to. |
| Output mapping | Where a composition goes on the outputs. I did not read it in detail. |
| Master beat | The clock of the show. |
| Processor | A service that gives the engine a ready list of ids (visible items, update order, assets). |
| Snapshot, stream, tail | The full project in Redis. The log of messages. The id of the newest entry. |

Names that collide:

| Problem | Where |
|---|---|
| **Control** means three things | A domain object (`IControl`), an Avalonia control, and the folder `Views/Controls`. |
| **Model** means three things | A data record (`BlurModel`). The live control: `DEVELOPMENT.md` says "the view of a model", and `ControlFactory` names the same thing `viewModelType`. A 3D model (`ModelEntity`, `ModelEntityManager`). |
| **Manager** means five things | A list (`PrefabManager`), its inner part (`CollectionManager`), a singleton (`AssetManager`, `OutputMappingManager`, `UndoManager`), a view (`ModifierManager`, `RepositoryManager`), a dialog helper. |
| **Service** | `PrefabService` is the name and the visibility of a prefab. `ManagerReorderService` is a ViewModel. |
| **Entity** | `Entity` is the mesh object. The view is `ModelEntity`. `ControlRepository.ModelEntities` is `Entity`, `TextEntity` and `FullscreenQuad`. `LightEntity` is a light. |
| **Texture** | `Texture` is the pair of diffuse and mask. `ControlRepository.Textures` and `MainViewModel.TextureManager` hold texture sources. |
| **Filter = texture modifier** | The classes are filters (`ITextureFilter`, folder `Filters`). The list is `TextureModifierManager` and the interface is `ITextureModifiable`. A texture modifier is not a `Modifier`. |
| **Message** | A wire message (`IMessage`) and a notice to the user (`MessageNotice`, `MessageDialog`). The sync views are in `Views/Messenger`, the code is in `Networking`, the classes say `Sync`. |
| **Command** | `...Command` is for XAML and `...UndoCommand` is an undo step. `ValueChangedCommand` and `CompositeCommand` are undo steps without the `Undo` word. |
| **Visible, Visibility, Hidden** | `PrefabService.Visibility`, `PrefabManager.Hidden`, and `VisibilityComponent.IsVisible => !Visible.Value`, where true means hidden. |
| **Repository** | `ControlRepository` is the index. `RepositoryManager` and `RepositoryTab` are views of the six lists that `MainViewModel` owns. |
| **Tree, Outliner** | `ProjectTree` is the tree. `Views/Controls/Outliner` is another control, used in the `Mesh` view. |
| **Delete** | The rule says no `Delete` in code. Fixed on 2026-11-19: the menu has `RemoveCompositionCommand` and `RemoveLayerCommand`, and only the menu headers say Delete. |
| Two enums with one name | `HorizontalAlignment` in `Assets.Mesh` and in `Text`. `WaveType` and `WaveTypeEnum`. |

## 5. Rule violations

### ViewModels and services that know views or Avalonia

| Class | What it uses |
|---|---|
| `MainMenuViewModel` | Clean since 2026-11-19: no Avalonia type. The six composition and layer commands run after the menu popup closes through the view behaviour `RunCommandAfterClose` in CMiX.UI. |
| `MainWindowController` | Takes an Avalonia `Window` as the command parameter and changes its state. |
| `ShortcutSettings`, `ShortcutMap` | Avalonia input types. |
| `PanelFocusCycle` | Uses `CMiX.Studio.Avalonia.Views` and Avalonia. |
| `MessageNotice`, `MoveBlockedNotice`, `UnsavedChangesPrompt` | Open windows from `Views.Dialogs`. They sit behind interfaces, so their users stay clean. |
| `ProjectTree`, `TreeNode` | No Avalonia type. The tree reaches the view through `TreeViewHooks` (one set per view) and 8 request events. Since 2026-11-19 the tree is a hub with parts, see `ViewModels/Tree/README.md`. |

Core has no Avalonia reference.

### Statics

| Static | Where | Note |
|---|---|---|
| `ValueInteraction` | Core | Static state: the open drag scope and its values. `GenericValue` reads it in its setter. |
| `WeakReferenceMessenger.Default` | Core, `ProjectSyncTarget.Apply` | Every `ReceivableControl` listens on this global messenger. `ControlMessenger` (injected) is only the way out. |
| `App.MessageNotice`, `App._crashSaveProject` | Studio | A comment in `App.axaml.cs` gives the reason for each. |
| `InteractionHost.BeginScopeHandler`, `Icons.Register`, `OutsideClick.Install` | CMiX.UI | The library has no container. |
| `Application.Current` | 2 converters, `CategoryPalette`, 3 notice services, `MainWindowController` | To find a resource or the main window. |

### Optional and hidden dependencies

- `ProjectTree`: fixed on 2026-11-19, all seven constructor parameters are required.
- `UndoManager` is optional in practice. `ReceivableControl.UndoManager` is a settable property, the owner sets it after construction (`Project`, `MainViewModel`, `RedisAddressRegistry`), and 24 call sites use `UndoManager?.`.
- `PrefabManager(..., reorderServiceFactory = null)` and `Modifier(..., nestedBindables = null)`.
- `GenericValue<T>` has an empty public constructor that leaves the messenger and the factory null.
- `ControlFactory` takes `IServiceProvider` and resolves types by name. A control from the container and a control from the factory are not the same.
- `ProjectSession` owns the six repository lists since 2026-11-19; `MainViewModel` and `ProjectFileViewModel` take it. The settable `MainMenu.RepositoryManagers` is gone.
- `MainViewModel` sets the fixed ids of six lists and activates them by hand, then clears `ControlActivationService`.

### Models and Core that hold editor concerns

- `IModifierModel.IsExpanded` and the 21 modifier models: the open state of a panel is in the project file and on the wire.
- `ManagerDataModel.SelectedIndex`: selection is in the file, on the wire and in undo. The engine reads it, so this can be intended. See the open questions.
- `MeshModel.IsSelected`: I am not sure what it selects.
- 26 Core controls hold view state: `IsExpanded`, `Layer.SelectedTabItemIndex`, `IModulator.IsHovered`, five `...IsExpanded` flags on `Entity`. The engine loads all of it.
- Core controls expose `ICommand` for XAML.

### Other findings

- 62 Core files have a namespace that differs from their folder, for example `Project.cs` is in the folder `Project` and the namespace `CMiX.Core.Compositing`.
- `CMiX_ENGINE.vl` holds the name `CMiX.Core.Presentation.ViewModels.Beat.MasterBeat` 8 times. No such namespace exists in Core now. I do not know if vvvv still resolves it.
- The six lists of `MainViewModel` (textures, entities, cameras, lights, beats, color palettes) are not part of `ProjectModel`. A snapshot apply empties them.

## 6. The ten worst classes

| # | Class | Lines | Ctor params | Commands | Why |
|---|---|---|---|---|---|
| 1 | `ProjectTree` (Studio) | 169, six partial files, 873 in all | 7 | 27 | Done on 2026-11-19: the state and the commands stay on the hub, the logic is in parts (`TreeSearch`, `TreeEdit`, `TreeOpenState`, `TreeAddCatalogue`, `TreeDragState`, `TreeDrop`, `TreeFileDropRules`, `TreeViewSet`). See `ViewModels/Tree/README.md`. |
| 2 | `SyncPeer` (Core) | 1137 | 3 | 2 | Connect, join, push, pull, gap check, send, apply, and the state for the sync panel. Service and ViewModel at once. |
| 3 | `ProjectTreeView` code-behind (Studio) | 233 | 0 | 0 | Done on 2026-11-19: the hub builds the parts (`TreeKeyInput`, `TreePointerInput`, `TreeFocusKeeper`, `TreeScreenRequests`, `TreeMenus`, `TreeRowReveal`, `TreeFileDropInput`, `TreeHints`). |
| 4 | `PrefabManager` (Core) | 350 | 9, 1 optional | 7 | List ViewModel, undo, send, receive, model converter. Every domain object has several. VL uses its names. |
| 5 | `GenericValue<T>` (Core) | 190 | 4 or 0 | 1 | Value, undo, throttle, send and receive in one setter. It reads a static. |
| 6 | `MainViewModel` (Studio) | 160 | 18 | 2 | A bag of everything for the bindings. It also sets ids and activates lists. A false `IControl`. |
| 7 | `MainMenu` (Studio) | 68, as `MainMenuViewModel`, plus `ProjectFileViewModel` 148 and `ProjectSession` 130 | 1 | 7 | Done on 2026-11-19: the project file is a ViewModel of its own, the session and the six lists are one service, the dispatcher and `Application.Current` are gone, Quit is a request event. |
| 8 | `TreeNode` (Studio) | 430 | 7 | 0 | Done in part on 2026-11-19: the counts are in `RowCounts`, the drag look in `DragLook`, the search look computed by the row itself, and the row reports to the tree through `TreeNodeHost`. It still knows many Core types by name. |
| 9 | `ControlFactory` (Core) | 222 | 3 | 0 | Creation by name convention, defaults, activation, and the naming of prefabs with its own cache. |
| 10 | `Layer` (Core) | 137 | 10 | 0 | Stands for the pattern of all controls: five lists, view state, and `ToModel` and `FromModel` written by hand. |

---

# Part 2: What it should be (proposal)

## 1. Layer rules

Projects. Keep the graph of today. It is good.

| Project | May reference | Must not reference |
|---|---|---|
| `CMiX.Core` | Packages without UI | Avalonia, `CMiX.UI`, the Studio |
| `CMiX.UI` | Avalonia | `CMiX.Core`, the Studio |
| `CMiX.Studio.Avalonia` | `CMiX.Core`, `CMiX.UI` | |
| `CMiX.Console`, the engine patch | `CMiX.Core` | Any UI project |

Shapes.

| Shape | May depend on | Must not |
|---|---|---|
| Model | Other models, plain types | Anything with behaviour. View state, unless the owner decides it belongs to the show. |
| Message | Models | Controls, services |
| Service | Services, models, messages. All through the constructor, all required. | Views, Avalonia, statics, `IServiceProvider` (except one factory) |
| Control (the live domain object in Core) | Services, other controls, its model | View state, Avalonia, statics |
| ViewModel (Studio) | Controls, services | Views, Avalonia types, `Application.Current`, the dispatcher |
| View | Its ViewModel or control by binding, `CMiX.UI` | Services, Redis, the undo stack |

The row "Control" is not one of the five shapes. It is in the table because the code has it today. Open question 1 decides if it stays.

A ViewModel never asks the view. The view pushes its state (focus, pointer, drag) into properties of the ViewModel. A ViewModel that must open a dialog or a toast uses a service, as `IMessageNotice` does today.

## 2. The standard structure for one feature

For a domain object named `X` in the area `Area`:

| File | Place | Content |
|---|---|---|
| `XModel.cs` | `CMiX.Core/Area/` | The record. Each value has its name and its default. |
| `X.cs` | `CMiX.Core/Area/` | The control: the values, the lists, `ToModel`, `FromModel`. No view state. |
| `X.axaml`, `X.axaml.cs` | `CMiX.Studio.Avalonia/Views/Area/` | The view. The code-behind holds only `InitializeComponent`. |
| `XTests.cs` | `CMiX.Core.Tests/Area/` | The round trip of the model and the behaviour. |
| `XViewTests.cs` | `CMiX.Studio.Avalonia.Tests/` | The bindings and the Reset menu. |

For a feature of the Studio with no domain object (the tree, the sync panel, the main menu):

| File | Place | Content |
|---|---|---|
| `FeatureViewModel.cs` | `ViewModels/Feature/` | State and commands. One class for one job: the tree, the search and the Add catalogue are three classes. |
| `FeatureView.axaml` | `Views/Feature/` | The view. |
| `IFeatureXxx.cs` and its class | `Services/` | What the ViewModel needs from the window or the disk. |

The namespace follows the folder, with the one known exception of the domain views.

## 3. Naming rules

These add to the rules in `.claude/CLAUDE.md`. They do not replace them.

- One word for one thing. **Control** is an `IControl`. An Avalonia control in a sentence is "UI control".
- **Model** is only a `...Model` record. Do not call a control a model or a view model.
- **Filter** for an `ITextureFilter`, in every layer. **Modifier** only for an `IModifier`.
- **List** in sentences for a `PrefabManager`. The class name stays until the owner decides (see the open questions).
- **Service** only for the Service shape. **Manager** for no new class.
- A Studio ViewModel ends with `ViewModel`. A view of a Studio feature ends with `View`. A domain view has the name of its control.
- An undo step ends with `UndoCommand`: `ValueChangedUndoCommand`, `CompositeUndoCommand`.
- No `Delete` in code: `RemoveCompositionCommand`.
- A flag says what true means: `IsHidden` or `IsVisible`, never `Visible` that means hidden.
- **Sync** for the feature in every layer: folder, views, classes.
- **Notice** for a text to the user. **Message** only for the wire.

## 4. Order of the cleanup

The order goes from low risk to high risk. First the classes that only the Studio uses, because no file, no wire and no patch depends on their names. Then the Core services. Then the Core classes with pinned names. The pattern of all controls is last, because it needs the answers to the open questions.

| Step | Class | First action | Why here | Complexity |
|---|---|---|---|---|
| 1 | `MainViewModel` | Half done on 2026-11-19: the six lists are in `ProjectSession`. Left: remove `IControl`. | Small, Studio only, and it shows the true dependencies of the window. | 2 |
| 2 | `MainMenu` | Done on 2026-11-19 on the branch `refactor/tree-structure`: `ProjectSession` (Service), `ProjectFileViewModel`, `MainMenuViewModel`, the behaviour `RunCommandAfterClose`. | Studio only. | 3 |
| 3 | `ProjectTree` | Done on 2026-11-19 on the branch `refactor/tree-structure`. | The bound state stayed on the hub in partial files, the logic went into parts. | 4 |
| 4 | `TreeNode` | Done on 2026-11-19: `DragLook`, `RowCounts`, `TreeNodeHost`, and the row computes its own search look. | The row keeps its identity and its list mirroring. | 3 |
| 5 | `ProjectTreeView` | Done on 2026-11-19, with one deviation: `TreeViewHooks` stays, one set per view, because two views can show one tree and the tree must know which view answers. The next step is a per-view state object that `TreeViewSet` reads. | After steps 3 and 4. | 4 |
| 6 | `ControlFactory` | Move the naming of prefabs into its own service. | Core, and no name on the wire or in a patch changes. | 2 |
| 7 | `GenericValue<T>` | Make `ValueInteraction` an injected object. Remove the empty constructor. | Every value depends on it, so do it before the lists. | 4 |
| 8 | `SyncPeer` | Split the state for the panel from the peer. | Well tested and isolated. The Redis tests need a server. | 4 |
| 9 | `PrefabManager` | Make `UndoManager` required. Separate the commands for XAML from the list logic. | VL pins its names. Needs step 7. | 5 |
| 10 | `Layer` and all controls | Depends on the open questions 1 and 2. | The largest change. It touches every saved file and both peers. | 5 |

## 5. Parked: the split of the domain objects

The owner parked this on 2026-11-13. It is too large to do now. It is not rejected.

**The problem.** A Core control such as `Blur` is three shapes at once: the domain data, the ViewModel of its panel (`IsExpanded`, commands), and the undo source and message receiver (through its values). About 120 classes have this pattern, and the engine loads all of it.

**The target.** Three parts in place of one class:

- `Blur` in Core: data only. No command, no view state, no undo, no messenger. The engine reads it.
- `BlurViewModel` in the Studio: the panel state and the commands. It wraps one `Blur`.
- One edit service in Core: it changes the model, records one undo step and sends one message. It replaces the undo and send code inside each `GenericValue`.

**Why it is large.** Each of the 120 classes gets a ViewModel. The VL patches read `PrefabManager` and `ManagerData` by name. The `...Model` records are the file format and the wire format. The engine applies a message through the value that listens for its id. The modulation (`ModulatableValue`) has no design for the split yet.

**Until the owner starts it:**

- Do not report this pattern as a finding for each class. It is known. Report it once at most, with a pointer to this section.
- Do not start the split, in whole or for one class.
- Do not make it worse: put no new view state (expanded, hovered, selected tab) and no new command for XAML into a Core control or a model. Put it in the Studio.
- A cleanup inside a Core control is still welcome when it does not depend on the split.

**A cheap first step, when the owner wants one:** move the view state (`IsExpanded` and the like) out of Core. Check first what it does to saved projects, because `IsExpanded` of a modifier is in the file.

## 6. Open questions

Only the owner can decide these. Question 1 is parked: see section 5.

1. **One class or two for a domain object?** Today `Blur` is the domain state and the ViewModel, and the engine loads both. Split it (state in Core, ViewModel in the Studio), or keep it as the one shape "control" and only take the view state out?
2. **Is view state part of the show?** `IsExpanded` of a modifier is saved and sent. The tab of a layer is not. Which view state belongs in the file, and does any of it belong on the wire?
3. **Is the selection of a list show state or editor state?** It is in the file, on the wire and in undo, and the patches read it. Does the engine need it, and must a selection be an undo step?
4. **Can pinned names change?** `PrefabService`, `PrefabManager`, `ManagerData` and `TextureModifierManager` are misleading, and the patches, the saved files and the wire use them. Is a rename worth a change of the patches and an entry in `LegacyTypeNames`?
5. **The global messenger.** Receive goes through the static `WeakReferenceMessenger.Default`, and each message visits each control. Accept it as `ValueInteraction` is accepted, or replace it with a lookup by id in `ControlRepository`?
6. **The six lists of the main window.** They are not in `ProjectModel`, and a snapshot empties them. Is that intended?
7. **Will an engine write again?** `DEVELOPMENT.md` describes how. The answer changes how strict "one writer" can be in the design.
8. **The old name in the patch.** Is `CMiX.Core.Presentation.ViewModels.Beat.MasterBeat` in `CMiX_ENGINE.vl` a dead text or a red node?
