# The row look of the tree, from the mockup to ListTreeProject

Source: `docs/ui-kit/mockups/inspector-with-tree.html`, the After look, decided by the owner on 2026-11-25.
Open it with the kit server (`.claude/launch.json`, entry `ui-kit`) at `http://localhost:8765/mockups/inspector-with-tree.html`.
The file view of the Claude browser pane does not load the kit tokens.

Status: all phases built on 2026-11-25. Phase 1: 1.1 `62225ee70`, 1.2 `7e69effa4`, 1.3 `5f9414fcc`. Phase 2: 2.1 `a8fb630d8`,
2.2 `c585b4bc9`, 2.3 `8e60b98e2`. Phase 3: 3.1 `bad285528`, 3.2 `bfe7a9a9c`. Phase 4: 4.1 `cbec91d2b`, 4.2 `5862518b4`, 4.3 the
commit of the documents. Phase 5: 5.1 `a38729b31`, 5.2 `c8b959947`. Step 5.2 took the highlight range on `TextBoxEditable`, drawn by
the new `TextBlockHighlighted`, which the group name uses too. `Update-Tokens.ps1` is not run: the mockup session runs it.

Out of scope: the new inspector of `docs/ui-kit/mockups/inspector.html` (title, groups, cards, list rows). It is a plan of its own.

## 1. Decisions of the owner (from the mockup)

1. One base height for every control and every row: `ControlHeightMD` is 26 (was 24). The tree rows are 26 (were 32).
2. An icon sits in a square cell of the row height, in the centre: the same space on all four sides (5 px with a 26 px row
   and a 16 px icon). The toggle cells touch: 10 px from the mask icon to the eye icon.
3. A mark is on the row only, header or item. No card area and no rail down the children, for the hover and the selection.
4. Every row with a state (hover, cursor, selected, inspected) has its background at full width. A row at rest keeps the
   band from its indent.
5. The states each have their own form:

| State | Surface, full width | Left border 3 px | Frame | Text and toggles |
|---|---|---|---|---|
| Rest | band white 5 % from the indent | none | none | ListTreeTextRest |
| Hover | ListTreeRowHover | kind colour at 45 % | none | TreeTextRest |
| Cursor | band white 5 %, full width | as its other state | 1 px white at 45 %, inside the row, also on a selected row | as its other state |
| Selected | ListTreeRowSelected | full kind colour | none | white |
| Inspected | kind colour 18 % over TreeBackground, 25 % on hover | 45 % kind colour on hover | none | white |
| Selected and inspected | the inspected tint | full kind colour | none | white |

6. The cursor frame hides during a drag: only the drop target has an outline (color-system review).
7. Count badges of a closed card (BadgesKindCount): the kind icon (16 px, kind colour) with the count at its bottom right,
   9 px bold white with an outline of 2.5 px in TreeBackground. No square behind it. 8 px between two badges.

The cursor frame is new to principles 6 and 8 of `docs/tree-port-plan.md` (a frame all around only for the drop).
Step 4.3 adds that line.

## 2. Findings of the check at `af3536f0b`

1. `RowListTree` has no `:cursor` class. The cursor frame binds `IsVisible` to `TreeNode.IsCursor` (`RowListTree.axaml:220`).
2. The row controls and the card frames come from a pool (`ListTreeSpareControls<T>`). A reused row gets a new node. The cursor and
   the inspected row change with no rebuild. So `:cursor` and `:inspected` must come from a binding to the node in XAML, not
   from `ShowMarks`: a class that code sets would keep the look of the last node.
3. Only the grab sets a class on the view (`ListTreeGrab.GrabbingClass`, `grabbing`). The pointer drag sets none. During a drag a
   bright row is neither `:dimmed` nor `:dragged`, so a selector on these two classes misses it.
4. `KindCount` holds only `Category` and `Count`. `TreeIcons.OfCategory` already gives an icon per category: Composition for
   Composable, MeshLight for Light, and the name of the category for the others. An entity row shows the Geometry icon, so the
   badge of the entities could show Geometry or Entity (decision R1).
5. `ListTreeKindColors` has `HoverBorder`, `DropOutline` and `SelectionBorder`. The inspected tint needs two more fields.
6. After the hover and the selection leave the card frames, `ListTreeMarks.IsInSelection`, `IsInHoveredCard`, `IsHoveredCard` and
   `ListTreeSurfaces.HoveredCard` have no reader. `RowListTree.ShowMarks` and `FrameListTreeCard.ShowMarks` read them today.
