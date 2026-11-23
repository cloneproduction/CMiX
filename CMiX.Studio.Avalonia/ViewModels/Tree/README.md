# The project tree

The tree shows the project: compositions, layers, entities, cameras, lights, textures, filters and modifiers as rows. The modulators are not rows: the inspector of their owner shows and adds them. The user selects, searches, adds, removes, moves and drags rows. This page says how the code is organized. Read it before you change the tree.

Two views show the tree: `TreeViewProject`, the old view that this page describes, and `ListTreeProject`, the new view. The model half below is shared. The new view is described in `../ListTree/README.md`.

## The one pattern

The tree has two halves. Each half is a hub that owns small parts. A part has one job and its own state.

```
 MainWindow key bindings, toolbar, search box, inspector
            |  bind to
            v
 +------------------------------+     requests (8 events)     +------------------------------+
 |  ProjectTree  (model hub)    | --------------------------> |  TreeViewProject (screen hub)|
 |  ViewModels/Tree             | <-------------------------- |  Views/Tree                  |
 |  one ObservableObject,       |     hooks (TreeViewHooks,   |  one UserControl,            |
 |  six partial files           |      one set per view)      |  one part per concern        |
 +------------------------------+                             +------------------------------+
            |  owns                                                       |  owns
            v                                                             v
   TreeSearch, TreeEdit, TreeOpenState, ...                 TreeKeyInput, TreePointerInput, TreeMenus, ...
            |  read and write                                             |  read
            v                                                             v
   Project, PrefabManager lists, UndoManager (CMiX.Core)      TreeView, TreeViewItem rows (Avalonia)
```

- `ProjectTree` is the model hub. It is the one object that XAML binds. Its surface is split into partial files, one per concern. It holds the bound state and the commands. The logic is in the parts.
- `TreeViewProject` is the screen hub. Its constructor builds the parts and connects them. It holds no logic.
- The model never touches the screen. When it needs the screen to do something (reveal a row, open a menu, start a rename, start a grab) it raises a request event. `TreeScreenRequests` answers them, in the active view only.
- The screen tells the model facts (is the pointer over the tree, is the tree focused, is a text box focused) through `TreeViewHooks`. Each view gives its own set. `TreeViewSet` holds them and picks the active view. Two views can show one tree.
- Every key command goes through one gate, `CanRunTreeKey`, in `ProjectTree.Commands.cs`.

## Where the data comes from

A `TreeNode` is one row. It wraps one `IControl` item of a `PrefabManager` list (a layer, an entity, a filter) or one `TreeGroup` (a list such as "Entities"). The rows follow their lists: an add, a remove or a move in the list changes the children of the row. The tree never writes a model and never saves anything. It changes the project through the list methods (`AddItem`, `RemoveItem`, `MoveItemTo`, `DuplicateItem`) and the undo goes through `UndoManager`, so a move in the tree is one undo step like a move in a list.

## The model half, folder `ViewModels/Tree`

The hub, `ProjectTree`, in six partial files:

| File | Holds |
|---|---|
| `ProjectTree.cs` | The constructor, the root row, the parts, `Roots`, `Shortcuts`, `Interface`, the hook queries, `Dispose`. |
| `ProjectTree.Commands.cs` | Every command as a `[RelayCommand]` method with its `Can...` method, `CanRunTreeKey`, `RefreshCommands`. |
| `ProjectTree.Cursor.cs` | `CursorNode`, `TargetNode`, `SelectedNode`, `InspectedNode`, the cursor moves, the selection, and the order rule: the selection raises first, then the cursor moves. |
| `ProjectTree.Search.cs` | `SearchText`, `IsSearchActive`, `SearchMatches`, the calls into `TreeSearch`. |
| `ProjectTree.Drag.cs` | `IsDragging`, `BeginDrag`, `EndDrag`, `CanDrag`, `CanDrop`, `Locate`, `DropAsync`. |
| `ProjectTree.Requests.cs` | The eight `...Requested` events, `FocusTree`, `FocusSearch`. |

The parts, grouped by what the user does:

| Group | Part | One job |
|---|---|---|
| Navigate | `TreeWalk` | Walks the rows: all rows, visible rows, parents, depth, find. |
| Navigate | `TreeOpenState` | Expand a branch, open or close a level, collapse all, collapse others, open the parents of a row. |
| Navigate | `TreeSearch` | The matches, the rows the search opened, the kept cursor row. `Release(row)` takes a row out of the search when the user changes it. |
| Navigate | `TreeViewSet` | The attached views, the active view, the aggregate hook answers. |
| Edit | `TreeEdit` | Duplicate, move up, move down, remove, hide, show all, with the notifications. |
| Edit | `TreeAddCatalogue` | What can be added under a row: `GroupsFor` for the Add panel, `ChoicesFor` for the toolbar. |
| Edit | `TreeAddIds`, `AddableTypes`, `TreeIcons`, `TreeCategory`, `TreeGroup` | The fixed tables: add ids, labels, icons, kinds, the groups of a row. |
| Drag | `TreeDragState` | Marks every row with a `DragLook` while a drag runs. |
| Drag | `TreeDropService` | The rules: where a row may drop, and the drop itself. |
| Drag | `TreeDropText` | The words of a blocked move. |
| Drag | `TreeDrop` | The move as one undo step, the holders opened, the notice on a block. |
| Drag | `TreeDropSlots` | The places a grab can step through with the keys. |
| Drag | `TreeRowOrder` | The last visible row, the row above, the composition of a row. |
| Drag | `TreeFileDropRules` | Files from disk: which row takes them and what they add. |

