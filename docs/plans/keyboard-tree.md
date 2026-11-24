# Plan: keyboard-only project tree

Goal: the owner uses the project tree with the keyboard only. The mouse actions of today stay and do not interfere with the keys.

The design comes from a team review: a game designer, an Avalonia engineer, a Blender specialist, a UI designer, an esports coach and a UI consistency lead. This file is the plan. Nothing in it is built yet.

## 1. Findings

The analysis read the code on branch `fix/drag-capture-loss`. Paths are relative to `CMiX.Studio.Avalonia` unless they start with `CMiX.UI`.

1. Two targets. E and Alt+E act on the soft selection. Rename, Remove, Duplicate, Move, W and Shift+A act on the selected row (`ProjectTree.SelectedPlace()`, `ViewModels/ProjectTree.cs:697`). After Down three times, Delete removes a row that the user does not look at.
2. E, W and Shift+A need the pointer over the tree (`IsPointerOverTree()`, `ProjectTree.cs:70, 79, 82`). A keyboard-only user cannot use them.
3. Three different gates decide when a tree key runs: pointer only (`ProjectTree.cs:70, 79, 82`), pointer or focus (`CanRunTreeKey`, `:170`) and search (`:85`).
4. No key moves the focus into the tree or between panels.
5. The row toggles are Tab stops (`Views/Tree/RowToggles.axaml:40`, `ComposableToggles.axaml`). Tab walks through every toggle of every row.
6. When both sides show the tree, the `Watch()` of each view overwrites the hooks of the model, and the `Unwatch()` of one view clears them for the other (`Views/Tree/ProjectTreeView.axaml.cs:324-330`).
7. The cursor and the selection have the same frame (`Themes/Styles/ProjectTree.axaml:133-142, 210-213`). The cursor only shows on `:focus-visible`, so it goes away when the tree loses the focus.
8. Hover uses the selection fill (`ProjectTree.axaml:203-206` and `214-217`). A row under a resting mouse looks selected. Drop inside also uses the selection fill (`:219-221`).
9. `ShortcutMap.Conflicts` groups by scope only (`Settings/ShortcutMap.cs:86`). A tree gesture on Ctrl+Z does not conflict with Edit.Undo.
10. The context menu has no `:selected` style (`CMiX.UI` `ContextMenu.axaml:101-107`). The arrow keys in the W menu show nothing.
11. The Studio has four focus looks (TextBox, TextBoxKeyGesture, tree, proposal) and three shortcut text looks (tooltip, menu, `MainMenu.axaml:55`). Opacities are numbers in place (0.5, 0.56, 0.6, 0.4, 0.8).
12. Tooltips stay open while the user presses keys (`Views/Tree/TreeTooltips.cs:27-29`).
13. The mouse drag (`Views/Tree/TreeRowDrag.cs`) needs a pressed button: `Begin` takes an `IPointer` (`:132`), `OnPointerMoved` returns when the left button is up (`ProjectTreeView.axaml.cs:541`), and `StartDrop` reads the drop place again at the pointer (`TreeRowDrag.cs:216`). The drop rules are in the model: `CanDrop`, `Locate`, `DropAsync`.
14. Each row keeps its open state in the model (`TreeNode.cs:162`). A parent that closes and opens again shows its children as before. The state is not saved in the project.
15. The search records the rows that it opened and closes only those (`ProjectTree.cs:417, 437`).
16. The ItemsPanel is a plain StackPanel (`ProjectTreeView.axaml:56-59`), so every row has a container. Virtualization is not a risk.

## 2. Decisions

### Decided by the owner

- G grabs as in Blender: the row follows the mouse at once, a left click places it, a right click or Escape cancels.
- No F keys.
- Every mouse action of today stays: click, chevron, toggles, double-click rename, drag with hover open, right-click menu, tooltips, wheel, edge scroll.
- Shift+A Add panel, E open or close, Alt+E collapse others, W menu, Ctrl+F search.
- Shift+Alt+E collapses every row (D9).
- No flash on a row after an action (D15).

