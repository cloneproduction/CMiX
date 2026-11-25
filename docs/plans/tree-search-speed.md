# Plan: a fast search in the project tree

Status: plan only. Nothing is built. The owner says go before step 1.1.

Goal: a key in the search field of the tree shows its result in one frame on a big project. The rule of the match is one rule for every search field of CMiX.

Branch: `refactor/tree-structure`. Other sessions commit in this checkout. Every commit stages by path (`git add <files>`, never `git add -A`). Before each step, `git status --short` must show no change in the files of the step. If it shows one, the step stops and asks.

## 1. Findings

The analysis read the code at `0651c89ae` on 2026-11-25. The numbers come from a Release build, headless, a project with 6,441 rows (10 compositions, 40 layers each, `TreeFixtures.AddKinds` on each layer). The timing tests were temporary and are not in the repository. A headless window draws nothing, so the Studio is slower than these numbers.

| Key | Matching only | Matching and view | New row controls |
|---|---|---|---|
| `b` (the matches change) | 1.3 ms | 294 ms | 35 |
| `blu` (same matches) | 0.7 ms | 3 to 10 ms | 0 |
| `t` (4,021 matches) | 2.3 ms | 273 ms | 35 |
| `tr`, then an empty text | 1.5 ms | 172 ms, then 86 ms | some |

1. The matching is not slow. `TreeSearch.Refresh` walks 6,441 rows with `string.Contains` in under 2 ms. A full text index such as LIFTI saves less than 2 ms. LIFTI stays a later choice for a fuzzy search (section 5).
2. The row controls are slow. `ListTreeRowControls.ShowRows` keys a control by the key of its row. When the matches change, other rows come into the row window: it removes the old controls and builds new `RowListTree` controls. 33 new rows cost 235 to 258 ms. The same 33 rows with a new `DataContext` cost 24 to 41 ms.
3. The rebuild of the rows is cheap: `ListTreeArrangement.Of` 2 to 11 ms, `ListTreeNodeWatch.Watch` 1 to 20 ms, for 2,021 to 3,621 shown rows.
4. The close of the rows that the search opened runs in the key press: `TreeSearch.SyncRows` calls `CloseRowsInView`, and `ListTreeScrollAnchor.KeepStill` runs `ApplyTreeChanges` and `Scroller.UpdateLayout`. This costs 40 to 136 ms. Part of it is finding 2.
5. Every key runs the whole path. The binding of `TextBoxSearchTree` has no delay.
6. Four search fields have four copies of the rule, with two comparisons:
   - `ViewModels/Tree/TreeNode.cs:212` `MatchesSearch`: `OrdinalIgnoreCase`, on the header and the kind label.
   - `CMiX.UI/Controls/Collection/ListGroupedSearch.axaml.cs:153`: `CurrentCultureIgnoreCase`.
   - `CMiX.UI/Controls/Collection/ListSearchableItemPicker.axaml.cs:111`: `CurrentCultureIgnoreCase`.
   - `CMiX.UI/Controls/Parts/ComboBoxSearch.cs:52`: `CurrentCultureIgnoreCase`.

## 2. Work of other sessions

- The session "Avalonia tree control with declarative dragging" holds no edit and plans none in the files of this plan (its answer on 2026-11-25). It asks to keep the row window (`ListTreeRowWindow`) and the render slide (`ListTreeRowSlide`). This plan keeps both.
- Branch `tests/leaks` (worktree `.claude/worktrees/memprobe`) adds `WindowLeakTests` and changes `Tests/Support/TestWindows.cs`. Step 1.1 keeps no control after `ShowRows` returns, so a closed window keeps no row alive.
- `docs/plans/keyboard-tree.md` names `TreeSearchBox` and `ProjectTreeView`. These files are gone. Its search steps are built. No conflict.

## 3. Decisions

| # | Decision | Default |
|---|---|---|
| D1 | The rule of the match is `OrdinalIgnoreCase` in every field. The three CMiX.UI fields change from `CurrentCultureIgnoreCase`. | Yes |
| D2 | The rule lives in CMiX.UI, folder `Search`, type `SearchRules`, a static class of pure functions. CMiX.UI references no other project, and the Studio references CMiX.UI. | Yes |
| D3 | The delay of the search field is 120 ms, a constant `Motion.SearchDelay` next to `Motion.FlashDuration`. | Yes |
| D4 | No `SearchIndex` class now. The scan costs under 2 ms. An index comes only with a fuzzy search (section 5). | Yes |

