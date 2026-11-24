# Plan: split the Services folder of the Studio

Goal: the folder `CMiX.Studio.Avalonia/Services` goes away. Its types move into folders that carry the name of their job. Each file holds one type. The names of one family use one word. The startup registration is written once. The rules of this plan go into `.claude/CLAUDE.md`, so the next type lands in the right place without a discussion.

The analysis read the code on branch `refactor/tree-structure` on 2026-11-24. The plan was built on the same day. The folder is gone and the rules are in `.claude/CLAUDE.md`, section "Folders and names of the Studio".

## 1. Findings

Paths are relative to `CMiX.Studio.Avalonia`.

1. The folder holds 20 files and 23 types with four jobs: tell the user something (10 files), read and write settings files (5 files), hold the running project (3 files), and two misfits (2 files).
2. Three files hold more than one type: `Services/StudioSettings.cs` (RedisSettings, InterfaceSettings, StudioSettings), `Services/ShortcutMap.cs` (ShortcutFile, ShortcutMap), `Services/IUnsavedChangesPrompt.cs` (UnsavedChangesChoice, IUnsavedChangesPrompt).
3. `MessageNotice`, `MoveBlockedNotice` and `UnsavedChangesPrompt` look up the main window with the same pattern match. `ViewModels/MainWindowController.cs` does it a fourth time.
4. The settings file has three readers: `StudioSettings.Load(path)` and `Save(path)` as statics, `StudioSettingsFile.Load()` and `Save()` as a wrapper, and `InterfaceOptions` that calls the static with its own path.
5. `ManagerReorderService` is a view model: an `ObservableObject` with two `RelayCommand` and one `[ObservableProperty]`, one per list, made by `ManagerReorderServiceFactory` of CMiX.Core. Six XAML files bind to it.
6. `PanelFocusCycle` walks the visual tree and names `PanelSide` and `ListTreeProject`. It is view code.
7. Three types end with Service, seventeen do not. The user facing family uses Notice, Prompt and Notification for three near things. `InterfaceOptions` and `InterfaceSettings` are two names for one idea.
8. `App` has a property `MessageNotice`, so `App.axaml.cs` must write `Services.MessageNotice` and `Services.EmptyViewLocator`.
9. `App.axaml.cs:66-101` registers 22 types. `CMiX.Studio.Avalonia.Tests/TestServiceProviderFactory.cs` repeats the list with four fakes.
10. The four interfaces (`IMessageNotice`, `IMoveBlockedNotice`, `IUnsavedChangesPrompt`, `INotificationService`) each have a test fake. The other types are concrete and injected as such. This is right and stays.
11. `MainWindowController` implements `IControl` and has an `ID` that nothing reads.
12. The parent repository holds CMiX only as a submodule copy. No VL patch names a Studio namespace. The moves do not touch vvvv.

## 2. Decisions

Decided by the owner on 2026-11-24. Yes to all.

| # | Decision |
|---|---|
| D1 | Three new folders: `Notices` (tell the user), `Settings` (files of the user), `Session` (the running project). The namespace follows the folder. |
| D2 | `ManagerReorderService` moves to `ViewModels`. `PanelFocusCycle` moves to `Views`. |
| D3 | One type per file. A private nested type stays where it is (`PathDropService.Kind`). |
| D4 | `PathDropService` becomes `PathDropRules`. `TreeFileDropRules` already uses the word Rules. |
| D5 | `NotificationService` becomes `StatusNotice`, `INotificationService` becomes `IStatusNotice`. The family is then IMessageNotice, IMoveBlockedNotice, IStatusNotice, IUnsavedChangesPrompt. A Notice tells. A Prompt asks. |
| D6 | `InterfaceOptions` becomes `InterfaceState`. `InterfaceSettings` stays the shape in the file. The Tree already uses State for live observable objects. |
| D7 | `ManagerReorderService` keeps its name. CMiX.Core pins it through `IManagerReorderService` and `ManagerReorderServiceFactory`. A rename is its own plan. |
| D8 | One static class `MainWindowOwner` with the pure function `Find()` replaces the four lookups. |
| D9 | `StudioSettingsFile` is the one door to the file. `StudioSettings` keeps `FileName` and `DefaultPath` and loses `Load` and `Save`. `InterfaceState` takes a `StudioSettingsFile`. |
| D10 | One method `AddCMiXStudio` in `DependencyInjection/ServiceCollectionExtensions.cs` registers every Studio type. `App.axaml.cs` and `TestServiceProviderFactory` call it. A test adds its fakes after the call. The last registration wins in Microsoft.Extensions.DependencyInjection. |
| D11 | `MainWindowController` drops `IControl` and `ID`. |
| D12 | The rules of this plan go into `.claude/CLAUDE.md` as the section "Folders and names of the Studio". The text is in step 5.1. |

