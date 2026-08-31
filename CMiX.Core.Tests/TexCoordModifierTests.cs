using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class TexCoordModifierTests
    {
        [Fact]
        public void TexCoordModifier_HasSixChannelsAcrossTwoGroupsAndTwoSingles()
        {
            var provider = TestServiceProviderFactory.Create();
            var texCoord = provider.GetRequiredService<TexCoordModifier>();

            Assert.Equal(6, texCoord.Channels.Count);
            Assert.Same(texCoord.Channels[0], texCoord.Location.X);
            Assert.Same(texCoord.Channels[1], texCoord.Location.Y);
            Assert.Same(texCoord.Channels[2], texCoord.Scale.X);
            Assert.Same(texCoord.Channels[3], texCoord.Scale.Y);
            Assert.Same(texCoord.Channels[4], texCoord.Rotation);
            Assert.Same(texCoord.Channels[5], texCoord.Uniform);
        }

        [Fact]
        public void TexCoordModifier_IsDiscoverableOnEntity()
        {
            var attributes = typeof(TexCoordModifier).GetCustomAttributes(typeof(ModifierPanelAttribute), false);
            var owners = System.Array.ConvertAll(attributes, a => ((ModifierPanelAttribute)a).PanelOwner);

            Assert.Contains(typeof(Entity), owners);
        }

        [Fact]
        public void TexCoordModifier_IsAlsoAnIModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var texCoord = provider.GetRequiredService<TexCoordModifier>();

            Assert.IsAssignableFrom<IModifier>(texCoord);
        }

        [Fact]
        public void LocationAndScaleGroups_ShareOneModulatorManagerIndependently()
        {
            var provider = TestServiceProviderFactory.Create();
            var texCoord = provider.GetRequiredService<TexCoordModifier>();
            texCoord.ModulatorManager.AddItem(typeof(BeatModifier));
            var beatModifier = (BeatModifier)texCoord.ModulatorManager.ManagerData.Items[0];

            texCoord.Location.X.Binding.SetModulatorCommand.Execute(beatModifier);
            texCoord.Scale.Y.Binding.SetModulatorCommand.Execute(beatModifier);

            Assert.Equal(beatModifier.ID, texCoord.Location.X.Binding.ModulatorID);
            Assert.Null(texCoord.Location.Y.Binding.ModulatorID);
            Assert.Null(texCoord.Scale.X.Binding.ModulatorID);
            Assert.Equal(beatModifier.ID, texCoord.Scale.Y.Binding.ModulatorID);
        }

        [Fact]
        public void TexCoordModifier_ToModel_FromModel_RoundTripsChannelValuesAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var texCoord = provider.GetRequiredService<TexCoordModifier>();

            texCoord.Location.X.Value.Value = 5f;
            var modulatorId = Guid.NewGuid();
            texCoord.Rotation.Binding.ModulatorID = modulatorId;

            var model = texCoord.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<TexCoordModifier>();
            reloaded.FromModel(model);

            Assert.Equal(5f, reloaded.Location.X.Value.Value);
            Assert.Equal(modulatorId, reloaded.Rotation.Binding.ModulatorID);
        }

        [Fact]
        public void TexCoordModifier_ToModel_FromModel_RoundTripsModifierModeSelectorAndSamplerState()
        {
            var provider = TestServiceProviderFactory.Create();
            var texCoord = provider.GetRequiredService<TexCoordModifier>();

            texCoord.ModifierModeSelector.Mode.Value = ModifierMode.ToSpread;
            texCoord.ModifierModeSelector.Count.Value = 5;
            texCoord.SamplerState.BorderColor.Value = "#112233FF";
            texCoord.SamplerState.AddressU.Value = TextureAddressMode.Wrap;
            texCoord.SamplerState.AddressV.Value = TextureAddressMode.Border;

            var model = texCoord.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<TexCoordModifier>();
            reloaded.FromModel(model);

            Assert.Equal(ModifierMode.ToSpread, reloaded.ModifierModeSelector.Mode.Value);
            Assert.Equal(5, reloaded.ModifierModeSelector.Count.Value);
            Assert.Equal("#112233FF", reloaded.SamplerState.BorderColor.Value);
            Assert.Equal(TextureAddressMode.Wrap, reloaded.SamplerState.AddressU.Value);
            Assert.Equal(TextureAddressMode.Border, reloaded.SamplerState.AddressV.Value);
        }
    }
}
