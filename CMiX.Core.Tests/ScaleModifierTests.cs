using CMiX.Core.Animations;
using CMiX.Core.Modulation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ScaleModifierTests
    {
        [Fact]
        public void ScaleModifier_HasThreeChannelsLabeledXYZ()
        {
            var provider = TestServiceProviderFactory.Create();
            var scale = provider.GetRequiredService<ScaleModifier>();

            Assert.Equal(3, scale.Channels.Count);
            Assert.Equal("X", scale.Channels[0].Label);
            Assert.Equal("Y", scale.Channels[1].Label);
            Assert.Equal("Z", scale.Channels[2].Label);
        }

        [Fact]
        public void TwoScaleModifiers_HaveDistinctModulatorManagers()
        {
            var provider = TestServiceProviderFactory.Create();
            var scale1 = provider.GetRequiredService<ScaleModifier>();
            var scale2 = provider.GetRequiredService<ScaleModifier>();

            Assert.NotSame(scale1.ModulatorManager, scale2.ModulatorManager);
        }

        [Fact]
        public void ModulatorManager_CanAddBeatModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var scale = provider.GetRequiredService<ScaleModifier>();

            scale.ModulatorManager.AddItem(typeof(BeatModifier));

            Assert.Single(scale.ModulatorManager.ManagerData.Items);
            Assert.IsType<BeatModifier>(scale.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void ScaleModifier_ToModel_FromModel_RoundTripsChannelValuesAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var scale = provider.GetRequiredService<ScaleModifier>();

            scale.Channels[0].Value.Value = 4f;
            var modulatorId = Guid.NewGuid();
            scale.Channels[0].Binding.ModulatorID = modulatorId;
            scale.Channels[0].Binding.Depth.Value = 0.5f;

            var model = scale.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<ScaleModifier>();
            reloaded.FromModel(model);

            Assert.Equal(4f, reloaded.Channels[0].Value.Value);
            Assert.Equal(modulatorId, reloaded.Channels[0].Binding.ModulatorID);
            Assert.Equal(0.5f, reloaded.Channels[0].Binding.Depth.Value);
        }
    }
}