7. `ControlHeightMD` sizes every control of CMiX.UI and of the Studio. 12 test files of the Studio and 7 of CMiX.UI use 24 or 32 next
   to a height. `ListTreeSizes.Default` (`new(32, 3, 16)`) repeats the 32 of the token `ListTreeRowHeight`.
8. The first sample of step 4.1 had `Margin="0,0,-5,-4"`, a hardcoded value. It needs a token.
9. The search match mark of proposal S3 (`docs/ui-kit/sandbox.js`) used the left mark, which this look gives to the hover and the
   selection. The owner chose a new search look in the mockup on 2026-11-25 (decision R4, phase 5). S3 is not in the code: step 4.4 of
   the search plan was never built.
10. The name of a row is a `TextBoxEditable` (`nameBox`) for an item and a `TextBlock` (`groupName`) for a group. Neither shows runs
   of text with their own colour today.

## 3. Decisions for this plan

| # | Decision | Default |
|---|---|---|
| R1 | The icon of each category in a badge comes from `TreeIcons.OfCategory`. The entities keep the Entity icon, not the Geometry icon of an entity row. | Yes |
| R2 | The outline of the count. Avalonia has no stroke on a `TextBlock`. A small control `TextOutlined` in CMiX.UI draws the `FormattedText` geometry with a pen, then the fill. | Yes, built in step 4.2 |
| R3 | One class on the view while a drag or a grab runs: `dragging` on `ListTreeProject`. The grab keeps `grabbing` and also sets `dragging`. | Yes |
| R4 | The search look of the mockup, decided by the owner: a match keeps its normal look and only its matched letters take `AccentHighlightBrush`; a parent shown only for a match is dimmed as in a drag; "No match" stays. Phase 5 builds it. The mockup session removes proposal S3 after phase 5. | Decided |
| R5 | `ControlHeightMD` moves in its own step, apart from the tree sizes, because it changes every control. | Yes |

## 4. Phases and steps

Rules for every step: one commit; the build and the tier 1 tests pass; comments in STE, one line; no hardcoded size or colour;
stage by path only, because other sessions commit in this checkout; no attribution line. Each step checks the look with a test
that measures a value (a size, a colour, a position), not with a screenshot.

### Phase 1. Sizes

#### Step 1.1 The tree row sizes (complexity 2, Opus)

Files: `CMiX.UI/Themes/Tokens/Geometry.axaml`, `ViewModels/ListTree/ListTreeSizes.cs`, the tree tests that pin 32.

```xml
<!-- Before -->
<x:Double x:Key="ListTreeRowHeight">32</x:Double>
<x:Double x:Key="ListTreeToggleSize">32</x:Double>
<x:Double x:Key="ListTreeToggleGap">8</x:Double>
<x:Double x:Key="ListTreeToggleColumnWidth">72</x:Double>
<!-- After -->
<x:Double x:Key="ListTreeRowHeight">26</x:Double>
<x:Double x:Key="ListTreeToggleSize">26</x:Double>
<x:Double x:Key="ListTreeToggleGap">0</x:Double>
<x:Double x:Key="ListTreeToggleColumnWidth">52</x:Double>
```
```csharp
// Before, ListTreeSizes
public static ListTreeSizes Default { get; } = new(32, 3, 16);
// After
public static ListTreeSizes Default { get; } = new(26, 3, 16);
```

A new test pins `ListTreeSizes.Default.RowHeight` to the token `ListTreeRowHeight`, so the two never part again. The subagent lists
the tests that pin 32 and the reason of each change in its report. `ListTreeToggleSizes.Write` adds the gap once before the first
slot: with a gap of 0 the eye ends 5 px from the right edge.

Tests: `ListTreeTokensTests`, `ListTreeLayoutTests`, `PanelListTreeRowsTests`, `TogglesComposableTests`, `ListTreeRowDragTests`,
`ListTreeRowDragDropTests`, `ListTreeScreenRequestsTests`, `NameBoxPixelTests`, `NameBoxPositionTests`.

Commit: `Makes the rows of the tree 26 high`.

#### Step 1.2 The base height of the controls (complexity 3, Opus)

Files: `CMiX.UI/Themes/Tokens/Geometry.axaml`, the tests that pin 24.

