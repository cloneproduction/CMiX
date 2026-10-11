// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Core.Networking
{
    // Reads one generic credential of the Windows Credential Manager.
    public interface ICredentialStore
    {
        // Returns false when the name is empty or no credential has that name. The out values are then empty.
        bool TryRead(string name, out string user, out string password);
    }
}
