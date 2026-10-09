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
                        PeerName = "MyStudio"
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
    }
}
