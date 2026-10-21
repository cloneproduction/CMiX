// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Networking
{
    // The way each host starts its peer.
    public static class SyncPeerStartup
    {
        // The Studio reads and writes. It lists the peers and compacts the log.
        public static void StartAsStudio(this SyncPeer peer, ControlMessenger messenger, Action<Action> dispatcher, SyncOptions options)
        {
            peer.SetDispatcher(dispatcher);
            messenger.Register(peer);
            peer.CompactionEnabled = true;
            peer.IsWriter = true;
            peer.ListPeersEnabled = true;
            peer.Start(options with { Role = "studio" }, autoJoin: false);
        }

        // Engines read and apply. Only the Studio writes, so the engine peer is not a sender.
        // With a queue, the patch drains the queue once per frame, so every apply runs on the mainloop.
        // The read batch is the size of one such apply. Without a queue, every apply runs inline.
        public static void StartAsEngine(this SyncPeer peer, SyncOptions options, MainloopQueue queue = null, int readBatch = 256)
        {
            if (queue != null)
            {
                if (readBatch > 0)
                    peer.ReadBatch = readBatch;
                peer.SetDispatcher(queue.Post);
            }

            peer.Start(options, autoJoin: true);
        }
    }
}
