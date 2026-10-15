using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class PrefabServiceIsUpdatingTests
    {
        [Fact]
        public void FreshLayer_IsUpdatingByDefault()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var layer = (Layer)factory.Create(typeof(Layer));

            Assert.True(layer.PrefabService.IsUpdating.Value);
        }

        [Fact]
        public void IsUpdating_ResetRestoresTheDefault()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var layer = (Layer)factory.Create(typeof(Layer));

            layer.PrefabService.IsUpdating.Value = false;
            layer.PrefabService.IsUpdating.Reset();

            Assert.True(layer.PrefabService.IsUpdating.Value);
        }

        [Fact]
        public void IsUpdating_ToModel_FromModel_RoundTrips()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var layer = (Layer)factory.Create(typeof(Layer));
            layer.PrefabService.IsUpdating.Value = false;

            var model = (LayerModel)layer.ToModel();
            var reloaded = (Layer)factory.Create(model);

            Assert.False(model.PrefabService.IsUpdating.Value);
            Assert.False(reloaded.PrefabService.IsUpdating.Value);
        }
    }
}
