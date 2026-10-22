using CMiX.Core.Compositing;
using CMiX.Core.Persistence;
using CMiX.Core.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class OutputMappingManagerTests : IDisposable
    {
        private readonly TempDirectoryFixture _tempDir = new();
        private string _directory => _tempDir.Path;

        public void Dispose() => _tempDir.Dispose();

        private static void AssertFixedIds(OutputMapping mapping, int slotIndex)
        {
            var fieldIds = ManagerIDs.OutputMappingSlotFields[slotIndex];
            Assert.Equal(ManagerIDs.OutputMappingSlots[slotIndex], mapping.ID);
            Assert.Equal(fieldIds.Name, mapping.Name.ID);
            Assert.Equal(fieldIds.Resolution, mapping.Resolution.ID);
            Assert.Equal(fieldIds.ResolutionX, mapping.Resolution.X.ID);
            Assert.Equal(fieldIds.ResolutionY, mapping.Resolution.Y.ID);
            Assert.Equal(fieldIds.TexcoordSemantic, mapping.TexcoordSemantic.ID);
        }

        [Fact]
        public void OutputMappingManager_And_Slots_Use_Fixed_ManagerIDs()
        {
            var provider1 = TestServiceProviderFactory.Create();
            var project1 = provider1.GetRequiredService<Project>();

            var provider2 = TestServiceProviderFactory.Create();
            var project2 = provider2.GetRequiredService<Project>();

            Assert.Equal(ManagerIDs.OutputMappingManager, project1.OutputMappingManager.ID);
            Assert.Equal(project1.OutputMappingManager.ID, project2.OutputMappingManager.ID);

            for (int i = 0; i < OutputMappingManager.SlotCount; i++)
            {
                AssertFixedIds(project1.OutputMappingManager.Items[i], i);

                var slot1 = project1.OutputMappingManager.Items[i];
                var slot2 = project2.OutputMappingManager.Items[i];
                Assert.Equal(slot1.ID, slot2.ID);
                Assert.Equal(slot1.Name.ID, slot2.Name.ID);
                Assert.Equal(slot1.Resolution.ID, slot2.Resolution.ID);
                Assert.Equal(slot1.Resolution.X.ID, slot2.Resolution.X.ID);
                Assert.Equal(slot1.Resolution.Y.ID, slot2.Resolution.Y.ID);
                Assert.Equal(slot1.TexcoordSemantic.ID, slot2.TexcoordSemantic.ID);
            }
        }

        [Fact]
        public void OutputMappingManager_KeepsFixedIds_AfterSaveAndReload()
        {
            var provider1 = TestServiceProviderFactory.Create();
            var project1 = provider1.GetRequiredService<Project>();
            project1.OutputMappingManager.Items[3].Resolution.X.Value = 640;
            project1.OutputMappingManager.Items[3].Name.Value = "Renamed";

            var path = Path.Combine(_directory, "roundtrip.cmix");
            var model = (ProjectModel)project1.ToModel();
            ProjectSerializer.Save(model, path);
            var loadedModel = ProjectSerializer.Load(path);

            var provider2 = TestServiceProviderFactory.Create();
            var project2 = provider2.GetRequiredService<Project>();
            project2.FromModel(loadedModel);

            for (int i = 0; i < OutputMappingManager.SlotCount; i++)
                AssertFixedIds(project2.OutputMappingManager.Items[i], i);

            Assert.Equal(640, project2.OutputMappingManager.Items[3].Resolution.X.Value);
            Assert.Equal("Renamed", project2.OutputMappingManager.Items[3].Name.Value);
        }

        // A reset gives every slot the values of a new project. The slot instances, their fixed ids
        // stay correct.
        [Fact]
        public void Reset_RestoresTheDefaultsAndKeepsTheInstances()
        {
            var provider = TestServiceProviderFactory.Create();
            var manager = provider.GetRequiredService<Project>().OutputMappingManager;
            var slot3 = manager.Items[3];
            slot3.Name.Value = "LeftWall";
            slot3.Resolution.X.Value = 3840;

            manager.Reset();

            Assert.Same(slot3, manager.Items[3]);
            Assert.Equal("Output 4", slot3.Name.Value);
            Assert.Equal(1920, slot3.Resolution.X.Value);
            Assert.Equal(1080, slot3.Resolution.Y.Value);
            Assert.Same(manager.Items[0], manager.SelectedItem);

            for (int i = 0; i < OutputMappingManager.SlotCount; i++)
                AssertFixedIds(manager.Items[i], i);
        }
    }
}
