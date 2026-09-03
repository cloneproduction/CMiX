using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Lights;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class TranslateModifierTests
    {
        [Fact]
        public void TranslateModifier_HasThreeBindablesLabeledXYZ()
        {
            var provider = TestServiceProviderFactory.Create();
            var position = provider.GetRequiredService<TranslateModifier>();

            Assert.Equal(3, position.Bindables.Count);
            Assert.Equal("X", position.Bindables[0].Label);
            Assert.Equal("Y", position.Bindables[1].Label);
            Assert.Equal("Z", position.Bindables[2].Label);
        }

        [Fact]
        public void TranslateModifier_IsDiscoverableOnBothEntityAndLightEntity()
        {
            var attributes = typeof(TranslateModifier).GetCustomAttributes(typeof(ModifierPanelAttribute), false);
            var owners = System.Array.ConvertAll(attributes, a => ((ModifierPanelAttribute)a).PanelOwner);

            Assert.Contains(typeof(Entity), owners);
            Assert.Contains(typeof(LightEntity), owners);
        }

        [Fact]
        public void TranslateModifier_IsAlsoAnIModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var position = provider.GetRequiredService<TranslateModifier>();

            Assert.IsAssignableFrom<IModifier>(position);
        }

        [Fact]
        public void ModulatorManager_CanAddBeatModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var position = provider.GetRequiredService<TranslateModifier>();

            position.ModulatorManager.AddItem(typeof(BeatModulator));

            Assert.Single(position.ModulatorManager.ManagerData.Items);
            Assert.IsType<BeatModulator>(position.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void TranslateModifier_ToModel_FromModel_RoundTripsBindableValuesAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var position = provider.GetRequiredService<TranslateModifier>();

            position.Bindables[0].Value.Value = 4f;
            var modulatorId = Guid.NewGuid();
            position.Bindables[0].ModulatorID.Value = modulatorId;

            var model = position.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<TranslateModifier>();
            reloaded.FromModel(model);

            Assert.Equal(4f, reloaded.Bindables[0].Value.Value);
            Assert.Equal(modulatorId, reloaded.Bindables[0].ModulatorID.Value);
        }

        [Fact]
        public void TranslateModifier_ToModel_FromModel_RoundTripsModifierModeSelector()
        {
            var provider = TestServiceProviderFactory.Create();
            var position = provider.GetRequiredService<TranslateModifier>();

            position.ModifierModeSelector.Mode.Value = ModifierMode.ToSpread;
            position.ModifierModeSelector.Count.Value.Value = 5;

            var model = position.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<TranslateModifier>();
            reloaded.FromModel(model);

            Assert.Equal(ModifierMode.ToSpread, reloaded.ModifierModeSelector.Mode.Value);
            Assert.Equal(5, reloaded.ModifierModeSelector.Count.Value.Value);
        }
    }
}
