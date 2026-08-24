using System.Text.Json;
using CMiX.Core.Compositing;
using CMiX.Core.Persistence;
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
    }
}