D4 changes the answer of the first audit: that answer proposed `SearchIndex<TKey>`. The numbers show that a pure function is enough.

How others do it: Blender filters the outliner with a plain substring match on the name, with no index. VS Code quick open uses a fuzzy scorer on the fly, with no index, up to some thousands of items.

## 4. Phases and steps

Each step is one commit. Each step runs the tests of the types it changes (tier 1). Each phase ends with the suite of each changed project (tier 2). The end of the plan runs the three suites once (tier 3).

### Phase 1. The speed of the tree

#### Step 1.1 Reuse the row controls (complexity 4, Opus)

Files: `Views/ListTree/ListTreeRowControls.cs`, new test `CMiX.Studio.Avalonia.Tests/ListTreeRowControlsTests.cs`.

In one `ShowRows`, a control whose row leaves the window takes a row that comes into the window. A control that is left over at the end goes, as today. The pool lives only inside `ShowRows`, so a closed window keeps no control. A control with the focus stays with its row (`IsKept`), so a rename is never reused.

```csharp
// Before
ShowRow(control ?? AddRow(row.Key, before.PlaceOf(row.Key) ?? place), row, place);
...
foreach (var key in rowsByKey.Keys.Where(key => !kept.Contains(key)).ToArray())
    RemoveRow(key);

// After
var free = new Stack<RowListTree>(TakeLeavingRows(rows, shown));
...
ShowRow(control ?? ReuseRow(free, row.Key, before.PlaceOf(row.Key) ?? place) ?? AddRow(row.Key, before.PlaceOf(row.Key) ?? place), row, place);
...
foreach (var unused in free)
    RemoveRow(unused);
```

A reused row takes its start top like a new row (`SetTop`), so `ListTreeRowSlide.From` slides it as today. It loses the flash class (`FlashClass`). `ShowRow` sets `DataContext`, `IconKey`, `ItemCount`, `Place` and the light, as today.

Tests: `ListTreeRowControlsTests`:
- `ShowRows_MatchesThatChange_ReuseTheRowControls`: a search that changes the matches in the window adds no `RowListTree` to the panel.
- `ShowRows_AReusedRow_ShowsItsNewNode`: after the search, each realized row has the `DataContext` of the row at its place.
- `ShowRows_AFocusedRow_IsNotReused`.

Also run: `PanelListTreeRowsTests`, `ListTreeLayoutTests`, `ListTreeRenameTests`, `ListTreeHiddenLookTests`, `ListTreeHoverAndSelectionTests`, `ProjectTreeSearchTests`, `ListTreeDragTests`.

Commit: `Reuses the row controls of the tree when other rows come into view`.

#### Step 1.1b Keep the spare row controls (complexity 3, Opus)

Built 1.1 at `6852a251d`: an open tree searches in 3 to 7 ms. A search from a closed tree still takes 270 to 320 ms, because a closed tree has 1 control and the pool of 1.1 lives for one pass. The owner chose to keep the spare controls, with code that stays easy to read.

Pattern: the recycle pool of Avalonia `VirtualizingStackPanel` (`release/11.2.0`, `RecycleElement`, `GetRecycledElement`, `PushToRecyclePool`). The pool is a field of the panel. A spare control stays a child of the panel with `IsVisible = false`. The focused control is not pooled. The pool has no size limit and goes with the panel.

Files: new `Views/ListTree/ListTreeSpareRows.cs`, `Views/ListTree/ListTreeRowControls.cs`, `CMiX.Studio.Avalonia.Tests/ListTreeRowControlsTests.cs`.

`ListTreeSpareRows` holds the hidden controls. `ListTreeRowControls` asks it for a control and gives it the controls that leave. The pool takes only controls that were shown, so it never holds more than the most rows the window showed at one time. No size limit is needed.

