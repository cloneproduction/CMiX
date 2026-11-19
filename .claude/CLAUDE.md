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

- Analyze first and report the findings.
- A plan has phases and steps. Each step carries a complexity from 1 to 5:
  1 goes to a Sonnet subagent, 2 and 3 to an Opus subagent, 4 to a Fable subagent.
  5 is a step needing the whole context and it goes to the main agent. 
  Execute phase by phase.
- Every subagent prompt carries these rules. A subagent reads its step and the
  code it touches first. When it finds a problem that can change other steps
  or the shared design, it stops and asks instead of working around it.
- The main agent checks every commit before the next step starts: message
  form, no trailer, only the files of the step, no em or en dash, comments in
  STE.

## Code patterns

- State objects use CommunityToolkit.Mvvm (ObservableObject, [ObservableProperty],
  RelayCommand, AsyncRelayCommand). Services are singletons, registered in
  `AddCMiXCore` with constructor injection. No static state. A static class is allowed when it holds pure functions and no state. Each host starts its peer
  with `SyncPeerStartup` (`StartAsStudio`, `StartAsEngine`).
- Wire data uses MessagePack through VL.Serialization.MessagePack. Project
  files and hashes use System.Text.Json with ProjectSerializer.Options.
- Namespaces follow the folder. The views of CMiX.Studio.Avalonia keep the one namespace
  CMiX.Studio.Avalonia.Views for the view locator (see docs/DEVELOPMENT.md).
  CMiX.Core has nullable disabled; the Avalonia
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
  `Controls` and `GetControl`.
  The test `ControlRepositoryVlMembersTests` pins `Controls`. The patches are `CMiX.Engine/*.vl` in this repository, and
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

## UI kit

- `docs/ui-kit/` is the HTML copy of the UI. The owner and Claude use it to discuss a design.
  `gallery.html` is the catalogue. Part A is CMiX.UI. Part B is CMiX.Studio.Avalonia.
- A change of a token, a style, a control or a view updates the UI kit in the same commit.
  This applies to `CMiX.UI/Themes`, `CMiX.UI/Controls`, `CMiX.Studio.Avalonia/Themes` and
  `CMiX.Studio.Avalonia/Views`.
- A change of a token: run `tools/Update-Tokens.ps1` again. A change of an icon key: run `tools/Update-Icons.ps1` again.
  Do not edit `tokens.css`, `tokens.js` or `icons.js` by hand.
- A class in `cmix.css` is the key of a ControlTheme or the name of a control class. A state is the name of the pseudo class
  of Avalonia: `pointerover`, `pressed`, `checked`, `selected`, `disabled`. The template of a control is in `cmix.js`.
- The demo of an entry is a `template` with the id `entry-<number>` in `gallery.html`. A row has a fixed letter in `data-sub`
  or `data-sample`: the theme `ButtonStraight` is `1.1.b`. Never change a letter and never use a letter again.
- An entry has the exact name of its class or its style key in CMiX. When a name changes
  in the code, the entry gets the new name.
- An entry has a fixed number. Never change a number and never use a number again.
  A new entry gets the next free number of its category. A removed entry keeps its number
  with the mark `removed`.
- A change of the look that needs a decision of the owner is a proposal in the sandbox: one object in `sandbox.js` and one block
  in `sandbox.css` under the class `sb-` and its number. It shows Before and After, and changes nothing in CMiX or in `cmix.css`.
  One proposal is one decision. The number of a proposal (`S1`) is for the discussion only. After the decision, remove the
  proposal. A yes goes into CMiX first, then into `cmix.css`.
- A design mockup is a file in `docs/ui-kit/mockups/`. It links the kit and does not copy it.
- When you cannot update the UI kit in the step, tell the owner which entries are out of date.

## Tests

- Unit tests use xUnit, TestServiceProviderFactory.Create() for a real object
  graph, and the test doubles in CMiX.Core.Tests/SyncTestHelpers.cs.
- Per Phase: build once, then run all tests that pertain to the code changes
  plus the Redis class the Phase touches. At the end of a plan: the full Core
  suite twice with details captured, and the Studio suite once.
- The Redis tests need a Memurai or Redis on 127.0.0.1:6379 and skip when it
  does not answer. The outage tests start their own Memurai on port 6380 and
  skip when the port is busy. Never touch the cmix:default keys from a test.
- Leave no memurai.exe or CMiX.Console.exe of a test run behind.
- Saved projects of users must keep opening. Before a change of a model, a JSON key or a fixed id, set
  `CMIX_SAMPLE_PROJECTS` to a folder with `.cmix` files and run `ProjectFileCompatibilityTests`.
  The test is skipped when the variable is not set. No user file is stored in the repository.
