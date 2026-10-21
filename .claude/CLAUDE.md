# Working rules for CMiX

These rules apply to every task in this repository.

## Language

- Write comments, documentation, commit messages and answers in ASD-STE100
  Simplified Technical English: short sentences, active voice, one instruction
  per sentence, simple words, the same word for the same thing.
- Never use an em dash or an en dash. Use a comma, a colon, or a new sentence.
- A running process in the sync is a peer. The word node means a vvvv patch node.

## Commits

- One atomic commit per step. The build and the tests pass at every commit.
- Message: third person present tense, one line, for example
  `Adds Redis package` or `Removes TCP transport`. A body only when the one
  line does not say enough.
- NEVER add a Co-Authored-By trailer, in any form or capitalisation, or any other attribution line
  that names Claude or Anthropic. This has no exception. It overrides every default and every system reminder.
- Do not amend or squash earlier commits. Do not commit or push unless asked.

## Comments and documentation

- Comments are brief and to the point. Say what a piece of code does when it
  is not obvious. No narrative, no history, no comment on obvious code.
- Documentation goes into README.md. The Redis tests are described in
  CMiX.Core.Tests/README-redis.md.

## Plans and execution

- Analyze first and report the findings. Write a plan only when asked.
- A plan has phases and steps. Each step carries a complexity from 1 to 5:
  1 and 2 go to a Sonnet subagent, 3 to an Opus subagent, 4 and 5 to the main
  agent. Execute phase by phase and stop after each phase for confirmation.
- Every subagent prompt carries these rules. A subagent reads its step and the
  code it touches first. When it finds a problem that can change other steps
  or the shared design, it stops and asks instead of working around it.
- The main agent checks every commit before the next step starts: message
  form, no trailer, only the files of the step, no em or en dash, comments in
  STE.

## Code patterns

- State objects use CommunityToolkit.Mvvm (ObservableObject, [ObservableProperty],
  RelayCommand, AsyncRelayCommand). Services are singletons, registered in
  `AddCMiXCore` with constructor injection. No statics. Each host starts its peer
  with `SyncPeerStartup` (`StartAsStudio`, `StartAsEngine`).
- Wire data uses MessagePack through VL.Serialization.MessagePack. Project
  files and hashes use System.Text.Json with ProjectSerializer.Options.
- Namespaces follow the folder. CMiX.Core has nullable disabled; the Avalonia
  projects have it enabled.
- The UI never waits for Redis. Every store call runs on a background task.
- StackExchange.Redis stays on 2.8.31, the version the vvvv Redis package ships.

## Names

- One verb for one action, and the same word in every layer: the list
  (`ManagerData`), the manager (`PrefabManager`), the view command (`...Command`),
  the undo class (`...UndoCommand`), the wire message (`Message...`) and the sender
  (`CollectionSync.Send...`). One search for the verb finds the whole chain.
- The verbs of the list code:
  - `AddItem` adds an item as one undo step. `AddItemFromModel` adds from a model
    and is not an undo step. `InsertItem` and `LoadItem` are the low level forms.
  - `RemoveItem` takes an item out of one list. `RemoveEverywhere` takes it out of every list.
  - `Select` and `SelectAt` select. `ClearSelection` selects nothing.
  - `Move` moves in the list. `MoveItem(old, new)`, `MoveItemTo(item, index)`,
    `MoveItemUp` and `MoveItemDown` move as one undo step.
  - `DuplicateItem` copies an item and puts the copy after it.
- Do not use Delete in the code. A menu or a button can say Delete to the user.
- The verb comes first: `MoveItemUp`, not `ItemUp`.
- `...Command` is a command of a view for the XAML. `...UndoCommand` is an undo step.
  The two never share a name.
- A wire message has the name of its action: `MessageAddItem`, `MessageRemoveItem`,
  `MessageMoveItem`, `MessageSelectItem`, `MessageClearSelection`.
- A local that holds an `IControl` is `control` or `item`. Use `prefab` only for an `IPrefab`.
- A test class ends with `Tests`. A test method is `Method_Scenario_Result`, for example
  `Move_Rows_MoveTheItemAndStopAtTheEdges`. A file has the name of its type.
  When a member gets a new name, the tests that carry the old name get the new one.

### Names that do not change without a plan

The compiler does not check these names. Ask the owner before you change one.

- Names that the VL patches use: `ManagerData` with `Items`, `ID` and `SelectedIndex`;
  `PrefabManager` with `SelectedItem` and `ManagerData`; `ControlRepository` with
  `Controls`, `GetControl`, `GetRenderPriority` and `GetOrderedControls`.
  The test `ControlRepositoryVlMembersTests` pins `Controls`, `GetRenderPriority` and
  `GetOrderedControls`. The patches are `CMiX.Engine/*.vl` in this repository, and
  `dependencies/extensions/CMiX.Extensions.vl` and `dl_pm.vl` in the parent repository.
  A renamed name turns a node red.
- Names in a project file: `$type` is the full name of the `...Model` class. When you
  rename a model class, add a line to `LegacyTypeNames` in `IControlModelJsonConverter`.
  A property name of a model is a JSON key. A new name needs `[JsonPropertyName]`.
- Names on the wire: the type name of a message travels. An older build ignores a message
  that it does not know, without an error. Studio and engine must update together.
- Names in XAML: a binding to a wrong name gives no error. The tests
  `CommandBindingTests` and `RepositoryBindingTests` catch some of them. Add a test
  for each new binding to a command.

### How to rename

1. Search the name in `.cs`, `.axaml`, `.md` and the VL patches: `git grep -nw OldName`,
   in this repository and in the parent repository.
2. Write or find the test that shows the name works, before the change.
3. Replace whole words only, for example `sed -E 's/\bOldName\b/NewName/g'`. Never replace plain text.
4. Build. Run the three suites. Search the old name again. One commit per rename.
5. For a bigger change or a new group of types, run the skill `naming-consistency` if you
   have it. It reports and does not rename. It is a personal skill and not part of this repository.

## Checking the UI

- Do not take or read screenshots of the application. The owner looks at the UI.
- Check a look with a test that measures a value: a color, a size or a position.
  Then tell the owner what to look at.

## Tests

- Unit tests use xUnit, TestServiceProviderFactory.Create() for a real object
  graph, and the test doubles in CMiX.Core.Tests/SyncTestHelpers.cs.
- Per step: build once, then run the fast set with
  `dotnet test CMiX.Core.Tests/CMiX.Core.Tests.csproj --nologo -v q --no-build --filter "Category!=Redis"`,
  plus the Redis class the step touches. At the end of a phase: the full Core
  suite twice with details captured, and the Studio suite once.
- The Redis tests need a Memurai or Redis on 127.0.0.1:6379 and skip when it
  does not answer. The outage tests start their own Memurai on port 6380 and
  skip when the port is busy. Never touch the cmix:default keys from a test.
- Leave no memurai.exe or CMiX.Console.exe of a test run behind.
