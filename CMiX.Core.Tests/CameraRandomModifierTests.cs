using CMiX.Core.Animations;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Cameras;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class CameraRandomModifierTests
    {
        [Fact]
        public void CameraRandomModifier_HasOneChannelLabeledWidth()
        {
            var provider = TestServiceProviderFactory.Create();
            var cameraRandom = provider.GetRequiredService<CameraRandomModifier>();

            Assert.Single(cameraRandom.Channels);
            Assert.Equal("Width", cameraRandom.Channels[0].Label);
            Assert.Same(cameraRandom.Channels[0], cameraRandom.Width);
        }

        [Fact]
        public void CameraRandomModifier_IsDiscoverableOnCamera()
        {
            var attributes = typeof(CameraRandomModifier).GetCustomAttributes(typeof(ModifierPanelAttribute), false);
            var owners = System.Array.ConvertAll(attributes, a => ((ModifierPanelAttribute)a).PanelOwner);

            Assert.Contains(typeof(Camera), owners);
        }

        [Fact]
        public void CameraRandomModifier_IsAlsoAnIModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var cameraRandom = provider.GetRequiredService<CameraRandomModifier>();

            Assert.IsAssignableFrom<IModifier>(cameraRandom);
        }

        [Fact]
        public void ModulatorManager_CanAddBeatModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var cameraRandom = provider.GetRequiredService<CameraRandomModifier>();

            cameraRandom.ModulatorManager.AddItem(typeof(BeatModifier));

            Assert.Single(cameraRandom.ModulatorManager.ManagerData.Items);
            Assert.IsType<BeatModifier>(cameraRandom.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void CameraRandomModifier_ToModel_FromModel_RoundTripsChannelAndNonModulatableFields()
        {
            var provider = TestServiceProviderFactory.Create();
            var cameraRandom = provider.GetRequiredService<CameraRandomModifier>();

            cameraRandom.Width.Value.Value = 0.3f;
            cameraRandom.PingPong.Value = true;
            cameraRandom.Axis.Value = CameraAxis.FOV;

            var model = cameraRandom.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<CameraRandomModifier>();
            reloaded.FromModel(model);

            Assert.Equal(0.3f, reloaded.Width.Value.Value);
            Assert.True(reloaded.PingPong.Value);
            Assert.Equal(CameraAxis.FOV, reloaded.Axis.Value);
        }
    }
}
