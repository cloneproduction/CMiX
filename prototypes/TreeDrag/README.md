# TreeDrag prototype

A small prototype of a tree with declarative drag, after "Declarative Dragging" (https://declarative-dragging.github.io/).
It is separate from CMiX.sln and does not replace the tree of CMiX.

## Run

```
dotnet test
dotnet run --project TreeDrag.Demo
```

## What it does now (step 0)

- `TreeDrag.Core` holds pure functions: `Layout`, `Move`, `LegalSlots`, `Candidates`, `Nearest`. No UI types.
- `TreePanel` makes one flat row per visible node and arranges each row with `Layout(Root)`.
- When the pointer moves more than 4 pixels after a press, the drag starts and the panel computes all candidates. On move, it picks the nearest spot and moves the other rows
  to their place in that tree with a transition on the render offset. A blue frame shows the spot.
- On release, the candidate tree becomes the new `Root` and the dragged rows animate into their slot. Esc animates back.
- A click on the arrow of a row expands or collapses the node with `Toggle`. The rows jump, they do not animate yet.

## Next steps

Each step is small and can be tried in the demo. Complexity is 1 to 5.

1. Done: expand and collapse with `Toggle` and a click on the arrow.
2. Done: on release, the dragged rows animate from the pointer into their slot.
3. Done: the drag starts when the pointer moves more than 4 pixels.
4. Done: `SelectedId` on the panel. A click or a drag selects the row, a click on the empty area clears it.
5. Done: the author gives `CanDrop(node, parent)`. `LegalSlots` skips the rejected parents and keeps the current slot.
6. Drop into a collapsed node: a slot that appends to a collapsed parent, with a spot on that parent row. (3)
7. Big trees: prune the candidates to slots near the pointer, measure the time with a test. (2)
8. Items control: move to a `SelectingItemsControl` with `TreePanel` as the items panel, for keyboard and selection. (4)
9. Composition animations instead of transitions, when the transitions are too slow. (3)
10. A test with Avalonia.Headless that drags a row and checks the new `Root`. (2)
11. Integration into CMiX: one tree of the Studio uses the control behind a switch. (5)
