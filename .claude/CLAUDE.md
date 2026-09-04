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
- Never add a Co-Authored-By trailer or any other attribution line.
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
  RelayCommand, AsyncRelayCommand). Services are singletons in
  InjectionBuilder.ConfigureAllServices with constructor injection. No statics.
- Wire data uses MessagePack through VL.Serialization.MessagePack. Project
  files and hashes use System.Text.Json with ProjectSerializer.Options.
- Namespaces follow the folder. CMiX.Core has nullable disabled; the Avalonia
  projects have it enabled.
- The UI never waits for Redis. Every store call runs on a background task.
- StackExchange.Redis stays on 2.8.31, the version the vvvv Redis package ships.

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
