# The project tree view

`ListTreeProject` is the view of the project tree. It is a flat list of rows: every visible row is a direct child of one panel, and a pure function gives the place of each row. The design and the plan of the port are in `docs/tree-port-plan.md`. The model half and the shared parts are in `../Tree/README.md`.

The entry Tree of the combo box of a side panel shows it (`PanelChoice.Tree`). Two side panels can show the tree side by side.

The modulators are not in the tree. The inspector of their owner shows and adds them.

## The two hubs

- `ProjectTree` (folder `ViewModels/Tree`) is the model hub. The view uses its commands, the gate `CanRunTreeKey`, the `Tree.*` shortcuts and the requests. The view has no model of its own.
- `ListTreeProject` (folder `Views/ListTree`) is the screen hub. It builds one `ListTreeContext` and attaches its `TreeViewHooks` to the tree while it is in the visual tree. Its behaviors do the work.
- `ListTreeViewState` holds what the view knows: the pointer is over the tree, the tree has the focus, a text box has the focus, the focus is in another panel, the panel has the focus. The view pushes these values. The hooks of `TreeViewSet` read them.

## The pure core, folder `ViewModels/ListTree`

The core has no Avalonia type. Most of it is records and static classes of pure functions.

- Snapshot. `ListTreeSnapshot.Of(roots)` copies the live `TreeNode` rows into `ListTreeRow` records: key, node, depth, card, group, open, children. A row is identified by its `ListTreeRowKey`: the model it shows (the item, or the list of a group row) and the key of its parent row. The `TreeNode` is not the identity, because a move to another list builds new nodes, and one item can show under several layers.
- Layout. `ListTreeLayout.Of(snapshot, sizes)` gives a `ListTreePlace` for each visible row: the top, the row height, the height of its card area, the depth and the indent. A row is full width, so a place has no x. `ListTreeSizes.Default` is a row of 32, a gap of 3 (a pitch of 35) and an indent of 16 per level.
- Option E. A group row with no item is not in the snapshot. A drop Inside a card can still go to its empty list: the slot then targets the hidden group, and `ListTreeSlot.ShownTarget` is the key of the visible card that gets the outline.

## The drag

- `ListTreeSlots` lists every place in the visible tree where the dragged row can land, from the rules of `TreeDropService`, one slot for each list and index.
- `ListTreeCandidates` keeps the slots that the real drop takes for every row of the block, and gives each one a `ListTreeCandidate`: the slot, the place of the moved row, and the moved tree.
- `ListTreeLiveRows` finds the rows of the live snapshot by key and by node, and the node of a model. A drag makes it once for each set of candidates.
- `ListTreePick` picks the nearest candidate. The pointer x gives the depth: 24 px of pointer movement is one level, with a weight of 1.5 on x (`ListTreeDragNumbers`). The current candidate stays until another one is 6 px closer (hysteresis). A drag starts after 4 px.
- `ListTreeMove` gives the snapshot as if the block were dropped on the slot. A row that the move does not change is the same object as in the live snapshot. `ListTreeCandidate` builds this snapshot and its layout only for the candidate that the drag shows, then keeps them.
- The rows slide to their places in the moved layout. The dragged row snaps into its landing place. The rows where the drop can land, and their direct children, stay bright (`ListTreeBrightRows`). The other rows dim to a fixed grey, never to an opacity. The line under the rows says where the row goes (`ListTreeDragNote`).
- A rest of 500 ms next to a closed card, or the key `Tree.Expand`, opens the card (`ListTreeNearCard`, `ListTreeDragRest`). The key closes again a card that this drag opened (`ListTreeDragSession.ToggleNearCard`). The card stays open after the drop and after a cancel. The mouse wheel scrolls the tree during a drag, and the tree scrolls at its edges.
- A block: a drag of several selected rows moves them as one block (`ListTreeBlock`, `ListTreeDragNodes`). A slot is a candidate only when the drop takes every row of the block. `TreeDrop.MoveBlock` reads every row against the target before a row moves, so a refused row refuses the whole block. `ProjectTree.DropBlockAsync` is one undo step.
- The drop selects the moved rows. A dropped card gets back the open state it had before the drag, and the cards under it too (`ListTreeOpenState`). Escape cancels, and every row slides back.
- The keyboard grab (`Tree.Grab`) steps through the same candidates with the keys (`ListTreeGrabSteps`). A pointer move past a distance gives the mark to the pointer (`ListTreeGrab.MouseTakeOver`).
- Alt copy is not in the tree. A copy is Shift+D, then a drag.

