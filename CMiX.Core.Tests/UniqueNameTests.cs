using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // UniqueName gives the next free name in the Blender style: Layer, Layer.001, Layer.002.
    public class UniqueNameTests
    {
        private static (ControlFactory factory, PrefabManager layers) Create()
        {
            var provider = TestServiceProviderFactory.Create();
            var compositions = provider.GetRequiredService<PrefabManager>();
            compositions.AddItem(typeof(Composition));
            var layers = ((Composition)compositions.SelectedItem).LayerManager;
            layers.AddItem(typeof(Layer));
            return (provider.GetRequiredService<ControlFactory>(), layers);
        }

        [Fact]
        public void UniqueName_OfAUsedName_AddsTheFirstFreeNumber()
        {
            var (factory, _) = Create();

            Assert.Equal("Layer.001", factory.UniqueName("Layer"));
        }

        [Fact]
        public void UniqueName_OfANumberedName_DropsTheOldNumber()
        {
            var (factory, layers) = Create();
            layers.AddItem(typeof(Layer));

            Assert.Equal("Layer", ((Layer)layers.ManagerData.Items[0]).PrefabService.Name.Value);
            Assert.Equal("Layer.002", factory.UniqueName("Layer.001"));
        }

        [Fact]
        public void UniqueName_OfANameTheUserTyped_AddsANumber()
        {
            var (factory, _) = Create();

            Assert.Equal("Hero.001", factory.UniqueName("Hero"));
        }

        [Fact]
        public void UniqueName_AfterARename_SeesTheNewName()
        {
            var (factory, layers) = Create();
            ((Layer)layers.SelectedItem).PrefabService.Name.Value = "Hero.001";

            Assert.Equal("Hero.002", factory.UniqueName("Hero"));
        }
    }
}
