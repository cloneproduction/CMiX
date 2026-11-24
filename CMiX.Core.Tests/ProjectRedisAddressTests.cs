using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // The Redis addresses are part of the project.
    public class ProjectRedisAddressTests
    {
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
        public void AddingAnAddress_ChangesTheStateHash()
        {
            var project = NewProject();
            var before = ProjectStateHash.Compute(project);

            AddAddress(project, "cmix:a");

            Assert.NotEqual(before, ProjectStateHash.Compute(project));
        }
    }
}
