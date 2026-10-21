# CMiX development notes

CMiX is a realtime graphic composition tool. It pairs a desktop Studio application, used
to build and control compositions live, with a vvvv gamma engine that renders
the output. The Studio and the engines exchange their state through a Redis
server, so composing and rendering can run on separate machines.

## Repo layout

- CMiX.Core: shared domain model and services (compositions, layers, prefabs,
  networking, undo) used by both the Studio and the engine.
- CMiX.UI: the controls, styles and helpers of the UI that do not need the
  model. It does not reference CMiX.Core. See "The UI library" below.
- CMiX.Studio.Avalonia: the Studio UI, built on Avalonia. It holds the views of
  the model and uses CMiX.UI for the generic controls. This is where active UI
  work happens.
- CMiX.Engine: the vvvv gamma patch that renders the composition output. The
  patch references CMiX.Core with a project dependency, so vvvv builds the
  project and gets its packages from it.
- CMiX.Console: a headless engine. It joins the sync and applies the updates,
  without a window.
- CMiX.Core.Tests, CMiX.UI.Tests and CMiX.Studio.Avalonia.Tests: automated
  tests for the shared model, the UI library and the Studio views respectively.

## Build

    dotnet build CMiX.Studio.Avalonia\CMiX.Studio.Avalonia.csproj

## Test

    dotnet test CMiX.Core.Tests\CMiX.Core.Tests.csproj
    dotnet test CMiX.UI.Tests\CMiX.UI.Tests.csproj
    dotnet test CMiX.Studio.Avalonia.Tests\CMiX.Studio.Avalonia.Tests.csproj

## Values, defaults and reset

The default of a value lives in its model, never in the control. A model holds
each value as a named property with its default. For example, BlurModel has
`Strength = ModulatableValueModel<float>.Of("Strength", 0.5f)`.

`ControlFactory.Create` builds a new control, loads a default model into it, and
calls `SetDefaults()` on every new value. This takes the loaded value as the
value that Reset restores. Then it loads the real model.

To add a value to a filter or a modifier:

1. Add a named property with its label and default to the model.
2. Add the value to `Bindables` in the constructor. Add a line for it in
   `ToModel` and in `FromModel`.
3. In a test, build the control with `ControlFactory.Create(typeof(X))`. Do not
   use `GetRequiredService<X>()`: a control that DI builds has no default.

`FilterAndModifierRoundTripTests` fails when a value is missing in `ToModel` or
`FromModel`. `ResetMenuTests` (Studio tests) fails when an editor in a view has no
working Reset menu. `ValueResetMenuTests` (CMiX.UI.Tests) checks the editors alone.

A value editor shows a Reset menu only when it has a `ResetCommand`. The menu
does not open in edit mode. It does not open for a right-click in a popup below
the editor.

The editors with a Reset menu are `SliderCMiX`, `DragValue`,
`ToggleButtonCaptioned`, `ComboBoxCaptioned` and `ToggleButtonCMiX`. Use
`ToggleButtonCMiX`, not `ToggleButton`, for a toggle that edits a model value.
Bind the `ResetCommand` of an editor to the `ResetCommand` of the value.
`IntegerValue` is a `DragValue` for whole numbers: it starts with `IsInteger` on and
steps of 1. It takes the same bindings as `DragValue`:
`Value="{Binding Name.Value}"` and `ResetCommand="{Binding Name.ResetCommand}"`.
An editor that is one part of a group (a vector) also takes a `ResetAllCommand`, and
its Reset menu then has a Reset All row. `ValueBindingTests` fails when a
`DragValue` or an `IntegerValue` in an AXAML file has no `Value` binding.

## Views and folders

`ViewModelToViewTemplate` finds the view of a model by name. The view has the type
name of the model, for example `Blur` for the model `Blur`. It looks at the
controls in the namespace `CMiX.Studio.Avalonia.Views` and in its child
namespaces. The child namespaces `Controls`, `Managers`, `Menus`, `Modulation` and
`Windows` hold support controls, so the locator skips them. Two views with the same name
make the locator throw, and the message names both types.