### Defaults that the owner must confirm

The plan uses the team majority. Each line names the steps that change if the owner picks the other option.

| # | Question | Default | Other option | Steps |
|---|---|---|---|---|
| D1 | Keys act on the cursor, focus wins over hover, hover never moves the cursor | Yes | Keep hover or focus | 1.3 |
| D2 | W and Shift+A from a key open at the cursor row; right-click opens at the pointer | Yes | Always at the pointer | 2.1 |
| D3 | Toolbar buttons act on the cursor row | Yes | Selected row | 1.2 |
| D4 | Search Enter moves the cursor only, it does not select | Yes | Select the match | 5.6 |
| D5 | Rename key | Ctrl+R | R | 5.6 |
| D6 | Next and previous match | Enter, Shift+Enter in the field | Also N, Shift+N in the tree | 5.6 |
| D7 | Space confirms a grab | No | Yes | 6.3 |
| D8 | One level open and close, on the whole tree | Q, Shift+Q | = and minus | 5.3 |
| D9 | Collapse all | Decided: Shift+Alt+E | | 5.3 |
| D10 | Reveal the selected row | F | Period | 5.4 |
| D11 | Alt+H shows every row again | Now | Later | 5.1 |
| D12 | Hint strip | On, can be hidden, no auto hide | Grab only | 6.4 |
| D13 | Mouse distance that takes the drop mark back from the keys during G | 14 px | 8 px | 6.3 |
| D14 | Arrow keys in a menu | Hover fill | White cursor frame | 2.2 |
| D15 | Flash | Decided: no flash | | 3.1, 5.1, 5.4 |
| D16 | Disabled opacity | 0.5 | 0.56 | 3.1, 3.2 |
| D17 | Shortcut text grey | Decided: `ShortcutTextBrush` #85858C (Grey800 gave 4.43:1); on a hovered or keyboard row `ShortcutTextHoverBrush` (Grey900); the main menu dropdown is on the popup card | | 3.6, 3.8, 3.9 |
| D18 | Panel accent line while a popup of the panel is open | Stays on | Goes off | 3.5 |
| D19 | Badge during grab | No badge, panel frame and hint strip only | Accent badge | 6.3 |
| D20 | The code word for soft selection | Cursor, as a planned rename | Keep soft selection | 1.1 |

## 3. Rules for every step

Copy this section into every subagent prompt.

- Read the step and the code it touches first. When you find a problem that can change other steps or the shared design, stop and ask. Do not work around it.
- Write comments, documentation and commit messages in ASD-STE100. Never use an em dash or an en dash.
- One atomic commit per step. The build and the tests pass at every commit. Message in third person present tense, one line, for example `Adds the tree cursor target`.
- NEVER add a Co-Authored-By trailer or any line that names Claude or Anthropic.
- Do not commit or push unless the main agent asks. Do not amend or squash.
- Do not take or read screenshots. Check a look with a test that measures a color, a size or a position.
- A new key is a ShortcutMap name, a MainWindow binding, a binding test and a README line. Only navigation keys are hard coded, in the tunnel handler.
- The rules live in the model (`ProjectTree`). The view measures and places. No synthetic pointer events.
- No color or opacity number in a style file. Use a token. When a token is missing, add it to CMiX.UI first.
- Names: one verb for one action in every layer. A `...Command` is a view command, an `...UndoCommand` is an undo step. Tests are `Method_Scenario_Result`.
- When Visual Studio locks the Studio bin, build the tests with `-p:OutDir=<other folder>`.

## 4. Phases

Complexity: 1 Sonnet subagent, 2 and 3 Opus subagent, 4 Fable subagent, 5 main agent.

### Phase 1: One target and one gate

No visual change. This phase removes the live hazard of finding 1.

