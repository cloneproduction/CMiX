using System.Text.Json;
using System.Text.Json.Nodes;
using CMiX.Core.Assets;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class SequencePlayerTests
    {
        private static ControlFactory CreateFactory() =>
            TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();

        [Fact]
        public void NewSequencePlayer_HasTheDefaultValues()
        {
            var player = (SequencePlayer)CreateFactory().Create(typeof(SequencePlayer));

            Assert.Equal(60f, player.FPS.Value);
            Assert.True(player.Looping.Value);
            Assert.True(player.Playing.Value);
            Assert.False(player.DoSeek.Value);
            Assert.True(player.DoSeek.IsMomentary);
            Assert.IsAssignableFrom<IAssetTextureSource>(player);
        }

        [Fact]
        public void DoSeek_PressAndReleaseThenUndo_LeavesDoSeekFalse()
        {
            var provider = TestServiceProviderFactory.Create();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var player = (SequencePlayer)provider.GetRequiredService<ControlFactory>().Create(typeof(SequencePlayer));
            provider.GetRequiredService<ControlActivationService>().ActivateAll();

            player.DoSeek.Value = true;
            player.DoSeek.Value = false;
            var error = Record.Exception(() => undoManager.Undo());

            Assert.Null(error);
            Assert.False(player.DoSeek.Value);
        }

        [Fact]
        public void FromModel_OfAFolderAsset_LoadsAnImageSequence()
        {
            using var temp = new TempDirectoryFixture();
            var factory = CreateFactory();

            var player = (SequencePlayer)factory.Create(typeof(SequencePlayer));
            player.AssetSelector.SetAssetFromPath(temp.Path);
            var model = player.ToModel();

            var loaded = (SequencePlayer)factory.Create(typeof(SequencePlayer));
            loaded.FromModel(model);

            var sequence = Assert.IsType<ImageSequence>(loaded.AssetSelector.Asset);
            Assert.Equal(temp.Path, sequence.FilePath);
        }

        [Fact]
        public void FromModel_OfAnOldFileWithAButtonDoSeek_LoadsAFalseValueWithTheID()
        {
            var id = new Guid("5a1d3c2e-7b4f-4e8a-9c61-2f0d8e4b7a93");
            var node = JsonSerializer.SerializeToNode(new SequencePlayerModel(), ProjectSerializer.Options)!;
            node["DoSeek"] = new JsonObject { ["ID"] = id.ToString() };

            var loaded = JsonSerializer.Deserialize<SequencePlayerModel>(node, ProjectSerializer.Options)!;

            Assert.Equal(id, loaded.DoSeek.ID);
            Assert.False(loaded.DoSeek.Value.Value);
            Assert.Null(loaded.DoSeek.ModulatorID.Value);
        }
    }
}