The domain views keep the one namespace `CMiX.Studio.Avalonia.Views`, whatever the
folder. A view can move to another folder without a change to its namespace or its
`x:Class`. The folders are:

- `Views/Modulation`: the modulator views, and the modulatable value editors
  (`DragValueModulatable`, `SliderModulatable`, ...) with the modulator assign button.
  The editors and the button are support controls. Their namespace is
  `CMiX.Studio.Avalonia.Views.Modulation`, which the locator skips.
- `Views/Modifiers`: the modifier views. The subfolders `Camera`, `Color`, `Layer`,
  `Material`, `Text` and `Transform` follow `CMiX.Core/Modulation/Modifiers`.
- `Views/Controls`: the Studio controls that are not views, for example the
  Outliner. The generic controls are in CMiX.UI.

The value editors start a drag through `InteractionHost`. The App connects it to
`ValueInteraction` in `App.Initialize`.

## The UI library

`CMiX.UI` holds what does not need the model: the value editors, the color picker,
the text inputs, the panels, the popups, the attached properties, the behaviors,
the converters and the theme. It does not reference `CMiX.Core` or the Studio.
`LibraryBoundaryTests` in CMiX.UI.Tests fails when it does.

A type belongs in the library when it uses no Core type and has no domain name. A
control binds to its model by property name only, for example `Value`, `Caption`
and `ResetCommand`.

An application uses the library in two steps:

1. Add `<ui:CMiXTheme />` to `Application.Styles`, after the base theme. The theme
   holds the tokens (colors, sizes, brushes, converters) and all the styles.
2. In AXAML, declare `xmlns:ui="https://github.com/cloneproduction/CMiX.UI"`. This
   one name maps all the namespaces of the library.

The folders and namespaces follow each other. The top folders are `Controls`,
`Panels`, `AttachedProperties`, `Behaviors`, `Converters`, `Interaction`,
`Mathematics` and `Themes`.

`Controls` has one folder for each type of data that an editor changes. The names
come from the value editors of vvvv:

| Folder | The data | In it |
|---|---|---|
| `Number` | one number | `DragValue`, `SliderCMiX`, `NumericInput`, `NumericTextHelper`, `IntegerValue`, `RangeValue`, `NumbersOnly` |
| `Vector` | several numbers | `Vector2`, `Vector3`, `Integer2`, `Integer3`, `AxisSelector2`, `AxisSelector3` |
| `Boolean` | on, off, or a press | `ToggleButtonCaptioned`, `ToggleButtonCMiX`, `ButtonCaptioned` |
| `Enum` | one item of a fixed list | `ComboBoxCaptioned` |
| `String` | text or an address | `TextBoxEditable`, `EditableValue`, `IPBox`, `SelectOnFocus`, `TextBlockTrimmed` |
| `Path` | a file or a folder | `PathSelector` |
| `ColorPicker` | a color | `ColorSelector`, `ColorPickerFull`, `ColorPickerWheel`, `ColorPickerSliders` |
| `Collection` | a list of items | `PanelItem`, `PanelItemList`, `ListItemIcon`, `ListBoxDetail`, `ListBoxEditable` |
| `Parts` | nothing: what every editor is made of | `ContentCaptioned`, `UserControlCaptioned`, `ValueMenu`, `ContextMenuGuard`, `DragEditHelper`, `PopupFrame` |

`AppIcon`, `ListBoxCMiX` and `PopupButton` stay in the root of `Controls`. They do not change a value.

### Heights of lists and drop downs

A list or a drop down shows a whole number of rows, so no row is cut at the bottom, and the wheel moves it
by one row. The height of a row is the token `ControlHeightMD`. No file holds a height as a number. The
sizes come from the tokens in `Themes/Tokens/Geometry.axaml`:

- `ListBoxCMiX`: `MinItemCount` rows (the token `ListVisibleItems`) plus the top and the bottom of
  `ListScrollMargin`. The user can drag the strip under the list to make it taller. The height then goes by
  whole rows, and it never goes below `MinItemCount` rows. There is no largest height.
