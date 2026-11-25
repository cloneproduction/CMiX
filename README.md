# CMiX

CMiX is a realtime graphic composition tool.

## Requirements

- Windows and .NET 10
- A Redis or Memurai server
- vvvv gamma for the engine in `CMiX.Engine`. vvvv has its own license: https://vvvv.org/licensing

## Build

    dotnet build CMiX.Studio.Avalonia\CMiX.Studio.Avalonia.csproj

The notes for developers are in [docs/DEVELOPMENT.md](docs/DEVELOPMENT.md).

## Distribution

The Fallout pipeline in `build/Build.cs` publishes the Studio for distribution. The script `build.ps1` restores the tool and runs it. The pipeline needs the .NET 10 SDK. The targets `Pack` and `Push` also need Chocolatey.

Targets:

- `Clean`: empties `artifacts`.
- `Publish`: publishes the Studio to `artifacts/CMiX.Studio` and copies the settings file over `studio-settings.json`.
- `Zip`: zips the folder to `artifacts/CMiX.Studio_<version>_win-x64.zip`. This is the default target.
- `Pack`: builds the Chocolatey package in `artifacts` from the zip.
- `Push`: pushes the Chocolatey package to a feed.

This command builds `artifacts\CMiX.Studio` and the zip:

    .\build.ps1

This command ships your settings file as `studio-settings.json`. Without the parameter, the file in `CMiX.Studio.Avalonia` ships. Keep a file with project values outside the repository.

    .\build.ps1 --settings-file C:\path\to\studio-settings.json

This command adds the Chocolatey package:

    .\build.ps1 Pack

This command pushes the package. Fallout also reads both values from the environment variables `FEED` and `API_KEY`.

    .\build.ps1 Push --feed <url> --api-key <key>

The file `version.txt` holds the version of the zip, the package and the executable. The package installs the Studio into `C:\tools\CMiX.Studio` and adds a desktop shortcut. The Studio needs the .NET 10 runtime (package `dotnet-10.0-runtime`). An upgrade replaces `studio-settings.json` in that folder with the shipped file.

## Project files

The Studio saves a project as a `.cmix` file. The file stores the path of every asset relative to its own folder. A project and its assets move together to another machine when the folders between them stay the same. This example has a project and an asset:

    VJ\compositions\bar.cmix
    VJ\textures\foo.jpg

The file stores the asset as `..\textures\foo.jpg`.

An asset on another drive than the project file keeps its absolute path. A project file from an older version keeps its absolute paths until the next save.

The emergency save in the temp folder holds paths relative to that folder. Open it from there and the paths work. Copy the file to another folder before you open it and the paths break.

The start check reports "Not in sync" when you open a project on a machine other than the one that pushed the Redis snapshot, because the absolute paths differ. Click Push on that machine.

A project file saved before version 1.0.0 stores the Gamma of a gradient, the Frequency and Contrast of a bubble noise and the Cell Count of a checker board as plain values. `ModulatableValueUpgradeJsonConverter` reads that shape into the modulatable shape when the file opens, and the next save writes the new shape. Remove the class, its line in `ProjectSerializer.Options`, its test class and this note when no project file from before that version needs to open.

## Project tree

The side panel shows the tree in its entry Tree. The view is `ListTreeProject`, a flat list of rows. `CMiX.Studio.Avalonia/ViewModels/ListTree/README.md` describes the view and its drag.

The tree shows the project as rows. The one row at the top is the group Compositions, and the compositions are under it. A composition holds the group Layers (with its layers under it), then its Filters and Modifiers. A layer holds Entities, Filters, Modifiers, Cameras and Lights. An item can hold textures, filters and modifiers. They are groups under the item. The modulators are not in the tree: the inspector of their owner shows and adds them. The groups Compositions and Layers have a hide button like the other groups, and they have no panel in the inspector yet. A layer dropped on a composition goes to the end of its Layers group.