```xml
<!-- Before -->
<x:Double x:Key="ControlHeightMD">24</x:Double>
<!-- After -->
<x:Double x:Key="ControlHeightMD">26</x:Double>
```

A shared base: the subagent first lists every test and every view that pins 24 or computes from it (the sizes of lists and drop
downs in `Geometry.axaml` lines 59 to 72, popups, tooltips, the scroll bar). It reports the list before it changes a test. A view
that sets 24 by hand is a finding: it moves to the token in the same step.

Tests: tier 2 at once, the CMiX.UI suite and the Studio suite.

Commit: `Makes the base height of the controls 26`.

#### Step 1.3 The icon cell (complexity 2, Opus)

Files: `CMiX.UI/Themes/Tokens/Geometry.axaml`, a new test in `ListTreeTokensTests` or the row look tests.

```xml
<!-- Before -->
<Thickness x:Key="ListTreeRowContentMargin">2,0,0,0</Thickness>
<Thickness x:Key="ListTreeIconMargin">8,0</Thickness>
<!-- After -->
<Thickness x:Key="ListTreeRowContentMargin">0</Thickness>
<Thickness x:Key="ListTreeIconMargin">5,0</Thickness>
```

Tests: `Row_TheKindIcon_HasTheSameSpaceOnEachSide` (5 px left, right, top, bottom on a 26 px row), `Row_TheTwoToggles_Touch`
(10 px from the mask icon to the eye icon).

Commit: `Puts the icons of a tree row in a square cell`.

Phase 1 end: the CMiX.UI suite and the Studio suite once.

### Phase 2. The marks on the row only

#### Step 2.1 The cursor and the inspected classes (complexity 3, Opus)

Files: `ViewModels/Tree/TreeNode.cs`, `ViewModels/Tree/ProjectTree.Cursor.cs`, `Views/ListTree/RowListTree.axaml`, tests.

`TreeNode.IsInspected` is new. `ProjectTree.InspectedNode` sets it as `CursorNode` sets `IsCursor`. The row gets the classes
`cursor` and `inspected` from bindings to the node (finding 2), so a reused row always shows its own node. This step adds the
classes only. No look changes yet.

```csharp
// After, ProjectTree.Cursor.cs
private set
{
    var old = inspectedNode;
    if (!SetProperty(ref inspectedNode, value))
        return;

    if (old != null)
        old.IsInspected = false;
    if (value != null)
        value.IsInspected = true;
}
```
```xml
<!-- After, RowListTree.axaml, on the root UserControl -->
<Classes.cursor><Binding Path="IsCursor" /></Classes.cursor>
<Classes.inspected><Binding Path="IsInspected" /></Classes.inspected>
```

The styles of the next steps use `Views|RowListTree.cursor` and `Views|RowListTree.inspected` (style classes, not pseudo classes).

Tests: `Inspect_ANode_MarksOnlyThatNode`, `Row_AReusedRow_TakesTheCursorClassOfItsNewNode`, `Row_TheInspectedNode_HasTheInspectedClass`.
Also run `ListTreeRowControlsTests`, `ListTreeFocusTests`, `ListTreeKeysTests`, `PanelInspectorSelectionTests`.

Commit: `Gives the tree rows a class for the cursor and the inspected node`.

#### Step 2.2 No card area for the hover and the selection (complexity 4, Opus)

Files: `Views/ListTree/RowListTree.axaml`, `RowListTree.axaml.cs`, `FrameListTreeCard.axaml`, `FrameListTreeCard.axaml.cs`,
`ViewModels/ListTree/ListTreeMarks.cs`, `Views/ListTree/ListTreeSurfaces.cs`, the tests of the card looks.

`FrameListTreeCard` keeps only the drop target state. `RowListTree:selected` and `:pointerover` set the surface and the left border
on a card header too. The text styles of `:inselection` and `:inhoveredcard` go: the children of a selected card stay at rest.

```xml
<!-- Before: RowListTree.axaml -->
<Style Selector="Views|RowListTree:selected:not(:card)">
<Style Selector="Views|RowListTree:pointerover:not(:card):not(:inselection):not(:dimmed)">
<!-- After -->
<Style Selector="Views|RowListTree:selected">
<Style Selector="Views|RowListTree:pointerover:not(:selected):not(:dimmed)">
```
```csharp
// Before, FrameListTreeCard.ShowMarks
PseudoClasses.Set(HoveredClass, marks.IsHoveredCard(node));
PseudoClasses.Set(SelectedClass, marks.IsSelected(node));
PseudoClasses.Set(InSelectionClass, marks.IsInSelection(node));
PseudoClasses.Set(DropTargetClass, marks.IsDropTarget(node));
// After
PseudoClasses.Set(DropTargetClass, marks.IsDropTarget(node));
```