```csharp
// Before, in ListTreeRowControls.ShowRows
var free = new Stack<RowListTree>(TakeLeavingRows(shown));
...
ShowRow(control ?? ReuseRow(free, row.Key, start) ?? AddRow(row.Key, start), row, place);
...
foreach (var unused in free)
    RemoveRow(unused);

// After
spareRows.KeepAll(TakeLeavingRows(shown));
...
ShowRow(control ?? TakeRow(row.Key, start), row, place);

// TakeRow: a spare control, else a new control
private RowListTree TakeRow(ListTreeRowKey key, ListTreePlace start) => spareRows.Take() ?? AddRow(start);
```

```csharp
// New: Views/ListTree/ListTreeSpareRows.cs
// The row controls that no row shows now. They stay hidden in the panel until a row comes into the window.
internal sealed class ListTreeSpareRows
{
    private readonly Stack<RowListTree> rows = new();

    public void KeepAll(IEnumerable<RowListTree> leaving) { ... }  // IsVisible false, DataContext null, Snap, StopFlash
    public RowListTree? Take() { ... }                           // IsVisible true
}
```

`RealizeOutsideWindow` takes a spare control too. Every part that reads `panel.Children.OfType<RowListTree>()` skips a hidden control: `ListTreeSurfaces.ShowMarks`, the hit tests, the focus code, and the test helpers. The subagent searches them all first. If a part cannot skip a hidden control in a simple way, the step stops and asks.

Tests: `ListTreeRowControlsTests`:
- `ShowRows_ASearchFromAClosedTree_ReusesTheSpareRows`: close all, search, empty the text, search again. The second search adds no `RowListTree` to the panel.
- `ShowRows_ASpareRow_IsHiddenAndHasNoNode`.
- `ShowRows_TheSpareRows_AreNeverMoreThanTheRowsOfTheWindow`.
- The test of 1.1 that allows a few new controls now asserts zero new controls.

Also run the tests of step 1.1.

Commit: `Keeps the spare row controls of the tree hidden until a row needs one`.

#### Step 1.2 Wait for a pause in typing (complexity 1, Sonnet)

Dropped on 2026-11-25. The `Binding` of Avalonia 11.2.1 has no `Delay` (error AVLN2000). A pause in code needs a flush on Enter and a stop on Escape and on the clear button, and the owner wants code that stays easy to read. After 1.1b a key that keeps the matches costs 3 to 9 ms. The text below is the old step.

Files: `CMiX.UI/Themes/Motion.cs` (the class that holds `FlashDuration`), `Views/Tree/TextBoxSearchTree.axaml`, `CMiX.Studio.Avalonia.Tests/TextBoxSearchTreeTests.cs`.

```csharp
// After, in Motion
public const int SearchDelayMilliseconds = 120;
```
```xml
<!-- Before -->
Text="{Binding SearchText, Mode=TwoWay}"
<!-- After -->
Text="{Binding SearchText, Mode=TwoWay, Delay={x:Static ui:Motion.SearchDelayMilliseconds}}"
```

The subagent first checks that `Delay` of Avalonia 11.2.1 also delays a clear (the clear button and Escape set the text from the model, so they do not wait). If a test of the search sets the text in the field and reads the matches at once, the test waits for the delay. If the delay breaks Escape or the clear button, the step stops and asks.

Tests: `TextBoxSearchTreeTests`, `ProjectTreeSearchExitTests`, `ProjectTreeSearchOpenTests`.

Commit: `Waits for a pause in typing before the tree searches`.

#### Step 1.3 Measure the close, then fix it if needed (complexity 3, Opus)

Files: `ViewModels/Tree/TreeSearch.cs` (`SyncRows`), `Behaviors/ListTree/ListTreeScrollAnchor.cs` (`KeepStill`), only if the fix is needed.

The subagent measures again with a temporary timing test after steps 1.1 and 1.2. When the close of the rows by the search costs under 16 ms, the step makes no commit and reports the number. Else the close of the search rows is posted to the dispatcher, so the key press does not wait for the layout:

```csharp
// Before, in TreeSearch.SyncRows
rowCloser.CloseRowsInView(selected, () => { ... });
// After
rowCloser.CloseRowsInViewLater(selected, () => { ... });
```