| Step | Work | Files | Tests | Cx |
|---|---|---|---|---|
| 1.1 | Rename the soft selection to cursor (`SoftSelectedNode` to `CursorNode`, the `MoveSoftSelection...` methods to `MoveCursor...`). Follow "How to rename". | `ProjectTree.cs`, `ProjectTreeView.axaml.cs`, tests | Existing tree key tests, renamed | 2 |
| 1.2 | Add `TargetNode` (cursor, else selection) and `TargetPlace()`. Rename, Remove, Duplicate, Move, W, Shift+A, E, Alt+E and the toolbar use it. Merge `ExpandTarget` and `CollapseTarget` into it. | `ProjectTree.cs`, `ProjectTreeView.axaml.cs`, `ProjectTreeToolbar` | `RemoveByKey_AfterDownTwice_RemovesTheCursorRow` and one test per command | 3 |
| 1.3 | Make one gate. `CanRunTreeKey` is true when the tree has the focus, or the pointer is over the tree and no other panel has the focus. Add `IsFocusInOtherPanel`. Remove the pointer only gates of E, W, Shift+A. | `ProjectTree.cs`, `MainWindow.axaml(.cs)`, README | Routing matrix: focus in tree, pointer over, focus in other panel, text box | 4 |
| 1.4 | When a row closes over the cursor, move the cursor to that row. | `ProjectTree.cs`, `TreeNode.cs` | `Collapse_CursorUnder_MovesCursorToTheRow` | 2 |
| 1.5 | Fix the hook overwrite of two tree views (finding 6). | `ProjectTreeView.axaml.cs`, `ProjectTree.cs` | Two views watch, one unwatches, the other still works | 3 |
| 1.6 | `Conflicts` also checks the Edit and File scopes against every scope. | `ShortcutMap.cs` | `Conflicts_TreeGestureOnUndo_ReportsTheConflict` | 2 |
| 1.8 | With two tree views, only the active view (`ProjectTree.ActiveView()` rule) handles a request: RenameRequested, AddRequested, MenuRequested, CollapseOthersRequested, TreeFocusRequested. Today every view handles it, so W opens two menus. Found in step 1.5. | `ProjectTree.cs`, `ProjectTreeView.axaml.cs` | Two views, one request, one handler | 3 |
| 1.9 | A click on a row always puts the cursor on that row, also when the row is already selected. Today the cursor follows only a change of `SelectedNode`. Found in step 1.4. | `ProjectTree.cs`, `ProjectTreeView.axaml.cs` | Down, then a click on the selected row, puts the cursor there | 2 |
| 1.10 | `ActiveView()` also counts the focus in the side panel of a view (its search box, its bar): rows first, then panel, then pointer, then first view. Add the hook `IsPanelFocused`. `TreeSearchBox` answers `SearchFocusRequested` only in the active panel. With the tree in both panels, Escape in a search box and Ctrl+F stay in their own panel. Found in steps 1.3 and 1.8. | `TreeViewHooks.cs`, `ProjectTree.cs`, `ProjectTreeView.axaml.cs`, `TreeSearchBox.axaml.cs` | Two hosts: focus in the second search box, FocusTree and FocusSearch go to the second panel only | 3 |
| 1.7 | The tunnel handler swallows the keys of Avalonia's TreeView that the design does not use (Add, Subtract, Multiply, Divide) and blocks Ctrl+click until multi-select exists. Any key closes the open tooltip; no tooltip opens until the mouse moves. | `ProjectTreeView.axaml.cs`, `TreeTooltips.cs` | Key tests, tooltip test | 2 |

### Phase 2: Menus at the cursor row

