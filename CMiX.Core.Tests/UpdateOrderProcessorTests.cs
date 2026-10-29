using CMiX.Core.Compositing;
using CMiX.Core.Materials;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Processors;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Texturing.Sources;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class UpdateOrderProcessorTests
    {
        // A prefab of no known kind. ControlFactory reads the PrefabService of every prefab in the repository, so a test sets one.
        private sealed class ControlOfNoKnownKind : IPrefab
        {
            public Guid ID { get; set; } = Guid.NewGuid();
            public PrefabService PrefabService { get; set; }
            public IControlModel ToModel() => throw new NotSupportedException();
            public void FromModel(IControlModel model) => throw new NotSupportedException();
        }

        [Fact]
        public void OrderedIds_ControlsOfEveryKind_FollowTheUpdateOrder()
        {
            var provider = TestServiceProviderFactory.Create();
            var processor = provider.GetRequiredService<UpdateOrderProcessor>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var collectionManager = provider.GetRequiredService<CollectionManager>();
            var factory = provider.GetRequiredService<ControlFactory>();
            var project = provider.GetRequiredService<Project>();
            var composition = factory.Create(typeof(Composition));
            var layer = factory.Create(typeof(Layer));
            var entity = factory.Create(typeof(Entity));
            var camera = factory.Create(typeof(Camera));
            var material = factory.Create(typeof(Material));
            var texture = factory.Create(typeof(CheckerBoard));
            var modifier = factory.Create(typeof(AlphaModifier));
            var unknown = new ControlOfNoKnownKind { PrefabService = provider.GetRequiredService<PrefabService>() };
            repository.AddRoot(unknown);
            foreach (var control in new IControl[] { composition, layer, entity, camera, material, texture, modifier })
                repository.AddControl(control, collectionManager);

            var order = processor.OrderedIds.ToList();

            Assert.Equal(unknown.ID, order[0]);
            Assert.True(order.IndexOf(modifier.ID) < order.IndexOf(texture.ID));
            Assert.True(order.IndexOf(texture.ID) < order.IndexOf(material.ID));
            Assert.True(order.IndexOf(material.ID) < order.IndexOf(entity.ID));
            Assert.True(order.IndexOf(material.ID) < order.IndexOf(camera.ID));
            Assert.True(order.IndexOf(entity.ID) < order.IndexOf(layer.ID));
            Assert.True(order.IndexOf(camera.ID) < order.IndexOf(layer.ID));
            Assert.True(order.IndexOf(layer.ID) < order.IndexOf(composition.ID));
            Assert.True(order.IndexOf(composition.ID) < order.IndexOf(project.ID));
        }

        [Fact]
        public void OrderedIds_TwoControlsOfOneKind_KeepTheOrderOfAdding()
        {
            var provider = TestServiceProviderFactory.Create();
            var processor = provider.GetRequiredService<UpdateOrderProcessor>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var collectionManager = provider.GetRequiredService<CollectionManager>();
            var factory = provider.GetRequiredService<ControlFactory>();
            var first = factory.Create(typeof(Layer));
            var second = factory.Create(typeof(Layer));
            repository.AddControl(first, collectionManager);
            repository.AddControl(second, collectionManager);

            var order = processor.OrderedIds.ToList();

            Assert.True(order.IndexOf(first.ID) < order.IndexOf(second.ID));
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

        [Fact]
        public void OrderedIds_AControlThatIsNoPrefab_IsNotInTheList()
        {
            var provider = TestServiceProviderFactory.Create();
            var processor = provider.GetRequiredService<UpdateOrderProcessor>();
            var project = provider.GetRequiredService<Project>();
            var registry = provider.GetRequiredService<RedisAddressRegistry>();

            registry.AddAddress();
            var address = registry.Addresses.Single();

            Assert.DoesNotContain(address.ID, processor.OrderedIds);
            Assert.Contains(project.ID, processor.OrderedIds);
        }
    }
}
