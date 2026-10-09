using CMiX.Core.Compositing;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ModulatorCompositionIDTests
    {
        private static (Composition composition, Layer layer, IServiceProvider provider) CreateLayer()
        {
            var provider = TestServiceProviderFactory.Create();
            var compositionManager = provider.GetRequiredService<PrefabManager>();

            compositionManager.AddItem(typeof(Composition));
            var composition = (Composition)compositionManager.SelectedItem;

            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;

            return (composition, layer, provider);
        }

        public static IEnumerable<object[]> ModulatorTypes => new[]
        {
            typeof(LFOModulator), typeof(RandomModulator), typeof(BeatRandomModulator),
            typeof(FFTModulator), typeof(TrackingModulator),
        }.Select(type => new object[] { type });

        [Theory]
        [MemberData(nameof(ModulatorTypes))]
        public void EveryModulatorInAModifier_ReceivesTheCompositionID(Type modulatorType)
        {
            var (composition, layer, _) = CreateLayer();
            layer.ModifierManager.AddItem(typeof(ScaleModifier));
            var modifier = (Modifier)layer.ModifierManager.SelectedItem;

            modifier.ModulatorManager.AddItem(modulatorType);

            var modulator = (IHasCompositionID)modifier.ModulatorManager.SelectedItem;
            Assert.Equal(composition.ID, modulator.CompositionID);
        }

        [Fact]
        public void AModulatorAlreadyInAModifier_ReceivesTheCompositionID_WhenTheModifierLandsInAComposition()
        {
            var (composition, layer, provider) = CreateLayer();
            var looseManager = provider.GetRequiredService<PrefabManager>();
            looseManager.AddItem(typeof(ScaleModifier));
            var modifier = (Modifier)looseManager.SelectedItem;
            modifier.ModulatorManager.AddItem(typeof(LFOModulator));
            var modulator = (IHasCompositionID)modifier.ModulatorManager.SelectedItem;
            Assert.Equal(Guid.Empty, modulator.CompositionID);

            layer.ModifierManager.AddExistingItem(modifier);

            Assert.Equal(composition.ID, modulator.CompositionID);
        }

        [Fact]
        public void AModulatorInATextureFilter_ReceivesTheCompositionID()
        {
            var (composition, layer, _) = CreateLayer();
            layer.TextureModifierManager.AddItem(typeof(Blur));
            var filter = (TextureFilterBase)layer.TextureModifierManager.SelectedItem;

            filter.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Equal(composition.ID, ((IHasCompositionID)filter).CompositionID);
            Assert.Equal(composition.ID, ((IHasCompositionID)filter.ModulatorManager.SelectedItem).CompositionID);
        }
    }
}