Each row has a kind: composable (a composition and a layer), entity, camera, light, texture, filter or modifier. A group has the kind of its items. The kind sets the color of the row.

- The colors of the kinds are in `Colors.axaml`, with the names `Category` and the kind. A composable inside a composable has one more color for each level, `CategoryComposableLevel1` to `CategoryComposableLevel3`.
- A row is filled with its color mixed with the gray of the panel. The fill under the pointer is as bright as the fill of a selected row, with no border and a white name. A selected row also has a border of one pixel of the screen in the bright color of its kind, and its name is white. A group and its items have the same tone. What a row holds of another kind is weaker by one step for each change of kind (`TreeNode.KindDepth`), down to a floor. `CategoryPalette` holds the strength of each step, and a test checks the contrast of the text on every kind, level and state.
- The fill of a row starts at its indent, so no fill touches the left border of the tree. The space between two rows is 3 pixels (the token `ListTreeRowGap`). The space between a row and its toggles stays 1 pixel.
- A collapsed row shows what it holds: a small square for each kind, in the color of the kind, with the number of items. An open row shows none. A new row, made by the user, starts open, and the rows above it open too. When a project opens, every row starts collapsed.
- The toggles are icons inside the colored row, at its right end, with no fill behind them. The mask toggle is the first, on a composition and a layer. The visibility toggle is the last, at the right end. A place without a toggle (a group, or the mask of an entity) stays empty, so the icons of every row line up. A new toggle is a new slot in `TogglesRow`. A group row (Entities, Filters, Cameras and so on) has a hide button in the place of the visibility toggle. It hides the whole list in the engine (`PrefabManager.Hidden`, applied in `VisibleItemsProcessor`). The items keep their own visibility, so showing the list again brings back what was shown. While a list is hidden, the visibility toggles of every row below it look greyed, as if disabled, but they still work (`TreeNode.IsUnderHiddenParent`). The state is saved in the project file (`PrefabManagerModel.Hidden`) and sent to the peers; a file without it opens with the list visible.
- The title bar of the inspector has the color of the kind of the selected row. It shows the icon of the row, then its name. The name of an item can be edited there. At the right end are, for a composition or a layer, the mask toggle, then the visibility toggle. A group shows its name only.
- The tabs beside the tree behave as normal tabs: a click on a row of the tree does not change the selected tab. Every icon has the one size `IconSizeXS`, also in the menus.
- The tooltips of the rows of the tree (the eye, the mask, the hide button of a group, the numbers of a collapsed row) are opened by the tree itself (`TreeTooltips`), not by the tooltip service of Avalonia, which did not open them in the running app. The tooltips of the toolbar still use the service.
- While a row is dragged, the rows slide to the places of the moved tree, and the dragged row sits at its landing place with a full width surface. One line under the rows says where the row goes (`ListTreeDragSession`).
- A row at rest is a faint band from its indent. A card (a row with rows under it) shows its hover and its selected surface on its whole area (`FrameListTreeCard`).
- Every search field is a `TextBoxSearch`. A drop-down gets one with `ComboBoxSearch.IsSearchable`, or `IsSearchable` on a `ComboBoxCaptioned` (the blend modes use it). The field filters the rows, and Enter picks the first one left.
- Every panel (the tree, the lists, the inspector and the tabs) shows its scroll bar over the content (`ScrollViewerOverlay`). The content keeps a gutter for the bar, so it does not move when the bar appears. A tab view in the inspector does not scroll on its own: the inspector scrolls it.