`CloseRowsInViewLater` is a new member of `IRowCloser` and `TreeViewSet`. `TreeOpenState` keeps `CloseRowsInView`: a close by a key of the user stays at once. A new member name is a question for the owner before the step.

Tests: `ListTreeScreenRequestsTests`, `ProjectTreeSearchTests`, `ProjectTreeSearchOpenTests`, `ProjectTreeCursorCloseTests`.

Commit: `Closes the rows of the search after the key press`.

Phase 1 end: the Studio suite once.

### Phase 2. One rule for every search field

#### Step 2.1 Add SearchRules (complexity 2, Opus)

Files: new `CMiX.UI/Search/SearchRules.cs`, new `CMiX.UI.Tests/SearchRulesTests.cs`.

```csharp
namespace CMiX.UI.Search
{
    // The rule of every search field: the text is in one of the texts, with no regard to case.
    public static class SearchRules
    {
        public static bool Matches(string query, string? text) => ...;

        public static bool MatchesAny(string query, string? first, string? second) => ...;
    }
}
```

An empty query matches every text. A null text matches no query but the empty query. The query is trimmed by the caller, as today.

Tests: `Matches_AnEmptyQuery_MatchesEveryText`, `Matches_ADifferentCase_Matches`, `Matches_ANullText_MatchesOnlyTheEmptyQuery`, `MatchesAny_TheSecondText_Matches`.

Commit: `Adds SearchRules, the one rule of the search fields`.

#### Step 2.2 The tree uses SearchRules (complexity 1, Sonnet)

Files: `ViewModels/Tree/TreeNode.cs`.

```csharp
// Before
private bool MatchesSearch(string text) =>
    Header.Contains(text, StringComparison.OrdinalIgnoreCase)
    || (kindLabel != null && kindLabel.Contains(text, StringComparison.OrdinalIgnoreCase));
// After
private bool MatchesSearch(string text) => SearchRules.MatchesAny(text, Header, kindLabel);
```

Tests: `ProjectTreeSearchTests`.

Commit: `Uses SearchRules in the search of the tree`.

#### Step 2.3 The three CMiX.UI fields use SearchRules (complexity 1, Sonnet)

Files: `CMiX.UI/Controls/Collection/ListGroupedSearch.axaml.cs`, `CMiX.UI/Controls/Collection/ListSearchableItemPicker.axaml.cs`, `CMiX.UI/Controls/Parts/ComboBoxSearch.cs`.

```csharp
// Before
item.Text.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase)
// After
SearchRules.Matches(SearchText, item.Text)
```

Tests: `ListGroupedSearchTests`, `ListSearchableItemPickerTests`, `ComboBoxSearchTests`.

Commit: `Uses SearchRules in the search fields of CMiX.UI`.

#### Step 2.4 Documentation (complexity 1, Sonnet)

Files: `docs/DEVELOPMENT.md`, section "The search of the tree": the row controls are reused, the delay, and the rule is `SearchRules`. The status line of this plan.

Commit: `Describes the reused rows and the rule of the search`.

Phase 2 end: the CMiX.UI suite and the Studio suite once. End of plan: the three suites once.

## 5. Later: a fuzzy search with LIFTI

Only when the owner wants a search that accepts a typing error or finds by field (`kind=Blur`). LIFTI 7.0.2, MIT, net8.0 to net10.0, in memory. Each change makes a new snapshot, so the changes go in one batch. Its add is async and its search is sync. It matches whole words: `lur` does not find `Blur` without `*lur*`.

Shape: a class `SearchIndex<TKey>` in `CMiX.UI/Search`, with `Set(key, texts)`, `Remove(key)` and `Find(query)`. It wraps LIFTI. The tree gives it the header and the kind of each row on add, remove and rename. The UI never waits for it: `Set` and `Remove` run in one batch after a change of the list. `SearchRules` stays for the small lists.

## 6. Checks in the Studio after phase 1

- Type a name on a big project in a Release build: the rows follow the keys with no stop.
- The rows slide as before when the search opens and closes the parents.
- A rename that is open while you type in the search keeps its text.
- Escape and the clear button empty the search at once.
- The selected row and its parents stay open after the search ends.

UI kit: no entry changes. `TextBoxSearchTree` keeps its look.
