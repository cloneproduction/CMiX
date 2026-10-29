using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Processors;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class FilterListProcessorTests
    {
        private static (FilterListProcessor Processor, Layer Layer) Create(params Type[] filterTypes)
        {
            var provider = TestServiceProviderFactory.Create();
            var project = provider.GetRequiredService<Project>();
            project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)project.CompositionManager.ManagerData.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.ManagerData.SelectedItem;
            foreach (var type in filterTypes)
                AddEnabledFilter(layer, type);

            return (provider.GetRequiredService<FilterListProcessor>(), layer);
        }

        // A new filter has its blend off, so the tests switch it on.
        private static void AddEnabledFilter(Layer layer, Type type)
        {
            layer.TextureModifierManager.AddItem(type);
            ((ITextureFilter)layer.TextureModifierManager.ManagerData.Items.Last()).Blend.IsEnabled.Value = true;
        }

        private static List<IControl> Filters(Layer layer) => layer.TextureModifierManager.ManagerData.Items.ToList();

        [Fact]
        public void GetEnabledIds_FiltersWithTheBlendOn_ListsThemInTheOrderOfTheList()
        {
            var (processor, layer) = Create(typeof(Blur), typeof(Invert), typeof(Dither));

            Assert.Equal(Filters(layer).Select(filter => filter.ID), processor.GetEnabledIds(layer));
        }

        [Fact]
        public void GetEnabledIds_AFilterWithTheBlendOff_LeavesTheListAndComesBack()
        {
            var (processor, layer) = Create(typeof(Blur), typeof(Invert));
            var filters = Filters(layer);
            var blur = (ITextureFilter)filters[0];

            blur.Blend.IsEnabled.Value = false;
            Assert.Equal(new[] { filters[1].ID }, processor.GetEnabledIds(layer));

            blur.Blend.IsEnabled.Value = true;
            Assert.Equal(filters.Select(filter => filter.ID), processor.GetEnabledIds(layer));
        }

        [Fact]
        public void GetEnabledIds_AHiddenFilter_LeavesTheListAndComesBack()
        {
            var (processor, layer) = Create(typeof(Blur), typeof(Invert));
            var filters = Filters(layer);

            ((IPrefab)filters[1]).PrefabService.Visibility.Value = true;
            Assert.Equal(new[] { filters[0].ID }, processor.GetEnabledIds(layer));

            ((IPrefab)filters[1]).PrefabService.Visibility.Value = false;
            Assert.Equal(filters.Select(filter => filter.ID), processor.GetEnabledIds(layer));
        }

        [Fact]
        public void GetEnabledIds_AFilterAddedOrRemoved_FollowsTheList()
        {
            var (processor, layer) = Create(typeof(Blur));
            var blur = Filters(layer)[0];

            AddEnabledFilter(layer, typeof(Invert));
            Assert.Equal(2, processor.GetEnabledIds(layer).Count);

            layer.TextureModifierManager.RemoveItem(blur);
            Assert.DoesNotContain(blur.ID, processor.GetEnabledIds(layer));
        }

        [Fact]
        public void GetEnabledIds_ANewFilter_IsNotListedUntilTheBlendIsOn()
        {
            var (processor, layer) = Create();
            layer.TextureModifierManager.AddItem(typeof(Blur));
            var blur = (ITextureFilter)Filters(layer)[0];

            Assert.Empty(processor.GetEnabledIds(layer));

            blur.Blend.IsEnabled.Value = true;
            Assert.Single(processor.GetEnabledIds(layer));
        }

        [Fact]
        public void GetEnabledIds_ANullOwner_IsEmpty()
        {
            var (processor, _) = Create();

            Assert.Empty(processor.GetEnabledIds(null));
        }
    }
}