While you drag a row, the rows where it can land and their direct children stay bright: for a modifier the groups Modifiers, for a filter the groups Filters, for a layer the compositions. Every other row dims to a readable grey. The drop target has one outline in its kind colour (`FrameListTreeOutline`). A row dropped on a card goes to its list of that kind, so an entity dropped on a layer goes to its Entities. The pointer picks the nearest place, and a move of 24 pixels to the right or to the left is one level (`ListTreePick`). A row drag is a pointer drag of the tree, not a drag of the system: the tree captures the pointer, so the mouse wheel scrolls the tree during the drag, Escape ends the drag without a drop, and the cursor is the move cursor (`ListTreeRowDrag`). The tree scrolls while the pointer of a drag is near its top or bottom edge, and faster when it is closer to the edge. A file drag from the file manager is a drag of the system; it scrolls the tree by the edge zone when the tree takes the files. A rest of half a second next to a closed card, or the key E, opens the card. E closes again a card that this drag opened. A tree that is higher than its view has room under its last row, so its last row can reach the top of the view. The places come from the rules of the drop itself (`TreeDropService`), so a bright row always takes the drop. A row that takes the kind of the dragged row but refuses it by a rule (another layer of the composition also holds the entity, or the owner cannot hold the modifier) dims, takes no drop, and its tooltip gives the reason.

A collapsed row that leads to a place for the dragged row opens when the pointer rests on it for 0.6 s, so you can reach a place deep in the tree. `Tree.Expand` (E) during a drag opens the row under the pointer at once, and a second E closes it again. A row that was open before the drag stays open. At the end of the drag, the rows that the drag opened close again. After a drop, the row that took the drop and its parents stay open, so you see where the row went. Escape ends the drag without a drop, also when the search box has the focus, and closes every row that the drag opened.

A row can move inside its list. It can also move to a list of the same kind of another owner: an entity to another layer, a modifier to another entity, a filter or a texture to another owner of the same kind of list.

### Adding to the tree

One panel adds things under a row. It has a search field, then the groups that the row can take, in this order: Layer, Content, Texture, Filter, Modifier and Project. An entry that the row cannot take is not in the panel, and a group with no entry is not in it either. A layer takes content (entities, fullscreen quads, text, cameras and lights), filters and modifiers. An entity takes textures and modifiers. A group of rows takes only its own kind. An entry has a small square in the color of its kind, and a group does not show a number. A group with many rows starts closed, and the search opens them. A long list has a scroll bar. The menu of a row shows Add, Duplicate, Rename, the moves and Remove with an icon each. A row that cannot do one of them shows it greyed, and a group row shows all of them greyed.

The panel opens in three ways:

- The Add row of the menu of a row. The panel opens to the right of the menu, with its search field on the Add row.
- The Add button of the toolbar over the tree. The panel opens with the middle of its search field under the pointer and names the target row. A click from the keyboard opens it under the button. With nothing selected, the panel offers a composition.
- The shortcut `Tree.Add` (Shift+A) opens the Add panel of the target row with the middle of its search field on the middle of that row, with the title Add. With nothing selected, the panel opens under the pointer and offers a composition. It works on the condition of every tree shortcut (see Keyboard shortcuts). The panel shows every category open: a category is a name, with no expander (`ListGroupedSearch.Collapsible`).

A right click opens the menu of a row with its Add row under the pointer, and puts the target on that row, also when the row is already selected. After the user picks another row of a menu, such as Duplicate, the next menu opens with that row under the pointer. A key opens the menu under the row, at the left edge of its name: `Tree.Menu` (W) opens the menu of the target row, and the Menu key opens the menu of the row with the keyboard focus.

Every panel ends with the group Project, so a composition can be added under any row. The search field has the focus when the panel opens. Down and Up move over the rows, Enter picks a row, and Escape clears the text and then closes the panel.

### Searching the tree

The search field is always shown while the panel shows the tree: it is in the bar of the panel, at the right, on the line of the button that picks what the panel shows. `Tree.Search` (Ctrl+F) puts the focus in the field and selects its text. It works on the condition of every tree shortcut (see Keyboard shortcuts). A row matches when its name or its kind holds the text, with no regard to case, so a renamed Echo filter is found by "Echo". While the field holds text, the tree shows only the matches and their parents, and a match has a mark. A cross inside the field at its right empties it.