- `ListBoxEditable`: `MinItemCount` rows, or `ListEditableVisibleItems` rows when it is 0, the spaces of `ListEditableSpacing` between them, and
  `ListEditablePadding` around them. The list has no scroll viewer, so it grows with its items.
- `ComboBox`: `ComboBoxVisibleItems` rows.

The attached property `ScrollStep` does the work for a `ScrollViewer`: `ItemHeight` is the height of a row and
`VisibleItemCount` is the number of rows. The wheel goes on to the panel around the list when the list is at
its start or end. To change how many rows a list shows, change its token. A new list takes its numbers from
the tokens too.

### The scale of sizes

All the sizes of the UI come from one scale in `Themes/Tokens/Geometry.axaml`. No AXAML file holds a height as
a number: the test `HeightLiteralTests` fails for a new `Height`, `MinHeight` or `MaxHeight` that is a number.
It has a short list of exceptions, each with its reason, and the size of a `Window` is not checked.

XAML cannot add two tokens, so the tokens that follow `ControlHeightMD` are numbers, and `ScaleTokensTests` keeps
them in step with it:

| Token | Value |
|---|---|
| `ControlHeightXS`, `SM`, `XL`, `XXL` | `ControlHeightMD` minus 8, minus 4, plus 6, plus 10 |
| `IconSizeXS` and `IconSizeMD` | `ControlHeightMD` minus 8 |
| `IconSizeSM` | `IconSizeXS` minus 4 |
| `IconSizeLG` | `ControlHeightMD` |
| `TextAreaHeight2` and `TextAreaHeight4` | 2 and 4 times (`ControlHeightMD` minus 2) |
| `FontSizeMD` | about half of `ControlHeightMD` |

To make the UI bigger or smaller, change `ControlHeightMD` and move the other tokens of the table by the same
amount. The test tells you which one is not in step. Even numbers keep the text and the icons on whole pixels.
The text size comes from `FontSizeMD`, which the style in `Styles/TopLevel.axaml` gives to every window and popup.
The small parts that are not controls (`IndicatorSize`, `SplitterSize`, `ScrollBarSize` and the like) are separate
tokens and do not move with the scale.

### Panels of a list

The panels in `Collection` get their data only through their own properties. They never
read the DataContext, so they know no model. The application sets the properties in its
own XAML, for example `Title="{Binding PrefabService.Name.Value}"`. The test
`CollectionPanelContractTests` fails when a binding in these files reads the DataContext.

- `PanelItem` is an item of a list: a `Title`, a body, a toggle (`IsChecked`), a close
  button and a drag handle. `PanelItemList` is a list of them with an add button.
  `ListItemIcon` is a row with an icon, a name and a toggle. `ListBoxDetail` is a list
  with a delete button, an add area and the detail of the selected item.
- A variant of a panel is a theme, not a switch. The default theme of `PanelItem` has a
  body that collapses, an eye, a close button and a drag handle. An application writes
  its own theme from the library parts (`ToggleButtonPower`, `ToggleButtonVisibility`,
  `ButtonIconClose`, `ButtonDragHandle`) and sets it with `Theme=`. A theme that does not
  use a property just leaves it out.
- The list is reordered only when the application sets `ReorderService` on it.
- `ListBoxEditable` is a list of items of one type, with a column of buttons on its right: add and
  remove, then move up and move down. The application sets the
  `ItemTemplate`, so an item can be a `DragValue`, a `ColorSelector` or any other editor. A button
  shows only when its command (`AddCommand`, `RemoveCommand`, `MoveUpCommand`, `MoveDownCommand`)
  is set. `AddCommandParameter` is the parameter of the add command, and the other commands get the
  selected item. When `ReorderService` is set, each row has a drag handle. The `BeatStepModulator`
  uses it for its list of steps.
- A control that shows content of the application, such as `AddContent`, is a composed
  `UserControl` and not a templated control. A templated control takes over the template
  parent of the content that it shows in its template. Then a `TemplateBinding` inside that
  content, such as the owner type of the add panel of an entity, reads from the wrong
  control and finds nothing. `EntityModifiersSectionTests` keeps this case.