The members of finding 6 lose their last reader in this step. The subagent asks the owner before it removes them, with the list.
The tests that pin the card hover and the card selection change to the row: the subagent lists each one in its report.

Tests: `ListTreeHoverAndSelectionTests`, `ListTreeSurfacesTests`, `ListTreeMultiSelectionTests`, `ListTreeDragTests`,
`ListTreeBlockDragTests`, `CategoryPaletteTests`.

Commit: `Marks the hover and the selection on the tree row only`.

#### Step 2.3 The full width surface of a row with a state (complexity 2, Opus)

Files: `Views/ListTree/RowListTree.axaml`, the row look tests.

`Border#band` is transparent on a row with a state. The Background of the row, full width, carries the surface. A cursor row with no
other state gets `ListTreeRowRestBandBrush` as its Background.

```xml
<!-- Before -->
<Style Selector="Views|RowListTree:pointerover Border#band, Views|RowListTree:selected Border#band">
<!-- After -->
<Style Selector="Views|RowListTree:pointerover Border#band, Views|RowListTree:selected Border#band,
                 Views|RowListTree.cursor Border#band, Views|RowListTree.inspected Border#band">
    <Setter Property="Background" Value="Transparent" />
</Style>
<Style Selector="Views|RowListTree.cursor:not(:pointerover):not(:selected):not(.inspected)">
    <Setter Property="Background" Value="{DynamicResource ListTreeRowRestBandBrush}" />
</Style>
```

Tests: `Row_AtRest_HasTheBandFromItsIndent`, `Row_TheCursorRow_HasTheBandAtFullWidth`.

Commit: `Gives a tree row with a state its surface at full width`.

### Phase 3. The cursor and the inspected row

#### Step 3.1 The cursor frame and the drag class (complexity 3, Opus)

Files: `CMiX.UI/Themes/Tokens/Brushes.axaml` (or the file of the `ListTree*` brushes), `Views/ListTree/RowListTree.axaml`,
`Behaviors/ListTree/ListTreeRowDrag.cs`, `ListTreeGrab.cs`, the drag and grab tests.

New token `ListTreeRowCursorFrame` `#73FFFFFF` and its brush `ListTreeRowCursorFrameBrush`. The frame is full width, neutral, on a
selected row too. The view gets the class `dragging` while a row drag or a grab runs (R3, finding 3), and the frame hides under it.

```xml
<!-- Before -->
<Border x:Name="cursorFrame" Margin="{Binding IndentMargin, ...}" ... IsVisible="{Binding IsCursor}" />
<Setter Property="BorderBrush" Value="{Binding Converter={StaticResource RowTone}, ConverterParameter=KindSelected}" />
<Style Selector="Views|RowListTree:selected Border#cursorFrame"> <Setter Property="Opacity" Value="0" />
<!-- After -->
<Border x:Name="cursorFrame" ... IsVisible="False" />
<Style Selector="Views|RowListTree.cursor Border#cursorFrame">
    <Setter Property="IsVisible" Value="True" />
    <Setter Property="BorderBrush" Value="{DynamicResource ListTreeRowCursorFrameBrush}" />
</Style>
<Style Selector="Views|ListTreeProject.dragging Views|RowListTree Border#cursorFrame">
    <Setter Property="IsVisible" Value="False" />
</Style>
```
```csharp
// After, the start and the end of a row drag and of a grab
Context.Owner.Classes.Set(ListTreeProject.DraggingClass, true);
```

Tests: `CursorFrame_ASelectedCursorRow_ShowsTheFrame`, `CursorFrame_DuringARowDrag_Hides` (also a bright row),
`CursorFrame_DuringAGrab_Hides`, a test that reads the brush of the frame.

Commit: `Draws the cursor of the tree as a neutral frame that hides during a drag`.

#### Step 3.2 The inspected tint (complexity 3, Opus)

Files: `Themes/CategoryPalette.cs`, `Themes/ListTreeKindColors.cs`, `Converters/ListTreeKindBrushConverter.cs`,
`Views/ListTree/RowListTree.axaml`, `Views/Inspector/PanelInspector.axaml`, `CategoryPaletteTests`, the row look tests.

