using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Processors;
using CMiX.Core.Texturing.Sources;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class AssetListProcessorTests
    {
        private static (AssetListProcessor Processor, PrefabManager Textures) Create()
        {
            var provider = TestServiceProviderFactory.Create();
            return (provider.GetRequiredService<AssetListProcessor>(), provider.GetRequiredService<PrefabManager>());
        }

        private static T Add<T>(PrefabManager textures, string path = null) where T : class, IControl
        {
            textures.AddItem(typeof(T));
            var source = (T)textures.SelectedItem;
            if (path != null)
                ((IAssetTextureSource)source).AssetSelector.FilePath.Value = path;

            return source;
        }

        [Fact]
        public void GetAssets_NoSource_IsEmpty()
        {
            var (processor, _) = Create();

            Assert.Empty(processor.GetAssets());
        }

        [Fact]
        public void GetAssets_SourcesThatLoadAFile_AreListedWithTheirPathInTheOrderOfTheRepository()
        {
            var (processor, textures) = Create();
            var image = Add<Image>(textures, "a.png");
            var video = Add<VideoPlayer>(textures, "b.mp4");
            var sequence = Add<SequencePlayer>(textures, "c");

            Assert.Equal(
                new[] { new AssetEntry(image.ID, "a.png"), new AssetEntry(video.ID, "b.mp4"), new AssetEntry(sequence.ID, "c") },
                processor.GetAssets());
        }

        [Fact]
        public void GetAssets_ASourceWithoutAFile_IsNotListed()
        {
            var (processor, textures) = Create();
            var image = Add<Image>(textures, "a.png");
            Add<CheckerBoard>(textures);

            Assert.Equal(new[] { new AssetEntry(image.ID, "a.png") }, processor.GetAssets());
        }

        [Fact]
        public void GetAssets_APathThatChanged_IsInTheNewList()
        {
            var (processor, textures) = Create();
            var image = Add<Image>(textures, "a.png");
            var before = processor.GetAssets();

            image.AssetSelector.FilePath.Value = "b.png";
            var after = processor.GetAssets();

            Assert.Equal(new[] { new AssetEntry(image.ID, "a.png") }, before);
            Assert.Equal(new[] { new AssetEntry(image.ID, "b.png") }, after);
        }

        [Fact]
        public void GetAssets_NothingChanged_GivesTheSameListAgain()
        {
            var (processor, textures) = Create();
            Add<Image>(textures, "a.png");

            Assert.Same(processor.GetAssets(), processor.GetAssets());
        }

        [Fact]
        public void GetAssets_ASourceAddedOrRemoved_FollowsTheRepository()
        {
            var (processor, textures) = Create();
            var first = Add<Image>(textures, "a.png");
            var second = Add<VideoPlayer>(textures, "b.mp4");
            Assert.Equal(2, processor.GetAssets().Count);

            textures.RemoveItem(first);

            Assert.Equal(new[] { new AssetEntry(second.ID, "b.mp4") }, processor.GetAssets());
        }

        [Fact]
        public void GetAssets_AHiddenSource_StaysInTheList()
        {
            var (processor, textures) = Create();
            var image = Add<Image>(textures, "a.png");

            image.PrefabService.Visibility.Value = true;

            Assert.Equal(new[] { new AssetEntry(image.ID, "a.png") }, processor.GetAssets());
        }
    }
}
