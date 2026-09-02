using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Lights;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class CircularSpreadModifierTests
    {
        [Fact]
        public void CircularSpread_HasFourChannels()
        {
            var provider = TestServiceProviderFactory.Create();
            var circularSpread = provider.GetRequiredService<CircularSpreadModifier>();

            Assert.Equal(4, circularSpread.Channels.Count);
            Assert.Equal("X", circularSpread.Channels[0].Label);
            Assert.Equal("Y", circularSpread.Channels[1].Label);
            Assert.Equal("Phase", circularSpread.Channels[2].Label);
            Assert.Equal("Factor", circularSpread.Channels[3].Label);
        }

        [Fact]
        public void CircularSpread_IsDiscoverableOnBothEntityAndLightEntity()
        {
            var attributes = typeof(CircularSpreadModifier).GetCustomAttributes(typeof(ModifierPanelAttribute), false);
            var owners = System.Array.ConvertAll(attributes, a => ((ModifierPanelAttribute)a).PanelOwner);

            Assert.Contains(typeof(Entity), owners);
            Assert.Contains(typeof(LightEntity), owners);
        }

        [Fact]
        public void CircularSpread_IsAlsoAnIModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var circularSpread = provider.GetRequiredService<CircularSpreadModifier>();

            Assert.IsAssignableFrom<IModifier>(circularSpread);
        }

        [Fact]
        public void AllFourChannels_CanShareOneModulatorIndependently()
        {
            var provider = TestServiceProviderFactory.Create();
            var circularSpread = provider.GetRequiredService<CircularSpreadModifier>();
            circularSpread.ModulatorManager.AddItem(typeof(BeatModulator));
            var beatModulator = (BeatModulator)circularSpread.ModulatorManager.ManagerData.Items[0];

            circularSpread.X.SetModulatorCommand.Execute(new ModulatorOutputSelection(beatModulator, beatModulator.Outputs[0]));
            circularSpread.Factor.SetModulatorCommand.Execute(new ModulatorOutputSelection(beatModulator, beatModulator.Outputs[0]));

            Assert.Equal(beatModulator.ID, circularSpread.X.ModulatorID.Value);
            Assert.Null(circularSpread.Y.ModulatorID.Value);
            Assert.Null(circularSpread.Phase.ModulatorID.Value);
            Assert.Equal(beatModulator.ID, circularSpread.Factor.ModulatorID.Value);
        }

        [Fact]
        public void CircularSpread_ToModel_FromModel_RoundTripsChannelValuesAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var circularSpread = provider.GetRequiredService<CircularSpreadModifier>();

            circularSpread.X.Value.Value = 2f;
            var modulatorId = Guid.NewGuid();
            circularSpread.Phase.ModulatorID.Value = modulatorId;

            var model = circularSpread.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<CircularSpreadModifier>();
            reloaded.FromModel(model);

            Assert.Equal(2f, reloaded.X.Value.Value);
            Assert.Equal(modulatorId, reloaded.Phase.ModulatorID.Value);
        }

        [Fact]
        public void CircularSpread_ToModel_FromModel_RoundTripsModifierModeSelector()
        {
            var provider = TestServiceProviderFactory.Create();
            var circularSpread = provider.GetRequiredService<CircularSpreadModifier>();

            circularSpread.ModifierModeSelector.Mode.Value = ModifierMode.ToSpread;
            circularSpread.ModifierModeSelector.Count.Value.Value = 5;

            var model = circularSpread.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<CircularSpreadModifier>();
            reloaded.FromModel(model);

            Assert.Equal(ModifierMode.ToSpread, reloaded.ModifierModeSelector.Mode.Value);
            Assert.Equal(5, reloaded.ModifierModeSelector.Count.Value.Value);
        }
    }
}
