using CMiX.Core.Materials;
using CMiX.Core.Materials.Modifiers;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class SelectTextureModifierTests
    {
        [Fact]
        public void SelectTextureModifier_HasNoChannels()
        {
            var provider = TestServiceProviderFactory.Create();
            var selectTexture = provider.GetRequiredService<SelectTextureModifier>();

            Assert.Empty(selectTexture.Channels);
        }

        [Fact]
        public void SelectTextureModifier_IsDiscoverableOnMaterial()
        {
            var attributes = typeof(SelectTextureModifier).GetCustomAttributes(typeof(ModifierPanelAttribute), false);
            var owners = System.Array.ConvertAll(attributes, a => ((ModifierPanelAttribute)a).PanelOwner);

            Assert.Contains(typeof(Material), owners);
        }

        [Fact]
        public void SelectTextureModifier_IsAlsoAnIModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var selectTexture = provider.GetRequiredService<SelectTextureModifier>();

            Assert.IsAssignableFrom<IModifier>(selectTexture);
        }

        [Fact]
        public void SelectTextureModifier_ToModel_FromModel_RoundTripsTextureFrom()
        {
            var provider = TestServiceProviderFactory.Create();
            var selectTexture = provider.GetRequiredService<SelectTextureModifier>();

            selectTexture.TextureFrom.Value = TextureFrom.Mask;

            var model = selectTexture.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<SelectTextureModifier>();
            reloaded.FromModel(model);

            Assert.Equal(TextureFrom.Mask, reloaded.TextureFrom.Value);
        }
    }
}