The parents of the matches show open, but the search does not open or close any row. When the text is empty again, every row shows its own open state again. The selection does not change. The parents of the selected row and of the cursor row open then, and the tree shows the cursor row, so you can go on with it. With no match, the tree shows the line "No match".

Enter in the field moves the cursor row of the tree to the next match, and Shift+Enter to the match before. At the last match, the next one is the first. The selection does not change, and the focus stays in the field. Down puts the focus in the tree on the cursor row, and Enter there selects the row. Escape clears the text, and Escape in an empty field gives the focus back to the rows.

Text in the field means that the search is on. Down, a click on a row or a move to another panel keeps the text, and the tree stays filtered. While the focus is not in the field, its border has the accent color, and with the focus in the tree the hint strip shows "Esc clear search" and "Ctrl+F edit search". Escape in the tree clears the search: the focus and the cursor row stay, and the parents of the cursor row stay open. A grab, an open menu and a rename take Escape first. With no search, Escape in the tree does nothing. `Tree.ClearSearch` (Alt+F) clears the search from anywhere and puts the focus on the cursor row.

There is no search during a drag of a row. The box takes no text and Ctrl+F does nothing. During a search, no row drag and no grab starts, but you can still drop a file on the shown rows.

A tooltip is a dark card with a tip that points at the pointer. It shows a title and, for the buttons of the toolbar, the shortcut. A disabled button says why in its tooltip.

## Keyboard shortcuts

The Studio reads its keyboard shortcuts from `%AppData%\CMiX\studio-shortcuts.json`. This file is separate from `studio-settings.json`. When the file is missing, the Studio writes it with the defaults. When the Studio cannot write it, the Studio starts with the defaults.

The file maps a command name to a gesture:

    {
      "Shortcuts": {
        "Edit.Undo": "Ctrl+Z",
        "Tree.Rename": "Ctrl+R"
      }
    }

| Name | Default | Action |
| --- | --- | --- |
| `Edit.Undo` | Ctrl+Z | Undo |
| `Edit.Redo` | Ctrl+Shift+Z | Redo |
| `Edit.RedoAlt` | Ctrl+Y | Redo |
| `File.Open` | Ctrl+O | Open a project file |
| `Window.NextPanel` | Ctrl+Tab | Put the focus in the next panel |
| `Window.PreviousPanel` | Ctrl+Shift+Tab | Put the focus in the previous panel |
| `Window.FocusPanel1` to `Window.FocusPanel4` | Ctrl+1 to Ctrl+4 | Put the focus in panel 1 to 4. A panel that is not there does nothing |
| `Tree.Focus` | none | Put the focus on the cursor row of the tree |
| `Tree.Add` | Shift+A | Open the Add panel on the target row |
| `Tree.Menu` | W | Open the menu of the target row under the row |
| `Tree.Expand` | E | Expand or collapse the target row |
| `Tree.CollapseOthers` | Alt+E | Collapse others: open the parents of the target row and close every other row |
| `Tree.ExpandBranch` | Shift+E | Open the target row and every row under it |
| `Tree.CloseLevel` | Q | Close the deepest open level of the rows that show |
| `Tree.OpenLevel` | Shift+Q | Open the shallowest closed level of the rows that show |
| `Tree.CollapseAll` | Shift+Alt+E | Close every row of the tree |
| `Tree.Search` | Ctrl+F | Put the focus in the search field of the tree |
| `Tree.ClearSearch` | Alt+F | Clear the search of the tree and put the focus on the cursor row. It works from anywhere, also from a text box, while a search is active and no row is dragged or grabbed. The main menu has no access key on F for this reason |
| `Tree.Rename` | Ctrl+R | Rename the target row |
| `Tree.Remove` | Delete | Remove the target item. A notification at the bottom of the window shows "Removed" and the name, with the `Edit.Undo` key |
| `Tree.Duplicate` | Shift+D | Duplicate the target item |
| `Tree.MoveUp` | Alt+Up | Move the target item up |
| `Tree.MoveDown` | Alt+Down | Move the target item down |
| `Tree.Open` | Enter | Select the row that has the keyboard focus |
| `Tree.Reveal` | F | Reveal the selected row: open its parents, put the cursor on it and scroll it to the middle of the tree |
| `Tree.Hide` | H | Hide or show the target row, as a click on its eye. On a group row, hide or show the whole list. A notification shows "Hidden:" or "Shown:" and the name, with the `Edit.Undo` key |
| `Tree.ShowAll` | Alt+H | Show every hidden row of the tree as one undo step. A notification shows "All rows shown" with the `Edit.Undo` key. With no hidden row, nothing changes |
| `Tree.Grab` | G | Grab the target row, as G in Blender: the row follows the pointer at once, with no pressed button. The arrow keys move the mark to the next valid place; after a key, the pointer takes the mark back when it moves more than 14 px. A left click or Enter puts the row down, one undo step. A click where the row cannot go does nothing. A right click, Escape, a focus in another panel or another window ends the grab with no move |

