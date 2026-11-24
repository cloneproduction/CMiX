# Simplify the new list tree: maintainability and clean architecture

Paste everything below the line into a new session in D:\GitHub\CMiX_UI.

---

You review and simplify the new tree control of CMiX, `ListTreeProject`, on the branch `refactor/tree-structure`. The goal is code that a programmer new to it can read, change and test with confidence. The behaviour must not change.

## Read first
1. `.claude/CLAUDE.md`. Follow every rule in it: ASD-STE100 Simplified Technical English, no em dash or en dash, the commit rules (one line, third person present tense, NEVER a Co-Authored-By or any line that names Claude or Anthropic), the naming rules ("One verb for one action", "The order of the words", "Names that do not change without a plan", "How to rename"), the code patterns and the test rules.
2. `docs/tree-port-plan.md`, section 5.2 "Rules of the code" and section 7 (owner decisions).
3. `CMiX.Studio.Avalonia/ViewModels/Tree/README.md` and the ListTree README, if one exists.

## Scope
- `CMiX.Studio.Avalonia/ViewModels/ListTree`: the pure core, 48 files, about 2100 lines.
- `CMiX.Studio.Avalonia/Views/ListTree`: 19 files, about 1300 lines.
- `CMiX.Studio.Avalonia/Behaviors/ListTree`: 25 files, about 2200 lines.
- The 45 `ListTree*` test files in `CMiX.Studio.Avalonia.Tests`.
- The members that the new tree added to shared types: `ProjectTree` (DropService, EnsureCursor, DropBlockAsync, SelectedNodes and the selection members), `TreeDropService` (PlaceOf, RefusalOf, SharingOf), `TreeNode.ToggleExpand`, `TreeDrop.MoveBlock`.

Out of scope:
- The old tree (`TreeViewProject`, `Views/Tree`, `TreeView*`). It stays as it is, so the owner can compare the two trees. Do not merge new parts into old parts, and do not change old parts.
- The UI kit (`docs/ui-kit`) and the mockups.
- The look. Do not change a colour, a size or a timing.

## What to look for, in this order
1. **Architecture:**
   - Is each layer clean? The pure core has no Avalonia types. The views only show. The behaviors hold the input and the screen logic.
   - Do dependencies go one way?
   - Does a behavior reach into another behavior through `Part<T>()` where an event or a small interface would be clearer?
   - Find cycles between parts.
2. **Too many small types:** there are about 90 files. Find types that exist only to pass a value through, records with one use, and steps that split one idea over several files. Propose merges where one file reads better than three, and do the opposite for a type that does several jobs.
3. **Known size problems:**
   - `PanelListTreeRows` has 255 lines, over the size limit.
   - `ListTreeContext` has 11 constructor parameters.
   - `ListTreeMoveCount` and `ListTreeCountedTree` have about 180 lines together.
   - `ListTreeGrab`, `ListTreeRowDrag` and `ListTreeDragSession` overlap in what they know about a drag.
4. **Duplication inside the new tree:**
   - The same rule written twice.
   - The pointer drag and the keyboard grab doing the same steps.
   - The four file drop paths.
   - The parts that copy old tree code: the owner keeps those copies, so only report them.
5. **Names:** check against the CLAUDE.md rules: the kind first, one verb per action, `...Command` against `...UndoCommand`, tests named `Method_Scenario_Result`. Run the `naming-consistency` skill if you have it.
6. **Comments:** one line, STE. Remove a comment that only repeats the code.
7. **Tests:**
   - Find tests that pin private details instead of behaviour.
   - Find duplicated setup that a shared fixture would remove.
   - Find slow tests.

Avoid the shapes that the owner rejected: version 56 of the mockup, "fewer rules, regions from the indent", was rejected. Keep every behaviour of section 3.5 of the plan and the owner decisions of section 7.

## How to work
1. **Analyze and report first. Change no code in this step.**
   - Give a short list of findings. Each finding has the files it touches, why it hurts maintainability, the proposed change with a short before and after sample, and a risk of low, medium or high.
   - Group the findings into a plan of phases and steps. Each step has a complexity from 1 to 5.
   - Stop, and wait for the owner to choose the steps.
2. **Then apply the steps the owner chose, one atomic commit per step.**
   - The build and the tests pass at every commit.
   - A rename follows "How to rename" in CLAUDE.md.
   - A step that would change the behaviour, a name that the VL patches use, or a JSON key stops, and you ask the owner.
3. **Build and test.**
   - Studio and Visual Studio may lock the outputs of the repository. If a build fails on a locked file, build and test in a scratch copy of the repository.
   - Run `dotnet test CMiX.Studio.Avalonia.Tests --filter "FullyQualifiedName~ListTree|FullyQualifiedName~ProjectTree|FullyQualifiedName~TreeView"`.
   - Do not run the full Studio suite: it has a known memory growth.
   - The known failure `PanelInspectorBudgetTests.SelectedNode_OnALargeComposition_BuildsOneViewAndStaysInTheTimeBudget` does not count.
   - Leave no testhost process behind.
4. **Speed must not get worse.** The row window, the kept card areas, the kept rows in the moved snapshot and the render slide are speed fixes, so keep them. Judge any lag in a Release build, not in Debug.
5. **Commits:**
   - Commit only in CMiX_UI. Do not push.
   - Never commit `.claude/launch.json`, `.claude/prompts`, or the owner's uncommitted UI kit files.
6. **Do not take screenshots of the application.** The owner looks at the UI.

## Final report
Keep it short:
- the commits with their hashes;
- the files and the line count before and after for each of the three folders;
- what you did not do, and why;
- any open question for the owner.