- A Studio control tells the library control which properties of the model to use.
  `ModifierManager` is a `PanelItemList` of `PanelItem` rows. `RepositoryManager` is a
  `ListBoxDetail` with the icon templates of the prefabs. `ModulatorHoverBehavior` tells
  a modulator that the pointer is over its panel, because the library panel knows no
  modulator.

Use `TextBlockTrimmed` instead of `TextBlock` for a name that can be long, such as the name
of a file, a folder or a prefab. The text stays on one line and ends with "..." when it does
not fit. The full text shows on hover, but only while the text is cut. `ToolTipText` replaces
the hover text, for example with the whole path of a file.

To place a new control, ask "what data does it change?". A curve editor changes a
curve, so it gets a new folder `Curve`, even when the first use is a color adjustment.
The folder follows the data, not the screen. The screen is a view in the Studio.

A folder name is also a namespace, so it must not hide a .NET or Avalonia type that
the code in `CMiX.UI.Controls` uses by its simple name. That is why the folder for
colors is `ColorPicker`, not `Color`: the code uses the Avalonia type `Color`. Inside
the folders `String`, `Boolean`, `Enum` and `Path`, write `string`, `bool`,
`System.Enum` and `System.IO.Path`, not `String`, `Boolean`, `Enum` and `Path`. The folder `TextInput` does not exist for the
same reason: a namespace `TextBox` would hide the Avalonia `TextBox`.

### Names of the controls

The name of a control starts with the standard control or the thing it is, and ends
with what makes it different: `StackPanelPositioned`, `ExpanderIndented`,
`ComboBoxCaptioned`. All the variants of a control then sit together in a list and
in the autocomplete. The Color, Popup and Drag families work the same way.

- The ending `CMiX` marks the default CMiX version of a standard control:
  `SliderCMiX`, `ListBoxCMiX`, `ToggleButtonCMiX`. It derives from the
  standard control and keeps its API. A variant with a special purpose gets its own
  word instead, as `ExpanderIndented` does. A later variant never needs a rename.
- A control with no standard equivalent has its own name: `DragValue`, `RangeValue`,
  `Vector2`, `ColorSelector`.
- The dimension is a digit: `Vector3`, `Integer2`, `AxisSelector3`. Never 2D, 3D, XY
  or XYZ.
- The kind stays at the end for converters (`...Converter`), for a real
  `Behavior<T>` (`...Behavior`) and for static helpers (`...Helper`). An attached
  feature has a plain name: `NumbersOnly`, `SelectOnFocus`, `DragHandle`, `ScrollStep`,
  `RoundedClip`. Its on/off switch is `IsEnabled`.
- The word `Captioned` is at the end of a name only when the control is the standard
  control with a caption added. `ButtonCaptioned` is a `Button` and
  `ToggleButtonCaptioned` is a `ToggleButtonCMiX`, so both keep the whole API of the
  standard control. `ComboBoxCaptioned` is a `UserControl` with a `ComboBox` inside, on
  purpose. It keeps its own `SelectedItem` and sends only real picks to the model.
  A plain `ComboBox` loses a selection that comes before its items, and it sends an
  empty selection to the model while its items are replaced.
  `ComboBoxCaptionedSelectionTests` keeps this protection.
- `UserControlCaptioned` is the base of the other controls that have a `Caption`.
- The word `Value` ends the names of the editors of one value: `DragValue`,
  `IntegerValue`, `RangeValue`, `EditableValue`. No standard control is called
  `Value`, so there is nothing to put first. These names stay as they are.
- In the Studio, the ending `Modulatable` marks a control that takes modulators. An
  editor has a modulator button: `DragValueModulatable`, `SliderModulatable`,
  `Vector2Modulatable`. Their base is `UserControlModulatable`. The button is
  `ButtonModulatorAssign`. A panel has a list of modulators: `PanelModulatable` is the
  base of the modifier views.
- `CMiXTheme` is the one name that keeps the brand in front, like `FluentTheme` in
  Avalonia.

