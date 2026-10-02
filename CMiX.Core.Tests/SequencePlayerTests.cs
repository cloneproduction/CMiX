using CMiX.Core.Assets;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Sources;
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
            Assert.True(player.Loop.Value);
            Assert.True(player.Play.Value);
            Assert.IsAssignableFrom<IAssetTextureSource>(player);
        }

        [Fact]
        public void SequencePlayer_KeepsEveryValueThroughSaveAndLoad()
        {
            using var temp = new TempDirectoryFixture();
            var factory = CreateFactory();

            var player = (SequencePlayer)factory.Create(typeof(SequencePlayer));
            player.FPS.Value = 24f;
            player.Loop.Value = false;
            player.SeekFrame.Value = 7;
            player.Play.Value = false;
            player.Resolution.X.Value = 640;
            player.Resolution.Y.Value = 360;
            player.AssetSelector.SetAssetFromPath(temp.Path);
            var model = player.ToModel();

            var loaded = (SequencePlayer)factory.Create(typeof(SequencePlayer));
            loaded.FromModel(model);

            Assert.Equal(24f, loaded.FPS.Value);
            Assert.False(loaded.Loop.Value);
            Assert.Equal(7, loaded.SeekFrame.Value);
            Assert.False(loaded.Play.Value);
            Assert.Equal(640, loaded.Resolution.X.Value);
            Assert.Equal(360, loaded.Resolution.Y.Value);
            var sequence = Assert.IsType<ImageSequence>(loaded.AssetSelector.Asset);
            Assert.Equal(temp.Path, sequence.FilePath);
        }
    }
}