```csharp
// Before, ListTreeKindColors
public sealed record ListTreeKindColors(Color HoverBorder, Color DropOutline, Color SelectionBorder);
// After
public sealed record ListTreeKindColors(Color HoverBorder, Color DropOutline, Color SelectionBorder, Color Inspected, Color InspectedHover);
```
```csharp
// After, CategoryPalette
public const double ListTreeRowInspectedStrength = 0.18;
public const double ListTreeRowInspectedHoverStrength = 0.25;
```
```xml
<!-- After, RowListTree.axaml -->
<Style Selector="Views|RowListTree.inspected">
    <Setter Property="Background" Value="{Binding Converter={StaticResource KindMark}, ConverterParameter=Inspected}" />
</Style>
<Style Selector="Views|RowListTree.inspected:pointerover">
    <Setter Property="Background" Value="{Binding Converter={StaticResource KindMark}, ConverterParameter=InspectedHover}" />
</Style>
```

The inspected text and toggles use `ListTreeTextSelectedBrush`. The selection border stays on a selected and inspected row. The
header of `PanelInspector` takes the inspected tint of the inspected node as its background.

Tests: `ForListTree_AKind_GivesTheInspectedTints`, `Row_TheInspectedRow_HasTheInspectedTint`, `Row_ASelectedInspectedRow_KeepsTheSelectionBorder`,
`PanelInspectorHeaderTests` for the header.

Commit: `Tints the inspected row of the tree and the inspector header`.

Phase 3 end: the Studio suite once.

### Phase 4. Badges and documents

#### Step 4.1 The icon of a kind count (complexity 1, Sonnet)

Files: `ViewModels/Tree/KindCount.cs`, `BadgesKindCountTests`.

```csharp
// Before
public sealed record KindCount(TreeCategory Category, int Count)
// After
public sealed record KindCount(TreeCategory Category, int Count)
{
    public string IconKey => TreeIcons.OfCategory(Category);
}
```

Tests: `KindCount_ACategory_GivesItsIcon`, `OfCategory_EachCategory_IsAKeyOfTheIcons` (each key is in the icon set).

Commit: `Gives each kind count the icon of its kind`.

#### Step 4.2 The badge with the icon and the count (complexity 4, Opus)

Files: new `CMiX.UI/Controls/TextOutlined.cs` (R2), `CMiX.UI/Themes/Tokens/Geometry.axaml` (the offset and the outline width as
tokens, finding 8), `Views/Tree/BadgesKindCount.axaml`, `BadgesKindCountTests`, a CMiX.UI test of `TextOutlined`.

```xml
<!-- Before: a square of the kind colour at 60 % with the number -->
<Border Classes="kindcount" Height="{DynamicResource TreeKindSquareSize}" ...><TextBlock Classes="count" Text="{Binding Count}" /></Border>
<!-- After: the kind icon, the count at its bottom right -->
<Panel Width="{DynamicResource IconSizeXS}" Height="{DynamicResource IconSizeXS}">
    <ui:AppIcon IconKey="{Binding IconKey}" Foreground="{Binding Converter={StaticResource TreeTone}, ConverterParameter=KindSelected}" />
    <ui:TextOutlined Classes="count" Text="{Binding Count}" HorizontalAlignment="Right" VerticalAlignment="Bottom"
                     Margin="{DynamicResource TreeKindCountOffset}" OutlineThickness="{DynamicResource TreeKindCountOutline}" />
</Panel>
```

The "+n" badge and the missing sign keep their place. The spacing between two badges is 8 px from a token. The subagent shows the
plan of `TextOutlined` (its properties and its render) to the owner before it builds it.

Tests: `Badge_AKindCount_ShowsTheIconAndTheCountAtItsBottomRight`, `TextOutlined_ACount_DrawsTheOutlineThenTheFill`.

Commit: `Shows a kind count as the kind icon with its number`.

#### Step 4.3 Documents and the UI kit (complexity 1, Sonnet)

Files: `docs/tree-port-plan.md` (the cursor in principles 6 and 8), `ViewModels/ListTree/README.md` (the states), the status of this
plan, and one line under step 4.4 and section 7 of `docs/plans/tree-search-speed.md`: the match look moved to phase 5 of this plan. Run `docs/ui-kit/tools/Update-Tokens.ps1`. Tell the owner which UI kit entries are out of date.

