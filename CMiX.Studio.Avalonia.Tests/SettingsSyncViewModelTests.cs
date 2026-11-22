// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.IO;
using CMiX.Core.Networking;
using CMiX.Studio.Avalonia.Services;
using CMiX.Studio.Avalonia.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class SettingsSyncViewModelTests
    {
        // Port 1 fails at once, so no test reaches a Redis server or the cmix:default keys.
        private static void FillGoodValues(SettingsSyncViewModel viewModel)
        {
            viewModel.Ip = "127.0.0.1";
            viewModel.Port = "1";
            viewModel.Database = "0";
            viewModel.User = "default";
            viewModel.Password = string.Empty;
            viewModel.KeyPrefix = "cmix:test-settingssync";
            viewModel.PeerName = "Studio";
        }

        private static void RemoveFolderOf(StudioSettingsFile file)
        {
            var folder = Path.GetDirectoryName(file.FilePath)!;
            if (Directory.Exists(folder))
                Directory.Delete(folder, true);
        }

        [Fact]
        public void Connect_APortOfZero_ShowsThePortMessageAndSavesNothing()
        {
            var provider = TestServiceProviderFactory.Create();
            var viewModel = provider.GetRequiredService<SettingsSyncViewModel>();
            var file = provider.GetRequiredService<StudioSettingsFile>();
            FillGoodValues(viewModel);
            viewModel.Port = "0";

            viewModel.ConnectCommand.Execute(null);

            Assert.Equal("The port must be a whole number above zero.", viewModel.ErrorText);
            Assert.False(File.Exists(file.FilePath));
        }

        [Fact]
        public void Connect_ANegativeDatabase_ShowsTheDatabaseMessage()
        {
            var provider = TestServiceProviderFactory.Create();
            var viewModel = provider.GetRequiredService<SettingsSyncViewModel>();
            var file = provider.GetRequiredService<StudioSettingsFile>();
            FillGoodValues(viewModel);
            viewModel.Database = "-1";

            viewModel.ConnectCommand.Execute(null);

            Assert.Equal("The database must be a whole number of zero or more.", viewModel.ErrorText);
            Assert.False(File.Exists(file.FilePath));
        }

        [Fact]
        public void Connect_ASaveFailure_ShowsTheMessage()
        {
            var blocker = Path.Combine(Path.GetTempPath(), $"cmix-settingssync-blocker-{Guid.NewGuid():N}");
            File.WriteAllText(blocker, string.Empty);
            var provider = TestServiceProviderFactory.Create(services =>
                services.AddSingleton(_ => new StudioSettingsFile(Path.Combine(blocker, StudioSettings.FileName))));
            var viewModel = provider.GetRequiredService<SettingsSyncViewModel>();
            var applied = false;
            viewModel.ConnectionApplied += (_, _) => applied = true;
            FillGoodValues(viewModel);

            try
            {
                viewModel.ConnectCommand.Execute(null);

                Assert.NotEqual(string.Empty, viewModel.ErrorText);
                Assert.False(applied);
            }
            finally
            {
                File.Delete(blocker);
            }
        }

        [Fact]
        public void Connect_GoodValues_SavesTheFileAndKeepsTheInterfacePart()
        {
            var provider = TestServiceProviderFactory.Create();
            var viewModel = provider.GetRequiredService<SettingsSyncViewModel>();
            var file = provider.GetRequiredService<StudioSettingsFile>();
            var peer = provider.GetRequiredService<SyncPeer>();
            file.Save(new StudioSettings { Interface = new InterfaceSettings { ShowKeyHints = false } });
            var applied = false;
            viewModel.ConnectionApplied += (_, _) => applied = true;
            FillGoodValues(viewModel);

            try
            {
                viewModel.ConnectCommand.Execute(null);
                var saved = file.Load();

                Assert.Equal(string.Empty, viewModel.ErrorText);
                Assert.Equal(1, saved.Redis.Port);
                Assert.Equal("cmix:test-settingssync", saved.Redis.KeyPrefix);
                Assert.False(saved.Interface.ShowKeyHints);
                Assert.True(applied);
            }
            finally
            {
                peer.Dispose();
                RemoveFolderOf(file);
            }
        }

        [Fact]
        public void LoadConnection_AFile_FillsTheBoxesAndClearsTheError()
        {
            var provider = TestServiceProviderFactory.Create();
            var viewModel = provider.GetRequiredService<SettingsSyncViewModel>();
            var file = provider.GetRequiredService<StudioSettingsFile>();
            file.Save(new StudioSettings
            {
                Redis = new RedisSettings { Ip = "10.0.0.5", Port = 6390, Database = 2, KeyPrefix = "cmix:test-load", UseCredentialManager = true, CredentialName = "MyRedis" }
            });
            viewModel.ErrorText = "An old message";

            try
            {
                viewModel.LoadConnectionCommand.Execute(null);

                Assert.Equal("10.0.0.5", viewModel.Ip);
                Assert.Equal("6390", viewModel.Port);
                Assert.Equal("2", viewModel.Database);
                Assert.Equal("cmix:test-load", viewModel.KeyPrefix);
                Assert.True(viewModel.UseCredentialManager);
                Assert.Equal("MyRedis", viewModel.CredentialName);
                Assert.Equal(string.Empty, viewModel.ErrorText);
            }
            finally
            {
                RemoveFolderOf(file);
            }
        }

        [Fact]
        public void IsUserEditable_ACredentialNameAndTheToggle_IsFalse()
        {
            var viewModel = TestServiceProviderFactory.Create().GetRequiredService<SettingsSyncViewModel>();

            viewModel.UseCredentialManager = true;
            viewModel.CredentialName = "MyRedis";

            Assert.False(viewModel.IsUserEditable);
        }

        [Fact]
        public void ShowEndpoint_ACredential_NamesTheUserSource()
        {
            var provider = TestServiceProviderFactory.Create();
            var viewModel = provider.GetRequiredService<SettingsSyncViewModel>();
            var file = provider.GetRequiredService<StudioSettingsFile>();
            var peer = provider.GetRequiredService<SyncPeer>();
            ((FakeCredentialStore)provider.GetRequiredService<ICredentialStore>()).Add("MyRedis", "vault-user", "secret");
            file.Save(new StudioSettings { Redis = new RedisSettings { UseCredentialManager = true, CredentialName = "MyRedis" } });
            peer.Start(SyncOptions.Defaults with { Port = 1, User = "vault-user", KeyPrefix = "cmix:test-settingssync" });

            try
            {
                viewModel.ShowEndpointCommand.Execute(null);

                Assert.Equal("127.0.0.1:1 db 0, user vault-user from credential MyRedis", viewModel.EndpointText);
            }
            finally
            {
                peer.Dispose();
                RemoveFolderOf(file);
            }
        }
    }
}