A row, `TreeNode`, with its small types: `RowCounts` (the kind counts, the hidden count, the missing count, each bubbling to the parent) and `DragLook` (how a row looks during a drag). The row computes its own search look. The row reports to the hub through the six methods of `TreeNodeHost`, so the row never holds the whole tree.

## The screen half, folder `Views/Tree`

The hub, `TreeViewProject.axaml.cs`: builds the parts, attaches and detaches them with the visual tree, subscribes once to the tree, and keeps the one-line accessors the tests read.

| Group | Part | One job |
|---|---|---|
| Input | `TreeKeyInput` | Keys while the tree has the focus: Escape, the navigation keys, Right opens Add. |
| Input | `TreePointerInput` | Press, move, release, wheel, double tap, middle click, Ctrl click, the chosen row. |
| Input | `TreeFocusKeeper` | Keeps the focus in the tree and gives it back after a layout or a row button. |
| Input | `TreeFileDropInput` | Files dragged from disk over the rows. |
| Input | `TreeScreenRequests` | Answers the eight requests of the model, in the active view only. |
| Menus | `TreeMenus` | The context menu and the Add panel (`AddPanel`). |
| Menus | `TreeHints`, `TreeKeyHints` | The key hint strip under the rows: the driver, and the keys of each mode. |
| Menus | `TreeTooltips` | The tooltips of the rows. |
| Drag | `TreeDragSession` | One drag from start to drop: the ghost, the marks, the rest timer, the auto scroll. |
| Drag | `TreeRowDrag`, `TreeGrab` | The two starters: the pointer, and the keyboard grab. |
| Drag | `TreeDragGhost`, `TreeDropMarker`, `TreeDropMark`, `DragAutoScroller` | The label under the pointer, the white marks, the scroll at the edges. |
| Rows | `TreeRowIndex`, `TreeRows`, `TreeRowAnchor` | Finds the item of a row and its place on screen. |
| Rows | `TreeRowMetrics`, `TreeRowResources`, `TreeScrollAnchor`, `TreeRowReveal`, `TreeRowShadow` | Row sizes in whole pixels, keeping a row still while rows close, scrolling a row into view, the shadow. |
| Rows | `TreeRowBrush`, `TogglesRow`, `BadgesKindCount`, `BadgeCategory` | The colours and the small controls inside a row. |
| Rows | `TreeViewContext` | What every part needs from the view: the tree view, the row index, the tree, the scroller. Its getters read live values. |

The look of a row is in `Themes/Styles/ProjectTree.axaml` (the item theme) and in the styles block of `TreeViewProject.axaml` (the cursor frame). The tokens of the tree are in `CMiX.UI/Themes/Tokens/Geometry.axaml`.

## Where to go

| To change | Open |
|---|---|
| What a key does | `ProjectTree.Commands.cs`, then the part the one-line method calls. Keys with the focus in the rows: `TreeKeyInput`. |
| The search | `TreeSearch`. Nothing else holds search state. |
| Which row may drop where | `TreeDropService`. The words of a refusal: `TreeDropText`. |
| How a row looks during a drag | `DragLook` and `ProjectTree.axaml`. |
| What the Add menu offers | `TreeAddCatalogue` and `TreeAddIds`. |
| Where a menu or the Add panel opens | `TreeMenus`. |
| The focus after a click or a layout | `TreeFocusKeeper`. |
| The size or the pitch of the rows | `TreeRowMetrics` and the Tree tokens in `Geometry.axaml`. |

## Rules of this folder

- No boolean flag parameter. Two named methods, or a small record such as `DragLook`.
- No tuple across a method boundary. A small record, such as `TargetPlace`, `DropPlace`, `RowSizes`.
- A comment says why, in one or two lines. A name says what.
- A part gets what it needs in its constructor. The constructor only assigns fields.
- The tests are the contract. A moved member keeps its name, or the tests take the new name in the same commit.

The next step after this structure, not done yet: each view pushes a small state object (pointer over, focused, text box focused) that `TreeViewSet` reads, in place of the `Func` hooks it calls.
