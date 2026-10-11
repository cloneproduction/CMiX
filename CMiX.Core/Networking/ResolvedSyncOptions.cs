// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Networking
{
    // The options a peer runs with, and a text that says where the user name came from.
    public record ResolvedSyncOptions(SyncOptions Options, string UserSource);
}
