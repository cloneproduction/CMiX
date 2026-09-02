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
    public class HSVModifierTests
    {
        [Fact]
        public void HSVModifier_HasFourChannelsLabeledHueSaturationValueAlpha()
        {
            var provider = TestServiceProviderFactory.Create();
            var hsv = provider.GetRequiredService<HSVModifier>();

            Assert.Equal(4, hsv.Channels.Count);
            Assert.Equal("Hue", hsv.Channels[0].Label);
            Assert.Equal("Saturation", hsv.Channels[1].Label);
            Assert.Equal("Value", hsv.Channels[2].Label);
            Assert.Equal("Alpha", hsv.Channels[3].Label);
            Assert.Same(hsv.Channels[0], hsv.Hue);
            Assert.Same(hsv.Channels[1], hsv.Saturation);
            Assert.Same(hsv.Channels[2], hsv.Value);
            Assert.Same(hsv.Channels[3], hsv.Alpha);
        }

        [Fact]
        public void HSVModifier_IsDiscoverableOnBothEntityAndLightEntity()
        {
            var attributes = typeof(HSVModifier).GetCustomAttributes(typeof(ModifierPanelAttribute), false);
            var owners = System.Array.ConvertAll(attributes, a => ((ModifierPanelAttribute)a).PanelOwner);

            Assert.Contains(typeof(Entity), owners);
            Assert.Contains(typeof(LightEntity), owners);
        }

        [Fact]
        public void HSVModifier_IsAlsoAnIModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var hsv = provider.GetRequiredService<HSVModifier>();

            Assert.IsAssignableFrom<IModifier>(hsv);
        }

        [Fact]
        public void AllFourChannels_CanShareOneModulatorIndependently()
        {
            var provider = TestServiceProviderFactory.Create();
            var hsv = provider.GetRequiredService<HSVModifier>();
            hsv.ModulatorManager.AddItem(typeof(BeatModifier));
            var beatModifier = (BeatModifier)hsv.ModulatorManager.ManagerData.Items[0];

            hsv.Hue.SetModulatorCommand.Execute(new ModulatorOutputSelection(beatModifier, "Value"));
            hsv.Alpha.SetModulatorCommand.Execute(new ModulatorOutputSelection(beatModifier, "Value"));

            Assert.Equal(beatModifier.ID, hsv.Hue.ModulatorID.Value);
            Assert.Null(hsv.Saturation.ModulatorID.Value);
            Assert.Null(hsv.Value.ModulatorID.Value);
            Assert.Equal(beatModifier.ID, hsv.Alpha.ModulatorID.Value);
        }

        [Fact]
        public void HSVModifier_ToModel_FromModel_RoundTripsChannelValuesAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var hsv = provider.GetRequiredService<HSVModifier>();

            hsv.Hue.Value.Value = 0.5f;
            var modulatorId = Guid.NewGuid();
            hsv.Saturation.ModulatorID.Value = modulatorId;

            var model = hsv.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<HSVModifier>();
            reloaded.FromModel(model);

            Assert.Equal(0.5f, reloaded.Hue.Value.Value);
            Assert.Equal(modulatorId, reloaded.Saturation.ModulatorID.Value);
        }

        [Fact]
        public void HSVModifier_ToModel_FromModel_RoundTripsModifierModeSelectorAndColorMode()
        {
            var provider = TestServiceProviderFactory.Create();
            var hsv = provider.GetRequiredService<HSVModifier>();

            hsv.ModifierModeSelector.Mode.Value = ModifierMode.ToSpread;
            hsv.ModifierModeSelector.Count.Value = 5;
            hsv.ColorMode.Value = ColorMode.HSV;

            var model = hsv.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<HSVModifier>();
            reloaded.FromModel(model);

            Assert.Equal(ModifierMode.ToSpread, reloaded.ModifierModeSelector.Mode.Value);
            Assert.Equal(5, reloaded.ModifierModeSelector.Count.Value);
            Assert.Equal(ColorMode.HSV, reloaded.ColorMode.Value);
        }
    }
}
