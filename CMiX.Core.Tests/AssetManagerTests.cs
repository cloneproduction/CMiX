// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Assets;
using Xunit;

namespace CMiX.Core.Tests
{
    public class AssetManagerTests : IDisposable
    {
        private readonly string _folder = Path.Combine(Path.GetTempPath(), "cmix-assetmanager-" + Guid.NewGuid().ToString("N"));

        public AssetManagerTests() => Directory.CreateDirectory(_folder);

        public void Dispose() => Directory.Delete(_folder, true);

        [Fact]
        public void CollectAssetFilePaths_AFileAndAFolder_GivesTheFileAndEveryFileOfTheFolder()
        {
            var file = WriteFile(Path.Combine(_folder, "single.png"));
            var dropped = Path.Combine(_folder, "dropped");
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

            var result = assetManager.CollectAssetFilePaths(new[] { Path.Combine(_folder, "missing") });

            Assert.Empty(result);
        }

        private static string WriteFile(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, "x");
            return path;
        }
    }
}