## 3. Rules for every step

Copy this section into every subagent prompt.

- Read the step and the code it touches first. When you find a problem that can change other steps or the shared design, stop and ask. Do not work around it.
- Write comments, documentation and commit messages in ASD-STE100. Never use an em dash or an en dash. Comments are one line.
- One atomic commit per step. The build and the tests pass at every commit. Message in third person present tense, one line, for example `Moves the notices to the Notices folder`.
- NEVER add a Co-Authored-By trailer or any line that names Claude or Anthropic.
- Do not commit or push unless the main agent asks. Do not amend or squash.
- A move is `git mv`. The namespace follows the new folder. Update every `using` and every `xmlns` in `.cs`, `.axaml` and the tests.
- A rename replaces whole words only: `git grep -nw OldName` first, then `sed -E 's/\bOldName\b/NewName/g'`, then `git grep -nw OldName` again must find nothing. The tests that carry the old name get the new name. A file has the name of its type.
- Do not take or read screenshots.
- Per step: build once, then run the Studio tests that name the moved or renamed types. The Studio suite is `dotnet test CMiX.Studio.Avalonia.Tests`.
- When Visual Studio locks the Studio bin, build the tests with `-p:OutDir=<other folder>`.

## 4. Phases and steps

Complexity: 1 goes to a Sonnet subagent, 2 to 4 to an Opus subagent, 5 to the main agent. Each step is one commit.

### Phase 1: folders

| Step | Work | Complexity |
|---|---|---|
| 1.1 | Make `Notices/`. Move IMessageNotice, MessageNotice, IMoveBlockedNotice, MoveBlockedNotice, IUnsavedChangesPrompt, UnsavedChangesPrompt, INotificationService, NotificationService, MainWindowDialogManager, EmptyViewLocator. Namespace `CMiX.Studio.Avalonia.Notices`. Drop the `Services.` qualifier in `App.axaml.cs` where the namespace change allows it. Fix the `xmlns` in `Views/MainWindow.axaml`. | 2 |
| 1.2 | Make `Settings/`. Move StudioSettings, StudioSettingsFile, JsonSettingsFile, InterfaceOptions, ShortcutMap. Namespace `CMiX.Studio.Avalonia.Settings`. Fix the `xmlns` in `Views/TabSettings.axaml`. | 2 |
| 1.3 | Make `Session/`. Move ProjectSession, UnsavedChanges, PathDropService. Namespace `CMiX.Studio.Avalonia.Session`. | 2 |
| 1.4 | Move ManagerReorderService to `ViewModels/` and PanelFocusCycle to `Views/`. Fix the `xmlns` in the six XAML files that bind ManagerReorderService and in `Views/MainWindow.axaml`. Remove the empty `Services` folder. `git grep -n "Avalonia.Services"` must find nothing. | 2 |

### Phase 2: one type per file

