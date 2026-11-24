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
