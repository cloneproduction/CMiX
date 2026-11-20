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
`DragValueInteger` is a `DragValue` for whole numbers: it starts with `IsInteger` on and
steps of 1. It takes the same bindings as `DragValue`:
`Value="{Binding Name.Value}"` and `ResetCommand="{Binding Name.ResetCommand}"`.
An editor that is one part of a group (a vector) also takes a `ResetAllCommand`, and
its Reset menu then has a Reset All row. `ValueBindingTests` fails when a
`DragValue` or a `DragValueInteger` in an AXAML file has no `Value` binding.

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
  SelectorPrefab. The generic controls are in CMiX.UI.

The names of the views and the controls follow the order rule in `.claude/CLAUDE.md`, section Names: the kind first, then the thing.

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
`Panels`, `AttachedProperties`, `Behaviors`, `Popups`, `Converters`, `Interaction`,
`Mathematics` and `Themes`. `Popups` holds the popup logic with no XAML face: `OutsideClick`,
`PointerAwayCloser`, `PopupAnchoring`, `PopupPlacement`, `TipContent`, `Notification` and `PanelNotification`.

`Controls` has one folder for each type of data that an editor changes. The names
come from the value editors of vvvv:

| Folder | The data | In it |
|---|---|---|
| `Number` | one number | `DragValue`, `SliderCMiX`, `NumericInput`, `NumericTextHelper`, `DragValueInteger`, `RangeValue`, `NumbersOnly` |
| `Vector` | several numbers | `Vector2`, `Vector3`, `Integer2`, `Integer3`, `AxisSelector2`, `AxisSelector3` |
| `Boolean` | on, off, or a press | `ToggleButtonCaptioned`, `ToggleButtonCMiX`, `ButtonCaptioned` |
| `Enum` | one item of a fixed list | `ComboBoxCaptioned` |
| `String` | text or an address | `TextBoxEditable`, `EditableValue`, `TextBoxIP`, `SelectOnFocus`, `TextBlockTrimmed` |
| `Path` | a file or a folder | `ComboBoxPath` |
| `ColorPicker` | a color | `SelectorColor`, `ColorPickerFull`, `ColorPickerWheel`, `ColorPickerSliders` |
| `Collection` | a list of items | `PanelItem`, `ListPanelItem`, `ListItemIcon`, `ListBoxRepository`, `ListBoxEditable` |
| `Parts` | nothing: what every editor is made of | `ContentCaptioned`, `UserControlCaptioned`, `ValueMenu`, `ContextMenuGuard`, `DragEditHelper`, `PopupFrame` |

`AppIcon`, `ListBoxCMiX` and `ButtonPopup` stay in the root of `Controls`. They do not change a value.

### Attached properties and behaviors

- An attached property carries a value or a switch. It is a static class with a `Changed` handler.
- An attached property holds no object of its own. When it needs one, it installs a behavior, as `TogglePopup` installs `ClosePopupBehavior`.
- A `Behavior<T>` is an object with a lifetime (`OnAttached`, `OnDetached`). It has state for each instance and its own events.
- A view declares a behavior in `Interaction.Behaviors`. A style or a control theme sets it with a `BehaviorCollectionTemplate`.
- Attached properties are in `CMiX.UI/AttachedProperties`. Behaviors are in `CMiX.UI/Behaviors`.
- Behaviors of the Studio are in `CMiX.Studio.Avalonia/Behaviors`.
- Popup logic with no XAML face is in `CMiX.UI/Popups`.
- An attached property that serves one control family stays with that family, as `NumbersOnly` in `Controls/Number`.
- A state that a control is in reaches a style through a pseudo class. The owner that knows the state sets it.
- When the owner is outside the control, it sets the pseudo class through `IPseudoClasses`.
- A variant that XAML chooses is a class.
- A class with no user is removed. A state property with no reader is not registered.
- `OutsideClick` is the one static class with state, because an Avalonia class handler is static.