`PathSelector` chooses a file or a folder: a list of known items, a Browse button and a
drop of files from the file explorer. It only finds the path and raises `PathChosen`
and `PathsDropped`. It does not know what the path becomes. The Studio has
`AssetPathSelector`, which turns the path into an asset of its repository. The drop
is done by `FileDropBehavior`, which any control can use. `TextureDropBehavior` of the
Studio is built on it.

### Names of the styles

A style (a `ControlTheme` with an `x:Key`) is named like a control: the control it
styles first, then what makes it different. `ButtonRounded`, `ToggleButtonInlineLeft`,
`ExpanderIndentedHeader`.

- No `Style` ending. The key is already a style.
- `...Default` is the default theme of a popup or menu type: `ContextMenuDefault`,
  `MenuFlyoutPresenterDefault`.
- `...CMiX` is the default CMiX theme of a standard control whose bare name would be
  the Avalonia type name: `ListBoxItemCMiX`, `TabItemCMiX`, `GridSplitterCMiX`.
- A part of a control ends with the part: `ExpanderItemHeader`, `SliderCMiXBrightnessThumb`.
- A style keeps no Studio word. The Studio concepts (layer, modifier, prefab) stay in
  the Studio styles.
- `ResourceReferenceTests` fails when an AXAML file names a resource key that nothing
  defines.

### Toggle buttons

A toggle shows that it is on in the way that fits its look. There are two families.

- A toggle with a surface (a box or a pill) turns its background to the accent color when it is on.
  These are `ToggleButtonRounded`, `ToggleButtonInlineLeft`, `ToggleButtonBox`, `ToggleButtonPopup` and
  the others. They build on the base `ToggleButton` theme.
- A toggle that is only an icon has no surface. The icon is dim at rest (opacity 0.6) and lit when the
  toggle is on or under the pointer (1.0). `ToggleButtonVisibility` and `ToggleButtonPower` are of this
  family. `ToggleButtonPlayPause` is dim at rest, and its icon changes when it is on.
- A theme for one more icon toggle builds on `ToggleButtonPower` and only sets the icon with `IconKey` in
  a style. `ToggleButtonUpdate` does this. The Studio does it for its mask toggle.
- The opacity is always a style, never a value on the template. A value there beats every style, so the
  on and hover states would not show. `ToggleButtonPowerTests` checks this.

### Scrolling

A click must not make a view scroll or jump. `CMiXTheme` turns off the two ways that Avalonia
scrolls on its own: `ScrollViewer.BringIntoViewOnFocusChange` (a click gives focus, and the view
scrolls to the control) and `ListBox.AutoScrollToSelectedItem` (a pick scrolls the list and the
views around it). A text box keeps its own setting. One ScrollViewer can turn the first one on
again with `BringIntoViewOnFocusChange="True"`. A change of the total height, for example a
panel above the click that grows or collapses, still moves the content. `ScrollViewerBehavior`
restores the offset when the height grows. `ScrollStabilityTests` shows both behaviors.

The application connects three things to the library:

- `InteractionHost.BeginScopeHandler`, set in `App.Initialize`. See above.
- `Icons.Register`. The library has the generic icon keys. The Studio adds its
  own in `StudioIcons.Register`, also in `App.Initialize`, before the XAML loads.
  `IconKeyTests` fails when an `IconKey` in the AXAML is not registered.
- `IReorderService`, the type of the attached property
  `ListBoxReorder.Service`. `ManagerReorderService` implements it.

The Studio has its own styles in `Themes/Generic.axaml` and its own converters in
`Themes/StudioConverters.axaml`. They build on the library theme.

To add a control to the library:

1. Put the class in the folder of the data it changes. The namespace is the folder.
2. If it is a new namespace, add an `XmlnsDefinition` in `CMiX.UI/XmlnsDefinitions.cs`.
3. Put its style in `Themes/Styles` and include the file in `Themes/CMiXTheme.axaml`.
4. Add a test in CMiX.UI.Tests. `ThemeTests` already loads every control with only
   `CMiXTheme`, so a missing resource fails there.

CMiX.UI.Tests has its own test application with only the base theme, the icon
styles and `CMiXTheme`. `PointerInput` is in `CMiX.UI.Tests/Support` and is linked
into the Studio tests.

