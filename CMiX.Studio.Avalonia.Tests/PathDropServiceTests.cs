using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Sources;
using CMiX.Studio.Avalonia.Session;
using CMiX.Studio.Avalonia.Tests.Support;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Image = CMiX.Core.Texturing.Sources.Image;

namespace CMiX.Studio.Avalonia.Tests
{
    // The rules of a dropped path for a texture list.
    public class PathDropServiceTests : IDisposable
    {
        private readonly TempFolder temp = new("path-drop");
        private readonly PathDropService service = new();

        public void Dispose() => temp.Dispose();

        [Fact]
        public void TextureTypeOf_AFolderWithImages_IsSequencePlayer()
        {
            var frames = temp.Folder("frames", "frame_0001.png");

            Assert.Equal(typeof(SequencePlayer), service.TextureTypeOf(frames));
        }

        [Fact]
        public void TextureTypeOf_AVideo_IsVideoPlayer()
        {
            Assert.Equal(typeof(VideoPlayer), service.TextureTypeOf(temp.File("clip.mp4")));
        }

        [Fact]
        public void TextureTypeOf_AnImage_IsImage()
        {
            Assert.Equal(typeof(Image), service.TextureTypeOf(temp.File("one.png")));
        }

        [Fact]
        public void TextureTypeOf_AnUnknownFile_IsNull()
        {
            Assert.Null(service.TextureTypeOf(temp.File("notes.xyz")));
            Assert.False(service.AcceptsTexture(temp.File("notes.txt")));
        }

        [Fact]
        public void AddTexture_AnImage_AddsAnImageWithThePath()
        {
            var provider = TestServiceProviderFactory.Create();
            var textures = provider.GetRequiredService<PrefabManager>();
            var path = temp.File("one.png");

            provider.GetRequiredService<PathDropService>().AddTexture(textures, path);

            var image = Assert.IsType<Image>(Assert.Single(textures.ManagerData.Items));
            Assert.Equal(path, image.AssetSelector.Asset.FilePath);
        }
    }
}
