using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // The Redis addresses are part of the project.
    public class ProjectRedisAddressTests : IDisposable
    {
        private readonly TempDirectoryFixture _tempDir = new();

        public void Dispose() => _tempDir.Dispose();

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
        public void SaveThenLoad_KeepsTheAddressesAndTheirIDs()
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

        [Fact]
        public void AddingAnAddress_ChangesTheStateHash()
        {
            var project = NewProject();
            var before = ProjectStateHash.Compute(project);

            AddAddress(project, "cmix:a");

            Assert.NotEqual(before, ProjectStateHash.Compute(project));
        }
    }
}
