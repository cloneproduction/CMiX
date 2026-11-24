// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.IO;
using CMiX.Core.Networking;
using CMiX.Core.Tests;
using CMiX.Studio.Avalonia.Settings;
using CMiX.Studio.Avalonia.Tests.Support;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class StudioSettingsFileTests
    {
        [Fact]
        public void Save_ThenLoad_KeepsTheRedisAndTheInterfaceParts()
        {
            using var temp = new TempFolder("studiosettingsfile");
            var file = new StudioSettingsFile(temp.PathOf(StudioSettings.FileName));
            var settings = new StudioSettings
            {
                Redis = new RedisSettings { Ip = "10.0.0.5", Port = 6390, CredentialName = "cmix-redis" },
                Interface = new InterfaceSettings { ShowKeyHints = false }
            };

            file.Save(settings);
            var loaded = file.Load();

            Assert.Equal("10.0.0.5", loaded.Redis.Ip);
            Assert.Equal(6390, loaded.Redis.Port);
            Assert.Equal("cmix-redis", loaded.Redis.CredentialName);
            Assert.False(loaded.Interface.ShowKeyHints);
        }

        [Fact]
        public void Load_MissingFile_ToSyncOptionsGivesAllFallbacks()
        {
            using var temp = new TempFolder("studiosettingsfile");
            var file = new StudioSettingsFile(temp.PathOf(StudioSettings.FileName));

            var options = file.Load().ToSyncOptions();

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
            using var temp = new TempFolder("studiosettingsfile");
            var file = new StudioSettingsFile(temp.PathOf(StudioSettings.FileName));
            File.WriteAllText(file.FilePath, "{ \"Redis\": { \"Ip\": \"\", \"Port\": 0, \"Database\": 0, \"User\": \"\", \"Password\": \"\", \"KeyPrefix\": \"\", \"PeerName\": \"\" } }");

            var options = file.Load().ToSyncOptions();

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
        public void SaveThenLoad_FullFile_RoundTrips()
        {
            using var temp = new TempFolder("studiosettingsfile");
            var file = new StudioSettingsFile(temp.PathOf(StudioSettings.FileName));
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
            file.Save(settings);

            var loaded = file.Load();

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

        [Fact]
        public void Load_BrokenFile_GivesDefaults()
        {
            using var temp = new TempFolder("studiosettingsfile");
            var file = new StudioSettingsFile(temp.PathOf(StudioSettings.FileName));
            File.WriteAllText(file.FilePath, "{ not json");

            var options = file.Load().ToSyncOptions();

            Assert.Equal("127.0.0.1", options.Ip);
            Assert.Equal(6379, options.Port);
            Assert.Equal("default", options.User);
            Assert.Equal("cmix:default", options.KeyPrefix);
            Assert.Equal("Studio", options.PeerName);
            Assert.Equal("studio", options.Role);
        }

        [Fact]
        public void Load_FileWithoutCredentialFields_GivesFalseAndEmptyName()
        {
            using var temp = new TempFolder("studiosettingsfile");
            var file = new StudioSettingsFile(temp.PathOf(StudioSettings.FileName));
            File.WriteAllText(file.FilePath, "{ \"Redis\": { \"Ip\": \"10.0.0.5\", \"Port\": 6400, \"Database\": 2, \"User\": \"u\", \"Password\": \"p\", \"KeyPrefix\": \"k\", \"PeerName\": \"n\" } }");

            var loaded = file.Load();

            Assert.False(loaded.Redis.UseCredentialManager);
            Assert.Equal("", loaded.Redis.CredentialName);
        }

        [Fact]
        public void FilePath_FromTheTestProvider_IsNotTheFileOfTheStudio()
        {
            var provider = TestServiceProviderFactory.Create();

            var file = provider.GetRequiredService<StudioSettingsFile>();

            Assert.NotEqual(StudioSettings.DefaultPath, file.FilePath);
            Assert.StartsWith(Path.GetTempPath(), file.FilePath);
        }

        [Fact]
        public void GetService_TheTestProvider_GivesTheFakeCredentialStore()
        {
            var provider = TestServiceProviderFactory.Create();

            var store = provider.GetRequiredService<ICredentialStore>();

            Assert.IsType<FakeCredentialStore>(store);
        }
    }
}
