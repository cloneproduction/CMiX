// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics;
using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;
using VL.Serialization.MessagePack;

namespace CMiX.Core.Networking
{
    public partial class SyncPeer
    {
        private readonly record struct DecodedEntry(StreamEntry Entry, MessageEnvelope Envelope, bool IsOwn);

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

        // One action per batch. The run check inside it covers the whole batch.
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
                    var (entry, envelope, own) = decoded;
                    var payload = envelope?.Payload;
                    // A timed out append that reached the store is sent again, so an entry can arrive twice.
                    var apply = !own && !_applied.IsDuplicate(envelope.MessageID);

                    if (apply)
                    {
                        try
                        {
                            _target.Apply(payload);
                            // A snapshot entry forgets the applied IDs and raises SnapshotApplied.
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

                    // Own entries and duplicates also move the position.
                    LastAppliedId = entry.Id;
                    if (entry.Id > TailId) TailId = entry.Id;

                    // MessageApplied comes after the position.
                    if (apply) MessageApplied?.Invoke(this, new MessageAppliedEventArgs(entry, payload));
                }
            }).ConfigureAwait(false);
        }

        private void ApplySnapshotOnDispatcher(ProjectModel model, StreamPosition position)
        {
            _target.ApplySnapshot(model);
            _applied.Forget();
            SnapshotApplied?.Invoke(this, EventArgs.Empty);
            // The positions come after the snapshot, Forget and SnapshotApplied.
            LastAppliedId = position;
            if (position > TailId) TailId = position;
        }
    }
}
