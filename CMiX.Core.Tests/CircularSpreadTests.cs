using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Lights;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class CircularSpreadTests
    {
        [Fact]
        public void CircularSpread_HasFourChannels()
        {
            var provider = TestServiceProviderFactory.Create();
            var circularSpread = provider.GetRequiredService<CircularSpread>();

            Assert.Equal(4, circularSpread.Channels.Count);
            Assert.Equal("X", circularSpread.Channels[0].Label);
            Assert.Equal("Y", circularSpread.Channels[1].Label);
            Assert.Equal("Phase", circularSpread.Channels[2].Label);
            Assert.Equal("Factor", circularSpread.Channels[3].Label);
        }

        [Fact]
        public void CircularSpread_IsDiscoverableOnBothEntityAndLightEntity()
        {
            var attributes = typeof(CircularSpread).GetCustomAttributes(typeof(ModifierPanelAttribute), false);
            var owners = System.Array.ConvertAll(attributes, a => ((ModifierPanelAttribute)a).PanelOwner);

            Assert.Contains(typeof(Entity), owners);
            Assert.Contains(typeof(LightEntity), owners);
        }

        [Fact]
        public void CircularSpread_IsAlsoAnIModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var circularSpread = provider.GetRequiredService<CircularSpread>();

            Assert.IsAssignableFrom<IModifier>(circularSpread);
        }

        [Fact]
        public void AllFourChannels_CanShareOneModulatorIndependently()
        {
            var provider = TestServiceProviderFactory.Create();
            var circularSpread = provider.GetRequiredService<CircularSpread>();
            circularSpread.ModulatorManager.AddItem(typeof(BeatModifier));
            var beatModifier = (BeatModifier)circularSpread.ModulatorManager.ManagerData.Items[0];

            circularSpread.X.Binding.SetModulatorCommand.Execute(beatModifier);
            circularSpread.Factor.Binding.SetModulatorCommand.Execute(beatModifier);

            Assert.Equal(beatModifier.ID, circularSpread.X.Binding.ModulatorID);
            Assert.Null(circularSpread.Y.Binding.ModulatorID);
            Assert.Null(circularSpread.Phase.Binding.ModulatorID);
            Assert.Equal(beatModifier.ID, circularSpread.Factor.Binding.ModulatorID);
        }

        [Fact]
        public void CircularSpread_ToModel_FromModel_RoundTripsChannelValuesAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var circularSpread = provider.GetRequiredService<CircularSpread>();

            circularSpread.X.Value.Value = 2f;
            var modulatorId = Guid.NewGuid();
            circularSpread.Phase.Binding.ModulatorID = modulatorId;

            var model = circularSpread.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<CircularSpread>();
            reloaded.FromModel(model);

            Assert.Equal(2f, reloaded.X.Value.Value);
            Assert.Equal(modulatorId, reloaded.Phase.Binding.ModulatorID);
        }

        [Fact]
        public void CircularSpread_ToModel_FromModel_RoundTripsModifierModeSelector()
        {
            var provider = TestServiceProviderFactory.Create();
            var circularSpread = provider.GetRequiredService<CircularSpread>();

            circularSpread.ModifierModeSelector.Mode.Value = ModifierMode.ToSpread;
            circularSpread.ModifierModeSelector.Count.Value = 5;

            var model = circularSpread.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<CircularSpread>();
            reloaded.FromModel(model);

            Assert.Equal(ModifierMode.ToSpread, reloaded.ModifierModeSelector.Mode.Value);
            Assert.Equal(5, reloaded.ModifierModeSelector.Count.Value);
        }
    }
}
