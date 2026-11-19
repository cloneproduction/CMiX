// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VL.Serialization.MessagePack;

namespace CMiX.Core.Networking
{
    public partial class SyncPeer
    {
        private sealed record DecodedEntry(StreamEntry Entry, MessageEnvelope Envelope, bool IsOwn);

        private async Task ReplayAsync(SyncRun run, StreamPosition position)
        {
            var ct = run.Cts.Token;
            while (true)
            {
                ct.ThrowIfCancellationRequested();
                var entries = await run.Store.ReadRangeAsync(position, run.Timings.ReadBatch).ConfigureAwait(false);
                if (entries.Count == 0) return;
                if (!IsCurrent(run)) return;

                await ApplyEntriesAsync(run, entries).ConfigureAwait(false);
                if (!IsCurrent(run)) return;

                position = entries[entries.Count - 1].Id;
            }
        }

        // The run is that of the reader that found the entries. Entries that were read before a
        // restart must change nothing after it. The batch runs as one action on the dispatcher
        // thread, and Start and Stop run on that thread too, so one run check at the start of the
        // action covers the whole batch. One action per batch keeps a frame based dispatcher from
        // applying one entry per frame.
        private async Task ApplyEntriesAsync(SyncRun run, IReadOnlyList<StreamEntry> entries)
        {
            if (entries.Count == 0) return;

            var batch = new List<DecodedEntry>(entries.Count);
            foreach (var entry in entries)
            {
                MessageEnvelope envelope = null;
                try
                {
                    envelope = MessagePackSerialization.Deserialize<MessageEnvelope>(new ReadOnlyMemory<byte>(entry.Envelope));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                var own = envelope == null || envelope.SenderID == _peerId || envelope.Payload == null;
                batch.Add(new DecodedEntry(entry, envelope, own));
            }

            await DispatchCurrentAsync(run, () =>
            {
                foreach (var decoded in batch)
                {
                    var entry = decoded.Entry;
                    var envelope = decoded.Envelope;
                    var own = decoded.IsOwn;
                    var payload = envelope?.Payload;
                    // An append that timed out on the client but reached the store is sent again.
                    // The entry then arrives twice, and a move applied twice gives a wrong order.
                    var apply = !own && !_applied.IsDuplicate(envelope.MessageID);

                    if (apply)
                    {
                        try
                        {
                            _target.Apply(payload);
                            if (payload is MessageProjectSnapshot)
                            {
                                _applied.Forget();
                                SnapshotApplied?.Invoke(this, EventArgs.Empty);
                            }

                            AppliedMessages++;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                            ErrorMessage = ex.Message;
                        }
                    }

                    LastAppliedId = entry.Id;
                    if (entry.Id > TailId) TailId = entry.Id;

                    if (apply) MessageApplied?.Invoke(this, new MessageAppliedEventArgs(entry, payload));
                }
            }).ConfigureAwait(false);
        }

        private void ApplySnapshotOnDispatcher(ProjectModel model, StreamPosition position)
        {
            _target.ApplySnapshot(model);
            _applied.Forget();
            SnapshotApplied?.Invoke(this, EventArgs.Empty);
            LastAppliedId = position;
            if (position > TailId) TailId = position;
        }
    }
}
