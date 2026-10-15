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
            Assert.True(player.Looping.Value);
            Assert.True(player.Playing.Value);
            Assert.IsAssignableFrom<IAssetTextureSource>(player);
        }

        [Fact]
        public void SequencePlayer_KeepsEveryValueThroughSaveAndLoad()
        {
            using var temp = new TempDirectoryFixture();
            var factory = CreateFactory();

            var player = (SequencePlayer)factory.Create(typeof(SequencePlayer));
            player.FPS.Value = 24f;
            player.Looping.Value = false;
            player.SeekTarget.Value = 7;
            player.Playing.Value = false;
            player.Resolution.X.Value = 640;
            player.Resolution.Y.Value = 360;
            player.AssetSelector.SetAssetFromPath(temp.Path);
            var model = player.ToModel();

            var loaded = (SequencePlayer)factory.Create(typeof(SequencePlayer));
            loaded.FromModel(model);

            Assert.Equal(24f, loaded.FPS.Value);
            Assert.False(loaded.Looping.Value);
            Assert.Equal(7, loaded.SeekTarget.Value);
            Assert.False(loaded.Playing.Value);
            Assert.Equal(640, loaded.Resolution.X.Value);
            Assert.Equal(360, loaded.Resolution.Y.Value);
            var sequence = Assert.IsType<ImageSequence>(loaded.AssetSelector.Asset);
            Assert.Equal(temp.Path, sequence.FilePath);
        }
    }
}