The part of a name before the dot is its scope. Two names of one scope must not have the same gesture. The `Edit`, `File` and `Window` keys work in every scope, so their gestures must not be the gesture of any other name. An entry with an unknown name, or with a gesture that does not parse, is ignored and the default stays. An empty gesture removes the shortcut. In the file, a digit key is `D1` to `D9` and `D0`, for example `"Ctrl+D1"`.

The panels are the side panels of the window, from left to right: panel 1 is the left panel. Each panel keeps the control that had the focus last, and the focus goes back to that control. When no control of the panel had the focus, the focus goes to the cursor row of the tree, or to the first control of the panel. The panel keys also work while a text box has the focus. The accent line under the bar of the panel shows the panel with the focus. `Tree.Focus` has no default gesture, because Ctrl+1 to Ctrl+4 go to the panels. Set one in the Shortcuts section of the Settings tab.

Every tree shortcut but `Tree.Open` and `Tree.ClearSearch` has one condition. The keyboard focus comes first: the shortcut works while the tree has the focus, with the pointer anywhere. It also works while the pointer is over the tree and no other panel has the focus, for example after a click on an empty part of the window. While a control of another panel has the focus, the pointer over the tree is not enough: click the tree first. No tree shortcut works while a text box has the focus. No tree shortcut works while a row is dragged, but `Tree.Expand`, which then opens or closes the row under the pointer. `Tree.Open`, Enter and the arrow keys need the focus in the tree. The file holds every default since its first save, so a default of an earlier version that is still in the file gives the default of now: a `Tree.Duplicate` of Ctrl+D becomes Shift+D, and a `Tree.Rename` of F2 becomes Ctrl+R.

The Shortcuts section of the Settings tab edits the shortcuts and saves each change to the file at once, and a missing file is created on save. A red dot marks a gesture that two commands of one scope have, and its tooltip names the other command.

A hint strip under the rows of the tree shows the useful keys of the current mode: the rows (Arrows move, Enter select, E open, W menu, Shift+A add, Ctrl+Z undo), the search field (Enter next, Shift+Enter previous, Down to tree, Escape clear) and an open menu (Escape close). It shows the gestures of the shortcut file, so a changed key shows correctly. The strip takes no click and no focus. The toggle "Show key hints under the tree" in the Interface section of the Settings tab hides it. The Studio saves the toggle at once in `studio-settings.json`, as `Interface.ShowKeyHints`.

While a text box has the focus, the text box gets the keys first. In the tree, the shortcuts do nothing while a name is edited.

