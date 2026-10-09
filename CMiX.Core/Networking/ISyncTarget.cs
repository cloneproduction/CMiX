// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Core.Networking.Messages;

namespace CMiX.Core.Networking
{
    // The local state a peer syncs. All three calls run on the dispatcher thread.
    public interface ISyncTarget
    {
        ProjectModel Capture();

        void ApplySnapshot(ProjectModel model);

        void Apply(IMessage message);
    }
}
