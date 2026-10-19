using System.Collections.ObjectModel;
using CMiX.Core.Compositing;
using CMiX.Core.Materials;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Texturing.Sources;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // GetRenderPriority, GetOrderedControls and Controls are used from VL patches.
    public class ControlRepositoryVlMembersTests
    {
        private static (ControlFactory Factory, ControlRepository Repository, CollectionManager CollectionManager, IServiceProvider Provider) Create()
        {
            var provider = TestServiceProviderFactory.Create();
            return (provider.GetRequiredService<ControlFactory>(), provider.GetRequiredService<ControlRepository>(),
                    provider.GetRequiredService<CollectionManager>(), provider);
        }

        [Fact]
        public void Controls_IsAnObservableCollectionOfIControl()
        {
            var (_, repository, _, _) = Create();

            Assert.IsType<ObservableCollection<IControl>>(repository.Controls);
        }

        [Fact]
        public void GetRenderPriority_OrdersTheKindsOfControl()
        {
            var (factory, _, _, provider) = Create();

            Assert.Equal(0, ControlRepository.GetRenderPriority(provider.GetRequiredService<Project>()));
            Assert.Equal(1, ControlRepository.GetRenderPriority(factory.Create(typeof(Composition))));
            Assert.Equal(2, ControlRepository.GetRenderPriority(factory.Create(typeof(Layer))));
            Assert.Equal(3, ControlRepository.GetRenderPriority(factory.Create(typeof(Entity))));
            Assert.Equal(3, ControlRepository.GetRenderPriority(factory.Create(typeof(TextEntity))));
            Assert.Equal(3, ControlRepository.GetRenderPriority(factory.Create(typeof(LightEntity))));
            Assert.Equal(3, ControlRepository.GetRenderPriority(factory.Create(typeof(Camera))));
            Assert.Equal(4, ControlRepository.GetRenderPriority(factory.Create(typeof(Material))));
            Assert.Equal(5, ControlRepository.GetRenderPriority(factory.Create(typeof(CheckerBoard))));
            Assert.Equal(6, ControlRepository.GetRenderPriority(factory.Create(typeof(AlphaModifier))));
            Assert.Equal(99, ControlRepository.GetRenderPriority(provider.GetRequiredService<ManagerData>()));
        }

        [Fact]
        public void GetOrderedControls_SortsByPriorityAndKeepsTheOrderOfAddingInsideAKind()
        {
            var (factory, repository, collectionManager, _) = Create();
            var layer1 = factory.Create(typeof(Layer));
            var entity = factory.Create(typeof(Entity));
            var composition = factory.Create(typeof(Composition));
            var layer2 = factory.Create(typeof(Layer));
            var camera = factory.Create(typeof(Camera));
            foreach (var control in new[] { layer1, entity, composition, layer2, camera })
                repository.AddControl(control, collectionManager);

            Assert.Equal(new[] { composition, layer1, layer2, entity, camera }, repository.GetOrderedControls());
        }
    }
}
