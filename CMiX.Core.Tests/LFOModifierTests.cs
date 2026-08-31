using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Transformation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class LFOModifierTests
    {
        [Fact]
        public void LFOModifier_HasThreeChannelsLabeledFromToRandomizePhase()
        {
            var provider = TestServiceProviderFactory.Create();
            var lfo = provider.GetRequiredService<LFOModifier>();

            Assert.Equal(3, lfo.Channels.Count);
            Assert.Equal("From", lfo.Channels[0].Label);
            Assert.Equal("To", lfo.Channels[1].Label);
            Assert.Equal("Randomize Phase", lfo.Channels[2].Label);
            Assert.Same(lfo.Channels[0], lfo.From);
            Assert.Same(lfo.Channels[1], lfo.To);
            Assert.Same(lfo.Channels[2], lfo.RandomizePhase);
        }

        [Fact]
        public void LFOModifier_IsDiscoverableOnEntity()
        {
            var attributes = typeof(LFOModifier).GetCustomAttributes(typeof(ModifierPanelAttribute), false);
            var owners = System.Array.ConvertAll(attributes, a => ((ModifierPanelAttribute)a).PanelOwner);

            Assert.Contains(typeof(Entity), owners);
        }

        [Fact]
        public void LFOModifier_IsAlsoAnIModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var lfo = provider.GetRequiredService<LFOModifier>();

            Assert.IsAssignableFrom<IModifier>(lfo);
        }

        [Fact]
        public void FromToAndRandomizePhase_CanIndependentlyShareOneModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var lfo = provider.GetRequiredService<LFOModifier>();
            lfo.ModulatorManager.AddItem(typeof(BeatModifier));
            var beatModifier = (BeatModifier)lfo.ModulatorManager.ManagerData.Items[0];

            lfo.From.Binding.SetModulatorCommand.Execute(beatModifier);
            lfo.RandomizePhase.Binding.SetModulatorCommand.Execute(beatModifier);

            Assert.Equal(beatModifier.ID, lfo.From.Binding.ModulatorID);
            Assert.Null(lfo.To.Binding.ModulatorID);
            Assert.Equal(beatModifier.ID, lfo.RandomizePhase.Binding.ModulatorID);
        }

        [Fact]
        public void LFOModifier_ToModel_FromModel_RoundTripsChannelValuesAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var lfo = provider.GetRequiredService<LFOModifier>();

            lfo.From.Value.Value = 0.25f;
            var modulatorId = Guid.NewGuid();
            lfo.To.Binding.ModulatorID = modulatorId;

            var model = lfo.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<LFOModifier>();
            reloaded.FromModel(model);

            Assert.Equal(0.25f, reloaded.From.Value.Value);
            Assert.Equal(modulatorId, reloaded.To.Binding.ModulatorID);
        }

        [Fact]
        public void LFOModifier_ToModel_FromModel_RoundTripsNonModulatableFields()
        {
            var provider = TestServiceProviderFactory.Create();
            var lfo = provider.GetRequiredService<LFOModifier>();

            lfo.ModifierModeSelector.Mode.Value = ModifierMode.ToSpread;
            lfo.ModifierModeSelector.Count.Value = 5;
            lfo.PingPong.Value = true;
            lfo.DirectionXYZ.DirectionX.Value = false;
            lfo.DirectionXYZ.DirectionY.Value = true;
            lfo.DirectionXYZ.DirectionZ.Value = true;
            lfo.TransformType.Value = TransformType.Rotation;

            var model = lfo.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<LFOModifier>();
            reloaded.FromModel(model);

            Assert.Equal(ModifierMode.ToSpread, reloaded.ModifierModeSelector.Mode.Value);
            Assert.Equal(5, reloaded.ModifierModeSelector.Count.Value);
            Assert.True(reloaded.PingPong.Value);
            Assert.False(reloaded.DirectionXYZ.DirectionX.Value);
            Assert.True(reloaded.DirectionXYZ.DirectionY.Value);
            Assert.True(reloaded.DirectionXYZ.DirectionZ.Value);
            Assert.Equal(TransformType.Rotation, reloaded.TransformType.Value);
        }
    }
}
