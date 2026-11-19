// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using Avalonia.Threading;

namespace CMiX.Studio.Avalonia.Views
{
    // A posted action runs only while the view is attached.
    internal sealed class TreeViewLifetime
    {
        public bool IsAttached { get; private set; }

        public void Attach() => IsAttached = true;

        public void Detach() => IsAttached = false;

        public void Post(Action action, DispatcherPriority priority) =>
            Dispatcher.UIThread.Post(() =>
            {
                if (IsAttached)
                    action();
            }, priority);
    }
}
