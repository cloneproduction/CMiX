// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Networking;
using CMiX.Core.Tests;
using CMiX.Studio.Avalonia.Settings;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class StudioSettingsTests
    {
        [Fact]
        public void FromSyncOptions_ThenToSyncOptions_GivesTheSameOptions()
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
