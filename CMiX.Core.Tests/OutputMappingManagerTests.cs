using CMiX.Core.Compositing;
using CMiX.Core.Persistence;
using CMiX.Core.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class OutputMappingManagerTests : IDisposable
    {
        private readonly string _directory;

        public OutputMappingManagerTests()
        {
            _directory = Path.Combine(Path.GetTempPath(), "CMiX.Core.Tests_" + Guid.NewGuid());
            Directory.CreateDirectory(_directory);
        }

        public void Dispose()
        {
            if (Directory.Exists(_directory))
                Directory.Delete(_directory, recursive: true);
        }

        private static void AssertFixedIds(OutputMapping mapping, int slotIndex)
        {
            var fieldIds = ManagerIDs.OutputMappingSlotFields[slotIndex];
            Assert.Equal(ManagerIDs.OutputMappingSlots[slotIndex], mapping.ID);
            Assert.Equal(fieldIds.Name, mapping.Name.ID);
            Assert.Equal(fieldIds.Resolution, mapping.Resolution.ID);
            Assert.Equal(fieldIds.ResolutionX, mapping.Resolution.X.ID);
            Assert.Equal(fieldIds.ResolutionY, mapping.Resolution.Y.ID);
            Assert.Equal(fieldIds.TexcoordSemantic, mapping.TexcoordSemantic.ID);
            Assert.Equal(fieldIds.Visibility, mapping.Visibility.ID);
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

            for (int i = 0; i < 10; i++)
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
                Assert.Equal(slot1.Visibility.ID, slot2.Visibility.ID);
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

            for (int i = 0; i < 10; i++)
                AssertFixedIds(project2.OutputMappingManager.Items[i], i);

            Assert.Equal(640, project2.OutputMappingManager.Items[3].Resolution.X.Value);
            Assert.Equal("Renamed", project2.OutputMappingManager.Items[3].Name.Value);
        }
    }
}
