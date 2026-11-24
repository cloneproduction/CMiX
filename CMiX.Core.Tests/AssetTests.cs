// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Assets;
using CMiX.Core.BaseControls;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // The file walks of the assets, the image test of a folder, and an image sequence from a path.
    public class AssetTests : IDisposable
    {
        private readonly TempDirectoryFixture _tempDir = new();

        private string Folder => _tempDir.Path;

        public void Dispose() => _tempDir.Dispose();

        private static string WriteFile(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, "x");
            return path;
        }

        private static AssetSelector CreateSelector() =>
            TestServiceProviderFactory.Create().GetRequiredService<AssetSelector>();

        [Fact]
        public void CollectAssetFilePaths_AFileAndAFolder_GivesTheFileAndEveryFileOfTheFolder()
        {
            var file = WriteFile(Path.Combine(Folder, "single.png"));
            var dropped = Path.Combine(Folder, "dropped");
            var top = WriteFile(Path.Combine(dropped, "top.png"));
            var nested = WriteFile(Path.Combine(dropped, "sub", "nested.obj"));
            var assetManager = new AssetManager(new AssetRepository());

            var result = assetManager.CollectAssetFilePaths(new[] { file, dropped });

            Assert.Equal(file, result[0]);
            Assert.Equal(new[] { nested, top }.OrderBy(p => p), result.Skip(1).OrderBy(p => p));
        }

        [Fact]
        public void CollectAssetFilePaths_AMissingPath_GivesNothing()
        {
            var assetManager = new AssetManager(new AssetRepository());

            var result = assetManager.CollectAssetFilePaths(new[] { Path.Combine(Folder, "missing") });

            Assert.Empty(result);
        }

        [Fact]
        public void ContainsImages_AFolderWithAnImage_IsTrue()
        {
            File.WriteAllText(Path.Combine(Folder, "frame_0001.png"), "x");

            Assert.True(AssetTypes.ContainsImages(Folder));
        }

        [Fact]
        public void ContainsImages_AFolderWithoutImages_IsFalse()
        {
            File.WriteAllText(Path.Combine(Folder, "notes.txt"), "x");
            File.WriteAllText(Path.Combine(Folder, "clip.mp4"), "x");

            Assert.False(AssetTypes.ContainsImages(Folder));
        }

        [Fact]
        public void ContainsImages_AnImageInASubFolder_IsFalse()
        {
            WriteFile(Path.Combine(Folder, "sub", "frame.png"));

            Assert.False(AssetTypes.ContainsImages(Folder));
        }

        [Fact]
        public void ContainsImages_AMissingFolder_IsFalse()
        {
            Assert.False(AssetTypes.ContainsImages(Path.Combine(Folder, "missing")));
        }

        [Fact]
        public void SetAssetFromPath_WithFolder_AddsImageSequence()
        {
            var selector = CreateSelector();

            selector.SetAssetFromPath(Folder);

            var sequence = Assert.Single(selector.AssetRepository.ImageSequences);
            Assert.Equal(Folder, sequence.FilePath);
            Assert.Equal(Folder, selector.FilePath.Value);
            Assert.Same(sequence, selector.Asset);
        }

        [Fact]
        public void SetAssetFromPath_WithSameFolderTwice_KeepsOneImageSequence()
        {
            var selector = CreateSelector();

            selector.SetAssetFromPath(Folder);
            var first = selector.Asset;
            selector.SetAssetFromPath(Folder);

            Assert.Single(selector.AssetRepository.ImageSequences);
            Assert.Same(first, selector.Asset);
        }

        [Fact]
        public void FromModel_WithFolderPath_CreatesImageSequence()
        {
            var selector = CreateSelector();
            var model = new AssetSelectorModel { FilePath = new GenericValueModel<string>(Folder) };

            selector.FromModel(model);

            Assert.IsType<ImageSequence>(selector.Asset);
            Assert.Single(selector.AssetRepository.ImageSequences);
        }

        [Fact]
        public void SetAssetFromPath_WithMissingPath_CreatesNoAsset()
        {
            var selector = CreateSelector();

            selector.SetAssetFromPath(Path.Combine(Folder, "missing"));

            Assert.Null(selector.Asset);
            Assert.Empty(selector.AssetRepository.ImageSequences);
        }

        [Fact]
        public void Name_OfAnImageSequence_IsTheLastFolderName()
        {
            var path = Path.Combine(Path.GetTempPath(), "frames");

            Assert.Equal("frames", new ImageSequence(path).Name);
            Assert.Equal("frames", new ImageSequence(path + Path.DirectorySeparatorChar).Name);
        }
    }
}
