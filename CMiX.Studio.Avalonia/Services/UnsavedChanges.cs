// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core;
using CMiX.Core.Compositing;

namespace CMiX.Studio.Avalonia.Services
{
    // Tells whether the project differs from the last time it was saved, opened or started new.
    // It compares the fingerprint of the whole project, so any change that a file would keep counts.
    public sealed class UnsavedChanges
    {
        private readonly Project _project;
        private string? _savedFingerprint;

        public UnsavedChanges(Project project)
        {
            _project = project;
        }

        // Before the first MarkSaved the project counts as changed.
        public bool HasChanges => _savedFingerprint != ProjectStateHash.Compute(_project);

        public void MarkSaved() => _savedFingerprint = ProjectStateHash.Compute(_project);
    }
}