| Step | Work | Files | Tests | Cx |
|---|---|---|---|---|
| 2.1 | Add `TreeRowAnchor.CenterOf(TreeViewItem, Visual)`. W and Shift+A from a key open at the cursor row. A right-click opens at the pointer and puts the cursor on the clicked row, also on the selected row (left clicks do this since step 1.9). The Menu key opens the menu of the focused row. No Shift+F10: the owner wants no F keys. | `TreeRows.cs` (or new `TreeRowAnchor.cs`), `AddPanel.cs`, `ProjectTreeView.axaml.cs` | Panel position equals row position; Menu key test | 3 |
| 2.2 | Give `MenuItemContext` a `:selected` look with the hover fill (D14). | `CMiX.UI` `ContextMenu.axaml` | Measure the fill of a selected menu row | 2 |
| 2.3 | Rename the commands that no longer act under the pointer: `AddUnderPointerCommand` to `AddAtTargetCommand`, `OpenMenuUnderPointerCommand` to `OpenMenuAtTargetCommand`, `ToggleExpandUnderPointerCommand` to `ToggleExpandTargetCommand`. Follow "How to rename". Found in step 2.1. | `ProjectTree.cs`, `MainWindow.axaml`, tests, docs | Binding tests with the new names | 2 |

### Phase 3: Shared tokens and the cursor look

The consistency lead asks that the new looks go into CMiX.UI so the Inspector, the managers and the menus use the same states.

| Step | Work | Files | Tests | Cx |
|---|---|---|---|---|
| 3.1 | Add tokens with their final names: `CursorFrameBrush`, `CursorFrameInactiveBrush`, `CursorFrameThickness`, `OpacityDisabled` (0.5), `OpacityDragSource` (0.4), `OpacityDragGhost` (0.8), `OpacityIconIdle` (0.6). Rename `DropBlockedBrush` to `WarningMutedBrush`, kept apart from `PopupCardWarning`. | `CMiX.UI/Themes/Tokens`, `CMiX.UI/Themes/Motion.cs`, `Brushes.axaml` and every user | Token tests | 2 |
| 3.2 | Replace the opacity and duration numbers in place with the tokens (0.56 becomes `OpacityDisabled`; `ProjectTree.axaml:125` uses `Motion.FadeDuration`). | `TextBox.axaml`, `Tab.axaml`, `ContextMenu.axaml`, `RowToggles.axaml`, `ProjectTree.axaml`, `TreeDragGhost.cs` | Measure the opacities | 2 |
| 3.3 | Add the attached `CursorRow.IsCursor` that sets the pseudo class `:cursor`. Add `TreeNode.IsCursor`. Style `.cursor` in `TreeViewItemCMiX` and `ListBoxItemBasic`. | `CMiX.UI` (new `CursorRow.cs`, `ListBox.axaml`), `TreeNode.cs`, `ProjectTree.axaml` | Class follows the model value | 3 |
| 3.4 | Tree look: hover uses FillHover, the selection has no frame, the cursor is a 2px `CursorFrameBrush` frame, 1px `CursorFrameInactiveBrush` when the tree does not have the focus. Drop inside uses a 2px accent frame. | `ProjectTree.axaml` | Measure fill, frame thickness and brush with focus in and out | 3 |
| 3.5 | Panel accent line: a 2px `AccentBrush` line under the header of the panel with keyboard focus, through `:focus-within`. The line stays on while a popup of the panel is open (D18). | `SidePanelHost`, `CMiX.UI` style | Measure the line with focus in, out, and menu open | 3 |
| 3.6 | Measure the contrast of Grey800 and Grey700 on the popup card. Make one shortcut text look in tooltips, menus and `MainMenu.axaml:55`. Add `ContentControlKeyCap` with the same text look. | `CMiX.UI` `ToolTip.axaml`, `ContextMenu.axaml`, `MainMenu.axaml`, new key cap theme | Measure the text brush in the three places | 2 |
| 3.7 | Gallery: add the states Cursor, Cursor and selected, Cursor with panel unfocused and Disabled for list, menu and tree rows; a focused and an unfocused panel; key caps beside tooltip and menu shortcut text. Move the panel focus line style (`PanelFocus.axaml`) into CMiX.UI so the gallery can show it. | `CMiX.UI.Gallery/MainWindow.axaml(.cs)`, `PanelFocus.axaml` | Gallery builds | 2 |
| 3.8 | The main menu dropdown gets the popup card background, as the context menu (owner choice: the shortcut grey reaches only 3.33:1 on Grey400). | `CMiX.UI` `Menu.axaml` | Measure the dropdown background and the shortcut contrast (4.5:1 or more) | 2 |
| 3.9 | The rows of the main menu dropdown use `RowHoverBrush` for hover and the keyboard row, as the context menu (rule: hover = hover fill). Today they use the full accent fill, where the shortcut text has about 1.3:1. The top bar items keep their accent. Found in step 3.8. | `CMiX.UI` `Menu.axaml` | Measure the hover and keyboard row fill; shortcut contrast on it | 2 |

