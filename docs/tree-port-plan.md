# Port of the new project tree to Avalonia

This document is for a Claude Code session, or a person, that ports the tree of the mockup
`docs/ui-kit/mockups/nested-boxes.html` (version 52) to CMiX.Studio.Avalonia.
Read it all before you change code. Audit the code against it first, then execute the plan phase by phase.

Status: plan, not started. Date: 2026-11-23. Owner decisions are marked "decided".

## 1. How to read the mockup

- Open `docs/ui-kit/mockups/nested-boxes.html` in Chrome. The list at the left has numbered versions. Version 52 is the target.
- A version is a set of CSS classes (`look-...`) on the panel. Later classes override earlier ones, so the CSS of one class alone is not the design. Section 3 gives the resolved values of version 52. Use them, not the CSS.
- The JS of the page holds the model: `layout()` (the layout function), `Panel.candidates()` (the drop places), `Panel.move()` (the nearest pick), `Panel.render()` (the states), `Panel.markActive()` (hover and selection classes).
- The page can not load the kit tokens in the Claude browser pane. In the pane, inject the tokens or read the values from section 3.

## 2. Principles

These are the rules the owner arrived at after about 50 versions. Each one has a reason. Keep them when a detail is not covered here.

1. **A flat tree, laid out by one pure function.** Every visible row is a direct child of one panel. A pure function gives the rect of each row from the model: depth times the indent, rows one under the other. Expand and collapse only change which rows the function returns. Reason: the nested TreeView needs many helper parts (row index, scroll anchor, row shadow layer, drop marks) to find and move rows; a flat panel knows every rect.
2. **Declarative drag.** At the start of a drag, list every legal drop place. For each place, lay out the tree as if the move were done, and take the rect of the dragged row in that layout. During the drag, the nearest candidate wins, and the other rows animate to their place in that layout. The release commits that candidate as one undo step. Reason: no hand-written hit testing, the user sees the result before the release (UIST paper "Declarative Dragging", https://declarative-dragging.github.io/).
3. **Depth from the pointer x.** With full width rows, every candidate has the same x. So the pick weights the horizontal distance to the depth of the candidate: 24 px of pointer movement is one level. Reason: without it, the places "end of a group" and "end of its parent" are only a few pixels apart.
4. **Full width rows, indent only the content.** A row spans the whole panel. The indent moves the content (icon and name). The toggles (visibility, mask) sit inside each row at the right edge and line up at every depth. Reason: nested cards that move in on both sides get narrower at each level, and a separate toggle column looked detached (both rejected).
5. **Colour only where it means something.** At rest, a row is a faint neutral band; only the icon carries the kind colour. No coloured row fills at rest, no hue wheel by depth on surfaces. Reason: full kind tints were "too colourful", all grey made kinds unreadable.
6. **State on a surface, marks on the left.** A state lights the surface of a card (header and the area of its children together), or the row of an item. A card header never gets its own coloured bar. A state mark is a left border only, never a frame all around (except the drop target, see 8). Reason: horizontal bars and frames all around were rejected many times.
7. **Hover is the selection, weaker.** Hover and selection use the same shape: a surface and a 3 px left border. Selection is stronger (12 % surface, full kind colour border, white text). Hover is weaker (5 % surface, border at 45 %). Reason: the cold viewer could not tell hover from selection when both used 8 %.
8. **One outline, only for the drop.** During a drag, only the current drop target gets a full outline, in its kind colour. Nothing else has an outline. Reason: when possible targets and the target both had outlines, the UI was confusing.
9. **During a drag, bright where it can land.** A row stays bright when the dragged thing can land inside it (a possible target) or next to it (a direct child of a possible target). Every other row dims to a readable grey. Same kind siblings stay bright, so the user sees the stack he drops into. Reason: the owner wants to see the stack of entities he drops into, and to see at once what can not take the drop.
10. **Readable dimming, never opacity.** Every state has a fixed lightness. Text: about APCA Lc 60 at rest, 45 dimmed, 30 out of play. Do not dim with opacity (opacity 0.2 made rows invisible). Steps of lightness at least 0.04 apart in OKLCH L. Reason: colour review by a digital signage specialist.
11. **One stacking order.** From the bottom: tree background, card areas outer to inner, rows, toggles, the landing slot, the drop outline, the dragged row. No shadow falls on rows. Reason: the selected row shadow of the current tree draws over other rows and over the selection area.
12. **The same gap everywhere.** Every row starts 35 px after the one before (32 px row, 3 px gap). A card ends at the bottom of its last child, with no padding inside its end. Reason: owner decision, uneven gaps looked wrong.

## 3. Specification of version 52 (measured values)

Colours are the kit tokens (`docs/ui-kit/tokens.css`): TreeBackground #0F0F12, TreeTextRest #C9C9CE, ActiveText #FFFFFF, Grey800 #808083, the Category colours. "White N %" means white mixed at N % over TreeBackground (opaque), unless it says "band", which is white at N % alpha over what is under it.

### Geometry

| Part | Value |
|---|---|
| Panel margin | 8 left, 8 top, 8 bottom, 0 right (as TreeMargin) |
| Row height | 32 |
| Gap between rows | 3, everywhere (pitch 35) |
| Indent per level | 16 (TreeIndent) |
| Content start | 2 |
| Chevron | none; the icon of a card toggles open and close on click |
| Icon | 16, margin 8 at each side |
| Name | 12 px font, padding 6 at each side; card names weight 600, items 400 |
| Toggles | inside the row at the right; slots 32 x 32 (the row height), gap 1, column 66; mask first, visibility at the far right |
| Corners | 2 (CornerRadiusAll) |
| Card | full width; ends at the bottom of its last child |

### States outside a drag

| State | Surface | Left border | Text | Icon |
|---|---|---|---|---|
| Rest, card header | band white 5 %, from the content indent | none | TreeTextRest at 90 % (#B6B6BB), weight 600 | kind colour |
| Rest, item | band white 5 %, from the content indent | none | #B6B6BB | kind colour |
| Hover, card | card area white 5 %; header band transparent (shows the area) | 3 px, kind colour at 45 %, full height of the card | TreeTextRest for the header and all its children | kind colour |
| Hover, item | row white 5 %, full width | 3 px, kind colour at 45 % | TreeTextRest | kind colour |
| Selected, card | card area white 12 %; header band transparent | 3 px, full kind colour, full height of the card | white for the header and all its children | kind colour (the icon is never white) |
| Selected, item | row white 12 %, full width | 3 px, full kind colour | white | kind colour |
| Selected and hovered | white 16 % | as selected | as selected | as selected |
| Toggles | none | none | TreeTextRest at 0.9; white on a selected row; a hovered toggle shows its icon white, no square behind it | |
| Collapsed card | as its state | as its state | as its state | a 2 px halo around the icon in the kind colour at 55 %; count squares (one per kind, 16 px, kind colour at 60 %) after the name |

Hover is tested on the row (header or item) and its toggles, not on the empty space between rows.
The selection wins over the hover.

### States during a drag

| Part | Value |
|---|---|
| Dragged row | snaps into its landing place at its new depth (it does not float under the pointer); row white 16 %, full width; white text; kind colour icon |
| Rows below the landing place | move down by one pitch to make room (from the declarative layout) |
| Landing slot | a 2 px line, full width, in the full kind colour of the target, at the top of the gap |
| Current drop target | 1.5 px outline on all four sides, kind colour at 70 %, 2 px outside the card, corner 2; card area white 5 % (as hover); no left border |
| Reorder inside the same parent | no outline and no area on the parent; only the slot |
| Bright rows | possible targets (rows that can hold the dragged kind) and their direct children: normal band, TreeTextRest text, kind colour icon |
| Dimmed rows | every other row: band white 2 %, text #9A9A9F, icon Grey800 |
| Hover and selection marks | hidden during the drag |
| Drop into a closed card | a hover of 500 ms on a closed card opens it, and the candidates are made again |
| A dragged card | is shown closed during the drag and opens again after the drop if it was open |
| Escape | cancels; every row animates back |
| Alt held | copies instead of moving; Alt can be pressed or released during the drag |
| After the drop | the moved row flashes once (0.7 s, accent colour fading out) |

### Animation

Row moves: 160 ms, ease out (MotionDuration). Surface colours: 120 ms to 350 ms ease. The dragged row has no transition.

## 4. Audit of the current code (2026-11-23)

Read only audit. The executing session must check it again (section 6), because the code may change before the port starts.
Paths are under `CMiX.Studio.Avalonia/` unless stated.

### 4.1 What to keep, adapt or replace

**Model, keep as is.** `TreeWalk`, `TreeOpenState`, `TreeSearch`, `TreeViewSet`, `TreeViewHooks`, `TreeEdit`, `TreeAddCatalogue`, `TreeAddIds`,
`AddableTypes`, `TreeIcons`, `TreeCategory`, `TreeGroup`, `TreeDropService`, `TreeDropText`, `TreeDrop`, `TreeFileDropRules`, `TreeRowOrder`,
`RowCounts`, `KindCount`, `TreeNodeHost`, the small records, the six partial files of `ProjectTree`, `Themes/CategoryPalette.cs`, `TreeToneConverter`.

**Model, adapt.**
- `TreeDropSlots`: it walks only the visible rows (`TreeDropSlots.cs:27`). The drag needs slots in collapsed cards and empty lists too.
- `TreeDragState` and `DragLook`: they encode the old look (candidate, neighbour, hint), not a layout. They become the bright and dimmed classes of principle 9.
- `TreeNode`: needs the option E flag and a stable row identity. `IsDropHint` and `IsDimmed` (`TreeNode.cs:201-203`) follow the old look.
- `ProjectTree.Drag.Locate(source, hovered, fraction)` is hit testing and goes away. `DropAsync` stays (one undo step, move then select).

**Views, keep.** `TogglesRow`, `BadgeCategory`, `TextBoxSearchTree`, `ToolbarProjectTree`, `TreeHints`, `TreeKeyHints`, `TreeTooltips`,
`TreeViewLifetime`, `FrameHold`, `NodeChosenEventArgs`, `TreeDragEnd`, `RowSizes`, `TreeRowResources`, `AddPanel`, `DragAutoScroller`.

**Views, adapt.** `BadgesKindCount` (binds `$parent[TreeViewItem]`), `TreeKeyInput`, `TreeFocusKeeper` (uses `TreeView.LayoutUpdated`),
`TreeFileDropInput`, `TreeMenus` (`TreeView.ContextMenu`), `TreeScreenRequests`, `TreeRowMetrics` (the pitch math stays), `TreeDragGhost`,
`TreeGrab` (the key stepping stays), `TreeCoverConverter`, `KindIconBrushConverter` (its empty group branch dies under option E).

**Views, replace.** `TreeViewProject.axaml` (nested `TreeView`, lines 48-134), `TreeRowIndex`, `TreeRows`, `TreeRowAnchor`, `TreeRowReveal`,
`TreeScrollAnchor`, `TreeViewContext`, `TreeRowShadow` (breaks principle 11), `TreeRowBrush`, `TreeDropMark`, `TreeDropMarker`, `DropLine`,
`DragAim`, `TreeDragSession`, `TreeRowDrag`, `TreePointerInput`, `TreeLiftConverter`, and the theme `TreeViewItemCMiX` in
`Themes/Styles/ProjectTree.axaml` (lines 48-258).

### 4.2 Model API the new panel reuses

- `TreeWalk.VisibleRows(roots)` gives the rows in order and skips closed cards; `TreeWalk.DepthOf(node)`; `ProjectTree.VisibleNodes()`.
- `TreeDropService`: `CanDrop(source, target, DropPosition)`, `Drop(...)` gives `DropOutcome`, `BlockReason`, `IsTargetByKind`, `InsideOf`.
  `DropPosition` is Before, After or Inside; an Inside drop appends. `Resolve` (slot to `ListPlace`) is private today.
- `TreeDropSlots(tree, source)`: `Slots` (`DropSlot(Target, Position, IsStart)`), deduplicated by `SlotKey(list, index)`; it already gives
  several depths at one y (the After of each closing block).
- `TreeDrop.Move(...)` opens the holders; `ProjectTree.DropAsync` is one undo step.
- `TreeNode`: `KindCounts`, `ShownKindCounts`, `Category`, `ComposableDepth`, `KindDepth`, `IsCursor`, `IsHidden`.
- Commands in `ProjectTree.Commands.cs`, all behind the gate `CanRunTreeKey`. The `Tree.*` shortcut ids are bound in `MainWindow.axaml:44-52`
  and pinned by `CommandBindingTests`: keep the command names.

**Missing for the declarative drag:**
1. All legal slots for a dragged node, including collapsed cards and empty lists.
2. A public resolve from a slot to `ListPlace` and to its row position.
3. A snapshot projection of the tree (row, kind, depth, expanded) and a pure `Move` on it, because `TreeNode` is a live view of the
   `PrefabManager` lists. The pure `Move` must reproduce three rules: items come before groups, a drop opens its holders, an Inside drop appends.
4. A stable row identity: the `TreeNode` or a path, not the `IControl`, because one item can show under several layers.
5. A layout record: pitch 35, indent 16 per level (do not change `TreeIndent`), option E filter.
6. `Nearest` with the depth weighting of principle 3, and the drag threshold. The prototype has `Candidates` and `Nearest`
   (`prototypes/TreeDrag/TreeDrag.Core/TreeOps.cs:82-131`).

### 4.3 Category rows

`TreeNode.AddGroups` (`TreeNode.cs:78-82`) always adds a row for each `TreeGroup.GroupsOf(item)`. Option E should filter in the walk, not
in the model: the drop rules need the group row as a model child (`GroupUnder`, `KindOfItem` in `TreeDropService.cs:94-108`).
The rule "a group row with no item is not visible" goes into `VisibleRows`, `LastVisible` and `TreeOpenState`. A hypothetical layout applies
it again after the move, because a drop into an empty list makes its group row appear. `ProjectTreeGroupsTests` pins the empty rows today.

### 4.4 Tests

68 tree test files, about 761 tests. 19 test the model and survive (among them `TreeDropServiceTests`, `TreeDropSlotsTests`,
`CategoryPaletteTests`, `ProjectTreeSelectionTests`, `ProjectTreeSearchTests`). 49 test the `TreeView` screen and must be rewritten for the
new view (among them `TreeViewProjectTests`, `ToolbarProjectTreeTests`, `TreeRowMenuTests`, `TreeRowKeysTests`, `TreeRowDragTests`,
`TreeGrabTests`). 10 more files outside the tree touch `TreeViewProject` (`MainWindow*Tests`, `PanelFocus*Tests`, `PanelInspectorTests`,
`SidePanelTests` and others). `ProjectTreeRowMetricsTests` pins the row height 24.

### 4.5 Tokens

- Tree tokens: `CMiX.UI/Themes/Tokens/Geometry.axaml:92-120`; colours in `Colors.axaml` and `Brushes.axaml`.
- The token script is `docs/ui-kit/tools/Update-Tokens.ps1` (CLAUDE.md says `tools/`; the path in CLAUDE.md is out of date).
- The old view stays usable behind the switch until the end, so add new keys for the new view instead of changing the old ones
  (for example a row height of 32 next to `TreeRowHeight` 24). Remove the old keys in the cleanup phase.
- Kind mixes (border 45 %, outline 70 %) are computed per kind. Avalonia brushes can not mix at run time with relative colours:
  compute them in `CategoryPalette` (it already mixes kind colours) and test them.

### 4.6 Risks

- **Stacking:** `TreeRowShadow` draws above every row; the flat panel sets one ZIndex per layer (principle 11) and drops the shadow.
- **Selection order:** `SelectedItem` binds TwoWay to `SelectedNode`. The new panel must keep the order "selection first, then the cursor".
- **Focus and rename:** focus, rename on a double tap and the `TextBoxEditable` of each row depend on `TreeViewItem` focus today.
- **Performance:** one layout per legal slot can be slow in a big project. Measure, and prune to the slots near the pointer if needed.
- **UI kit:** entries 21.1 to 21.10 of `gallery.html` model the old row, the drop marks and the grab. They stay as they are during the
  port and go out of date. Step 6.5 updates the kit once, after the switch over (see 5.1).
- **VL names:** no VL patch of this repository uses the tree types. The parent repository patches were not searched: search them before
  any rename.
- **Build:** while Studio runs, build and test in the scratch copy (see the memory notes).

## 5. Plan

### 5.1 Rules of the steps

Rules for every step (from `.claude/CLAUDE.md`): one atomic commit, build and tests pass, one line message in the third person, no
attribution line, STE English.

**The UI kit:** `.claude/CLAUDE.md` says the kit is updated only when the owner asks (changed on 2026-11-23). The steps of this port do
not update `docs/ui-kit`. The mockup `nested-boxes.html` version 52 is the reference of the look during the port. Step 6.5 brings the kit
up to date once, when the owner asks for it.

The complexity of a step
(1 to 5) gives the agent: 1 Sonnet, 2 and 3 Opus, 4 Fable, 5 the main agent. A subagent reads its step and the code first, and stops to ask
when a finding changes another step or the shared design. The main agent checks each commit before the next step.

The new view lives next to the old one behind a switch until Phase 6. Nothing of the old view is removed before the owner says the new
view is complete.

### 5.2 Rules of the code: keep what the reviews gave

The tree went through many reviews. The new control is a new view, but it keeps the structure and the decisions of those reviews.
It does not do them again. The code can change where a rule below is better served. Each change of a rule is a question to the owner.

The sources, in this order of priority: `.claude/CLAUDE.md`, `ARCHITECTURE.md` (Part 1 is the facts, Part 2 is a proposal that is not
approved), `docs/DEVELOPMENT.md`, `CMiX.Studio.Avalonia/ViewModels/Tree/README.md`, `docs/plans/keyboard-tree.md` (the decisions of the
owner), `docs/views-audit-2026-11-20.md` (theme 3 for the tree).

**The structure stays.**
- Two hubs: `ProjectTree` (the model hub, the one object that XAML binds) and the screen hub of the view. The screen hub only builds and
  connects its parts. Each part has one job and its own state.
- The model never touches the screen. It raises a request event, and the screen part `TreeScreenRequests` answers it in the active view.
- The screen tells the model facts through the hooks of `TreeViewSet`. Two views can show one tree. The new view is a second view of the
  same `ProjectTree` and does not get its own model.
- Every key command goes through the one gate `CanRunTreeKey`. The `Tree.*` shortcut ids and the command names stay.
- A tree change goes through the list methods (`AddItem`, `RemoveItem`, `MoveItemTo`, `DuplicateItem`) and is one undo step.

**The shapes (ARCHITECTURE.md Part 2, section 1).**
- The new pure parts of Phase 1 (snapshot, layout, slots, `Move`, `Nearest`, the bright and dimmed classes) are plain classes or records in
  `ViewModels/Tree`, with no Avalonia type. A static class is allowed only when it holds pure functions and no state.
- A ViewModel never asks the view. The view pushes its state into the model. The README names the next step: each view pushes a small
  state object (pointer over, focused, text box focused) in place of the `Func` hooks. Do it when the new view needs the hooks (step 3.1).
- The code behind of a view holds no logic. Pointer and drag math go into a part, not into the `UserControl`.

**The classes (README rules and the refactorer tripwires).**
- A part gets what it needs in its constructor. The constructor only assigns fields. Dependencies are required and injected.
- No boolean flag parameter. No tuple across a method boundary: a small named record.
- Commands are `[RelayCommand]` with `CanExecute = nameof(...)`. Properties are `[ObservableProperty]`.
- Tripwires, not targets: a class over about 200 lines or with more than 5 constructor parameters, a method over about 30 lines or with
  more than 3 levels of nesting. Crossing one means: stop and ask whether the class does two jobs.
- A comment is one line and says why. A name says what.

**The view (DEVELOPMENT.md).**
- The rule of attached properties and behaviors (section "Attached properties and behaviors"): a value or a switch is an attached
  property, an object with a lifetime is a `Behavior<T>`, a state reaches a style through a pseudo class that its owner sets.
- A color comes from a token. No theme writes a grey for a state. A size comes from a token.
- Names follow the order rule of `.claude/CLAUDE.md`: the kind first. `x:Name` is camelCase.
- A row toggle is not a Tab stop. The cursor and the selection have different looks. Hover never uses the selection fill (keyboard plan,
  findings 5, 7 and 8).

**The review gates.** At the end of Phases 1, 3 and 4, run the read only reviewers of `.claude/agents` on the new code:
`view-reviewer-mvvm`, `view-reviewer-avalonia` and `view-reviewer-reuse` (Phases 3 and 4), and a check against the tripwires above
(all three phases). The main agent fixes the findings, or asks the owner, before the next phase starts.

### Phase 0: decisions and check (no code)

| Step | Work | Complexity |
|---|---|---|
| 0.1 | Ask the owner the open questions of section 7, one at a time. Record the answers here. | 5 |
| 0.2 | Check sections 3 and 4 against the mockup and the code. Correct this document. | 5 |

### Phase 1: pure core, tests first (no screen)

| Step | Work | Complexity |
|---|---|---|
| 1.1 | A snapshot projection of the visible tree: row id (the `TreeNode`), kind, depth, expanded, option E filter. Tests. | 3 |
| 1.2 | The pure layout function of principle 1: snapshot to rects (pitch 35, indent 16, full width). Tests with closed cards and option E. | 2 |
| 1.3 | All legal slots for a dragged node from the `TreeDropService` rules, with collapsed cards and empty lists, and a public resolve to `ListPlace`. Tests against `TreeDropSlots`. | 4 |
| 1.4 | The pure `Move` on the snapshot, with the three rules of 4.2. Tests that compare it with a real `TreeDrop.Move`. | 4 |
| 1.5 | `Candidates` and `Nearest` with the depth weighting (24 px per level, x weight 1.5) and the drag threshold. Port the prototype tests. | 3 |
| 1.6 | The bright and dimmed classification of principle 9 (possible target, direct child of one, dimmed). Tests. | 2 |

### Phase 2: tokens and colours

| Step | Work | Complexity |
|---|---|---|
| 2.1 | New tree tokens for the new view: row 32, gap 3, border 3, outline 1.5 and offset 2, slot 2, the white mixes of section 3, the two text greys (#B6B6BB, #9A9A9F). Do not run `Update-Tokens.ps1` (step 6.5 runs it). | 2 |
| 2.2 | The per kind mixes (border 45 %, outline 70 %, halo 55 %, count squares 60 %) in `CategoryPalette`, with tests that measure the colours. | 3 |

### Phase 3: the new view, without drag

| Step | Work | Complexity |
|---|---|---|
| 3.1 | The switch between the old and the new view, both attached to the same `ProjectTree` through `TreeViewSet`. | 3 |
| 3.2 | The flat panel and the row control (icon that toggles, name in `TextBoxEditable`, count squares, `TogglesRow`), arranged by the layout function, with the ZIndex layers of principle 11. Tests that measure positions and sizes. | 4 |
| 3.3 | Rest, hover and selection on surfaces and left borders (section 3), hover tested on the row only. Tests that measure the colours. | 4 |
| 3.4 to 3.9 | Move one screen part at a time to the new view: keys, focus and rename, menus, tooltips, file drop, screen requests (reveal and scroll). One step each. | 2 to 3 |

### Phase 4: drag

| Step | Work | Complexity |
|---|---|---|
| 4.1 | Pointer drag on the candidates: snap at the new depth, rows animate (160 ms), 2 px slot line, target outline, dimming, Alt copy, Escape, auto scroll, 500 ms hover opens a closed card, flash after the drop, commit through `DropAsync`. Headless tests. | 4 |
| 4.2 | The keyboard grab on the same candidates. | 3 |
| 4.3 | Measure the drag on a big project. Prune the candidates near the pointer if it is slow. | 3 |

### Phase 5: category rows (option E) and the inspector

| Step | Work | Complexity |
|---|---|---|
| 5.1 | Option E in the walk (4.3). Update `ProjectTreeGroupsTests`. | 3 |
| 5.2 | The panel, Add and Hide jobs of a category row in the inspector of its owner (only after the owner answers question 2). | 4 |

### Phase 6: switch over and clean up

| Step | Work | Complexity |
|---|---|---|
| 6.1 | The new view becomes the default. The owner checks it. | 5 |
| 6.2 | Checkpoint commit. Then remove the replaced parts of 4.1, the `TreeViewItemCMiX` theme and the dead tokens, a few files per commit. | 3 |
| 6.3 | Rewrite the remaining screen tests for the new view. | 3 |
| 6.4 | Update `ViewModels/Tree/README.md`, the tree sections of `docs/DEVELOPMENT.md` and this document. | 2 |
| 6.5 | The UI kit, once: run `Update-Tokens.ps1`, add the entries of the new tree with the next free numbers of category 21, mark the old entries 21.1 to 21.10 `removed` where they no longer apply. | 3 |
| 6.6 | The full Core suite twice and the Studio suite once, with details captured. | 2 |

## 6. What the executing session must do first

1. Read this document, `.claude/CLAUDE.md`, the memory notes of the project (MEMORY.md and the files it lists about the tree), and the sources of 5.2.
2. Open the mockup at version 52 and check section 3 against it in Chrome.
3. Audit the current tree code against sections 2 and 4. Report any difference before you change code. When a finding changes the plan, stop and ask the owner.

## 7. Open questions for the owner (Phase 0)

1. Is there more to simplify in the tree, beyond the category rows?
2. Where do the panel, Add and Hide jobs of a category row go: sections in the inspector of the owner, or somewhere else? Is Hide for a whole list still needed?
3. Do the modulators stay in the tree, or move to a project pool first (see the modulation notes)?
4. Saved projects: `.claude/CLAUDE.md` requires the compatibility test before a model change, an older memory note says old saves may break. Which rule applies to this port?
5. The hover mark shows all around in the owner's Chrome but not in the browser pane. Check it together before Phase 3.
6. Expand and collapse: keep an animation (the old `PanelCollapse`), or let the rows move with the same 160 ms row animation?
7. The rest delay that opens a closed card during a drag: 500 ms (mockup) or 0.6 s (code)?
8. `docs/DEVELOPMENT.md` ("The colors of the states") says a selected row uses accent-inactive and an icon only control dims with opacity
   0.6. Version 52 uses a white surface with a kind border, and never dims with opacity. Does the tree become a new row kind in that table,
   or does the table change for every row?
9. The flat panel and its layout: a Studio part of the tree only, or a generic control in CMiX.UI that other lists can use later?
   And the name of the new view, by the order rule (for example `PanelTree`, `TreeViewProject` kept for the new view)?
