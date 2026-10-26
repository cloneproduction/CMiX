using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Processors;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class VisibilityProcessorTests
    {
        private static (VisibilityProcessor processor, ControlRepository repository, CollectionManager collectionManager, ControlFactory factory) Create()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var collectionManager = provider.GetRequiredService<CollectionManager>();
            var factory = provider.GetRequiredService<ControlFactory>();
            return (new VisibilityProcessor(repository), repository, collectionManager, factory);
        }

        [Fact]
        public void VisibleIds_AControlAdded_ListsItWhenVisible()
        {
            var (processor, repository, collectionManager, factory) = Create();
            var layer = (Layer)factory.Create(typeof(Layer));
            layer.PrefabService.Visibility.Value = false;

            repository.AddControl(layer, collectionManager);

            Assert.Contains(layer.ID, processor.VisibleIds);
        }

        [Fact]
        public void VisibleIds_VisibilityTurnedOff_DropsTheControl()
        {
            var (processor, repository, collectionManager, factory) = Create();
            var layer = (Layer)factory.Create(typeof(Layer));
            layer.PrefabService.Visibility.Value = false;
            repository.AddControl(layer, collectionManager);

            layer.PrefabService.Visibility.Value = true;

            Assert.DoesNotContain(layer.ID, processor.VisibleIds);
            Assert.False(processor.IsVisible(layer.ID));
        }

        [Fact]
        public void VisibleIds_AControlRemoved_DropsTheControlAndStopsListening()
        {
            var (processor, repository, collectionManager, factory) = Create();
            var layer = (Layer)factory.Create(typeof(Layer));
            layer.PrefabService.Visibility.Value = false;
            repository.AddControl(layer, collectionManager);

            repository.RemoveControl(layer, collectionManager);
            layer.PrefabService.Visibility.Value = true;
            layer.PrefabService.Visibility.Value = false;

            Assert.DoesNotContain(layer.ID, processor.VisibleIds);
        }

        [Fact]
        public void VisibleIds_ControlsAlreadyInTheRepository_AreListedAtStart()
        {
            var provider = TestServiceProviderFactory.Create();
            var repository = provider.GetRequiredService<ControlRepository>();
            var collectionManager = provider.GetRequiredService<CollectionManager>();
            var layer = (Layer)provider.GetRequiredService<ControlFactory>().Create(typeof(Layer));
            layer.PrefabService.Visibility.Value = false;
            repository.AddControl(layer, collectionManager);

            var processor = new VisibilityProcessor(repository);

            Assert.Contains(layer.ID, processor.VisibleIds);
        }

        [Fact]
        public void VisibleIds_TwoLayers_OnlyTheVisibleOneIsListed()
        {
            var (processor, repository, collectionManager, factory) = Create();
            var shown = (Layer)factory.Create(typeof(Layer));
            var hidden = (Layer)factory.Create(typeof(Layer));
            shown.PrefabService.Visibility.Value = false;
            hidden.PrefabService.Visibility.Value = true;
            repository.AddControl(shown, collectionManager);
            repository.AddControl(hidden, collectionManager);

            Assert.Equal(new[] { shown.ID }, processor.VisibleIds);
        }

        [Fact]
        public void Service_IsRegisteredOnce_AndListsControlsAddedLater()
        {
            var provider = TestServiceProviderFactory.Create();
            var processor = provider.GetRequiredService<VisibilityProcessor>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var collectionManager = provider.GetRequiredService<CollectionManager>();
            var layer = (Layer)provider.GetRequiredService<ControlFactory>().Create(typeof(Layer));
            layer.PrefabService.Visibility.Value = false;

            repository.AddControl(layer, collectionManager);

            Assert.Same(processor, provider.GetRequiredService<VisibilityProcessor>());
            Assert.Contains(layer.ID, processor.VisibleIds);
        }
    }
}