### Phase 4: Into and out of the tree

| Step | Work | Files | Tests | Cx |
|---|---|---|---|---|
| 4.1 | Row toggles get `Focusable=False`. The TreeView gets `KeyboardNavigation.TabNavigation="Once"`. | `RowToggles.axaml`, `ComposableToggles.axaml`, `ProjectTreeView.axaml` | Tab leaves the tree in one step; a toggle click still toggles | 1 |
| 4.2 | Add `PanelFocusCycle`: `Window.NextPanel` (Ctrl+Tab), `Window.PreviousPanel` (Ctrl+Shift+Tab), `Tree.Focus` and `Window.FocusPanel1..4` (Ctrl+1 to 4). It remembers the last focused control of each panel. | new `Views/PanelFocusCycle.cs`, `ShortcutMap.cs`, `MainWindow.axaml(.cs)`, README | Focus moves and comes back to the last control | 3 |
| 4.3 | Escape in another panel, when no editor takes it, goes back to the tree. A click moves the focus; the wheel never does. A click on a toggle of a row without the focus keeps the cursor and the selection (found in step 4.1). | `MainWindow.axaml.cs`, `ProjectTreeView.axaml.cs` | Escape, wheel and toggle click tests | 2 |
| 4.4 | A gesture written by hand as `Ctrl+1` in the settings file works as `Ctrl+D1` (KeyGesture.Parse reads a bare digit as a key value). Found in step 4.2. | `ShortcutMap.cs` | Load `Ctrl+1` and `Ctrl+3`, get `Ctrl+D1` and `Ctrl+D3` | 2 |

### Phase 5: Row actions and the big tree

| Step | Work | Files | Tests | Cx |
|---|---|---|---|---|
| 5.1 | `Tree.Hide` (H) toggles the visibility of the cursor row; for a group it is `Group.Manager.Hidden`. `Tree.ShowAll` (Alt+H). Each is one undo step (`ToggleVisibilityUndoCommand`). | `ProjectTree.cs`, undo class, `ShortcutMap.cs` | One undo step per command | 3 |
| 5.2 | Add `Toast` and `ToastHost` in CMiX.UI on `PopupCard`, as one window overlay raised by an injected service (extend the `IMoveBlockedNotice` pattern). The undo text comes from `ShortcutMap["Edit.Undo"]`. H and Remove show "Hidden: X" and "Removed X" with the undo key cap. | `CMiX.UI/Popups`, `MainWindow.axaml`, service registration | Toast shows the remapped undo key | 3 |
| 5.3 | Shift+E opens the whole branch. Q closes the deepest open level of the whole tree, Shift+Q opens the shallowest closed level. Shift+Alt+E collapses every row. | `ProjectTree.cs`, `ShortcutMap.cs` | One test per command, cursor stays visible | 2 |
| 5.4 | F reveals the selected row: it opens the parents, scrolls the row to the middle, and moves the cursor to it. The opened rows do not go into `searchOpened`. | `ProjectTree.cs`, `ProjectTreeView.axaml.cs` | Path open, cursor on the row | 2 |
| 5.5 | Indent guides at each `TreeIndent` step, the guide of the cursor branch darker. `KindCountBadges` caps at 3 plus "+n" and shows "n hidden" when a collapsed branch holds a hidden row. | `ProjectTree.axaml`, `KindCountBadges.axaml(.cs)` | Measure guide position; badge count tests | 2 |
| 5.6 | Search without F keys: Enter and Shift+Enter move the cursor to the next and previous match and do not select (D4); Down puts the focus in the tree at the match. Retire the F3 defaults in `retiredDefaults`. Rename stays on Ctrl+R. | `TreeSearchBox.axaml.cs`, `ProjectTree.cs`, `ShortcutMap.cs` | Search tests, retired default migration | 2 |

