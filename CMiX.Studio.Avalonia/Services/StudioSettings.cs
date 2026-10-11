// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.IO;
using System.Text.Json;
using CMiX.Core.Networking;

namespace CMiX.Studio.Avalonia.Services
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
    //
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

    public class StudioSettings
    {
        public const string FileName = "studio-settings.json";

        public static string DefaultPath => Path.Combine(AppContext.BaseDirectory, FileName);

        public RedisSettings Redis { get; set; } = new();

        private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

        public static StudioSettings Load() => Load(DefaultPath);

        // A missing, empty, broken or locked file falls back to defaults. The Studio must start
        // in every case.
        public static StudioSettings Load(string path)
        {
            try
            {
                if (!File.Exists(path)) return new StudioSettings();

                var json = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(json)) return new StudioSettings();

                return JsonSerializer.Deserialize<StudioSettings>(json, Options) ?? new StudioSettings();
            }
            catch (Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException)
            {
                return new StudioSettings();
            }
        }

        public void Save() => Save(DefaultPath);

        public void Save(string path)
        {
            var json = JsonSerializer.Serialize(this, Options);
            File.WriteAllText(path, json);
        }

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
