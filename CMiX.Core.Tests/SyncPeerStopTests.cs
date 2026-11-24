// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static CMiX.Core.Tests.SyncTestHelpers;

namespace CMiX.Core.Tests
{
    // Stop and the connection state: no wait, no late store call, no store call on the dispatcher thread. The store is in memory. Peers apply inline, because no dispatcher is set.
    public class SyncPeerStopTests
    {
        [Fact]
        public void StartAndStop_WhenTheStoreNeverConnects_ReturnAtOnce()
        {
            var messenger = new ControlMessenger();
            using var peer = CreatePeer(new RecordingSyncTarget(), new HangingSyncStore(), messenger);

            // Each call must return before the stop timeout ends. A call that waits for the store would return after it.
            var timeout = Task.Delay(Fast.StopTimeout);
            peer.StartAndJoin(Options("A"));
            peer.SendMessage(new MessageOnClick(Guid.NewGuid()));
            Assert.False(timeout.IsCompleted);
            Assert.Equal("Connecting", peer.Status);
            Assert.True(messenger.IsSendingBlocked);
            Assert.Equal(1, peer.PendingMessages);

            timeout = Task.Delay(Fast.StopTimeout);
            peer.Stop();
            Assert.False(timeout.IsCompleted);
            Assert.Equal("Not started", peer.Status);
            Assert.Equal(StreamPosition.Zero, peer.LastAppliedId);
            Assert.Equal(StreamPosition.Zero, peer.TailId);
            Assert.Empty(peer.Peers);
        }

        // The loops write the positions through the dispatcher. The reset of Stop must go the same
        // way, or a queued write of a loop lands after it.
        [Fact]
        public async Task Stop_WithADispatcher_ResetsThePositionsThroughIt()
        {
            using var dispatcher = new SingleThreadDispatcher();
            var store = new WrappingSyncStore(new InMemorySyncStore());
            using var peer = CreatePeer(new RecordingSyncTarget(), store);
            peer.SetDispatcher(dispatcher.Post);

            peer.StartAndJoin(Options("A"));
            await WaitUntilAsync(() => peer.IsJoined);

            var timeout = Task.Delay(Fast.StopTimeout);
            peer.Stop();
            var returnedBeforeTheTimeout = !timeout.IsCompleted;

            var probe = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            dispatcher.Post(() => probe.SetResult(true));
            Assert.Same(probe.Task, await Task.WhenAny(probe.Task, Task.Delay(5000)));

            Assert.True(returnedBeforeTheTimeout, "Stop waited for the stop timeout.");
            Assert.Equal(StreamPosition.Zero, peer.LastSentId);
            Assert.Equal(StreamPosition.Zero, peer.LastAppliedId);
            Assert.Equal(StreamPosition.Zero, peer.TailId);
            Assert.False(peer.IsJoined);
            Assert.Empty(peer.Peers);
        }

        // A test on a real server deletes its keys after Stopped, so no late call may write one.
        [Fact]
        public async Task Stopped_OfAStop_CompletesAfterTheLastStoreCall()
        {
            var store = new WrappingSyncStore(new InMemorySyncStore());
            using var peer = CreatePeer(new RecordingSyncTarget(), store);

            peer.StartAndJoin(Options("A"));
            await WaitUntilAsync(() => peer.IsJoined);
            var stopped = peer.Stopped;
            peer.Stop();

            Assert.Same(stopped, await Task.WhenAny(stopped, Task.Delay(5000)));

            var calls = store.CallThreads.Count;
            await SettleAsync();

            Assert.Equal(calls, store.CallThreads.Count);
        }

        [Fact]
        public async Task StoreCalls_WithADispatcher_NeverRunOnItsThread()
        {
            using var dispatcher = new SingleThreadDispatcher();
            var store = new WrappingSyncStore(new InMemorySyncStore());
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store);
            peer.SetDispatcher(dispatcher.Post);

            peer.StartAndJoin(Options("A"));
            await WaitUntilAsync(() => peer.IsJoined);
            peer.SendMessage(new MessageOnClick(Guid.NewGuid()));
            await WaitUntilAsync(() => peer.SentMessages == 2);
            await store.AppendAsync(Envelope("other", new MessageOnClick(Guid.NewGuid())));
            await WaitUntilAsync(() => target.Applied.Count == 1);

            Assert.NotEmpty(store.CallThreads);
            Assert.DoesNotContain(dispatcher.ThreadId, store.CallThreads);
        }

        [Fact]
        public async Task LostConnection_ShowsTheStoreReason_AndAReconnectClearsIt()
        {
            var inner = new InMemorySyncStore();
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store);
            peer.StartAndJoin(Options("A"));
            await WaitUntilAsync(() => peer.IsJoined);

            inner.SimulateDisconnect("test reason");
            await WaitUntilAsync(() => peer.ErrorMessage == "test reason", detail: () => $"error={peer.ErrorMessage}");

            inner.SimulateReconnect();
            await WaitUntilAsync(() => peer.ErrorMessage.Length == 0, detail: () => $"error={peer.ErrorMessage}");
        }

        [Fact]
        public async Task OnConnectionChanged_AfterAJoinError_KeepsTheJoinError()
        {
            var inner = new InMemorySyncStore();
            var store = new WrappingSyncStore(inner);
            var target = new RecordingSyncTarget();
            using var peer = CreatePeer(target, store);
            peer.StartAndJoin(Options("A"));
            await WaitUntilAsync(() => peer.IsJoined);

            inner.SimulateDisconnect("test reason");
            await WaitUntilAsync(() => peer.ErrorMessage == "test reason", detail: () => $"error={peer.ErrorMessage}");

            store.Fail = true;
            await peer.JoinAsync();
            var joinError = peer.ErrorMessage;
            Assert.NotEmpty(joinError);
            Assert.NotEqual("test reason", joinError);

            store.Fail = false;
            inner.SimulateReconnect();
            await WaitUntilAsync(() => peer.IsConnected, detail: () => $"status={peer.Status}");

            Assert.Equal(joinError, peer.ErrorMessage);
        }
    }
}