### Heights of lists and drop downs

A list or a drop down shows a whole number of rows, so no row is cut at the bottom, and the wheel moves it
by one row. The height of a row is the token `ControlHeightMD`. No file holds a height as a number. The
sizes come from the tokens in `Themes/Tokens/Geometry.axaml`:

- `ListBoxCMiX`: `MinItemCount` rows (the token `ListVisibleItems`), the spaces of `ListSpacing` between them, plus the top and the bottom of
  `ListScrollMargin`. The user can drag the strip under the list to make it taller. The height then goes by
  whole rows, and it never goes below `MinItemCount` rows. There is no largest height.
- `ListBoxEditable`: `MinItemCount` rows, or `ListEditableVisibleItems` rows when it is 0, the spaces of `ListSpacing` between them, and
  `ListEditablePadding` around them. The list has no scroll viewer, so it grows with its items.
- `ComboBox`: `ComboBoxVisibleItems` rows.

The attached property `ScrollStep` does the work for a `ScrollViewer`: `ItemHeight` is the height of a row, `ItemSpacing` is the space between two rows and
`VisibleItemCount` is the number of rows. The wheel goes on to the panel around the list when the list is at
its start or end. To change how many rows a list shows, change its token. A new list takes its numbers from
the tokens too.

### The scale of sizes

All the sizes of the UI come from one scale in `Themes/Tokens/Geometry.axaml`. No AXAML file holds a size as
a number: the test `SizeLiteralTests` fails for a new `Height`, `MinHeight`, `MaxHeight`, `Width`, `MinWidth` or `MaxWidth` that is a number.
In the files of the tree and of the scroll bar, it also fails for a `Margin` or a `Padding` that is a number.
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
  button and a drag handle. `ListPanelItem` is a list of them with an add button.
  `ListItemIcon` is a row with an icon, a name and a toggle. `ListBoxRepository` is a list
  with a delete button, an add area and the detail of the selected item.
- A variant of a panel is a theme, not a switch. The default theme of `PanelItem` has a
  body that collapses, an eye, a close button and a drag handle. An application writes
  its own theme from the library parts (`ToggleButtonPower`, `ToggleButtonVisibility`,
  `ButtonIconClose`, `ButtonDragHandle`) and sets it with `Theme=`. A theme that does not
  use a property just leaves it out.
- The list is reordered only when the application sets `ReorderService` on it.
- `ListBoxEditable` is a list of items of one type, with a column of buttons on its right: add and
  remove, then move up and move down. The application sets the
  `ItemTemplate`, so an item can be a `DragValue`, a `SelectorColor` or any other editor. A button
  shows only when its command (`AddCommand`, `RemoveCommand`, `MoveUpCommand`, `MoveDownCommand`)
  is set. `AddCommandParameter` is the parameter of the add command, and the other commands get the
  selected item. When `ReorderService` is set, each row has a drag handle. The `BeatStepModulator`
  uses it for its list of steps.
- A control that shows content of the application, such as `AddContent`, is a composed
  `UserControl` and not a templated control. A templated control takes over the template
  parent of the content that it shows in its template. Then a `TemplateBinding` inside that
  content, such as the owner type of the add panel of an entity, reads from the wrong
  control and finds nothing. `SectionEntityModifiersTests` keeps this case.
- A Studio control tells the library control which properties of the model to use.
  `ModifierManager` is a `ListPanelItem` of `PanelItem` rows. `ListRepository` is a
  `ListBoxRepository` with the icon templates of the prefabs. `ModulatorHoverBehavior` tells
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
  `Vector2`, `SelectorColor`.
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
  `RangeValue`, `EditableValue`. No standard control is called
  `Value`, so there is nothing to put first. These names stay as they are.
  `DragValueInteger` derives from `DragValue`, so its name starts with that kind.
