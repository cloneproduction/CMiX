// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Concurrent;
using System.Diagnostics;

namespace CMiX.Core.Networking
{
    // The engine peer posts its actions here from its background tasks. The patch runs them once
    // per frame on the mainloop, so the model changes only on the mainloop.
    public sealed class MainloopQueue
    {
        private readonly ConcurrentQueue<Action> _queue = new();
        private volatile bool _closed;

        public int Count => _queue.Count;

        // Returns at once. This is the delegate for SyncPeer.SetDispatcher.
        public void Post(Action action)
        {
            if (action == null)
                return;

            if (_closed)
                Run(action);
            else
                _queue.Enqueue(action);
        }

        // Runs the actions in order and returns how many it ran.
        public int Drain()
        {
            // An action that is posted during the drain waits for the next drain.
            var count = _queue.Count;
            var ran = 0;

            while (ran < count && _queue.TryDequeue(out var action))
            {
                Run(action);
                ran++;
            }

            return ran;
        }

        // The patch stops draining when it stops. An action that never runs makes SyncPeer.Stop
        // wait for its timeout. So after the close, every action runs at once.
        public void Close()
        {
            _closed = true;

            while (_queue.TryDequeue(out var action))
                Run(action);
        }

        private static void Run(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
