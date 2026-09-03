using CMiX.Core.Compositing;
using CMiX.Core.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ProjectStateHashTests
    {
        [Fact]
        public void ModelOverload_MatchesProjectOverload()
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));

            Assert.Equal(ProjectStateHash.Compute(project), ProjectStateHash.Compute((ProjectModel)project.ToModel()));
        }

        [Fact]
        public void TwoFreshProjects_HashIdentically()
        {
            var project1 = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            var project2 = TestServiceProviderFactory.Create().GetRequiredService<Project>();

            Assert.Equal(ProjectStateHash.Compute(project1), ProjectStateHash.Compute(project2));
        }

        [Fact]
        public void EditingOneSide_ChangesItsHash()
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();
            var before = ProjectStateHash.Compute(project);

            project.OutputMappingManager.Items[0].Visibility.Value = false;

            var after = ProjectStateHash.Compute(project);
            Assert.NotEqual(before, after);
        }

        [Fact]
        public void SameProject_HashesConsistently()
        {
            var project = TestServiceProviderFactory.Create().GetRequiredService<Project>();

            Assert.Equal(ProjectStateHash.Compute(project), ProjectStateHash.Compute(project));
        }
    }
}
