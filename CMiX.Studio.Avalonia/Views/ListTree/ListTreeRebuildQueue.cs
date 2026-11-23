// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia.Threading;

namespace CMiX.Studio.Avalonia.Views
{
    // One rebuild of the rows for a burst of changes of the tree, such as a close of all cards.
    internal sealed class ListTreeRebuildQueue
    {
        private readonly Action rebuild;

        public ListTreeRebuildQueue(Action rebuild)
        {
            this.rebuild = rebuild;
        }

        public bool IsPosted { get; private set; }

        public void Post()
        {
            if (IsPosted)
                return;

            IsPosted = true;
            Dispatcher.UIThread.Post(Run);
        }

        // A part that needs the new places at once, such as a reveal or a scroll anchor, runs the posted rebuild now.
        public void Run()
        {
            if (!IsPosted)
                return;

            IsPosted = false;
            rebuild();
        }
    }
}
