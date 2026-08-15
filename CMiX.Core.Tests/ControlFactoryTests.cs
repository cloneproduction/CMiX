using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Covers ControlFactory naming when a manager is loaded from a model rather than built up
    // through AddItem (D5 of the post parity audit). Create(IControlModel) only names a control
    // when the model carried no name (post 0b39de2a), so a loaded duplicate has to survive the
    // load unchanged, and only a control created afterward has to stay unique against it.
    public class ControlFactoryTests
    {
        [Fact]
        public void FromModel_TwoIdenticallyNamedEntities_KeepTheirLoadedNames_AndTheNextCreatedEntityStaysUnique()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var layer = (Layer)factory.Create(typeof(Layer));
            var layerModel = (LayerModel)layer.ToModel();

            var entityModelA = (EntityModel)factory.Create(typeof(Entity)).ToModel();
            var entityModelB = (EntityModel)factory.Create(typeof(Entity)).ToModel();

            // Hand author both loaded entities with the identical name. A project file written by
            // the app should never contain this, but nothing before the load path enforces it, so
            // the loader has to tolerate it rather than silently deduplicate on read.
            entityModelA.PrefabService = entityModelA.PrefabService with { Name = new GenericValueModel<string>("Entity") };
            entityModelB.PrefabService = entityModelB.PrefabService with { Name = new GenericValueModel<string>("Entity") };

            layerModel.ModelEntityManager.ManagerData.Items.Add(entityModelA);
            layerModel.ModelEntityManager.ManagerData.Items.Add(entityModelB);

            layer.FromModel(layerModel);

            // Snapshotted, since ManagerData.Items is the live collection AddItem below appends to.
            var loadedItems = layer.ModelEntityManager.ManagerData.Items.ToList();
            Assert.Equal(2, loadedItems.Count);
            Assert.All(loadedItems, item => Assert.Equal("Entity", ((IPrefab)item).PrefabService.Name.Value));

            layer.ModelEntityManager.AddItem(typeof(Entity));
            var newEntity = (IPrefab)layer.ModelEntityManager.SelectedItem;

            // The new entity must not collide with either loaded "Entity", and must not be folded
            // into the duplicate pair either.
            Assert.Equal("Entity.001", newEntity.PrefabService.Name.Value);
            Assert.DoesNotContain(loadedItems, item => ReferenceEquals(item, newEntity));
        }
    }
}