| Step | Work | Complexity |
|---|---|---|
| 2.1 | Move RedisSettings and InterfaceSettings out of `Settings/StudioSettings.cs` into `Settings/RedisSettings.cs` and `Settings/InterfaceSettings.cs`. The example JSON comment stays with StudioSettings. | 1 |
| 2.2 | Move ShortcutFile out of `Settings/ShortcutMap.cs` into `Settings/ShortcutFile.cs`. | 1 |
| 2.3 | Move UnsavedChangesChoice out of `Notices/IUnsavedChangesPrompt.cs` into `Notices/UnsavedChangesChoice.cs`. | 1 |

### Phase 3: names

| Step | Work | Complexity |
|---|---|---|
| 3.1 | Rename PathDropService to PathDropRules. The test class `PathDropServiceTests` becomes `PathDropRulesTests`. Check `Behaviors/PrefabFileDropBehavior.cs`, `ViewModels/MainViewModel.cs`, `ViewModels/Tree/ProjectTree.cs`, `ViewModels/Tree/TreeFileDropRules.cs`, `Tests/Support/TreeFixtures.cs`. | 2 |
| 3.2 | Rename NotificationService to StatusNotice and INotificationService to IStatusNotice. `Views/MainWindow.axaml` and `Views/MainWindow.axaml.cs` name them. The comment of IStatusNotice keeps "Shows a short message at the bottom of the main window". | 2 |
| 3.3 | Rename InterfaceOptions to InterfaceState. `ViewModels/MainViewModel.cs`, `ViewModels/Tree/ProjectTree.cs`, `Views/TabSettings.axaml`, `Tests/TabSettingsTests.cs`, `Tests/TreeKeyHintStripTests.cs`, `Tests/Support/TreeFixtures.cs`. | 2 |

### Phase 4: cleanups

| Step | Work | Complexity |
|---|---|---|
| 4.1 | Add `Notices/MainWindowOwner.cs`: a static class with `public static Window? Find()` that returns the main window of the desktop lifetime or null. MessageNotice, MoveBlockedNotice, UnsavedChangesPrompt and `ViewModels/MainWindowController.CloseMainWindow` use it. Behavior does not change. | 2 |
| 4.2 | StudioSettingsFile becomes the one door. Remove `StudioSettings.Load()`, `Load(path)`, `Save()` and `Save(path)`. StudioSettingsFile calls JsonSettingsFile itself. InterfaceState takes a `StudioSettingsFile?` in place of a path, and `Load` and the save on change go through it. `App.axaml.cs` and `TestServiceProviderFactory` build InterfaceState from the registered StudioSettingsFile. `Tests/StudioSettingsTests.cs` and `Tests/StudioSettingsFileTests.cs` move to the file class where they used the statics. | 3 |
| 4.3 | Add `DependencyInjection/ServiceCollectionExtensions.cs` with `AddCMiXStudio(this IServiceCollection services, string settingsPath, string shortcutsPath)`. It holds every registration of `App.axaml.cs:66-101` except `AddLogging` and `AddCMiXCore`. The ShortcutMap and InterfaceState factories load from the given paths. `App.axaml.cs` calls `AddCMiXStudio(StudioSettings.DefaultPath, ShortcutMap.DefaultPath)`. `TestServiceProviderFactory` calls it with its temp paths, then registers the four fakes and `FakeCredentialStore` after it. The test factory comment "Mirrors the registrations" goes away. | 3 |
| 4.4 | `MainWindowController` drops `IControl` and `ID`. Read `git grep -nw MainWindowController` first. When a view or a test reads `ID`, stop and ask. | 1 |
| 4.5 | `InterfaceState` gets two constructors in place of the optional parameter: `InterfaceState()` reads and saves nothing, `InterfaceState(StudioSettingsFile file)` reads and saves the file. An optional parameter that changes what a type does is against the rules of the owner. Found in step 4.2. | 1 |

### Phase 5: rules and documentation

