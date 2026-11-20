// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.Collections.Generic;

namespace CMiX.UI.Behaviors
{
    public class PathsDroppedEventArgs : EventArgs
    {
        public PathsDroppedEventArgs(IReadOnlyList<string> paths)
        {
            Paths = paths;
        }

        public IReadOnlyList<string> Paths { get; }
    }
}
