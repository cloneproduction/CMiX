using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Processors;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class RenderLayersProcessorTests
    {
        private static (RenderLayersProcessor Processor, Project Project) Create()
        {
            var provider = TestServiceProviderFactory.Create();
            return (provider.GetRequiredService<RenderLayersProcessor>(), provider.GetRequiredService<Project>());
        }

        private static Composition AddComposition(Project project, int layerCount)
        {
            project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)project.CompositionManager.ManagerData.SelectedItem;
            for (var i = 0; i < layerCount; i++)
                composition.LayerManager.AddItem(typeof(Layer));

            return composition;
        }

        private static List<IControl> Layers(Composition composition) => composition.LayerManager.ManagerData.Items.ToList();

        [Fact]
        public void Ids_NoComposition_IsEmpty()
        {
            var (processor, _) = Create();

            Assert.Empty(processor.Ids);
        }

        [Fact]
        public void Ids_TheSelectedComposition_ListsItsLayersInTheOrderOfTheList()
        {
            var (processor, project) = Create();
            var composition = AddComposition(project, 3);

            Assert.Equal(Layers(composition).Select(layer => layer.ID), processor.Ids);
        }

        [Fact]
        public void Ids_AnotherCompositionSelected_ListsTheLayersOfThatOne()
        {
            var (processor, project) = Create();
            var first = AddComposition(project, 2);
            var second = AddComposition(project, 1);

            Assert.Equal(Layers(second).Select(layer => layer.ID), processor.Ids);

            project.CompositionManager.SelectedItem = first;

            Assert.Equal(Layers(first).Select(layer => layer.ID), processor.Ids);
        }

        [Fact]
        public void Ids_AHiddenLayer_LeavesTheListAndComesBack()
        {
            var (processor, project) = Create();
            var composition = AddComposition(project, 3);
            var layers = Layers(composition);
            var hidden = (IPrefab)layers[1];

            hidden.PrefabService.Visibility.Value = true;
            Assert.Equal(new[] { layers[0].ID, layers[2].ID }, processor.Ids);

            hidden.PrefabService.Visibility.Value = false;
            Assert.Equal(layers.Select(layer => layer.ID), processor.Ids);
        }

        [Fact]
        public void Ids_AHiddenComposition_IsEmpty()
        {
            var (processor, project) = Create();
            var composition = AddComposition(project, 2);

            composition.PrefabService.Visibility.Value = true;

            Assert.Empty(processor.Ids);
        }

        [Fact]
        public void Ids_ALayerAddedOrRemoved_FollowsTheList()
        {
            var (processor, project) = Create();
            var composition = AddComposition(project, 2);
            var first = Layers(composition)[0];

            composition.LayerManager.AddItem(typeof(Layer));
            Assert.Equal(3, processor.Ids.Count);

            composition.LayerManager.RemoveItem(first);
            Assert.DoesNotContain(first.ID, processor.Ids);
        }
    }
}
