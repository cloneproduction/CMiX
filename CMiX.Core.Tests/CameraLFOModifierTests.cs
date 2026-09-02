using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Cameras;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class CameraLFOModifierTests
    {
        [Fact]
        public void CameraLFOModifier_HasTwoChannelsLabeledFromTo()
        {
            var provider = TestServiceProviderFactory.Create();
            var cameraLFO = provider.GetRequiredService<CameraLFOModifier>();

            Assert.Equal(2, cameraLFO.Channels.Count);
            Assert.Equal("From", cameraLFO.Channels[0].Label);
            Assert.Equal("To", cameraLFO.Channels[1].Label);
            Assert.Same(cameraLFO.Channels[0], cameraLFO.From);
            Assert.Same(cameraLFO.Channels[1], cameraLFO.To);
        }

        [Fact]
        public void CameraLFOModifier_IsDiscoverableOnCamera()
        {
            var attributes = typeof(CameraLFOModifier).GetCustomAttributes(typeof(ModifierPanelAttribute), false);
            var owners = System.Array.ConvertAll(attributes, a => ((ModifierPanelAttribute)a).PanelOwner);

            Assert.Contains(typeof(Camera), owners);
        }

        [Fact]
        public void CameraLFOModifier_IsAlsoAnIModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var cameraLFO = provider.GetRequiredService<CameraLFOModifier>();

            Assert.IsAssignableFrom<IModifier>(cameraLFO);
        }

        [Fact]
        public void FromAndTo_CanIndependentlyShareOneModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var cameraLFO = provider.GetRequiredService<CameraLFOModifier>();
            cameraLFO.ModulatorManager.AddItem(typeof(BeatModifier));
            var beatModifier = (BeatModifier)cameraLFO.ModulatorManager.ManagerData.Items[0];

            cameraLFO.From.SetModulatorCommand.Execute(new ModulatorOutputSelection(beatModifier, "Value"));
            cameraLFO.To.SetModulatorCommand.Execute(new ModulatorOutputSelection(beatModifier, "Value"));

            Assert.Equal(beatModifier.ID, cameraLFO.From.ModulatorID.Value);
            Assert.Equal(beatModifier.ID, cameraLFO.To.ModulatorID.Value);
        }

        [Fact]
        public void CameraLFOModifier_ToModel_FromModel_RoundTripsChannelsAndNonModulatableFields()
        {
            var provider = TestServiceProviderFactory.Create();
            var cameraLFO = provider.GetRequiredService<CameraLFOModifier>();

            cameraLFO.From.Value.Value = 0.1f;
            cameraLFO.To.Value.Value = 0.9f;
            cameraLFO.PingPong.Value = true;
            cameraLFO.Axis.Value = CameraAxis.Yaw;

            var model = cameraLFO.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<CameraLFOModifier>();
            reloaded.FromModel(model);

            Assert.Equal(0.1f, reloaded.From.Value.Value);
            Assert.Equal(0.9f, reloaded.To.Value.Value);
            Assert.True(reloaded.PingPong.Value);
            Assert.Equal(CameraAxis.Yaw, reloaded.Axis.Value);
        }
    }
}