- In the Studio, the ending `Modulatable` marks a control that takes modulators. An
  editor has a modulator button: `DragValueModulatable`, `SliderModulatable`,
  `Vector2Modulatable`. Their base is `UserControlModulatable`. The button is
  `ButtonModulatorAssign`. A panel has a list of modulators: `PanelModulatable` is the
  base of the modifier views.
- `CMiXTheme` is the one name that keeps the brand in front, like `FluentTheme` in
  Avalonia.

`ComboBoxPath` chooses a file or a folder: a list of known items, a Browse button and a
drop of files from the file explorer. It only finds the path and raises `PathChosen`
and `PathsDropped`. It does not know what the path becomes. The Studio has
`ComboBoxAssetPath`, which turns the path into an asset of its repository. The drop
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

### The colors of the states

A control has a color for each state, and the colors come from four brush tokens in `Themes/Tokens/Brushes.axaml`.
No theme writes a grey for a state. To change a grey for the whole application, change the token.

| Kind | Controls | Idle | Under the pointer | Pressed | On |
|---|---|---|---|---|---|
| Surface | button, toggle, combo box, drag value, slider, scroll bar, radio button | `StateIdleBrush` | `StateHoverBrush` | `StatePressedBrush` | accent-inactive, accent under the pointer |
| Row | list item, combo item, menu item, composable item | transparent | `RowHoverBrush` | – | selected: accent-inactive, accent under the pointer |
| Icon only | eye, power, update, mask, tabs of icons | opacity 0.6 | opacity 1 | – | opacity 1 |

A new control takes its colors from these tokens, in the same three steps. The project `CMiX.UI.Gallery` shows every
control in every state, and its four lists of greys change the tokens while it runs.

### Toggle buttons

A toggle shows that it is on in the way that fits its look. There are two families.

- A toggle with a surface (a box or a pill) turns its background to the accent color when it is on.
  These are `ToggleButtonRounded`, `ToggleButtonInlineLeft`, `ToggleButtonBox`, `ToggleButtonPopup` and
  the others. They build on the base `ToggleButton` theme.
- A toggle that is only an icon has no surface. `ToggleButtonIcon` is the base of this family: the icon is dim
  at rest (opacity 0.6) and lit under the pointer (1.0). `ToggleButtonPower` is also lit while it is on.
  `ToggleButtonPlayPause` is dim at rest, and its icon changes when it is on.
- A theme for one more icon toggle builds on `ToggleButtonIcon` (or on `ToggleButtonPower` when it is lit while
  on) and only sets the icon with `IconKey` in a style. A second icon for the on state is the same style under
  `:checked`. `ToggleButtonVisibility` (eye) and the mask toggle of the Studio do this. `ToggleButtonUpdate` sets one icon.
- The opacity is always a style, never a value on the template. A value there beats every style, so the
  on and hover states would not show. `ToggleButtonPowerTests` checks this.

### Scrolling

A click must not make a view scroll or jump. `CMiXTheme` turns off the two ways that Avalonia
scrolls on its own: `ScrollViewer.BringIntoViewOnFocusChange` (a click gives focus, and the view
scrolls to the control) and `ListBox.AutoScrollToSelectedItem` (a pick scrolls the list and the
views around it). A text box keeps its own setting. One ScrollViewer can turn the first one on
again with `BringIntoViewOnFocusChange="True"`. A change of the total height, for example a
panel above the click that grows or collapses, still moves the content. `ScrollViewerBehavior`
restores the offset when the height grows. The theme `ScrollViewerOverlayKeepOffset` applies it. `ScrollStabilityTests` shows both behaviors.

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

Every menu and popup has one look. The control `PopupFrame` holds it: the shadow, the dark card with a thin border, and the padding. No other file defines this look. A row of a menu can show a shortcut, an arrow, a quiet title and a warning color (see `ContextMenu.axaml`).

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

The popup is open while the toggle is checked. The property sets the placement target, switches the light dismiss off and adds `ClosePopupBehavior`. Do not set these by hand. `ButtonPopup` is a ready-made toggle with a popup. `PopupWiringGuardTests` fails when a `Popup` outside the Themes folder does not use the property.

