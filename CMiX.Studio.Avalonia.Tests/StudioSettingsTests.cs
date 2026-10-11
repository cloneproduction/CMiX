// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.IO;
using CMiX.Core.Networking;
using CMiX.Studio.Avalonia.Services;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // Covers StudioSettings against temp paths only. DefaultPath is never touched, so these tests
    // never read or write next to the test runner's own executable.
    public class StudioSettingsTests
    {
        private static string TempPath() => Path.Combine(Path.GetTempPath(), $"cmix-studiosettings-{Guid.NewGuid():N}.json");

        [Fact]
        public void Load_MissingFile_ToSyncOptionsGivesAllFallbacks()
        {
            var path = TempPath();

            var options = StudioSettings.Load(path).ToSyncOptions();

            Assert.Equal("127.0.0.1", options.Ip);
            Assert.Equal(6379, options.Port);
            Assert.Equal(0, options.Database);
            Assert.Equal("default", options.User);
            Assert.Equal("", options.Password);
            Assert.Equal("cmix:default", options.KeyPrefix);
            Assert.Equal("Studio", options.PeerName);
            Assert.Equal("studio", options.Role);
        }

        [Fact]
        public void Load_FileWithEmptyFieldsAndZeroPort_ToSyncOptionsGivesAllFallbacks()
        {
            var path = TempPath();
            try
            {
                File.WriteAllText(path, "{ \"Redis\": { \"Ip\": \"\", \"Port\": 0, \"Database\": 0, \"User\": \"\", \"Password\": \"\", \"KeyPrefix\": \"\", \"PeerName\": \"\" } }");

                var options = StudioSettings.Load(path).ToSyncOptions();

                Assert.Equal("127.0.0.1", options.Ip);
                Assert.Equal(6379, options.Port);
                Assert.Equal(0, options.Database);
                Assert.Equal("default", options.User);
                Assert.Equal("", options.Password);
                Assert.Equal("cmix:default", options.KeyPrefix);
                Assert.Equal("Studio", options.PeerName);
                Assert.Equal("studio", options.Role);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [Fact]
        public void SaveThenLoad_FullFile_RoundTrips()
        {
            var path = TempPath();
            try
            {
                var settings = new StudioSettings
                {
                    Redis = new RedisSettings
                    {
                        Ip = "10.0.0.5",
                        Port = 6400,
                        Database = 2,
                        User = "studio-user",
                        Password = "secret",
                        KeyPrefix = "cmix:studio",
                        PeerName = "MyStudio",
                        UseCredentialManager = true,
                        CredentialName = "MyRedis"
                    }
                };
                settings.Save(path);

                var loaded = StudioSettings.Load(path);

                Assert.Equal(settings.Redis.Ip, loaded.Redis.Ip);
                Assert.Equal(settings.Redis.Port, loaded.Redis.Port);
                Assert.Equal(settings.Redis.Database, loaded.Redis.Database);
                Assert.Equal(settings.Redis.User, loaded.Redis.User);
                Assert.Equal(settings.Redis.Password, loaded.Redis.Password);
                Assert.Equal(settings.Redis.KeyPrefix, loaded.Redis.KeyPrefix);
                Assert.Equal(settings.Redis.PeerName, loaded.Redis.PeerName);
                Assert.True(loaded.Redis.UseCredentialManager);
                Assert.Equal("MyRedis", loaded.Redis.CredentialName);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [Fact]
        public void Load_BrokenFile_GivesDefaults()
        {
            var path = TempPath();
            try
            {
                File.WriteAllText(path, "{ not json");

                var options = StudioSettings.Load(path).ToSyncOptions();

                Assert.Equal("127.0.0.1", options.Ip);
                Assert.Equal(6379, options.Port);
                Assert.Equal("default", options.User);
                Assert.Equal("cmix:default", options.KeyPrefix);
                Assert.Equal("Studio", options.PeerName);
                Assert.Equal("studio", options.Role);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [Fact]
        public void FromSyncOptionsThenToSyncOptions_RoundTrips()
        {
            var original = new SyncOptions("10.0.0.5", 6400, 2, "studio-user", "secret", "cmix:studio", "MyStudio", "engine");

            var roundTripped = StudioSettings.FromSyncOptions(original).ToSyncOptions();

            Assert.Equal(original.Ip, roundTripped.Ip);
            Assert.Equal(original.Port, roundTripped.Port);
            Assert.Equal(original.Database, roundTripped.Database);
            Assert.Equal(original.User, roundTripped.User);
            Assert.Equal(original.Password, roundTripped.Password);
            Assert.Equal(original.KeyPrefix, roundTripped.KeyPrefix);
            Assert.Equal(original.PeerName, roundTripped.PeerName);
            Assert.Equal("studio", roundTripped.Role);
        }

        [Fact]
        public void Load_FileWithoutCredentialFields_GivesFalseAndEmptyName()
        {
            var path = TempPath();
            try
            {
                File.WriteAllText(path, "{ \"Redis\": { \"Ip\": \"10.0.0.5\", \"Port\": 6400, \"Database\": 2, \"User\": \"u\", \"Password\": \"p\", \"KeyPrefix\": \"k\", \"PeerName\": \"n\" } }");

                var loaded = StudioSettings.Load(path);

                Assert.False(loaded.Redis.UseCredentialManager);
                Assert.Equal("", loaded.Redis.CredentialName);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        private static FakeCredentialStore StoreWithMyRedis()
        {
            var store = new FakeCredentialStore();
            store.Add("MyRedis", "alice", "secret");
            return store;
        }

        [Fact]
        public void Resolve_UseOff_IgnoresStore()
        {
            var settings = new StudioSettings
            {
                Redis = new RedisSettings { User = "fileUser", UseCredentialManager = false, CredentialName = "MyRedis" }
            };

            var resolved = settings.Resolve(StoreWithMyRedis());

            Assert.Equal("fileUser", resolved.Options.User);
            Assert.Equal("from file", resolved.UserSource);
        }

        [Fact]
        public void Resolve_UseOnWithEmptyName_UsesFile()
        {
            var settings = new StudioSettings
            {
                Redis = new RedisSettings { User = "fileUser", UseCredentialManager = true, CredentialName = "" }
            };

            var resolved = settings.Resolve(StoreWithMyRedis());

            Assert.Equal("fileUser", resolved.Options.User);
            Assert.Equal("from file", resolved.UserSource);
        }

        [Fact]
        public void Resolve_UseOnAndFound_TakesCredential()
        {
            var settings = new StudioSettings
            {
                Redis = new RedisSettings { User = "fileUser", UseCredentialManager = true, CredentialName = "MyRedis" }
            };

            var resolved = settings.Resolve(StoreWithMyRedis());

            Assert.Equal("alice", resolved.Options.User);
            Assert.Equal("secret", resolved.Options.Password);
            Assert.Equal("from credential MyRedis", resolved.UserSource);
            Assert.Equal("Studio", resolved.Options.PeerName);
            Assert.Equal("studio", resolved.Options.Role);
        }

        [Fact]
        public void Resolve_UseOnAndNotFound_UsesFile()
        {
            var settings = new StudioSettings
            {
                Redis = new RedisSettings { User = "", Password = "", UseCredentialManager = true, CredentialName = "Other" }
            };

            var resolved = settings.Resolve(StoreWithMyRedis());

            Assert.Equal("default", resolved.Options.User);
            Assert.Equal("", resolved.Options.Password);
            Assert.Equal("from file, credential Other not found", resolved.UserSource);
        }
    }
}
