using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using static CMiX.Core.Tests.SyncTestHelpers;

namespace CMiX.Core.Tests
{
    // Starts a Memurai of its own on a free port, kills it, and starts it again. The service on
    // 6379 is never touched.
    internal sealed class PrivateMemurai : IDisposable
    {
        private readonly string _exePath;
        private readonly string[] _arguments;
        private Process _process;

        public PrivateMemurai(string exePath, int port, params string[] arguments)
        {
            _exePath = exePath;
            Port = port;
            _arguments = arguments;
        }

        public int Port { get; }

        public void Start()
        {
            if (_process != null)
                throw new InvalidOperationException("The server already runs.");

            var info = new ProcessStartInfo(_exePath)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            info.ArgumentList.Add("--port");
            info.ArgumentList.Add(Port.ToString(CultureInfo.InvariantCulture));
            foreach (var argument in _arguments)
                info.ArgumentList.Add(argument);

            var process = Process.Start(info);

            // The pipes must be drained, or a full pipe stops the server.
            process.OutputDataReceived += (_, _) => { };
            process.ErrorDataReceived += (_, _) => { };
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            _process = process;

            if (!WaitForPort(true, TimeSpan.FromSeconds(10)))
                throw new TimeoutException($"Memurai did not answer on port {Port}.");
        }

        public void Kill()
        {
            var process = _process;
            _process = null;
            if (process == null)
                return;

            try
            {
                if (!process.HasExited)
                    process.Kill();

                process.WaitForExit(10000);
            }
            catch (Exception)
            {
            }
            finally
            {
                process.Dispose();
            }

            WaitForPort(false, TimeSpan.FromSeconds(10));
        }

        public void Dispose() => Kill();

        private bool WaitForPort(bool answers, TimeSpan timeout)
        {
            var watch = Stopwatch.StartNew();
            while (watch.Elapsed < timeout)
            {
                if (Answers() == answers)
                    return true;

                Thread.Sleep(100);
            }

            return Answers() == answers;
        }