### A click outside only closes

`OutsideClick` holds one rule for the whole application: a press outside the open popups (a popup, a dropdown, a menu, a flyout) only ends them. The control under the pointer gets neither the press nor its release, so a click outside does not select a row, and a right click outside opens no menu. A press outside a popup with a value in edit mode only ends the edit, and the next press outside closes the popup. A press in a popup closes the popups that opened after it, such as the Add panel beside a menu. A tooltip does not count. The Studio, the Gallery and the UI tests install it at start (`OutsideClick.Install`). A control whose press only ends what it does takes the press with `OutsideClick.Take`: a right click while a name is renamed cancels the rename and does nothing else.

### Close when the pointer goes away

A popup, a flyout or a context menu can close when the pointer moves away from it:

- A `Popup` uses the behavior `ClosePopupBehavior`, set by the attached property `TogglePopup.ClosesOnPointerAway`. It also closes the popup on a click on a button in it. `OutsideClick` owns the press outside the popup.
- A `Flyout` or a `MenuFlyout` uses the behavior `CloseFlyoutOnPointerAwayBehavior`. It only checks the distance.
- A `ContextMenu` with the theme `ContextMenuDefault` has it already, for example the Reset menu of a value editor. The theme sets the attached property `ContextMenuPointerAway.ClosesOnPointerAway`. Set it to false on a menu to switch the rule off.

All three use the class `PointerAwayCloser`. A new kind of popup needs only a small wrapper around it. A context menu can have a companion, a second control that counts as a part of the menu (`ContextMenuPointerAway.Companion`).

The pointer is away when it is farther than `PointerAwayDistance` (the same name on `TogglePopup` and on `ContextMenuPointerAway`) from the content and from the control that opened it. The default is 64. The pointer on the control that opened the popup is never away, also when the control is much wider than the popup. After the user types in a text box of the popup, the popup stays open, because a close would lose the text. The behaviors only check the distance when the pointer moves.

### The card, the tip and the place of a popup

A tooltip and the Add panel use one card. A menu uses the same colors. These parts are in CMiX.UI:

- `PopupCard` is the content of a tooltip or a panel: a dark card with a border of one pixel, the room of the shadow around it, and a tip that is hidden until the placement shows it. Its colors are the tokens `PopupCardBackgroundBrush` and `PopupCardBorderBrush`. Its padding is the token `PopupCardPadding`. `PopupFrame` has the same fill and border, so a menu and a card look alike.
- `PopupTip` is the small triangle on one edge of the card. `PopupCard.Place` puts it on the edge that faces the anchor, at the offset that the placement gives.
- `PopupPlacement.Place` is the one rule that puts a card next to an anchor. It has no UI, so a test can check numbers. It takes the anchor, the size of the card, the room that the card may use, the wanted side (top, bottom, left or right), the gap (8) and the margin (8). The card goes on the wanted side. With no room there, it goes on the opposite side. With room on neither side, it goes on the side with more room. Along the side, the card follows a point (`Along`), stops at the margin, and the tip keeps pointing at the point.
- `PopupAnchoring` applies the rule to a tooltip, a popup, a flyout or a context menu. Avalonia asks for the place with a callback and gives the size of the popup and the control. `PopupAnchoring` turns them into a request, runs the rule, and gives the corner of the card back. The coordinates are those of the window of the control, in device independent pixels. For a control inside a menu, the window is the popup of the menu. The room is the working area of the screen of the control. A popup is larger than its card by the room of the shadow (`PopupFrame.ShadowMargin`), and `PopupAnchoring` takes that room out.

Avalonia asks for the place again when an offset of a tooltip or a popup changes. A change of `PopupAnchoring.Along` alone moves nothing. A tooltip sets `Along` from the pointer until it opens, then it stays where it opened.

### Tooltips

A view sets `ToolTip.Tip` and nothing else. `ToolTipDefaults` gives the control the placement when it gets a tip:

- The tooltip opens 8 pixels from the control and never covers it. It opens at the pointer along the control and its tip points at the pointer. It stays where it opened. Near the edge of the screen it stops 8 pixels from the edge. With no room below, it opens above.
- `ToolTipDefaults.Side` sets the side of one control: `Bottom` by default, `Right` or `Left` for a control in a strip at the side of the window. A tooltip at the side opens at the pointer in y.
- A control that has its own `ToolTip.Placement` keeps it. `ToolTipSourceTests` fails when an AXAML file sets the placement or an offset of a tooltip.
- A plain string shows as a title. A `TipContent` shows a title, a shortcut, a description, the reason why a control is disabled, and a footer. A line that is null is not shown. The reason has its own color.
- `ButtonIcon` builds the `TipContent` from `Tip`, `TipShortcut`, `TipDescription` and `DisabledReason`. The reason shows while the button is disabled, also when a command disables it, and the tooltip then shows on a disabled button. The toolbar of the tree binds `TipShortcut` to the shortcut map, so a changed shortcut shows in the tip.

### The Add panel

The Add panel adds something under a node of the tree. One `AddPanel` serves each place that opens it. It is in the Studio, and it uses the list `ListGroupedSearch` of CMiX.UI.

- `ProjectTree.GetAddGroups` gives the same groups for every node: Layer, Content, Texture, Filter, Modifier, Modulator and Project. An entry that the node cannot take is there with `Enabled` false. `AddPanel` leaves out the entries that are not enabled and the groups that stay empty, so the panel shows only what the node can take. What a node can take comes from what it holds (`TreeGroup.GroupsOf`, see `AvailableAdds`). The composition is always enabled. The rows of the panel have a square in the color of their kind (`PickerItem.Kind`), no icon.
- The key `Tree.Add` is bound in the main window to `ProjectTree.AddAtTargetCommand`. It runs on the condition of every tree key (`CanRunTreeKey`, see The keys of the tree), and it raises `AddRequested`. The view opens the Add panel of the target row (`TargetNode`) with the middle of its search field on the middle of the row (`TreeRowAnchor.CenterOf`), with the title Add. With no target, or a row that is not shown, it opens at the pointer.
- The key `Tree.Menu` (W) is bound in the main window to `ProjectTree.OpenMenuAtTargetCommand`. It runs on the same condition. The view opens the menu of the target row under the row, at the left edge of the name (`TreeRowAnchor.FromNameOf`), or the menu of the project at the pointer when there is no target. The Menu key raises `ContextRequested` with no pointer place, and the menu of the focused row opens under the row in the same way. A right click opens the menu at the pointer and puts the cursor on the clicked row. The key `Tree.Expand` (E) runs `ProjectTree.ToggleExpandTargetCommand` on the same condition.
- `ListGroupedSearch` shows a search field and the groups with their number of rows. A group with more than 8 rows starts closed. The search opens every group that has a match. It knows no type of the app: the owner gives `PickerGroup` and `PickerItem` and gets the picked row back. The keys of the search field and the cursor over the rows are in `PickerKeys`, the part that `ListGroupedSearch` and `ListSearchableItemPicker` share.
- `AddPanel.Open` takes an anchor and a side. The panel puts itself on that side. On the right or the left of the anchor, the search field lines up with the middle of the anchor. A pick closes the panel, then runs the add.
- Three places open it: the Add row of the menu of a row (to the right of the menu), the Add button of the toolbar over the tree (with the search field under the pointer, or under the button for a click from the keyboard, with the header "Add to" and the node), and the shortcut `Tree.Add` (Shift+A), which opens the panel at once. Nothing adds a composition at once: with nothing selected, the button and the menu of the project open a panel with the Composition entry. Every group of the panel is open.
- The Add row of a menu is made by `PrefabRowMenu` from an `AddMenuRow`. The pointer on the row for a short time, a click, Enter and the Right key open the panel. The pointer on another row closes it. The menu and the panel close together, except for Escape in the panel, which closes only the panel. The panel is a companion of the menu (`ContextMenuPointerAway.Companion`), so the pointer on the panel is not away from the menu.