In the tree, the arrow keys, Page Up, Page Down, Home and End move the keyboard focus from row to row. Left collapses a row or goes to its parent. Right expands a row or goes to its first child. The row with the focus has a thin border in the bright color of its kind. The keyboard focus does not select the row: it writes no undo step and sends nothing to the peers. Enter or a click selects the row. The target row is the row of the arrow keys, else the selected row. The tree shortcuts and the toolbar buttons act on the target row.

`Tree.CollapseOthers` (Alt+E) keeps the path to the target row open. Its parents open and every other row closes. The row keeps its own state. The row stays at its place on the screen, or the tree scrolls to it when it was out of view.

`Tree.CloseLevel` (Q) and `Tree.OpenLevel` (Shift+Q) act on the whole tree, one level at a time. Q closes every open row at the greatest depth among the rows that show. Shift+Q opens every closed row at the smallest depth among the rows that show. `Tree.ExpandBranch` (Shift+E) opens the target row and every row under it. `Tree.CollapseAll` (Shift+Alt+E) closes every row. A row that closes over the cursor row takes the cursor. These keys write no undo step and send nothing to the peers.

After a rename or a move by key, the keyboard focus goes back to the row. A click on another control keeps the focus where you clicked.

## Redis addresses

The Settings tab holds the list of Redis addresses of the project. Click an address to edit it. The buttons at the side add, remove and move addresses. The project file stores the list.

The Redis Float modulator selects one address of the list in its combo box. It has one float output, `Value`. Assign a value to it in the same way as to any other modulator output. The row of the assign menu shows the address text, or `Value` when the modulator selects nothing. A change of the selection keeps the assignments. The Reset menu of the combo box clears the selection. When you delete the selected address, the selection clears and the values bound to the modulator go back to normal in the same undo step.

Each Redis Float modulator has one range with the start values 0 and 1. The vvvv side maps the Redis value of the selected address into the range. The range stays when you select another address.

The Redis Bool modulator selects an address in the same way. It has one bool output, `Value`, for a bool such as the Play or the Do Seek of a player. It has no gain. The vvvv side decides how the Redis value becomes a bool.

The Redis Int modulator selects an address in the same way. It has one int output, `Value`, for a whole number such as the Seek Target of a player or the Count of a modifier. It has no gain. The vvvv side decides how the Redis value becomes an int.

The list has a fixed manager ID, `ManagerIDs.RedisAddressManager`. The vvvv side creates the list at startup with this ID.

## FFT modulator

The FFT modulator reads one track of the FFT signal. The track index selects the track. It has four float outputs, `Bass`, `LowerMid`, `HigherMid` and `High`. Each band has one range with the start values 0 and 1. The vvvv side maps the band value of the track into the range of the band.

## Hidden types

A class with the attribute `[HiddenInMenu]` is not in the add menus. A saved project can still hold it, and it still works. To show the class again, remove the attribute.

These classes are hidden now: `RenderRandomEntityModifier`, `RenderSequenceEntityModifier` and `BillboardModifier`.

## Design research

`docs/ui-kit/` holds the HTML copy of the UI and the design mockups. The pages load the kit files, so open them through a local server, not as files:

```
python -m http.server 8765 --bind 127.0.0.1 --directory docs/ui-kit
```

Then open `http://127.0.0.1:8765/index.html`. This page is the menu of the UI kit and of every mockup, for a presentation:

- The menu on the left shows the pages by group: UI kit, tree research, modulation research.
- Arrow down or right opens the page after. Arrow up or left opens the page before.
- P or the Present button hides the menu.
- Each page has its own address, for example `index.html#nested-boxes`.
- Open alone opens the page in a new tab.

A click in a page gives the keys to the page. Click the menu to use the keys of the menu again.

The research of the modulation graph is in `docs/modulation-graph-research.md`.

## License

CMiX is free software under the [GNU LGPL, version 3 or later](LICENSE). The GPL text is in [COPYING](COPYING).

## Credits

Created by CloneProduction

Sponsored by Refik Anadol Studio
