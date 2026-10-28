using System.Text.Json;
using System.Text.Json.Nodes;
using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Persistence;
using CMiX.Core.Rendering;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ProjectSerializerTests : IDisposable
    {
        private readonly TempDirectoryFixture _tempDir = new();
        private string _directory => _tempDir.Path;

        public void Dispose() => _tempDir.Dispose();

        private static ProjectModel BuildMinimalProjectModel(IServiceProvider provider)
        {
            var project = provider.GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            return (ProjectModel)project.ToModel();
        }

        // An output used to have a visibility. A file from that time still loads, and the key is ignored.
        [Fact]
        public void Load_FileWithTheRemovedOutputVisibility_StillLoads()
        {
            var provider = TestServiceProviderFactory.Create();
            var path = Path.Combine(_directory, "old-outputs.cmix");
            ProjectSerializer.Save(BuildMinimalProjectModel(provider), path);
            var root = JsonNode.Parse(File.ReadAllText(path))!;
            foreach (var item in root["OutputMappingManager"]!["Items"]!.AsArray())
                item!.AsObject()["Visibility"] = JsonNode.Parse("{\"ID\":\"00000000-0000-0000-0000-000000000000\",\"Value\":false}");
            File.WriteAllText(path, root.ToJsonString());

            var loaded = ProjectSerializer.Load(path);

            Assert.Equal(OutputMappingManager.SlotCount, loaded.OutputMappingManager.Items.Count);
        }

        // A file saved with the old $type RedisAddressModulatorModel still loads and gives a RedisFloatModulator.
        [Fact]
        public void Load_AProjectWithTheOldRedisAddressModulatorType_GivesARedisFloatModulator()
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)project.CompositionManager.SelectedItem;
            composition.TextureModifierManager.AddItem(typeof(Blur));
            var blur = (Blur)composition.TextureModifierManager.SelectedItem;
            blur.ModulatorManager.AddItem(typeof(RedisFloatModulator));
            var path = Path.Combine(_directory, "old-modulator.cmix");
            ProjectSerializer.Save((ProjectModel)project.ToModel(), path);
            var json = File.ReadAllText(path);
            Assert.Contains("CMiX.Core.Modulation.Modulators.RedisFloatModulatorModel", json);
            File.WriteAllText(path, json.Replace(
                "CMiX.Core.Modulation.Modulators.RedisFloatModulatorModel",
                "CMiX.Core.Modulation.Modulators.RedisAddressModulatorModel"));

            var loaded = ProjectSerializer.Load(path);
            var target = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            target.CompositionManager.AddItemFromModel(loaded.CompositionManager.ManagerData.Items[0]);

            var loadedComposition = (Composition)target.CompositionManager.SelectedItem;
            var loadedBlur = (Blur)loadedComposition.TextureModifierManager.ManagerData.Items[0];
            Assert.IsType<RedisFloatModulator>(Assert.Single(loadedBlur.ModulatorManager.ManagerData.Items));
        }

        [Fact]
        public void Save_Then_Load_RoundTripsMinimalProject()
        {
            var provider = TestServiceProviderFactory.Create();
            var model = BuildMinimalProjectModel(provider);
            var path = Path.Combine(_directory, "roundtrip.cmix");

            ProjectSerializer.Save(model, path);
            var loaded = ProjectSerializer.Load(path);

            Assert.Equal(model.ID, loaded.ID);
            Assert.Single(loaded.CompositionManager.ManagerData.Items);
            Assert.Equal(
                model.CompositionManager.ManagerData.Items[0].ID,
                loaded.CompositionManager.ManagerData.Items[0].ID);
        }

        [Fact]
        public void Save_LeavesNoTempFileAndTargetParses()
        {
            var provider = TestServiceProviderFactory.Create();
            var model = BuildMinimalProjectModel(provider);
            var path = Path.Combine(_directory, "notemp.cmix");

            ProjectSerializer.Save(model, path);

            Assert.True(File.Exists(path));
            Assert.False(File.Exists(path + ".tmp"));

            var json = File.ReadAllText(path);
            var parsed = JsonSerializer.Deserialize<JsonDocument>(json);
            Assert.NotNull(parsed);
        }

        [Fact]
        public void Load_CorruptFile_ThrowsJsonException()
        {
            var path = Path.Combine(_directory, "corrupt.cmix");
            File.WriteAllText(path, "this is not json { : : garbage");

            Assert.Throws<JsonException>(() => ProjectSerializer.Load(path));
        }

        private static ProjectModel BuildModelWithAssetPath(string assetPath)
        {
            var model = BuildMinimalProjectModel(TestServiceProviderFactory.Create());
            model.Model = new AssetSelectorModel { FilePath = new GenericValueModel<string>(assetPath) };
            return model;
        }

        private static string ReadSavedAssetPath(string projectPath)
        {
            using var doc = JsonDocument.Parse(File.ReadAllText(projectPath));
            return doc.RootElement.GetProperty("Model").GetProperty("FilePath").GetProperty("Value").GetString();
        }

        // A drive root that is not the drive of the temp folder.
        private string OtherDriveRoot()
        {
            var tempRoot = Path.GetPathRoot(_directory);
            return tempRoot.StartsWith("Q", StringComparison.OrdinalIgnoreCase) ? @"R:\" : @"Q:\";
        }

        // Saves a project at <_directory>\VJ\compositions\bar.cmix with an asset at <_directory>\VJ\textures\foo.jpg.
        private string SaveProjectWithSiblingAsset()
        {
            var compositions = Path.Combine(_directory, "VJ", "compositions");
            Directory.CreateDirectory(compositions);
            var path = Path.Combine(compositions, "bar.cmix");
            var model = BuildModelWithAssetPath(Path.Combine(_directory, "VJ", "textures", "foo.jpg"));
            ProjectSerializer.Save(model, path);
            return path;
        }

        [Fact]
        public void Save_WithAnAssetInASiblingFolder_WritesARelativePath()
        {
            var path = SaveProjectWithSiblingAsset();

            Assert.Equal(@"..\textures\foo.jpg", ReadSavedAssetPath(path));
        }

        [Fact]
        public void Save_WithAnAssetOnAnotherDrive_KeepsTheAbsolutePath()
        {
            var assetPath = Path.Combine(OtherDriveRoot(), "textures", "foo.jpg");
            var model = BuildModelWithAssetPath(assetPath);
            var path = Path.Combine(_directory, "otherdrive.cmix");

            ProjectSerializer.Save(model, path);

            Assert.Equal(assetPath, ReadSavedAssetPath(path));
        }

        [Fact]
        public void Load_WithARelativePath_ResolvesAgainstTheProjectFolder()
        {
            var source = SaveProjectWithSiblingAsset();
            var target = Path.Combine(_directory, "Other", "VJ", "compositions", "bar.cmix");
            Directory.CreateDirectory(Path.GetDirectoryName(target));
            File.Copy(source, target);

            var loaded = ProjectSerializer.Load(target);

            Assert.Equal(Path.Combine(_directory, "Other", "VJ", "textures", "foo.jpg"), loaded.Model.FilePath.Value);
        }

        [Fact]
        public void Load_WithAnAbsolutePath_KeepsIt()
        {
            // A file in the old format: the asset path is absolute and on the same drive.
            var assetPath = Path.Combine(_directory, "VJ", "textures", "foo.jpg");
            var model = BuildModelWithAssetPath(assetPath);
            var path = Path.Combine(_directory, "absolute.cmix");
            File.WriteAllText(path, JsonSerializer.Serialize(model, ProjectSerializer.Options));

            var loaded = ProjectSerializer.Load(path);

            Assert.Equal(assetPath, loaded.Model.FilePath.Value);
        }

        [Fact]
        public void SaveThenLoad_WithAnEmptyPath_KeepsItEmpty()
        {
            var model = BuildModelWithAssetPath(string.Empty);
            var path = Path.Combine(_directory, "empty.cmix");

            ProjectSerializer.Save(model, path);
            var loaded = ProjectSerializer.Load(path);

            Assert.Equal(string.Empty, ReadSavedAssetPath(path));
            Assert.Equal(string.Empty, loaded.Model.FilePath.Value);
        }

        [Fact]
        public void Save_DoesNotChangeTheProjectStateHash()
        {
            var compositions = Path.Combine(_directory, "VJ", "compositions");
            Directory.CreateDirectory(compositions);
            var path = Path.Combine(compositions, "bar.cmix");
            var model = BuildModelWithAssetPath(Path.Combine(_directory, "VJ", "textures", "foo.jpg"));
            var before = ProjectStateHash.Compute(model);

            ProjectSerializer.Save(model, path);
            var after = ProjectStateHash.Compute(model);
            var loaded = ProjectSerializer.Load(path);

            Assert.Equal(before, after);
            Assert.Equal(before, ProjectStateHash.Compute(loaded));
        }
    }
}
