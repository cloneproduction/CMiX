using CMiX.Core.Assets;
using CMiX.Core.BaseControls;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ImageSequenceAssetTests
    {
        private static AssetSelector CreateSelector() =>
            TestServiceProviderFactory.Create().GetRequiredService<AssetSelector>();

        [Fact]
        public void SetAssetFromPath_WithFolder_AddsImageSequence()
        {
            using var temp = new TempDirectoryFixture();
            var selector = CreateSelector();

            selector.SetAssetFromPath(temp.Path);

            var sequence = Assert.Single(selector.AssetRepository.ImageSequences);
            Assert.Equal(temp.Path, sequence.FilePath);
            Assert.Equal(temp.Path, selector.FilePath.Value);
            Assert.Same(sequence, selector.Asset);
        }

        [Fact]
        public void SetAssetFromPath_WithSameFolderTwice_KeepsOneImageSequence()
        {
            using var temp = new TempDirectoryFixture();
            var selector = CreateSelector();

            selector.SetAssetFromPath(temp.Path);
            var first = selector.Asset;
            selector.SetAssetFromPath(temp.Path);

            Assert.Single(selector.AssetRepository.ImageSequences);
            Assert.Same(first, selector.Asset);
        }

        [Fact]
        public void FromModel_WithFolderPath_CreatesImageSequence()
        {
            using var temp = new TempDirectoryFixture();
            var selector = CreateSelector();
            var model = new AssetSelectorModel { FilePath = new GenericValueModel<string>(temp.Path) };

            selector.FromModel(model);

            Assert.IsType<ImageSequence>(selector.Asset);
            Assert.Single(selector.AssetRepository.ImageSequences);
        }

        [Fact]
        public void SetAssetFromPath_WithMissingPath_CreatesNoAsset()
        {
            using var temp = new TempDirectoryFixture();
            var selector = CreateSelector();

            selector.SetAssetFromPath(Path.Combine(temp.Path, "missing"));

            Assert.Null(selector.Asset);
            Assert.Empty(selector.AssetRepository.ImageSequences);
        }

        [Fact]
        public void Name_IsLastFolderName()
        {
            var path = Path.Combine(Path.GetTempPath(), "frames");

            Assert.Equal("frames", new ImageSequence(path).Name);
            Assert.Equal("frames", new ImageSequence(path + Path.DirectorySeparatorChar).Name);
        }
    }
}