        // A connect alone is not enough. Redis listens while it still loads its data.
        private bool Answers()
        {
            try
            {
                using var client = new TcpClient();
                var connect = client.BeginConnect("127.0.0.1", Port, null, null);
                if (!connect.AsyncWaitHandle.WaitOne(500))
                    return false;

                client.EndConnect(connect);

                var stream = client.GetStream();
                stream.ReadTimeout = 1000;
                var request = Encoding.ASCII.GetBytes("PING\r\n");
                stream.Write(request, 0, request.Length);

                var buffer = new byte[16];
                var read = stream.Read(buffer, 0, buffer.Length);
                return read > 0 && Encoding.ASCII.GetString(buffer, 0, read).StartsWith("+PONG", StringComparison.Ordinal);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    // Drives whole peers over a private Memurai, and kills that server to make an outage. The
    // Memurai service on 6379 keeps running, so the other integration tests are not disturbed.
    public sealed class SyncPeerRedisOutageTests : IAsyncLifetime
    {
        private const string MemuraiPath = @"C:\Program Files\Memurai\memurai.exe";
        private const int Port = 6380;
        private const string SkipReason = "Memurai is not installed at " + MemuraiPath;

        private readonly ITestOutputHelper _output;
        private readonly string _prefix = "cmix:test:" + Guid.NewGuid().ToString("N").Substring(0, 8);
        private readonly List<SyncPeer> _peers = new();

        public SyncPeerRedisOutageTests(ITestOutputHelper output) => _output = output;

        public Task InitializeAsync() => Task.CompletedTask;

        public Task DisposeAsync()
        {
            DisposePeers();
            return Task.CompletedTask;
        }

        private void DisposePeers()
        {
            foreach (var peer in _peers)
                peer.Dispose();

            _peers.Clear();
        }

        private static ProjectModel ModelWithOneComposition()
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            return (ProjectModel)project.ToModel();
        }

        private static Guid OrderedGuid(int index) => new Guid(index, 0, 0, new byte[8]);

        private static List<Guid> Clicks(RecordingSyncTarget target) =>
            target.Applied.OfType<MessageOnClick>().Select(message => message.ID).ToList();

        private SyncOptions Options(string name, string role) =>
            SyncOptions.Defaults with { Port = Port, KeyPrefix = _prefix, PeerName = name, Role = role };

        private SyncPeer NewPeer(ISyncTarget target, bool isWriter = true)
        {
            var peer = new SyncPeer(target, new ControlMessenger(), o => new RedisSyncStore(o)) { IsWriter = isWriter };
            _peers.Add(peer);
            return peer;
        }

        private async Task<SyncPeer> StartStudioAsync(ISyncTarget target)
        {
            var peer = NewPeer(target);
            peer.ListPeersEnabled = true;
            peer.CompactionEnabled = true;
            peer.CompactionDelay = TimeSpan.FromMilliseconds(500);
            peer.Start(Options("Studio", "studio"), autoJoin: false);
            await WaitUntilAsync(() => peer.IsJoined, 15000, () => $"status={peer.Status} error={peer.ErrorMessage}");
            return peer;
        }

        private async Task<SyncPeer> StartEngineAsync(string name, ISyncTarget target)
        {
            var peer = NewPeer(target, isWriter: false);
            peer.Start(Options(name, "engine"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined, 15000, () => $"status={peer.Status} error={peer.ErrorMessage}");
            return peer;
        }

        // A second writer, so the engine-to-Studio direction stays tested even though the engine
        // peer itself no longer writes.
        private async Task<SyncPeer> StartWriterAsync(string name, ISyncTarget target)
        {
            var peer = NewPeer(target);
            peer.Start(Options(name, "studio"), autoJoin: true);
            await WaitUntilAsync(() => peer.IsJoined, 15000, () => $"status={peer.Status} error={peer.ErrorMessage}");
            return peer;
        }

        private async Task<RedisSyncStore> ObserverStoreAsync()
        {
            var store = new RedisSyncStore(Options("observer", "test"));
            await store.ConnectAsync(CancellationToken.None);
            Assert.True(store.IsConnected);
            return store;
        }

        private async Task<(SyncPeer Studio, RecordingSyncTarget StudioTarget, SyncPeer Engine, RecordingSyncTarget EngineTarget)>
            StartPairAsync()
        {
            var studioTarget = new RecordingSyncTarget { Model = ModelWithOneComposition() };
            var studio = await StartStudioAsync(studioTarget);
            var engineTarget = new RecordingSyncTarget();
            var engine = await StartEngineAsync("Engine1", engineTarget);
            await WaitUntilAsync(() => engine.LastAppliedId == studio.LastAppliedId, 15000,
                () => $"studio={studio.LastAppliedId} engine={engine.LastAppliedId}");

            return (studio, studioTarget, engine, engineTarget);
        }

        private static List<Guid> Send(SyncPeer peer, int firstIndex, int count)
        {
            var ids = new List<Guid>();
            for (var i = 0; i < count; i++)
            {
                var id = OrderedGuid(firstIndex + i);
                ids.Add(id);
                peer.SendMessage(new MessageOnClick(id));
            }

            return ids;
        }

        // A leftover server from a killed test run still answers here. The test skips instead of
        // starting a second server that fails to bind and runs against the stale one.
        private static bool PortIsBusy(int port)
        {
            try
            {
                using var client = new TcpClient();
                var connect = client.BeginConnect("127.0.0.1", port, null, null);
                if (!connect.AsyncWaitHandle.WaitOne(1000))
                    return false;

                client.EndConnect(connect);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [SkippableFact]
        public async Task Peers_SurviveAnOutage_AndCatchUp()
        {
            Skip.IfNot(File.Exists(MemuraiPath), SkipReason);
            Skip.If(PortIsBusy(Port), $"Port {Port} is busy. Stop the process that uses it.");

            using var directory = new TempDirectoryFixture();
            var server = new PrivateMemurai(MemuraiPath, Port, "--save", "", "--appendonly", "yes", "--dir", directory.Path);
            try
            {
                server.Start();

                var (a, studioTarget, b, engineTarget) = await StartPairAsync();
                var writer2 = await StartWriterAsync("Writer2", new RecordingSyncTarget());

                var fromA = Send(a, 0, 5);
                await WaitUntilAsync(() => Clicks(engineTarget).Count == 5, 15000, () => $"applied={Clicks(engineTarget).Count}");

                var watch = Stopwatch.StartNew();
                server.Kill();
                await WaitUntilAsync(() => !a.IsConnected && !b.IsConnected, 30000, () => $"studio={a.Status} engine={b.Status}");
                var detected = watch.ElapsedMilliseconds;

                Assert.Equal("Reconnecting", a.Status);

                fromA.AddRange(Send(a, 5, 5));
                // The engine does not write. Writer2 stands in for the engine-to-Studio direction.
                var fromB = Send(writer2, 100, 3);

                Assert.True(a.PendingMessages >= 5, $"The studio holds {a.PendingMessages} messages.");
                Assert.True(writer2.PendingMessages >= 3, $"Writer2 holds {writer2.PendingMessages} messages.");

                await Task.Delay(5000);
                Assert.True(a.IsJoined, "The studio left the sync during the outage.");
                Assert.True(b.IsJoined, "The engine left the sync during the outage.");

                watch.Restart();
                server.Start();
                await WaitUntilAsync(() => a.IsConnected && b.IsConnected, 60000, () => $"studio={a.Status} engine={b.Status}");
                var reconnected = watch.ElapsedMilliseconds;

                watch.Restart();
                await WaitUntilAsync(() => a.PendingMessages == 0 && writer2.PendingMessages == 0, 30000,
                    () => $"studio={a.PendingMessages} writer2={writer2.PendingMessages}");
                // The engine applies every foreign sender, so it sees Writer2's 3 clicks too.
                await WaitUntilAsync(() => Clicks(engineTarget).Count == 13 && Clicks(studioTarget).Count == 3, 30000,
                    () => $"engine={Clicks(engineTarget).Count} studio={Clicks(studioTarget).Count}");
                await WaitUntilAsync(() => a.LastAppliedId == b.LastAppliedId && a.IsInSync && b.IsInSync, 30000,
                    () => $"studio={a.LastAppliedId}/{a.IsInSync} engine={b.LastAppliedId}/{b.IsInSync}");
                var caughtUp = watch.ElapsedMilliseconds;

                // The engine's applied clicks interleave fromA and fromB. Each sender's own
                // clicks stay in order.
                var engineClicks = Clicks(engineTarget);
                Assert.Equal(fromA, engineClicks.Where(id => fromA.Contains(id)).ToList());
                Assert.Equal(fromB, engineClicks.Where(id => fromB.Contains(id)).ToList());
                Assert.Equal(fromB, Clicks(studioTarget));

                var lateTarget = new RecordingSyncTarget();
                var c = await StartEngineAsync("Engine2", lateTarget);
                await WaitUntilAsync(() => c.LastAppliedId == a.LastAppliedId, 15000,
                    () => $"studio={a.LastAppliedId} late={c.LastAppliedId}");

                Assert.Equal(ProjectStateHash.Compute(studioTarget.Model), ProjectStateHash.Compute(lateTarget.Model));

                _output.WriteLine($"Outage seen after {detected} ms. Reconnected after {reconnected} ms. Caught up after {caughtUp} ms.");
            }
            finally
            {
                DisposePeers();
                server.Dispose();
            }
        }

        [SkippableFact]
        public async Task Peers_SurviveAnOutage_ThatLosesTheData()
        {
            Skip.IfNot(File.Exists(MemuraiPath), SkipReason);
            Skip.If(PortIsBusy(Port), $"Port {Port} is busy. Stop the process that uses it.");

            var server = new PrivateMemurai(MemuraiPath, Port, "--save", "", "--appendonly", "no");
            try
            {
                server.Start();

                var (a, studioTarget, b, engineTarget) = await StartPairAsync();
                var writer2 = await StartWriterAsync("Writer2", new RecordingSyncTarget());

                Send(a, 0, 5);
                await WaitUntilAsync(() => Clicks(engineTarget).Count == 5, 15000, () => $"applied={Clicks(engineTarget).Count}");

                var beforeOutage = a.LastAppliedId;

                var watch = Stopwatch.StartNew();
                server.Kill();
                await WaitUntilAsync(() => !a.IsConnected && !b.IsConnected, 30000, () => $"studio={a.Status} engine={b.Status}");
                var detected = watch.ElapsedMilliseconds;

                Send(a, 5, 5);
                // The engine does not write. Writer2 stands in for the engine-to-Studio direction.
                Send(writer2, 100, 3);
                await Task.Delay(5000);

                watch.Restart();
                server.Start();
                await WaitUntilAsync(() => a.IsConnected && b.IsConnected, 60000, () => $"studio={a.Status} engine={b.Status}");
                var reconnected = watch.ElapsedMilliseconds;

                await Task.Delay(15000);

                await using var store = await ObserverStoreAsync();
                var snapshot = await store.ReadSnapshotAsync();
                var tail = await store.ReadTailAsync();
                var entries = await store.ReadRangeAsync(StreamPosition.Zero, 1000);

                _output.WriteLine($"Outage seen after {detected} ms. Reconnected after {reconnected} ms.");
                _output.WriteLine($"Position before the outage: {beforeOutage}.");
                _output.WriteLine($"Studio: status={a.Status} joined={a.IsJoined} pending={a.PendingMessages} " +
                    $"lastApplied={a.LastAppliedId} tail={a.TailId} sent={a.SentMessages} applied={a.AppliedMessages} error='{a.ErrorMessage}'");
                _output.WriteLine($"Engine: status={b.Status} joined={b.IsJoined} pending={b.PendingMessages} " +
                    $"lastApplied={b.LastAppliedId} tail={b.TailId} sent={b.SentMessages} applied={b.AppliedMessages} error='{b.ErrorMessage}'");
                _output.WriteLine($"Store: snapshot={(snapshot == null ? "none" : snapshot.StreamId.ToString())} tail={tail} entries={entries.Count}");
                _output.WriteLine($"Studio applied {Clicks(studioTarget).Count} clicks. Engine applied {Clicks(engineTarget).Count} clicks.");

                Assert.True(a.IsConnected, "The studio did not reconnect.");
                Assert.True(b.IsConnected, "The engine did not reconnect.");

                // The studio writes the snapshot again, so a late engine still finds the state.
                await WaitUntilAsync(() => store.ReadSnapshotIdAsync().GetAwaiter().GetResult() != StreamPosition.Zero,
                    10000, () => "The store has no snapshot.");

                var lateTarget = new RecordingSyncTarget();
                var c = await StartEngineAsync("Engine2", lateTarget);
                await WaitUntilAsync(() => c.LastAppliedId == a.LastAppliedId, 15000,
                    () => $"studio={a.LastAppliedId} late={c.LastAppliedId}");

                Assert.Equal(ProjectStateHash.Compute(studioTarget.Model), ProjectStateHash.Compute(lateTarget.Model));
            }
            finally
            {
                DisposePeers();
                server.Dispose();
            }
        }
    }
}