### Phase 6: G grab

| Step | Work | Files | Tests | Cx |
|---|---|---|---|---|
| 6.1 | Split `TreeRowDrag` into `TreeDragSession` (source, ghost, marker, opened rows, `End`) and the press drag starter. No change of behavior. Run the edge capture tests of `fix/drag-capture-loss` again. | `TreeRowDrag.cs`, new `TreeDragSession.cs` | All drag tests pass unchanged | 4 |
| 6.2 | Add `TreeDropSlots`: the valid (target, position) pairs for a source, in row order, from `CanDrop` and `Locate`. | new `TreeDropSlots.cs` in the model | Slot list tests per kind | 2 |
| 6.3 | Add `TreeGrab`. G starts a session from the tracked pointer. The pointer moves the marker; Up and Down walk `TreeDropSlots`; the last input wins (14 px, D13); E opens the row at the marker. A left click or Enter places on a valid slot only; a click on an invalid slot does nothing. A right click, Escape, focus loss, window deactivation and capture loss cancel. The grab takes every click and the right click `ContextRequested`. The drop reads the marker slot, not the pointer. The panel gets a 2px accent frame and a move cursor; no badge (D19). One undo step. | new `TreeGrab.cs`, `ProjectTreeView.axaml.cs`, `ShortcutMap.cs`, `ProjectTree.axaml` | Grab with mouse, with keys, mixed; cancel paths; no click pass through | 5 |
| 6.4 | Add `KeyHintStrip` in CMiX.UI. The tree gives its texts for idle, grab, search and menu. It is on by default; a setting hides it (D12). | `CMiX.UI/Controls/Parts`, `ProjectTreeView.axaml`, settings | Strip text follows the mode | 3 |

### Phase 7: Later, not planned in detail

Isolate one branch (Shift+F, header badge "Isolated, Esc back"), M "Move to...", Shift+H, Left and Right depth in grab, key profiles (Default and Blender), binding by physical key for AZERTY, save the open rows in the project, Tab stops in the Inspector, multi-select, the mouse wheel must not change an Inspector slider without focus.

## 5. Parallel work

- The steps of one wave run at the same time. Each step runs in its own git worktree, on its own branch made from the plan branch.
- A wave starts when the main agent has checked and merged every commit of the wave before.
- Steps that change the same lines are never in the same wave. Steps that change other parts of the same file can be, and the main agent resolves the merge.
- The phases stay in order.

| Phase | Wave A | Wave B | Wave C | Wave D |
|---|---|---|---|---|
| 1 | 1.2, 1.5, 1.6, 1.7 | 1.3, 1.4, 1.8 | 1.9, 1.10 | 1.1 (the rename goes last, so no branch keeps the old names) |
| 2 | 2.1, 2.2 | 2.3 | | |
| 3 | 3.1, 3.5, 3.6 | 3.2, 3.3 | 3.4, 3.8, 3.9 | 3.7 |
| 4 | 4.1, 4.2 | 4.3, 4.4 | | |
| 5 | 5.2, 5.3, 5.4, 5.5 | 5.1, 5.6 (5.1 uses the toast of 5.2) | | |
| 6 | 6.1, 6.2, 6.4 | 6.3 | | |

## 6. Checks

- After each step, the main agent checks the commit: message form, no trailer, only the files of the step, no em or en dash, comments in STE.
- Per phase: build once, run the tests that pertain to the changes.
- At the end of the plan: the full Core suite twice with details captured, and the Studio suite once.
- After each visual step, the main agent tells the owner what to look at in the Studio.
