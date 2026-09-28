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
    public class ColorModifierTests
    {
        [Fact]
        public void ColorModifier_HasFourBindablesLabeledHueSaturationValueAlpha()
        {
            var provider = TestServiceProviderFactory.Create();
            var hsv = provider.GetRequiredService<ColorModifier>();

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
        public void ColorModifier_IsDiscoverableOnBothEntityAndLightEntity()
        {
            var attributes = typeof(ColorModifier).GetCustomAttributes(typeof(ModifierPanelAttribute), false);
            var owners = System.Array.ConvertAll(attributes, a => ((ModifierPanelAttribute)a).PanelOwner);

            Assert.Contains(typeof(Entity), owners);
            Assert.Contains(typeof(LightEntity), owners);
        }

        [Fact]
        public void ColorModifier_IsAlsoAnIModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var hsv = provider.GetRequiredService<ColorModifier>();

            Assert.IsAssignableFrom<IModifier>(hsv);
        }

        [Fact]
        public void AllFourBindables_CanShareOneModulatorIndependently()
        {
            var provider = TestServiceProviderFactory.Create();
            var hsv = provider.GetRequiredService<ColorModifier>();
            hsv.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)hsv.ModulatorManager.ManagerData.Items[0];

            hsv.Hue.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            hsv.Alpha.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            Assert.Equal(randomModulator.ID, hsv.Hue.ModulatorID);
            Assert.Null(hsv.Saturation.ModulatorID);
            Assert.Null(hsv.Value.ModulatorID);
            Assert.Equal(randomModulator.ID, hsv.Alpha.ModulatorID);
        }

        [Fact]
        public void ColorModifier_ToModel_FromModel_RoundTripsBindableValuesAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var hsv = provider.GetRequiredService<ColorModifier>();

            hsv.Hue.Value = 0.5f;
            var modulatorId = Guid.NewGuid();
            hsv.Saturation.ModulatorID = modulatorId;

            var model = hsv.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<ColorModifier>();
            reloaded.FromModel(model);

            Assert.Equal(0.5f, reloaded.Hue.Value);
            Assert.Equal(modulatorId, reloaded.Saturation.ModulatorID);
        }

        [Fact]
        public void ColorModifier_ToModel_FromModel_RoundTripsModifierModeSelectorAndColorMode()
        {
            var provider = TestServiceProviderFactory.Create();
            var hsv = provider.GetRequiredService<ColorModifier>();

            hsv.ModifierModeSelector.Mode.Value = ModifierMode.ToSpread;
            hsv.ModifierModeSelector.Count.Value = 5;
            hsv.ColorMode.Value = ColorMode.HSV;

            var model = hsv.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<ColorModifier>();
            reloaded.FromModel(model);

            Assert.Equal(ModifierMode.ToSpread, reloaded.ModifierModeSelector.Mode.Value);
            Assert.Equal(5, reloaded.ModifierModeSelector.Count.Value);
            Assert.Equal(ColorMode.HSV, reloaded.ColorMode.Value);
        }

        [Fact]
        public void AllFourBindables_ConstructAtTheirOwnDefault_AndResetReturnsToItAfterAChange()
        {
            var provider = TestServiceProviderFactory.Create();
            var hsv = provider.GetRequiredService<ColorModifier>();

            Assert.Equal(0f, hsv.Hue.Value);
            Assert.Equal(0f, hsv.Saturation.Value);
            Assert.Equal(1f, hsv.Value.Value);
            Assert.Equal(1f, hsv.Alpha.Value);

            hsv.Hue.Value = 0.2f;
            hsv.Hue.ResetCommand.Execute(null);

            Assert.Equal(0f, hsv.Hue.Value);
        }
    }
}