A test that opens a menu at the pointer must put the pointer at the left of the window first. A menu opens at the pointer, and the panel opens to its right, so a pointer that moves right with each click takes the panel out of the window. A click on a row of a popup in a headless test needs a pointer move onto the row first.

### Rows that a drag opens

`TreeDragSession` opens collapsed rows during a drag, so the user can reach a place deep in the tree.

- `OpensOnRest` tells which rows open: a collapsed row with children and a place for the dragged node under it (`HasDropTargetBelow`). A row that takes the node itself, such as a layer for an entity, counts too, because the places between its children show only when it is open.
- `TrackRest` runs on each move of the drag. A rest starts when the pointer comes onto such a row, and moves inside that row keep it counting. Another row, or the end of the drag, stops it. A `DispatcherTimer` with `RestDelay` (0.6 s, `DefaultRestDelay`) then calls `OpenRested`. A test can set `RestDelay`, or call `OpenRested` at once, because the headless platform does not fire a `DispatcherTimer`.
- `OpenedByDrag` lists every row that the drag opened, in order. A new drag starts a new list.
- `Tree.Expand` (E) during a drag runs `ProjectTree.ToggleExpandTargetCommand`, as outside a drag. While `ProjectTree.IsDragging` is true, the command calls `ProjectTree.ToggleDragRow`, which the view sets to `TreeDragSession.ToggleHovered`. `ToggleHovered` opens the row under the pointer at once, or closes it when this drag opened it. It does not close a row that was open before the drag. A row that E closed does not open again on a rest until the pointer leaves it.
- `CloseOpenedRows(kept)` closes the rows of `OpenedRows`, except `kept` and its parents. After a drop, `StartDrop` calls it with the target of the slot (`TreeDragSession.Slot`), so for a drop Before or After a row, the parents of that row stay open. After a cancel, or a drop with no slot, `Finish` closes every row of the list.
- `PanelCollapse` closes a row over a few frames, so the rows above the view get shorter after the close. `TreeScrollAnchor.HoldWhileClosing` puts the anchor row back after each layout of the tree, until the `PART_Collapse` panel of every closed row reaches `Progress` 0, or after 500 layout passes. The drop and the cancel both use it after `RestoreAnchor`.

### The label of a drag

`TreeDragGhost` shows the label of the dragged row on the overlay layer of the window. It takes no pointer events.

- The label is a `Border` with the fill and the corners of the row, a `StackPanel` with an `AppIcon` and a `TextBlock`. It is as wide as its content. It is a little transparent (`GhostOpacity`).
- `Show` takes the row, the icon key, the name, the reference control and the pointer. `iconMiddle` is the distance from the left edge of the label to the middle of the icon. `Move` puts that middle over the pointer, and the left edge stays inside the overlay layer.
- The label is above the pointer. The gap is the token `SpacingSM`. When there is no room above, the label goes under the pointer.
- `TreeDragSession.Track` moves the label after it marks the drop. It gives `Move` the top of the drop line (`DropLineTop`, from `TreeDropMarker.Line`). When that line is above the pointer, the label goes above the line.

### The keys of the tree

The keys of the tree are key bindings of `MainWindow`, not of the tree, so a click on another control does not stop them.