The headless renderer cannot hit-test through a geometry clip, such as the rounded
clip of a `BorderMask`. A test that presses an editor with the mouse clears the clips
first. See `InteractionHostTests`.

## Menus and popups

Every menu and popup has one look. The control `PopupFrame` holds it: the shadow, the rounded translucent panel and the padding. No other file defines this look.

| Popup | Theme | Use |
|---|---|---|
| Context menu | `ContextMenuDefault` | `<ContextMenu Theme="{StaticResource ContextMenuDefault}">`. It opens with its middle on the click. |
| Menu popup | `MenuFlyoutPresenterDefault` | A `MenuFlyout` with this `FlyoutPresenterTheme`. |
| Panel popup | `FlyoutPresenterPanel` | A `Flyout` for settings and other content. It is 300 wide. |

The themes and `PopupFrame` are in CMiX.UI.

`ContextMenuDefault` and `MenuFlyoutPresenterDefault` derive from the theme `MenuPanel`. A row is a `MenuItem` with the theme `MenuItemContext`. A row with the header `-` is a separator. A row with `IsChecked` has the accent background.

A `MenuFlyout` whose rows come from a list needs one `ObservableCollection` that the code updates in place. A new `ItemsSource` does not refresh a menu that was shown before. `ButtonModulatorAssign` is an example.

`PopupThemeGuardTests` fails when a `ContextMenu`, a `Flyout` or a `MenuFlyout` in an AXAML file of the Studio or of CMiX.UI does not use one of these themes.

### Open a popup with a toggle button

A `Popup` that a `ToggleButton` opens uses the attached property `TogglePopup.Toggle`:

```xml
<ToggleButton x:Name="MyToggle" />
<Popup ui:TogglePopup.Toggle="{Binding ElementName=MyToggle}" Placement="Bottom">
```

The popup is open while the toggle is checked. The property sets the placement target, switches the light dismiss off and adds `ClosePopupBehavior`. Do not set these by hand. `PopupButton` is a ready-made toggle with a popup. `PopupWiringGuardTests` fails when a `Popup` outside the Themes folder does not use the property.

### Close when the pointer goes away

A popup, a flyout or a context menu can close when the pointer moves away from it:

- A `Popup` uses the behavior `ClosePopupBehavior`. It also closes the popup on a click outside it and on a click on a button in it.
- A `Flyout` or a `MenuFlyout` uses the behavior `CloseFlyoutOnPointerAwayBehavior`. It only checks the distance.
- A `ContextMenu` with the theme `ContextMenuDefault` has it already, for example the Reset menu of a value editor. The theme sets the attached property `ContextMenuPointerAway.IsEnabled`. Set it to false on a menu to switch the rule off.

All three use the class `PointerAwayCloser`. A new kind of popup needs only a small wrapper around it.

The pointer is away when it is farther than `Distance` from the content and from the control that opened it. The default is 64. The pointer on the control that opened the popup is never away, also when the control is much wider than the popup. After the user types in a text box of the popup, the popup stays open, because a close would lose the text. The behaviors only check the distance when the pointer moves.

## Sync over Redis

### The three parts

A sync has three parts: the Studio, one or more engines, and one Redis server.
A **peer** is one running process that connects to Redis: the Studio, one vvvv
engine, or one CMiX.Console.

Redis is the source of truth. All peers connect to Redis. No peer connects to
another peer.

There is one writer. Only the Studio appends to the stream and writes the
snapshot. Engines read and apply. An engine never appends and never pushes.

### The keys

All keys live under one prefix. The default prefix is `cmix:default`. Give each
show its own prefix if several shows share one server.

| Key | Type | Content |
|---|---|---|
| `{prefix}:updates` | stream | one entry per message, field `env` with the MessagePack bytes of a `MessageEnvelope` |
| `{prefix}:snapshot` | hash | `model` (the MessagePack bytes of the `ProjectModel`), `streamId` (the stream position the model is valid for), `writtenBy` (the peer ID), `writtenAt` (the UTC time) |
| `{prefix}:tail` | string | the ID of the newest entry, set by the writer after each append |
| `{prefix}:peers:{peerId}` | hash with a 6 s TTL | `name`, `role`, `host`, `lastAppliedId` |
| `{prefix}:wake` | pub/sub channel | an empty message after each append, to wake the readers |