## The view, folder `Views/ListTree`

- `PanelListTreeRows` is the panel of the rows. It builds a `RowListTree` control only for the rows in its row window (`ListTreeRowWindow`): the view of the scroller, and half a view above and below it. A scroll that stays inside a quarter of a view keeps the controls. `RowOf` builds the control of a row outside the window on demand, so a reveal, a focus or a rename finds it.
- `ListTreeRowControls` holds the row controls by key. A row slides with a render offset (`ListTreeRowSlide`): the row gets its new top in the layout at once, and only its render moves for 160 ms, so a frame of the slide needs no layout.
- `ListTreeSurfaces` holds the card areas (`FrameListTreeCard`) in the row window, in tree order. The ZIndex of an area is `ListTreeLayers.CardArea` plus its depth, so an inner area draws over its outer area. It keeps the area of a card across the looks of a drag. It also marks the selection, the hover and the drop target. `FrameListTreeOutline` is the one outline of the drop target, full width, in the kind colour.
- `ListTreeLayers` gives the ZIndex of each layer, from the bottom: card areas, rows, the drop outline, the dragged row.
- The row uses the shared parts of `Views/Tree`: `TogglesRow`, `BadgesKindCount` (the kind squares with the count in white, and the orange Warning icon for a missing asset) and `TreeRowBrush`.

The look of a row, from the tokens `ListTree...` in `CMiX.UI/Themes/Tokens`:

- At rest, a row is a faint band from its indent. Only the icon has the kind colour.
- Under the pointer, a row gets the hover surface and a 3 px left mark in the kind colour at 45 %. A card gets the hover surface on its whole area.
- A selected row gets the selected surface, a 3 px left mark in the full kind colour, and a white name. A selected card gets the selected surface on its whole area. A selected row shows no cursor frame.
- The cursor frame is one pixel in the kind colour, on a row that is not selected.
- During a drag, the hover and the selection marks hide.

## The behaviors, folder `Behaviors/ListTree`

Each part of the view is a `ListTreeBehavior : Behavior<ListTreeProject>`. `ListTreeProject.axaml` declares them in `Interaction.Behaviors`, in the order that they watch the tree. A behavior watches the `ProjectTree` of the DataContext while the view is in the visual tree (`OnWatch`, `OnUnwatch`). It finds a peer with `ListTreeContext.Part<T>()`.

| Group | Behaviors |
|---|---|
| Input and focus | `ListTreeViewStateInput`, `ListTreeScreenRequests`, `ListTreeFocusKeeper`, `ListTreePressFocus`, `ListTreeRowClick`, `ListTreeKeyInput`, `ListTreeLastPointer` |
| Menus | `ListTreeMenus`, `ListTreeAddBeside`, `ListTreeHints`, `ListTreeTooltips` |
| File drop | `ListTreeFileDropInput` |
| Drag and grab | `ListTreeDragSession`, `ListTreeDragRest`, `ListTreeDrop`, `ListTreeRowDrag`, `ListTreeGrab`, `ListTreeGrabInput`, `ListTreeAutoScroll` |
| Scroll | `ListTreeRowReveal`, `ListTreeScrollAnchor`, `ListTreeEndRoom` |

## The clicks

The rule is `ListTreeRowPressRule`.

- A click on a row selects it.
- Ctrl and a click adds the row to the selection or takes it out. Shift and a click selects the shown rows from the anchor of the selection to the row.
- A click on the icon of a card opens or closes the card. It does not change the selection.
- A double click on the row, away from the name, the icon and the toggles, opens or closes a card.
- A double click on the name, or F2, renames in place.
- A middle click selects and inspects.

## The tests

The tests of the tree view have `ListTree` in their name. Run them with:

    dotnet test CMiX.Studio.Avalonia.Tests\CMiX.Studio.Avalonia.Tests.csproj --filter "FullyQualifiedName~ListTree"
    dotnet test CMiX.UI.Tests\CMiX.UI.Tests.csproj --filter "FullyQualifiedName~ListTree"

The second line runs the tests of the tokens of the tree.
