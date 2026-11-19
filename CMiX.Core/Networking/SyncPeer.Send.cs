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
        public void SendMessage(IMessage message)
        {
            if (message == null || !IsWriter) return;

            var envelope = new MessageEnvelope
            {
                SenderID = _peerId,
                MessageID = Guid.NewGuid(),
                Payload = message
            };
            _outgoing.Enqueue(MessagePackSerialization.Serialize(envelope));
            // Through the dispatcher, so this notification and the one after the append keep their order.
            Dispatch(() => OnPropertyChanged(nameof(PendingMessages)));
            MessageSent?.Invoke(this, new MessageSentEventArgs(message));
        }

        // The position comes first. The count comes after the entry left the queue, so a reader of
        // both never sees a caught-up peer while the entry is still on its way. Returns false when
        // the run is over. The entry then belongs to the store of that run only.
        private Task<bool> OnSentAsync(SyncRun run, StreamPosition id)
            => DispatchCurrentAsync(run, () =>
            {
                SentMessages++;
                LastSentId = id;
            });

        private Task OnDrainedAsync(SyncRun run)
            => DispatchCurrentAsync(run, () => OnPropertyChanged(nameof(PendingMessages)));
    }
}
