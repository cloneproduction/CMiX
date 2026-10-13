using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
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

        // Stands for the file dialog: the Browse button gets the path that the user would pick.
        private sealed class BrowsingTo : AssetPathSelector
        {
            private readonly string? _picked;

            public BrowsingTo(string? picked) => _picked = picked;

            protected override Task<string?> BrowseAsync() => Task.FromResult(_picked);
        }

        private static void ClickBrowse(Control selector)
        {
            var window = new Window { Content = selector, Width = 400, Height = 100 };
            window.Show();
            Dispatcher.UIThread.RunJobs();
            selector.GetVisualDescendants().OfType<Button>().Single(b => b.GetType() == typeof(Button)).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Dispatcher.UIThread.RunJobs();
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
        public void BrowsingToAFolder_AddsAnImageSequenceAndSelectsIt()
        {
            var player = (CMiX.Core.Texturing.Sources.SequencePlayer)TestServiceProviderFactory.Create()
                .GetRequiredService<ControlFactory>().Create(typeof(CMiX.Core.Texturing.Sources.SequencePlayer));
            var frames = System.IO.Path.Combine(_folder, "frames");
            Directory.CreateDirectory(frames);
            var selector = new BrowsingTo(frames) { DataContext = player.AssetSelector, SelectsFolder = true };

            ClickBrowse(selector);

            Assert.Contains(player.AssetSelector.AssetRepository.ImageSequences, s => s.FilePath == frames);
            Assert.Equal(frames, player.AssetSelector.Asset.FilePath);
        }

        [AvaloniaFact]
        public void BrowsingToAFile_AddsTheAssetAndSelectsIt()
        {
            var image = NewImage();
            var path = NewFile("browsed.png");
            var selector = new BrowsingTo(path) { DataContext = image.AssetSelector };

            ClickBrowse(selector);

            Assert.NotNull(image.AssetSelector.AssetRepository.FindByPath(path));
            Assert.Equal(path, image.AssetSelector.Asset.FilePath);
        }

        [AvaloniaFact]
        public void CancellingTheDialog_ChangesNothing()
        {
            var image = NewImage();
            var selector = new BrowsingTo(null) { DataContext = image.AssetSelector };

            ClickBrowse(selector);

            Assert.Empty(image.AssetSelector.AssetRepository.Images);
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