- `Tree.Rename`, `Tree.Remove`, `Tree.Duplicate`, `Tree.MoveUp` and `Tree.MoveDown` run `RenameSelectedByKeyCommand`, `RemoveItemByKeyCommand`, `DuplicateItemByKeyCommand`, `MoveItemUpByKeyCommand` and `MoveItemDownByKeyCommand` of `ProjectTree`. The `...ByKey` commands add `CanRunTreeKey` to the condition of the plain command. The toolbar buttons use the plain commands, without the conditions below.
- `CanRunTreeKey` is the one condition of every tree key of the window: `Tree.Add`, `Tree.Menu`, `Tree.Expand`, `Tree.CollapseOthers`, `Tree.Search` and the keys of the target row. It is true when no drag runs (`IsDragging`), no text box has the focus (`IsTextBoxFocused`), and either the tree has the focus (`IsTreeFocused`), or the pointer is over the tree (`IsPointerOverTree`) and no other panel has the focus (`IsFocusInOtherPanel`). The view gives the four answers through `TreeViewHooks`. `IsFocusInOtherPanel` of a view is true when the focused control is outside the `PanelSide` of the view. The focus on the window itself, or on no control, is in no panel. With two views, the tree asks every view, and the focus is in another panel only when every view says so. During a drag, only `Tree.Expand` runs, with the focus anywhere but in a text box. A key binding checks the command at each key press, so no change event is needed.
- `Tree.Open` stays in the view, with Enter and the arrow keys (`TreeKeyInput`). They need the focus in the tree.
- `Tree.CollapseOthers` (Alt+E) runs `CollapseOthersCommand` with the same conditions and a target. The target is `ProjectTree.TargetNode`: the cursor row, else the selected row. The edit commands, `Tree.Add`, `Tree.Menu`, `Tree.Expand` and the toolbar use the same target (`TargetPlace`, `TargetAdds`). The command raises `CollapseOthersRequested`. The view runs `CloseOtherRows` itself (`TreeOpenState.CollapseOthers`). It opens the parents of the target (`TreeWalk.ParentsOf`), takes them out of the search (`TreeSearch.Release`), closes every other open row that has children, and returns the closed rows. The target keeps its state. `TreeScreenRequests` calls `TreeScrollAnchor.CaptureRow` for the target. When the row is fully in view, `RestoreAnchor` and `HoldWhileClosing` keep it at its place. Else `RevealRow` runs after the close.
- `TreeFocusKeeper` gives the focus back to the tree. `focusIsInTree` is true while the focus is in the tree, and also when it goes from the tree to no control. `OnLostFocusInTree` and `CheckTheLostFocus` (after a layout, because a move makes the item of the row again) call `GiveTheFocusBack`. When no control has the focus, the row of the node, else the selected row, takes it. A control that has the focus is left alone.

### The search of the tree

The model holds the search. The view shows it.