Commit: `Describes the new row look of the tree`.

Phase 4 end: the Studio suite once.

### Phase 5. The search look (R4)

The source is the search field of the tree in the After look of `docs/ui-kit/mockups/inspector-with-tree.html`. The filter does not
change: `ListTreeSearchFilter` shows only the matches and their parents, open, as the Outliner of Blender.

#### Step 5.1 The dimmed parents of a search (complexity 2, Opus)

Files: `ViewModels/Tree/TreeNode.cs`, `Views/ListTree/RowListTree.axaml`, the row look tests.

A parent that shows only because it holds a match is dimmed, with the look of a dimmed row in a drag. A selected or inspected parent
keeps its own look. The class comes from a binding to the node, as in step 2.1, because the rows are reused.

```csharp
// After, TreeNode: true only during a search, because HasSearchMatchBelow is false outside a search
public bool IsSearchParent => HasSearchMatchBelow && !IsSearchMatch;
```
```xml
<!-- After, RowListTree.axaml -->
<Classes.searchparent><Binding Path="IsSearchParent" /></Classes.searchparent>
<Style Selector="Views|RowListTree.searchparent:not(:selected):not(.inspected) Border#band">
    <Setter Property="Background" Value="{DynamicResource ListTreeRowDimmedBandBrush}" />
</Style>
```

The text, the icon and the toggles of a search parent take `ListTreeTextDimmedBrush` and `ListTreeIconDimmedBrush`, with the same
selectors as `:dimmed`. `IsSearchParent` raises when `IsSearchMatch` or `HasSearchMatchBelow` changes. The name is a question for the
owner before the step. Other candidates: `IsShownForAMatch`, `IsMatchParent`.

Tests: `Row_AParentOfAMatch_IsDimmed`, `Row_AMatch_KeepsItsNormalLook`, `Row_ASelectedParentOfAMatch_KeepsTheSelectedLook`.

Commit: `Dims the parents that a search of the tree shows for a match`.

#### Step 5.2 The matched letters (complexity 4, Opus)

Files: to be decided by the report of the step: `CMiX.UI/Controls/TextBoxEditable` or a new part of it, `Views/ListTree/RowListTree.axaml`,
`ViewModels/Tree/TreeNode.cs`, `CMiX.UI/Search/SearchRules.cs` (the range of a match).

Only the matched letters of the name take `AccentHighlightBrush`. The rest of the row keeps its normal look.

First the subagent reads how the name is drawn (`TextBoxEditable` for an item, `TextBlock` for a group, finding 10) and reports one
way to draw runs of text, with a short before and after sample, then asks the owner. Two ways to compare in the report:

| Way | For | Against |
|---|---|---|
| `TextBlock.Inlines` with a `Run` for the match, shown while the name box does not edit | Plain Avalonia | Two controls for one name: the text and the edit box |
| A highlight range on `TextBoxEditable` (a start and a length, drawn under or over the text) | One control, also for the pickers of CMiX.UI | A new feature of a shared control |

The range of a match comes from one place, next to the rule:

```csharp
// After, SearchRules
public static int IndexOfMatch(string? query, string? text) => ...;   // -1 when there is no match
```

`SearchRules.Matches` stays as it is, so the four search fields keep their rule.

Tests: `IndexOfMatch_APartOfAWord_GivesItsStart`, `Row_AMatch_DrawsTheMatchedLettersInTheHighlight`, a test that reads the brush of
the run or of the range.

Commit: `Draws the matched letters of a tree search in the highlight colour`.

Phase 5 end: the three suites once.

## 5. Later

- The new inspector of `docs/ui-kit/mockups/inspector.html`.
- The matched letters in the pickers of CMiX.UI (`ListGroupedSearch`, `ListSearchableItemPicker`), if step 5.2 takes the way of
  `TextBoxEditable`.

## 6. Checks in the Studio after the plan

- The rows are 26 high, and each icon has the same space on each side.
- A hover or a selection marks the row only, also on a card header. The children of a selected card stay at rest.
- The cursor is a thin white frame at full width, also on a selected row, and it hides during a drag and a grab.
- The inspected row has the tint of its kind, and the inspector header has the same tint.
- A closed card shows the icon of each kind with its count at the bottom right.
- A search shows the matches in their normal look with the matched letters in the highlight colour, and their parents dimmed.
