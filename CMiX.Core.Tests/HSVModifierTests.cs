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
        public void HSVModifier_HasFourBindablesLabeledHueSaturationValueAlpha()
        {
            var provider = TestServiceProviderFactory.Create();
            var hsv = provider.GetRequiredService<HSVModifier>();

            Assert.Equal(4, hsv.Bindables.Count);
            Assert.Equal("Hue", hsv.Bindables[0].Label);
            Assert.Equal("Saturation", hsv.Bindables[1].Label);
            Assert.Equal("Value", hsv.Bindables[2].Label);
            Assert.Equal("Alpha", hsv.Bindables[3].Label);
            Assert.Same(hsv.Bindables[0], hsv.Hue);
            Assert.Same(hsv.Bindables[1], hsv.Saturation);
            Assert.Same(hsv.Bindables[2], hsv.Value);
            Assert.Same(hsv.Bindables[3], hsv.Alpha);
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
        public void AllFourBindables_CanShareOneModulatorIndependently()
        {
            var provider = TestServiceProviderFactory.Create();
            var hsv = provider.GetRequiredService<HSVModifier>();
            hsv.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)hsv.ModulatorManager.ManagerData.Items[0];

            hsv.Hue.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            hsv.Alpha.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            Assert.Equal(randomModulator.ID, hsv.Hue.ModulatorID.Value);
            Assert.Null(hsv.Saturation.ModulatorID.Value);
            Assert.Null(hsv.Value.ModulatorID.Value);
            Assert.Equal(randomModulator.ID, hsv.Alpha.ModulatorID.Value);
        }

        [Fact]
        public void HSVModifier_ToModel_FromModel_RoundTripsBindableValuesAndBinding()
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
            hsv.ModifierModeSelector.Count.Value.Value = 5;
            hsv.ColorMode.Value = ColorMode.HSV;

            var model = hsv.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<HSVModifier>();
            reloaded.FromModel(model);

            Assert.Equal(ModifierMode.ToSpread, reloaded.ModifierModeSelector.Mode.Value);
            Assert.Equal(5, reloaded.ModifierModeSelector.Count.Value.Value);
            Assert.Equal(ColorMode.HSV, reloaded.ColorMode.Value);
        }
    }
}
