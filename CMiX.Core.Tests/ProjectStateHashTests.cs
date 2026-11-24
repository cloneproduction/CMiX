using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ProjectStateHashTests
    {
        private static Project NewProject() => TestServiceProviderFactory.Create().GetRequiredService<Project>();

        [Fact]
        public void Compute_OfTheModelOfAProject_GivesTheHashOfTheProject()
        {
            var project = NewProject();
            project.CompositionManager.AddItem(typeof(Composition));

            Assert.Equal(ProjectStateHash.Compute(project), ProjectStateHash.Compute((ProjectModel)project.ToModel()));
        }

        [Fact]
        public void Compute_OfTwoNewProjects_GivesTheSameHash()
        {
            Assert.Equal(ProjectStateHash.Compute(NewProject()), ProjectStateHash.Compute(NewProject()));
        }

        [Fact]
        public void Compute_AfterAnEdit_GivesAnotherHash()
        {
            var project = NewProject();
            var before = ProjectStateHash.Compute(project);

            project.OutputMappingManager.Items[0].Resolution.X.Value = 640;

            Assert.NotEqual(before, ProjectStateHash.Compute(project));
        }

        // A model property that nothing writes gets new IDs on every save, and the project then always counts as changed.
        [Theory]
        [InlineData(typeof(Entity))]
        [InlineData(typeof(FullscreenQuad))]
        [InlineData(typeof(TextEntity))]
        [InlineData(typeof(CMiX.Core.Rendering.Cameras.Camera))]
        [InlineData(typeof(CMiX.Core.Rendering.Lights.LightEntity))]
        public void Compute_AProjectWithAnItemOfEachKind_HashesTheSameTwice(Type kind)
        {
            var project = NewProject();
            project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)project.CompositionManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;
            if (kind == typeof(CMiX.Core.Rendering.Cameras.Camera))
                layer.CameraManager.AddItem(kind);
            else if (kind == typeof(CMiX.Core.Rendering.Lights.LightEntity))
                layer.LightManager.AddItem(kind);
            else
                layer.ModelEntityManager.AddItem(kind);

            Assert.Equal(ProjectStateHash.Compute(project), ProjectStateHash.Compute(project));
        }

        [Fact]
        public void Compute_AfterARedisAddressIsAdded_GivesAnotherHash()
        {
            var project = NewProject();
            var before = ProjectStateHash.Compute(project);

            var manager = project.RedisAddressRegistry.AddressManager;
            manager.AddItem(typeof(RedisAddress));
            ((RedisAddress)manager.SelectedItem).Value = "cmix:a";

            Assert.NotEqual(before, ProjectStateHash.Compute(project));
        }
    }
}
