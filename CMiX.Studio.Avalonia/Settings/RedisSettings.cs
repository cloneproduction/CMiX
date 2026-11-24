// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

namespace CMiX.Studio.Avalonia.Settings
{
    // UseCredentialManager and CredentialName take the user name and the password from a generic
    // credential of the Windows Credential Manager. The file values stay the fallback.
    public class RedisSettings
    {
        public string Ip { get; set; } = "";
        public int Port { get; set; } = 0;
        public int Database { get; set; } = 0;
        public bool UseCredentialManager { get; set; } = false;
        public string CredentialName { get; set; } = "";
        public string User { get; set; } = "";
        public string Password { get; set; } = "";
        public string KeyPrefix { get; set; } = "";
        public string PeerName { get; set; } = "";
    }
}
