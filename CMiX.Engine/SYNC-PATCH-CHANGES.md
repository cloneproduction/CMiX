# Engine patch changes for the Redis sync

The sync no longer uses WatsonTcp. Redis Streams carry the updates. The Studio writes to
Redis. Every engine reads from Redis and applies what it reads.

This list tells you what to change by hand in `CMiX_ENGINE.Definitions.vl`. The line numbers
are the state of the file at the time of this list. Open the patch in vvvv and find the nodes
by their names. Use the line numbers only to confirm that you found the correct node.

The containers are at these lines:

| Container | Line |
|---|---|
| `Editors` | 44131 |
| `Messenger` | 45082 |
| `Init` | 45524 |
| `MainManager` | 45643 |

## 1. Remove the receive chain in the `Messenger` container

Remove these nodes and pads:

- `Client.WatsonTcpClient`, line 45088.
- `WatsonTcpClient.Connect` in the `If` region at line 45169, node at line 45179. Remove the
  region and its `Condition` pad at line 45190.
- `WatsonTcpClient.Disconnect`, line 45199.
- The `If` region at line 45218 with `WatsonTcpClient.Connected` at line 45228 and the
  `Connected` pad at line 45238.
- `WatsonTcpClient.Events`, line 45239.
- `WatsonTcpClientEvents.MessageReceived`, line 45250.
- The `ForEach` region at line 45260 with `WatsonTcp.MessageReceivedEventArgs.Data` at line
  45274 and `Reactive.EventPattern.EventArgs` at line 45284.
- The `ForEach (Keep)` region at line 45300. Its `Output 1` pin has the type annotation
  `MessageEnvelope` at line 45314. It holds `Serialization.MessagePack.Deserialize` at line
  45320.
- The debug ring buffer: `Reactive.HoldLatest` at line 45344, `Primitive.Object.GetType` at
  line 45357, `System.Conversion.ToString` at line 45366, `Collections.Spread.Queue` at line
  45374, the large IOBox at line 45386, the `Frame Count` pad at line 45423, the `Clear` pad
  at line 45428 and the `On Data` pad at line 45436.
- `Primitive.Object.IsAssigned`, line 45439.
- `WatsonTcpClient.Dispose`, line 45448.
- `Client.DeconnectionReason`, line 45159, and its `Deconnection Reason` pad at line 45168.

A grep of the file for `MessageValueChanged` and for `IControlModel` gives no hit. An earlier
review listed an orphaned `MessageValueChanged.Create` node with an `IControlModel` slot. That
node is no longer in the file. There is nothing to remove for it.

## 2. Remove the start of the client

Remove the `If` region at line 45097 with the `Client.Start` node at line 45107. Remove the
`Ip` pad at line 45118 and the `Port` pad at line 45123.

## 3. Remove the WatsonTcp package

Remove the line

    <NugetDependency Id="FU5tfxe8VkaNWXN1K2FCx6" Location="WatsonTcp" Version="6.0.12" />

at line 46559.

## 4. Change the transport in the `Init` container

Replace the `InjectionBuilder.ConfigureVvvvTransport(Provider)` node at line 45600 with
`InjectionBuilder.ConfigureEngineTransport(Provider, Options)`.

Build `Options` with the node `SyncOptions.Create(Ip, Port, Database, User, Password,
KeyPrefix, PeerName)`. The seven inputs come from the settings reader of the engine. You
implement that reader in vvvv.

`Create` sets the role to `engine`. It then applies the fallbacks:

| Input | Fallback |
|---|---|
| `Ip` empty | `127.0.0.1` |
| `Port` 0 or below | `6379` |
| `Database` | no fallback, the value goes to Redis as it is, and `0` is the default |
| `User` empty | `default` |
| `Password` empty | no password |
| `KeyPrefix` empty | `cmix:default` |
| `PeerName` empty | the machine name |

Keep the `InjectionBuilder.ConfigureAllServices` node at line 45565 and the
`InjectionBuilder.ConfigureVvvvServices` node at line 45575 as they are.

## 5. Remove the `GetRequiredService` node for `Client`

The file has three `GetRequiredService` nodes:

| Line | Container | What it does |
|---|---|---|
| 44138 | `Editors` | gives a `ManagerData`, and stays |
| 45147 | `Messenger` | gives a `Client`, and goes |
| 45984 | `MainManager` | the generic node with a `Service Type` pin, and stays |

The node at line 45147 feeds the input of the `Client.WatsonTcpClient` node at line 45088.
Remove the node with that node. The other two nodes stay.

## 6. Optional: show the status of the peer

Get the `SyncPeer` singleton with `GetRequiredService<SyncPeer>`. Connect IOBoxes to these
outputs:

- `Status`
- `IsInSync`
- `LastAppliedId`
- `AppliedMessages`
- `ErrorMessage`

## 7. The `MainManager` container does not change

The controls still arrive through the collection events of `ControlRepository.Controls`. That
node is at line 45794.

## 8. The reference to CMiX.Core

The patch references the Core project at line 46563:

    <ProjectDependency Location="../CMiX.Core/CMiX.Core.csproj" />

vvvv builds `CMiX.Core` and gets `StackExchange.Redis` 2.8.31 and its dependencies from it. Do
not add a package reference for Redis to the patch. If you add VL.IO.Redis later, it must stay
on `StackExchange.Redis` 2.8.31, the version `CMiX.Core` pins.

## 9. The `Thrash` copies

The patch copies in the `Thrash` folder still reference WatsonTcp. vvvv does not load them.
Do not change them.

## 10. What the engine does not do

- The engine never appends to the stream. `IsWriter` is false for an engine.
- The engine never pushes.
- On an empty store the engine joins at position zero. It then waits for the Studio.
