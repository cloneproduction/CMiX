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
        public void CircularSpread_HasFourBindables()
        {
            var provider = TestServiceProviderFactory.Create();
            var circularSpread = provider.GetRequiredService<CircularSpreadModifier>();

            Assert.Equal(4, circularSpread.Bindables.Count);
            Assert.Equal("X", circularSpread.Bindables[0].Label);
            Assert.Equal("Y", circularSpread.Bindables[1].Label);
            Assert.Equal("Phase", circularSpread.Bindables[2].Label);
            Assert.Equal("Factor", circularSpread.Bindables[3].Label);
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
        public void AllFourBindables_CanShareOneModulatorIndependently()
        {
            var provider = TestServiceProviderFactory.Create();
            var circularSpread = provider.GetRequiredService<CircularSpreadModifier>();
            circularSpread.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)circularSpread.ModulatorManager.ManagerData.Items[0];

            circularSpread.X.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            circularSpread.Factor.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            Assert.Equal(randomModulator.ID, circularSpread.X.ModulatorID);
            Assert.Null(circularSpread.Y.ModulatorID);
            Assert.Null(circularSpread.Phase.ModulatorID);
            Assert.Equal(randomModulator.ID, circularSpread.Factor.ModulatorID);
        }

        [Fact]
        public void CircularSpread_ToModel_FromModel_RoundTripsBindableValuesAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var circularSpread = provider.GetRequiredService<CircularSpreadModifier>();

            circularSpread.X.Value = 2f;
            var modulatorId = Guid.NewGuid();
            circularSpread.Phase.ModulatorID = modulatorId;

            var model = circularSpread.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<CircularSpreadModifier>();
            reloaded.FromModel(model);

            Assert.Equal(2f, reloaded.X.Value);
            Assert.Equal(modulatorId, reloaded.Phase.ModulatorID);
        }

        [Fact]
        public void CircularSpread_ToModel_FromModel_RoundTripsModifierModeSelector()
        {
            var provider = TestServiceProviderFactory.Create();
            var circularSpread = provider.GetRequiredService<CircularSpreadModifier>();

            circularSpread.ModifierModeSelector.Mode.Value = ModifierMode.ToSpread;
            circularSpread.ModifierModeSelector.Count.Value = 5;

            var model = circularSpread.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<CircularSpreadModifier>();
            reloaded.FromModel(model);

            Assert.Equal(ModifierMode.ToSpread, reloaded.ModifierModeSelector.Mode.Value);
            Assert.Equal(5, reloaded.ModifierModeSelector.Count.Value);
        }
    }
}
