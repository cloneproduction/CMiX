using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class SharedFullscreenQuadTests
    {
        [Fact]
        public void AFullscreenQuad_IsInModelEntitiesButNotInEntities()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var manager = provider.GetRequiredService<PrefabManager>();

            manager.AddItem(typeof(FullscreenQuad));
            var quad = manager.ManagerData.Items[0];

            Assert.Same(quad, Assert.Single(repository.ModelEntities));
            Assert.Empty(repository.Entities);
        }

        [Fact]
        public void AFullscreenQuad_CanBeSharedBetweenTwoLayers_AndLeavesTheRepositoryWhenBothLetGo()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var compositions = provider.GetRequiredService<PrefabManager>();
            compositions.AddItem(typeof(Composition));
            var layers = ((Composition)compositions.SelectedItem).LayerManager;
            layers.AddItem(typeof(Layer));
            layers.AddItem(typeof(Layer));
            var first = (Layer)layers.ManagerData.Items[0];
            var second = (Layer)layers.ManagerData.Items[1];

            first.ModelEntityManager.AddItem(typeof(FullscreenQuad));
            var quad = first.ModelEntityManager.ManagerData.Items[0];
            second.ModelEntityManager.AddExistingItem(quad);

            Assert.Same(quad, second.ModelEntityManager.ManagerData.Items[0]);
            Assert.Same(quad, Assert.Single(repository.ModelEntities));

            first.ModelEntityManager.RemoveItem(quad);
            Assert.Same(quad, repository.GetControl(quad.ID));
            Assert.Same(quad, Assert.Single(repository.ModelEntities));

            second.ModelEntityManager.RemoveItem(quad);
            Assert.Null(repository.GetControl(quad.ID));
            Assert.Empty(repository.ModelEntities);
        }
    }
}
