// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Networking
{
    // Connection and identity settings for one peer.
    public record SyncOptions(string Ip, int Port, int Database, string User, string Password, string KeyPrefix, string PeerName, string Role)
    {
        public static readonly SyncOptions Defaults = new("127.0.0.1", 6379, 0, "default", "", "cmix:default", "", "");

        // Password, PeerName and Role keep their value. An empty password means no password, and
        // the caller sets name and role.
        public SyncOptions WithFallbacks() => new(
            string.IsNullOrEmpty(Ip) ? Defaults.Ip : Ip,
            Port <= 0 ? Defaults.Port : Port,
            Database,
            string.IsNullOrEmpty(User) ? Defaults.User : User,
            Password ?? Defaults.Password,
            string.IsNullOrEmpty(KeyPrefix) ? Defaults.KeyPrefix : KeyPrefix,
            PeerName ?? Defaults.PeerName,
            Role ?? Defaults.Role);

        // The vvvv patch calls this as a node. Keep the parameter list stable.
        public static SyncOptions Create(string ip, int port, int database, string user, string password, string keyPrefix, string peerName)
            => new SyncOptions(ip, port, database, user, password, keyPrefix, peerName, "engine").WithFallbacks();
    }
}