### The rules

**Send.** The messenger gives the message to the peer. A peer that is not a
writer drops it. A writer wraps the message in an envelope with its peer ID and
a new message ID, serializes the envelope, and puts the bytes in one queue. One
task appends the entries in queue order, so a slider drag keeps its order. A
failed append waits 500 ms and tries the same entry again. The queue appends
only while the peer is joined.

**Receive.** One follower task reads the entries after the last applied
position. It waits for the wake signal, or for 250 ms, whichever comes first.
The peer skips an entry that it sent itself, but it still moves its position
forward. The peer also skips an entry with a message ID that it applied before.
It remembers the last 1000 message IDs for this. The follower applies one read
batch, up to 256 entries, in one action on the dispatcher. On the Studio that
is the UI thread. On the vvvv engine it is the mainloop.

**Join.** The peer reads the snapshot and applies the model. It then replays
the stream from the snapshot position to the tail, in batches of 256 entries.
Then it starts the follower. An engine joins by itself and tries again until it
succeeds.

**Push.** Only the Studio pushes. It drops the queue, captures the loaded
project, appends it as one snapshot entry, writes the snapshot key with the ID
of that entry, and trims the stream to that ID. The running peers get the
entry. A late peer gets the snapshot.

**Pull.** Only the Studio pulls, and only while it is not joined. A pull is a
join: the Redis state replaces the loaded project.

**Compaction.** Only the Studio compacts. After a structural edit it writes a
new snapshot at once. After a value change it writes a new snapshot 5 s later.
More value changes in that window do not restart the timer. The compactor then
trims the stream. It keeps every entry that is younger than 60 s, so a peer
that lags a little does not lose entries. The compactor waits with the capture
while own entries are not applied yet, so the snapshot position is never below
the entries of the edits that the snapshot holds.

**Gap.** A peer has a gap when the snapshot position is ahead of its own
position, and the oldest entry in the stream is newer than its own position.
The entries in between are trimmed, and only the snapshot has the state. An
empty stream has no gap. Every peer checks this on each heartbeat, and re-joins
from the snapshot on a gap.

**Stale pause.** A follower that made no read for 30 s checks the gap before it
applies anything. This catches a process that was suspended.

**Reconnect.** The Redis client reconnects by itself. The follower waits with a
backoff from 1 s to 10 s and checks the gap before it reads again. A reconnect
wakes the follower, so it does not sleep out the backoff. A compaction that
failed during the outage runs again when the store is back. The Studio also
writes the snapshot again when the heartbeat finds no snapshot, for example
after a Redis restart without the data.

**Presence.** Every peer writes its own peer key every 2 s with a 6 s TTL. The
Studio reads the peer list and shows the lag of each peer, the difference
between the tail and the position of that peer. Engines do not read the list.

**Start check.** The Studio runs a check after it connects. On an empty store it
pushes without a click, because there is no show to lose. When the stored state
equals the loaded project, the Studio is in sync. In every other case the state
shows "Not in sync", sending is blocked, and the user clicks Push or Pull.

**In sync.** A peer is in sync when it is connected, joined, and its position
equals the tail.

**The UI and the engine mainloop.** No Redis call runs on the UI thread. The
one accepted UI pause is the apply of a large snapshot, because the controls
are bound to the UI. The engine patch uses one `EngineHost` object. `Create`
makes it. `Start (Options, Read Batch)` builds the services and starts the
engine peer with a `MainloopQueue`. The getters `ServiceProvider` and
`MainloopQueue` give the rest of the patch what it needs. `Start` runs once,
and a second call throws. The patch drains the queue once per frame with
`Drain`. Every apply, every event and every property change of the peer then
run on the mainloop. The read batch is the number of entries that one such
action applies. The default is 256. A larger value gives a faster catch-up and
a longer frame. A patch that stops draining stalls the peer. `Dispose` calls
`SyncPeer.Stop` and then `Close` on the queue, so the peer stops at once. The
`Dispose` of the patch must call it. `CMiX.Console` has no mainloop, has no
`EngineHost`, and applies inline.

