using Avalonia.Headless.XUnit;
using CMiX.Core.Assets;
using CMiX.Core.Prefabs;
using CMiX.Studio.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The path a user chooses or drops becomes an asset of the repository.
    public class AssetPathSelectorTests : IDisposable
    {
        private readonly string _folder = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "cmix-assets-" + Guid.NewGuid().ToString("N"));

        public AssetPathSelectorTests()
        {
            Directory.CreateDirectory(_folder);
        }

        public void Dispose() => Directory.Delete(_folder, true);

        private sealed class Dropper : AssetPathSelector
        {
            public void Drop(IReadOnlyList<string> paths) => OnPathsDropped(paths);
        }

        private string NewFile(string name)
        {
            var path = System.IO.Path.Combine(_folder, name);
            File.WriteAllText(path, "x");
            return path;
        }

        private static CMiX.Core.Texturing.Sources.Image NewImage() =>
            (CMiX.Core.Texturing.Sources.Image)TestServiceProviderFactory.Create()
                .GetRequiredService<ControlFactory>().Create(typeof(CMiX.Core.Texturing.Sources.Image));

        [AvaloniaFact]
        public void ItAsksForTheAssetFileTypes()
        {
            var selector = new AssetPathSelector();

            Assert.Equal("Assets", selector.FileTypeName);
            Assert.Equal(AssetTypes.AllExtensions, selector.Extensions);
            Assert.Equal("Filename", selector.Caption);
        }

        [AvaloniaFact]
        public void AChosenPath_BecomesTheSelectedAsset()
        {
            var image = NewImage();
            var path = NewFile("one.png");
            var selector = new AssetPathSelector { DataContext = image.AssetSelector };

            selector.ChoosePath(path);

            Assert.NotNull(image.AssetSelector.AssetRepository.FindByPath(path));
            Assert.Equal(path, image.AssetSelector.Asset.FilePath);
        }

        [AvaloniaFact]
        public void DroppedFiles_AreAddedAndTheFirstIsSelected()
        {
            var image = NewImage();
            var first = NewFile("first.png");
            var second = NewFile("second.png");
            var selector = new Dropper { DataContext = image.AssetSelector };

            selector.Drop(new[] { first, second });

            var repository = image.AssetSelector.AssetRepository;
            Assert.NotNull(repository.FindByPath(first));
            Assert.NotNull(repository.FindByPath(second));
            Assert.Equal(first, image.AssetSelector.Asset.FilePath);
        }

        [AvaloniaFact]
        public void ADroppedFileOfAnUnknownType_IsIgnored()
        {
            var image = NewImage();
            var text = NewFile("notes.txt");
            var selector = new Dropper { DataContext = image.AssetSelector };

            selector.Drop(new[] { text });

            Assert.Null(image.AssetSelector.AssetRepository.FindByPath(text));
        }
    }
}
