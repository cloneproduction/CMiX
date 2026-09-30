# CMiX

CMiX is a VJ (visual jockey) tool. It pairs a desktop Studio application, used
to build and control compositions live, with a vvvv gamma engine that renders
the output. The Studio and the engines exchange their state through a Redis
server, so composing and rendering can run on separate machines.

## Repo layout

- CMiX.Core: shared domain model and services (compositions, layers, prefabs,
  networking, undo) used by both the Studio and the engine.
- CMiX.Studio.Avalonia: the Studio UI, built on Avalonia. This is where
  active UI work happens.
- CMiX.Engine: the vvvv gamma patch that renders the composition output. The
  patch references CMiX.Core with a project dependency, so vvvv builds the
  project and gets its packages from it.
- CMiX.Console: a headless engine. It joins the sync and applies the updates,
  without a window.
- CMiX.Core.Tests and CMiX.Studio.Avalonia.Tests: automated tests for the
  shared model and the Avalonia UI respectively.

## Build

    dotnet build CMiX.Studio.Avalonia\CMiX.Studio.Avalonia.csproj

## Test

    dotnet test CMiX.Core.Tests\CMiX.Core.Tests.csproj
    dotnet test CMiX.Studio.Avalonia.Tests\CMiX.Studio.Avalonia.Tests.csproj

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
It remembers the last 1000 message IDs for this. On the Studio the apply runs
on the UI thread.

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

**The UI.** No Redis call runs on the UI thread. The one accepted UI pause is
the apply of a large snapshot, because the controls are bound to the UI.

### The Studio settings file

The Studio reads `studio-settings.json` from the directory of its executable.
The file holds one `Redis` object with seven fields:

```json
{
  "Redis": {
    "Ip": "127.0.0.1",
    "Port": 6379,
    "Database": 0,
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
| `User` | `default` |
| `Password` | no password |
| `KeyPrefix` | `cmix:default` |
| `PeerName` | `Studio` |

The password is clear text for now. A later step can read the user name and the
password from the Windows Credential Manager. That is out of scope here.

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
    --help              Show this usage text.

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
