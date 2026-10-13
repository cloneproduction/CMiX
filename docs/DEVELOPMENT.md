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
working Reset menu. `EditorResetMenuTests` (CMiX.UI.Tests) checks the editors alone.

A value editor shows a Reset menu only when it has a `ResetCommand`. The menu
does not open in edit mode. It does not open for a right-click in a popup below
the editor.

The editors with a Reset menu are `CMiXSlider`, `DragValue`,
`CaptionedToggleButton`, `CaptionedComboBox` and `CMiXToggleButton`. Use
`CMiXToggleButton`, not `ToggleButton`, for a toggle that edits a model value.
Bind the `ResetCommand` of an editor to the `ResetCommand` of the value.
An `IntegerValue` needs both `DataContext="{Binding Name}"` and
`Value="{Binding Value}"`. Without the second one, the box does not follow the
model. `ResetMenuTests` fails when one is missing.

## Views and folders

`ViewModelToViewTemplate` finds the view of a model by name. The view has the type
name of the model, for example `Blur` for the model `Blur`. It looks at the
controls in the namespace `CMiX.Studio.Avalonia.Views` and in its child
namespaces. The child namespaces `Controls`, `Managers`, `Menus` and `Windows`
hold support controls, so the locator skips them. Two views with the same name
make the locator throw, and the message names both types.

The domain views keep the one namespace `CMiX.Studio.Avalonia.Views`, whatever the
folder. A view can move to another folder without a change to its namespace or its
`x:Class`. The folders are:

- `Views/Modulation`: the modulatable value editors, the modulator assign button
  and the modulator views.
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
| `Number` | one number | `DragValue`, `CMiXSlider`, `NumericInput`, `NumericText`, `IntegerValue`, `RangeValue`, `InputNumbersOnly` |
| `Vector` | several numbers | `Vector2`, `VectorXYZ`, `Integer2Value`, `Integer3Value`, `AxisSelector2D`, `AxisSelector3D` |
| `Boolean` | on, off, or a press | `CaptionedToggleButton`, `CMiXToggleButton`, `CaptionedButton` |
| `Enum` | one item of a fixed list | `CaptionedComboBox` |
| `String` | text or an address | `EditableTextBox`, `EditableValue`, `IPBox`, `SelectTextOnFocus` |
| `ColorPicker` | a color | `ColorSelector` and the color picker |
| `Parts` | nothing: what every editor is made of | `LabeledContent`, `CaptionedUserControl`, `DefaultResetMenu`, `ContextMenuGuard`, `DragEditHelper`, `PopupFrame`, `CMiXPopup` |

`AppIcon` and `CMiXListBox` stay in the root of `Controls`. They do not change a value.

To place a new control, ask "what data does it change?". A curve editor changes a
curve, so it gets a new folder `Curve`, even when the first use is a color adjustment.
The folder follows the data, not the screen. The screen is a view in the Studio.

A folder name is also a namespace, so it must not hide a .NET or Avalonia type that
the code in `CMiX.UI.Controls` uses by its simple name. That is why the folder for
colors is `ColorPicker`, not `Color`: the code uses the Avalonia type `Color`. Inside
the folders `String`, `Boolean` and `Enum`, write `string`, `bool` and `System.Enum`,
not `String`, `Boolean` and `Enum`. The folder `TextInput` does not exist for the
same reason: a namespace `TextBox` would hide the Avalonia `TextBox`.

The application connects three things to the library:

- `InteractionHost.BeginScopeHandler`, set in `App.Initialize`. See above.
- `Icons.Register`. The library has the generic icon keys. The Studio adds its
  own in `StudioIcons.Register`, also in `App.Initialize`, before the XAML loads.
  `IconKeyTests` fails when an `IconKey` in the AXAML is not registered.
- `IReorderService`, the type of the attached property
  `ListBoxReorderDragDrop.Service`. `ManagerReorderService` implements it.

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
| Panel popup | `PopupFlyoutStyle` | A `Flyout` for settings and other content. It is 300 wide. |

The themes and `PopupFrame` are in CMiX.UI.

`ContextMenuDefault` and `MenuFlyoutPresenterDefault` derive from the theme `MenuPanel`. A row is a `MenuItem` with the theme `ContextMenuSubItem`. A row with the header `-` is a separator. A row with `IsChecked` has the accent background.

A `MenuFlyout` whose rows come from a list needs one `ObservableCollection` that the code updates in place. A new `ItemsSource` does not refresh a menu that was shown before. `ModulatorAssignButton` is an example.

`PopupThemeGuardTests` fails when a `ContextMenu`, a `Flyout` or a `MenuFlyout` in an AXAML file of the Studio or of CMiX.UI does not use one of these themes.

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
are bound to the UI. The engine patch gives the peer a `MainloopQueue` through
`ConfigureEngineTransport (Provider, Options, Queue, Read Batch)`. A third
overload takes a credential name before the queue. It reads the generic
credential with that name and replaces the user name and the password of the
options. The patch does not use it yet. The patch drains the queue once per
frame with `Drain`. Every apply, every event and
every property change of the peer then run on the mainloop. The read batch is
the number of entries that one such action applies. The default is 256. A
larger value gives a faster catch-up and a longer frame. A patch that stops
draining stalls the peer. The patch calls `Close` after `SyncPeer.Stop`, so the
peer stops at once. `CMiX.Console` has no mainloop and applies inline.

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