### The Studio settings file

The Studio reads `studio-settings.json` from the directory of its executable.
The file holds one `Redis` object with nine fields:

```json
{
  "Redis": {
    "Ip": "127.0.0.1",
    "Port": 6379,
    "Database": 0,
    "UseCredentialManager": false,
    "CredentialName": "",
    "User": "default",
    "Password": "",
    "KeyPrefix": "cmix:default",
    "PeerName": "Studio"
  }
}
```

A missing file, an empty file or a broken file gives the fallbacks. An empty
field gives the same fallback:

| Field | Fallback |
|---|---|
| `Ip` | `127.0.0.1` |
| `Port` | `6379` |
| `Database` | `0` |
| `UseCredentialManager` | `false` |
| `CredentialName` | empty |
| `User` | `default` |
| `Password` | no password |
| `KeyPrefix` | `cmix:default` |
| `PeerName` | `Studio` |

When `UseCredentialManager` is true and `CredentialName` is not empty, the Studio
reads the generic credential with that name from the Windows Credential Manager.
The user name and the password of the credential replace `User` and `Password`.
When the credential does not exist, the file values apply. The sync panel shows
the user name and its source in the Redis line: `from file`,
`from credential <name>`, or `from file, credential <name> not found`. The
connection flyout disables User and Password while the toggle is on and the name
is not empty.

The password of a generic credential is stored as UTF-16, the form that the
Control Panel and `cmdkey` write. A credential that another tool stored as UTF-8
reads wrong. Example:

    cmdkey /generic:cmix_redis /user:alice /pass:secret

### Load

Up to 42 peers give about 275 commands per second while the show is idle, and
about 3,000 commands per second while the user drags a slider.

The Studio finds the peers with a SCAN over the whole database. Give CMiX its
own database number in the settings file if other tools fill the same database
with many keys.

### The headless engine

`CMiX.Console` is an engine without a window. It takes these arguments:

    --ip <address>      Redis server address. Default 127.0.0.1.
    --port <number>     Redis server port. Default 6379.
    --db <number>       Redis database index. Default 0.
    --user <name>       Redis user name. Default default.
    --password <text>   Redis password. Default empty.
    --prefix <text>     Key prefix for the store. Default cmix:default.
    --name <text>       Peer name. Default is the machine name.
    --credential <text> Name of a generic credential in the Windows Credential Manager.
                        Its user name and password replace --user and --password. Default empty.
    --help              Show this usage text.

The Console prints the user name and its source after it reads the arguments.

It prints the status, the applied count and the positions when they change.

To run two engines for a test, start it twice with two names:

    CMiX.Console --name Engine-1
    CMiX.Console --name Engine-2

Both join, and the peer list of the Studio shows both with lag 0.

### The tests

`CMiX.Core.Tests/README-redis.md` tells you how to run the sync tests. The fast
set skips the tests that need a server:

    dotnet test CMiX.Core.Tests\CMiX.Core.Tests.csproj --filter "Category!=Redis"

The Redis set needs a Redis or a Memurai on `127.0.0.1:6379`:

    dotnet test CMiX.Core.Tests\CMiX.Core.Tests.csproj --filter "Category=Redis"

The tests skip when no server answers.

### How to get engine input back

Today an engine does not write. To let an engine send values, do this:

1. Register the engine peer with the messenger again in
   `ConfigureEngineTransport`, and set `IsWriter`.
2. Handle the own entries of the engine after a snapshot apply. Keep a set of
   the message IDs that the engine sent since the last snapshot apply. An own
   entry that the snapshot overwrote is applied again. An own entry that came
   after the snapshot is skipped.
3. Make `MessageMoveItem` safe to apply twice, or make the compactor wait for
   the own entries on the engine too.

Until then, engine values reach the Studio through another path. One way is
Redis keys that the patch writes with the vvvv Redis package, and that the
Studio reads.

## Tiled Rendering

See `Tiled-Rendering-Documentation.md`