- `ProjectTree.SearchText` holds the text. `RefreshSearch` walks the tree and sets `TreeNode.IsSearchMatch` and `TreeNode.IsSearchDimmed` (`ShowSearchLook` and `ClearSearchLook` raise only the properties that change). It runs when the text changes, from `ListChanged` (an add, a remove, a move or a replace of a list), and after a rename (`TreeNode.OnHeaderChanged`). With an empty text and no match before, it returns at once.
- A row matches when its `Header` or its kind holds the text, with no regard to case. The kind is `AddableTypes.LabelOf` of the type of the item, the label of the Add panel, kept in `kindLabel`. A group row matches on its header only. `SearchMatches` holds the matches in tree order, and `SearchMatchCount` their number.
- While a search runs, every row that does not match has `IsSearchDimmed`, the parents of the matches too. `Border#DimLayer` in ProjectTree.axaml binds its class `dimmed` to `IsDimmed`, `IsSearchDimmed` and `IsDragActive` through `TreeCoverConverter`: the drag dimming always shows, and the search dimming shows only while no drag runs. A converter is used, not one more property of the node, because `ShowDragLook` and `ClearDragLook` raise a fixed list of changes, which a test pins.
- `TreeSearch` holds the search state. `SyncSearchRows` opens the parents of every match, and records in `openedBySearch` the rows that it opened from closed. When the matches change, it closes the rows it opened that lead to no match any more. An empty text closes all of them. `isSearchChangingRows` is true only while the search sets `IsExpanded`. Any other change of a row, by the user or by a drag, takes the row out of the search (`TreeSearch.Release`), so the search never changes that row back. `MoveCursorToNextMatch` and `MoveCursorToPreviousMatch` open the parents of its match through the same path. When the search ends (an empty text), `SyncSearchRows` takes the selected row and its parents out of the search, so they stay open. It does the same for the cursor row while a move to a match put the cursor there (`keptCursor`). A cursor that the arrow keys moved goes to the highest closed row, as a close does. It closes the other rows it opened through `CloseRowsInView`. The view answers it in `TreeScreenRequests`, which keeps the row of the node in place when it is in view, else the first row in view (`CaptureAnchor(null)`). Without a view, the rows only close.
- The view: `TextBoxSearchTree` is the field, in the bar of `PanelSide` at the right. The bar shows it only for the `TreePanelChoice` (a data template of the choice), with the `ProjectTree` as its data context. Its width is the token `TreeSearchWidth`. The field is the shared `TextBoxSearch` with `ShowsClear`: a clear button with the theme `ButtonIconClose` (no background) is inside the field at its right while it holds text, and the text always stops before its place. A click empties the text and keeps the focus in the field. There is no number of matches in the view. `ProjectTree.FocusSearch` (`FocusSearchCommand` on `Tree.Search`, Ctrl+F) raises `SearchFocusRequested`, and the field takes the focus and selects its text. The window binding runs on the condition of every tree key (`CanRunTreeKey`). Enter runs `MoveCursorToNextMatch`, and Shift+Enter `MoveCursorToPreviousMatch`. The cursor goes to the match, and `RevealRequested` brings its row into view. The selection does not change, and the field keeps the focus. Down runs `ProjectTree.FocusTreeCommand`. Escape runs `ProjectTree.LeaveSearch`: it clears the text, and in an empty field it runs `FocusTree`. `FocusTree` gives the focus to the target row (`TreeFocusRequested`).
- The way out of the search: `ProjectTree.IsSearchActive` is true while the text holds more than spaces (`searchActive` of `RefreshSearch`). `ClearSearchText` keeps the cursor row, so `SyncSearchRows` keeps its parents open, and empties the text. Escape in the rows runs it (`TreeKeyInput`, after the drag check and the name box check; a grab and a menu take Escape before the tree sees it). `ClearSearchCommand` on `Tree.ClearSearch` (Alt+F) runs `ClearSearchText` and `FocusTree`. It does not have the condition of the tree keys: it runs while `IsSearchActive` and not `IsDragging`, also from a text box. A grab handles every key at the window, so Alt+F does nothing during a grab. The XAML of `TextBoxSearchTree` binds the class `filtering` of the field while `IsSearchActive` and the focus is not in the field: the border of the field has `AccentBrush`. `TreeHints.Mode` is `Filter` while `IsSearchActive` and the focus is in the view: the hint strip starts with Esc "clear search" and `Tree.Search` "edit search". The main menu has no access key on F (`ProjectTreeSearchExitTests`).
- No search during a drag: the setter of `ProjectTree.IsDragging` raises `IsDragging` and the `CanExecuteChanged` of `FocusSearchCommand`. During a drag, the command cannot run, `FocusSearch`, `MoveCursorToNextMatch` and `MoveCursorToPreviousMatch` do nothing, the field is read-only (`TextBoxSearch.IsReadOnly`), and so its clear button is disabled. Escape in the field ends the drag through `LeaveSearch` and `TreeViewSet.CancelDrag`, which the tree view sets to `TreeRowDrag.Cancel`. `EndDrag` runs `SyncSearchRows` once, because no sync runs during the drag.

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
`SyncPeer.Stop` and then `Close` on the queue, so the peer stops at once. `Stop`
ends the `SyncRun` of the last `Start`. The `Dispose` of the patch must call it. `CMiX.Console` has no mainloop, has no
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
   `SyncPeerStartup.StartAsEngine`, and set `IsWriter`.
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