// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Text.Json;
using System.Text.Json.Nodes;
using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Save and load of a project through a file: ids, names, the output slot, asset paths, a corrupt file and an old $type.
    public class ProjectFileTests : IDisposable
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
        public void SaveThenLoad_OfAMinimalProject_KeepsTheModel()
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
        public void Save_OfAProject_LeavesNoTempFileAndAFileThatParses()
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

        // A project whose one asset is an image texture on an entity of a layer.
        private static ProjectModel BuildModelWithAssetPath(string assetPath)
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)project.CompositionManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;
            layer.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (Entity)layer.ModelEntityManager.SelectedItem;
            var textureManager = entity.Material.DiffuseTexture.TextureManager;
            textureManager.AddItem(typeof(Image));
            ((Image)textureManager.SelectedItem).AssetSelector.FilePath.Value = assetPath;
            return (ProjectModel)project.ToModel();
        }

        private static string ReadSavedAssetPath(string projectPath)
        {
            var node = FindFilePath(JsonNode.Parse(File.ReadAllText(projectPath)));
            return node["Value"].GetValue<string>();
        }

        private static JsonNode FindFilePath(JsonNode node)
        {
            switch (node)
            {
                case JsonObject obj:
                    if (obj["AssetSelector"]?["FilePath"] is JsonObject filePath)
                        return filePath;
                    foreach (var pair in obj)
                        if (pair.Value != null && FindFilePath(pair.Value) is { } found)
                            return found;
                    break;
                case JsonArray array:
                    foreach (var item in array)
                        if (item != null && FindFilePath(item) is { } foundInItem)
                            return foundInItem;
                    break;
            }
            return null;
        }

        // The asset path of the first image in a loaded model.
        private static string LoadedAssetPath(object model)
        {
            if (model is ImageModel image)
                return image.AssetSelector.FilePath.Value;
            if (model is string || model == null)
                return null;

            if (model is System.Collections.IEnumerable items)
            {
                foreach (var item in items)
                    if (LoadedAssetPath(item) is { } fromItem)
                        return fromItem;
                return null;
            }

            foreach (var property in model.GetType().GetProperties())
                if (property.GetIndexParameters().Length == 0 && !property.PropertyType.IsValueType
                    && LoadedAssetPath(property.GetValue(model)) is { } fromProperty)
                    return fromProperty;
            return null;
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

            Assert.Equal(Path.Combine(_directory, "Other", "VJ", "textures", "foo.jpg"), LoadedAssetPath(loaded));
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

            Assert.Equal(assetPath, LoadedAssetPath(loaded));
        }

        [Fact]
        public void SaveThenLoad_WithAnEmptyPath_KeepsItEmpty()
        {
            var model = BuildModelWithAssetPath(string.Empty);
            var path = Path.Combine(_directory, "empty.cmix");

            ProjectSerializer.Save(model, path);
            var loaded = ProjectSerializer.Load(path);

            Assert.Equal(string.Empty, ReadSavedAssetPath(path));
            Assert.Equal(string.Empty, LoadedAssetPath(loaded));
        }

        [Fact]
        public void Save_OfAProject_DoesNotChangeTheStateHash()
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

        [Fact]
        public void SaveThenLoad_OfAProject_KeepsTheNamesAndResolvesEveryControl()
        {
            var source = TestServiceProviderFactory.Create();
            var sourceProject = source.GetRequiredService<Project>();

            sourceProject.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)sourceProject.CompositionManager.SelectedItem;
            composition.PrefabService.Name.Value = "Opening Scene";

            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;
            layer.PrefabService.Name.Value = "Background";

            layer.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (Entity)layer.ModelEntityManager.SelectedItem;
            entity.PrefabService.Name.Value = "Hero";

            var path = Path.Combine(_directory, "names.cmix");
            ProjectSerializer.Save((ProjectModel)sourceProject.ToModel(), path);
            var loadedModel = ProjectSerializer.Load(path);
            var compositionModel = (CompositionModel)loadedModel.CompositionManager.ManagerData.Items[0];

            var target = TestServiceProviderFactory.Create();
            var targetProject = target.GetRequiredService<Project>();
            var repository = target.GetRequiredService<ControlRepository>();

            targetProject.CompositionManager.AddItemFromModel(compositionModel);

            var loadedComposition = (Composition)targetProject.CompositionManager.SelectedItem;
            var loadedLayer = (Layer)loadedComposition.LayerManager.ManagerData.Items[0];
            var loadedEntity = (Entity)loadedLayer.ModelEntityManager.ManagerData.Items[0];

            Assert.Equal("Opening Scene", loadedComposition.PrefabService.Name.Value);
            Assert.Equal("Background", loadedLayer.PrefabService.Name.Value);
            Assert.Equal("Hero", loadedEntity.PrefabService.Name.Value);

            Assert.Equal(composition.ID, loadedComposition.ID);
            Assert.Equal(layer.ID, loadedLayer.ID);
            Assert.Equal(entity.ID, loadedEntity.ID);

            Assert.Same(loadedComposition, repository.GetControl(loadedComposition.ID));
            Assert.Same(loadedLayer, repository.GetControl(loadedLayer.ID));
            Assert.Same(loadedEntity, repository.GetControl(loadedEntity.ID));
        }

        [Fact]
        public void SaveThenLoad_OfAValidOutputSlot_KeepsTheSelection()
        {
            var source = TestServiceProviderFactory.Create();
            var sourceProject = source.GetRequiredService<Project>();

            sourceProject.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)sourceProject.CompositionManager.SelectedItem;
            var slot = sourceProject.OutputMappingManager.Items[3];
            composition.SelectedOutputMapping = slot;

            var path = Path.Combine(_directory, "output-slot.cmix");
            ProjectSerializer.Save((ProjectModel)sourceProject.ToModel(), path);
            var loadedModel = ProjectSerializer.Load(path);
            var compositionModel = (CompositionModel)loadedModel.CompositionManager.ManagerData.Items[0];

            var target = TestServiceProviderFactory.Create();
            var targetProject = target.GetRequiredService<Project>();
            targetProject.CompositionManager.AddItemFromModel(compositionModel);

            var loadedComposition = (Composition)targetProject.CompositionManager.SelectedItem;
            Assert.Equal(slot.ID, loadedComposition.SelectedOutputMappingID.Value);
            Assert.Same(targetProject.OutputMappingManager.Items[3], loadedComposition.SelectedOutputMapping);
        }

        [Fact]
        public void SaveThenLoad_WithAnUnknownOutputSlot_KeepsTheDefaultSlot()
        {
            var source = TestServiceProviderFactory.Create();
            var sourceProject = source.GetRequiredService<Project>();

            sourceProject.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)sourceProject.CompositionManager.SelectedItem;
            composition.SelectedOutputMapping = sourceProject.OutputMappingManager.Items[3];

            var path = Path.Combine(_directory, "unknown-output-slot.cmix");
            ProjectSerializer.Save((ProjectModel)sourceProject.ToModel(), path);
            var loadedModel = ProjectSerializer.Load(path);
            var compositionModel = (CompositionModel)loadedModel.CompositionManager.ManagerData.Items[0];

            // Duplicate and open replace every GUID, so the saved slot ID names no slot.
            var replacedModel = compositionModel with
            {
                SelectedOutputMappingID = compositionModel.SelectedOutputMappingID with { Value = Guid.NewGuid() }
            };

            var target = TestServiceProviderFactory.Create();
            var targetProject = target.GetRequiredService<Project>();
            targetProject.CompositionManager.AddItemFromModel(replacedModel);

            var loadedComposition = (Composition)targetProject.CompositionManager.SelectedItem;
            var defaultSlot = targetProject.OutputMappingManager.Items[0];
            Assert.Equal(defaultSlot.ID, loadedComposition.SelectedOutputMappingID.Value);
            Assert.Same(defaultSlot, loadedComposition.SelectedOutputMapping);
        }

        [Fact]
        public void SaveThenLoad_WithTwoCompositions_KeepsBothAndTheirLayersSeparate()
        {
            var source = TestServiceProviderFactory.Create();
            var sourceProject = source.GetRequiredService<Project>();

            sourceProject.CompositionManager.AddItem(typeof(Composition));
            var compositionA = (Composition)sourceProject.CompositionManager.SelectedItem;
            compositionA.PrefabService.Name.Value = "CompositionA";
            compositionA.LayerManager.AddItem(typeof(Layer));
            var layerA = (Layer)compositionA.LayerManager.SelectedItem;
            layerA.PrefabService.Name.Value = "LayerA";

            sourceProject.CompositionManager.AddItem(typeof(Composition));
            var compositionB = (Composition)sourceProject.CompositionManager.SelectedItem;
            compositionB.PrefabService.Name.Value = "CompositionB";
            compositionB.LayerManager.AddItem(typeof(Layer));
            var layerB = (Layer)compositionB.LayerManager.SelectedItem;
            layerB.PrefabService.Name.Value = "LayerB";

            var path = Path.Combine(_directory, "multi-composition.cmix");
            ProjectSerializer.Save((ProjectModel)sourceProject.ToModel(), path);
            var loadedModel = ProjectSerializer.Load(path);
            var compositionModels = loadedModel.CompositionManager.ManagerData.Items;

            Assert.Equal(2, compositionModels.Count);

            var target = TestServiceProviderFactory.Create();
            var targetProject = target.GetRequiredService<Project>();
            foreach (var compositionModel in compositionModels)
                targetProject.CompositionManager.AddItemFromModel(compositionModel);

            var loadedCompositions = targetProject.CompositionManager.ManagerData.Items;
            Assert.Equal(2, loadedCompositions.Count);

            var loadedCompositionA = (Composition)loadedCompositions.Single(c => ((Composition)c).PrefabService.Name.Value == "CompositionA");
            var loadedCompositionB = (Composition)loadedCompositions.Single(c => ((Composition)c).PrefabService.Name.Value == "CompositionB");

            Assert.NotEqual(Guid.Empty, loadedCompositionA.ID);
            Assert.NotEqual(Guid.Empty, loadedCompositionB.ID);
            Assert.NotEqual(loadedCompositionA.ID, loadedCompositionB.ID);
            Assert.Equal(compositionA.ID, loadedCompositionA.ID);
            Assert.Equal(compositionB.ID, loadedCompositionB.ID);

            var loadedLayerA = (Layer)Assert.Single(loadedCompositionA.LayerManager.ManagerData.Items);
            var loadedLayerB = (Layer)Assert.Single(loadedCompositionB.LayerManager.ManagerData.Items);

            Assert.Equal("LayerA", loadedLayerA.PrefabService.Name.Value);
            Assert.Equal("LayerB", loadedLayerB.PrefabService.Name.Value);
        }

        private static Project NewProject() =>
            TestServiceProviderFactory.Create().GetRequiredService<Project>();

        private static void AddAddress(Project project, string text)
        {
            var manager = project.RedisAddressRegistry.AddressManager;
            manager.AddItem(typeof(RedisAddress));
            ((RedisAddress)manager.SelectedItem).Value = text;
        }

        private static IEnumerable<string> Addresses(Project project) =>
            project.RedisAddressRegistry.Addresses.Select(address => address.Value);

        [Fact]
        public void SaveThenLoad_OfRedisAddresses_KeepsTheirValuesAndIDs()
        {
            var source = NewProject();
            AddAddress(source, "cmix:a");
            AddAddress(source, "cmix:b");
            var path = Path.Combine(_tempDir.Path, "addresses.cmix");

            ProjectSerializer.Save((ProjectModel)source.ToModel(), path);
            var target = NewProject();
            target.ApplySnapshot(ProjectSerializer.Load(path));

            Assert.Equal(new[] { "cmix:a", "cmix:b" }, Addresses(target));
            Assert.Equal(source.RedisAddressRegistry.Addresses.Select(a => a.ID),
                         target.RedisAddressRegistry.Addresses.Select(a => a.ID));
        }
    }
}
