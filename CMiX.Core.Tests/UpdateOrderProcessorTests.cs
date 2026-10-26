using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Processors;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class UpdateOrderProcessorTests
    {
        [Fact]
        public void OrderedIds_ControlsOfDifferentKinds_HighestRenderPriorityFirst()
        {
            var provider = TestServiceProviderFactory.Create();
            var processor = provider.GetRequiredService<UpdateOrderProcessor>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var collectionManager = provider.GetRequiredService<CollectionManager>();
            var factory = provider.GetRequiredService<ControlFactory>();
            var composition = (Composition)factory.Create(typeof(Composition));
            var layer = (Layer)factory.Create(typeof(Layer));
            var entity = (Entity)factory.Create(typeof(Entity));

            repository.AddControl(composition, collectionManager);
            repository.AddControl(layer, collectionManager);
            repository.AddControl(entity, collectionManager);

            var expected = new[] { entity.ID, layer.ID, composition.ID };
            Assert.Equal(expected, processor.OrderedIds.Where(id => expected.Contains(id)));
        }

        [Fact]
        public void OrderedIds_AHiddenControl_LeavesTheListAndComesBack()
        {
            var provider = TestServiceProviderFactory.Create();
            var processor = provider.GetRequiredService<UpdateOrderProcessor>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var collectionManager = provider.GetRequiredService<CollectionManager>();
            var layer = (Layer)provider.GetRequiredService<ControlFactory>().Create(typeof(Layer));
            layer.PrefabService.Visibility.Value = false;
            repository.AddControl(layer, collectionManager);
            Assert.Contains(layer.ID, processor.OrderedIds);

            layer.PrefabService.Visibility.Value = true;
            Assert.DoesNotContain(layer.ID, processor.OrderedIds);

            layer.PrefabService.Visibility.Value = false;
            Assert.Contains(layer.ID, processor.OrderedIds);
        }

        [Fact]
        public void OrderedIds_AControlRemoved_LeavesTheList()
        {
            var provider = TestServiceProviderFactory.Create();
            var processor = provider.GetRequiredService<UpdateOrderProcessor>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var collectionManager = provider.GetRequiredService<CollectionManager>();
            var layer = (Layer)provider.GetRequiredService<ControlFactory>().Create(typeof(Layer));
            repository.AddControl(layer, collectionManager);

            repository.RemoveControl(layer, collectionManager);

            Assert.DoesNotContain(layer.ID, processor.OrderedIds);
        }
    }
}