| Step | Work | Complexity |
|---|---|---|
| 5.1 | Add the section below to `.claude/CLAUDE.md` after "Code patterns". Add one paragraph to `docs/DEVELOPMENT.md` that lists the folders of the Studio with one line each. In `ARCHITECTURE.md` the row "Service" of the Studio table (line 88) and the row of `ManagerReorderService` (line 209) get the new folders and names. In `docs/plans/keyboard-tree.md` line 132 the path `Services/PanelFocusCycle.cs` becomes `Views/PanelFocusCycle.cs`. | 1 |
| 5.2 | Full Core suite twice with details captured, Studio suite once. Check every commit of the plan: message form, no trailer, only the files of the step, no em or en dash, comments in STE. | 5 |

Text of step 5.1 for `.claude/CLAUDE.md`:

```
## Folders and names of the Studio

- A folder of CMiX.Studio.Avalonia has the name of the job of its types, not of their
  technical kind. There is no Services, Helpers or Utils folder. `Notices` tells the user
  something. `Settings` reads and writes the files of the user. `Session` holds the running
  project. `ViewModels` holds the classes that a view binds. `Views` holds the controls and
  the code that reads the visual tree. `DependencyInjection` holds `AddCMiXStudio`.
- A new type goes into the folder of its job. When no folder fits, make a folder with the
  name of the job. Do not put it into a folder because it is "a service".
- One type per file. A file has the name of its type. A private nested type is allowed.
- A class name says what the class does. Service, Helper and Util are not names. Manager
  stays the word of the Core managers. A type that tells the user something is a `...Notice`.
  A type that asks the user a question is a `...Prompt`. A type that holds the rules of one
  action is `...Rules`. A live observable copy of a setting is `...State`. The shape of a
  file is `...Settings` or `...File`.
- A type gets an interface only when a test replaces it or when it hides Avalonia or the
  platform. Else inject the concrete class.
- Every registration of the Studio is in `AddCMiXStudio`. `App.axaml.cs` and the tests call
  it. A test registers its fakes after the call.
- A static class is allowed when it holds pure functions and no state, for example
  `MainWindowOwner.Find()` and `JsonSettingsFile`.
```

## 5. Progress

- Phase 1 done on 2026-11-24: a07f05d18, a0402efd2, f311daa1e, 49973ad2f. Studio suite 1690 passed at the last commit.
- Phase 2 done on 2026-11-24: 5edaa2349, 28b51ee45, 6669a1ff6.
- Phase 3 done on 2026-11-24: bc6c99edb, 66e6bc5dc, 61f6908a8.
- Phase 4 done on 2026-11-24: ba70b56cd, 4ae4c8eab, ba064a712, 99a93d2c6, c25c0f33e. Studio suite 1691 passed at ba064a712.
- Phase 5 done on 2026-11-24: 31a4f6d4a, b87bdc9ea (step 5.1 took two commits: the first edited the wrong Service row of ARCHITECTURE.md). Step 5.2: Core suite 1332 passed and 2 skipped (sample projects, no variable) twice; the first run had one failure of BlendModeShaderTests because the test binary was outside the repository and the shader lookup walks up from it. Studio suite 1691 passed. All 17 commits checked.
- Another session commits test window changes on the same branch. A step reads the test files again before it edits them.

## 6. Order and checks

- Execute phase by phase. Phase 1 before Phase 2, because the splits work on the moved files. Phase 3 before Phase 4, because 4.2 and 4.3 name InterfaceState and StatusNotice.
- After Phase 1 and after Phase 3: `git grep -n "Avalonia.Services"`, `git grep -nw PathDropService`, `git grep -nw NotificationService`, `git grep -nw InterfaceOptions` must find nothing.
- The UI kit has no entry for these types. No kit entry gets out of date.
- The tests to run per phase: Phase 1 and 2 the whole Studio suite (a namespace touches every test). Phase 3 the tests that carry the renamed names. Phase 4 `StudioSettingsFileTests`, `StudioSettingsTests`, `MainWindowClosingTests`, `DialogMessageTests`, `MoveBlockedTests`, `UnsavedChangesPromptTests`, `SettingsSyncViewModelTests`, `TabSettingsTests`.
