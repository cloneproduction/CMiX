// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Core.Networking;
using CMiX.Core.Networking.Messages;

namespace CMiX.Studio.Avalonia.Tests
{
    // A sync target that cannot apply a project.
    public sealed class ThrowingSyncTarget : ISyncTarget
    {
        public ProjectModel Capture() => throw new InvalidOperationException("The capture failed.");

        public void ApplySnapshot(ProjectModel model) => throw new InvalidOperationException("The apply failed.");

        public void Apply(IMessage message) => throw new InvalidOperationException("The apply failed.");
    }
}
