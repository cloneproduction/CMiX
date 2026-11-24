// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.IO;
using CMiX.Core.Networking;

namespace CMiX.Studio.Avalonia.Settings
{
    // Redis connection settings for Studio. Studio reads this file from studio-settings.json,
    // next to the executable. Example file content:
    //
    // {
    //   "Redis": {
    //     "Ip": "127.0.0.1",
    //     "Port": 6379,
    //     "Database": 0,
    //     "UseCredentialManager": false,
    //     "CredentialName": "",
    //     "User": "default",
    //     "Password": "",
    //     "KeyPrefix": "cmix:default",
    //     "PeerName": "Studio"
    //   }
    // }
    public class StudioSettings
    {
        public const string FileName = "studio-settings.json";

        public static string DefaultPath => Path.Combine(AppContext.BaseDirectory, FileName);

        public RedisSettings Redis { get; set; } = new();

        public InterfaceSettings Interface { get; set; } = new();

        // The file values with fallbacks. Resolve adds the Credential Manager.
        // WithFallbacks would give the machine name, so the Studio sets its own default first.
        public SyncOptions ToSyncOptions()
        {
            var peerName = string.IsNullOrEmpty(Redis.PeerName) ? "Studio" : Redis.PeerName;
            return new SyncOptions(Redis.Ip, Redis.Port, Redis.Database, Redis.User, Redis.Password, Redis.KeyPrefix, peerName, "studio")
                .WithFallbacks();
        }

        public ResolvedSyncOptions Resolve(ICredentialStore store)
        {
            var name = Redis.UseCredentialManager ? Redis.CredentialName : string.Empty;
            return ToSyncOptions().WithCredential(name, store, "file");
        }

        public static StudioSettings FromSyncOptions(SyncOptions options) => new()
        {
            Redis = new RedisSettings
            {
                Ip = options.Ip,
                Port = options.Port,
                Database = options.Database,
                User = options.User,
                Password = options.Password,
                KeyPrefix = options.KeyPrefix,
                PeerName = options.PeerName
            }
        };
    }
}
