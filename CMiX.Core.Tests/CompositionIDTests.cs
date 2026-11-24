// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // The composition id reaches every control under a composition: sources, materials, modifiers, filters and modulators.
    public class CompositionIDTests
    {
        public static IEnumerable<object[]> ModulatorTypes => new[]
        {
            typeof(LFOModulator), typeof(RandomModulator), typeof(BeatRandomModulator),
            typeof(FFTModulator), typeof(TrackingModulator), typeof(LISAModulator), typeof(IntegratorModulator),
        }.Select(type => new object[] { type });

        private static (IServiceProvider provider, Composition composition, Layer layer) CompositionWithALayer()
        {
            var provider = TestServiceProviderFactory.Create();
            var compositionManager = provider.GetRequiredService<PrefabManager>();
            compositionManager.AddItem(typeof(Composition));
            var composition = (Composition)compositionManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));
            return (provider, composition, (Layer)composition.LayerManager.SelectedItem);
        }

        private static (Composition composition, Entity entity, ControlRepository repository) CompositionWithAnEntity()
        {
            var (provider, composition, layer) = CompositionWithALayer();
            layer.ModelEntityManager.AddItem(typeof(Entity));
            return (composition, (Entity)layer.ModelEntityManager.SelectedItem, provider.GetRequiredService<ControlRepository>());
        }

        [Theory]
        [InlineData(typeof(Entity))]
        [InlineData(typeof(FullscreenQuad))]
        public void AddItem_OfASourceInTheTextureOfAnEntityKind_GivesItTheCompositionID(Type kind)
        {
            var (_, composition, layer) = CompositionWithALayer();
            layer.ModelEntityManager.AddItem(kind);
            var item = layer.ModelEntityManager.SelectedItem;
            var texture = item.GetType().GetProperty("Texture")!.GetValue(item)!;
            var slot = (DiffuseTexture)texture.GetType().GetProperty("DiffuseTexture")!.GetValue(texture)!;

            slot.TextureManager.AddItem(typeof(BubbleNoise));

            Assert.Equal(composition.ID, ((BubbleNoise)slot.TextureManager.SelectedItem).CompositionID);
        }

        [Fact]
        public void AddItem_OfSourcesInTheMaterial_GivesThemTheCompositionID()
        {
            var (composition, entity, _) = CompositionWithAnEntity();

            entity.Material.DiffuseTexture.TextureManager.AddItem(typeof(CheckerBoard));
            entity.Material.MaskTexture.TextureManager.AddItem(typeof(BubbleNoise));

            Assert.Equal(composition.ID, entity.Material.CompositionID);
            Assert.Equal(composition.ID, entity.Material.DiffuseTexture.CompositionID);
            Assert.Equal(composition.ID, entity.Material.MaskTexture.CompositionID);
            Assert.Equal(composition.ID, ((IHasCompositionID)entity.Material.DiffuseTexture.TextureManager.SelectedItem).CompositionID);
            Assert.Equal(composition.ID, ((IHasCompositionID)entity.Material.MaskTexture.TextureManager.SelectedItem).CompositionID);
        }

        // Sees only the controls that a manager registered, so the material needs the test above.
        [Fact]
        public void Controls_OfAnEntityWithSourcesAndAModifier_AllHaveTheCompositionID()
        {
            var (composition, entity, repository) = CompositionWithAnEntity();
            entity.Texture.DiffuseTexture.TextureManager.AddItem(typeof(CheckerBoard));
            entity.Texture.MaskTexture.TextureManager.AddItem(typeof(BubbleNoise));
            entity.ModifierManager.AddItem(typeof(ColorModifier));

            var owned = repository.Controls.OfType<IHasCompositionID>().ToList();

            Assert.NotEmpty(owned);
            Assert.All(owned, control => Assert.Equal(composition.ID, control.CompositionID));
        }

        [Fact]
        public void CompositionID_SetTwiceBeforeAnInsert_GivesTheItemOnlyTheLastID()
        {
            var manager = TestServiceProviderFactory.Create().GetRequiredService<PrefabManager>();
            var first = Guid.NewGuid();
            var second = Guid.NewGuid();

            manager.CompositionID = first;
            manager.CompositionID = second;
            var item = new RecordingCompositionIDControl();
            manager.Collection.InsertItem(item, manager.ManagerData.Items.Count);

            Assert.Equal(new[] { second }, item.ReceivedIDs);
        }

        [Fact]
        public void CompositionID_OfAManagerWithALayer_ReachesTheLayerAndItsModifiers()
        {
            var manager = TestServiceProviderFactory.Create().GetRequiredService<PrefabManager>();
            manager.AddItem(typeof(Layer));
            var layer = (Layer)manager.SelectedItem;
            var id = Guid.NewGuid();

            manager.CompositionID = id;

            Assert.Equal(id, layer.CompositionID);
            Assert.Equal(id, layer.ModifierManager.CompositionID);
        }

        [Theory]
        [MemberData(nameof(ModulatorTypes))]
        public void AddItem_OfAModulatorInAModifier_GivesItTheCompositionID(Type modulatorType)
        {
            var (_, composition, layer) = CompositionWithALayer();
            layer.ModifierManager.AddItem(typeof(ScaleModifier));
            var modifier = (Modifier)layer.ModifierManager.SelectedItem;

            modifier.ModulatorManager.AddItem(modulatorType);

            var modulator = (IHasCompositionID)modifier.ModulatorManager.SelectedItem;
            Assert.Equal(composition.ID, modulator.CompositionID);
        }

        [Fact]
        public void AddExistingItem_OfAModifierWithAModulator_GivesTheModulatorTheCompositionID()
        {
            var (provider, composition, layer) = CompositionWithALayer();
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
        public void AddItem_OfAModulatorInATextureFilter_GivesItTheCompositionID()
        {
            var (_, composition, layer) = CompositionWithALayer();
            layer.TextureModifierManager.AddItem(typeof(Blur));
            var filter = (TextureFilterBase)layer.TextureModifierManager.SelectedItem;

            filter.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Equal(composition.ID, ((IHasCompositionID)filter).CompositionID);
            Assert.Equal(composition.ID, ((IHasCompositionID)filter.ModulatorManager.SelectedItem).CompositionID);
        }

        [Theory]
        [MemberData(nameof(TextureSourceModulationTests.SourceTypes), MemberType = typeof(TextureSourceModulationTests))]
        public void AddItem_OfAModulatorInASource_GivesItTheCompositionID(Type type)
        {
            var source = (TextureSourceBase)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(type);
            var id = Guid.NewGuid();
            source.CompositionID = id;

            source.ModulatorManager.AddItem(typeof(LFOModulator));

            var modulator = (IHasCompositionID)source.ModulatorManager.SelectedItem;
            Assert.Equal(id, modulator.CompositionID);
        }

        // Records every composition id that it receives.
        private class RecordingCompositionIDControl : IControl, IHasCompositionID
        {
            private Guid _compositionID;

            public List<Guid> ReceivedIDs { get; } = new();
            public Guid ID { get; set; } = Guid.NewGuid();

            public Guid CompositionID
            {
                get => _compositionID;
                set
                {
                    _compositionID = value;
                    ReceivedIDs.Add(value);
                }
            }

            public IControlModel ToModel() => null;
            public void FromModel(IControlModel model) { }
        }
    }
}
