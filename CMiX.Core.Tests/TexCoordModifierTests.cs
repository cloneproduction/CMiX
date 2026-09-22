using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modifiers;
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
        public void TexCoordModifier_HasSixBindablesAcrossTwoGroupsAndTwoSingles()
        {
            var provider = TestServiceProviderFactory.Create();
            var texCoord = provider.GetRequiredService<TexCoordModifier>();

            Assert.Equal(6, texCoord.Bindables.Count);
            Assert.Same(texCoord.Bindables[0], texCoord.Location.X);
            Assert.Same(texCoord.Bindables[1], texCoord.Location.Y);
            Assert.Same(texCoord.Bindables[2], texCoord.Scale.X);
            Assert.Same(texCoord.Bindables[3], texCoord.Scale.Y);
            Assert.Same(texCoord.Bindables[4], texCoord.Rotation);
            Assert.Same(texCoord.Bindables[5], texCoord.Uniform);
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
            texCoord.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)texCoord.ModulatorManager.ManagerData.Items[0];

            texCoord.Location.X.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            texCoord.Scale.Y.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            Assert.Equal(randomModulator.ID, texCoord.Location.X.ModulatorID);
            Assert.Null(texCoord.Location.Y.ModulatorID);
            Assert.Null(texCoord.Scale.X.ModulatorID);
            Assert.Equal(randomModulator.ID, texCoord.Scale.Y.ModulatorID);
        }

        [Fact]
        public void TexCoordModifier_ToModel_FromModel_RoundTripsBindableValuesAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var texCoord = provider.GetRequiredService<TexCoordModifier>();

            texCoord.Location.X.Value = 5f;
            var modulatorId = Guid.NewGuid();
            texCoord.Rotation.ModulatorID = modulatorId;

            var model = texCoord.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<TexCoordModifier>();
            reloaded.FromModel(model);

            Assert.Equal(5f, reloaded.Location.X.Value);
            Assert.Equal(modulatorId, reloaded.Rotation.ModulatorID);
        }

        [Fact]
        public void TexCoordModifier_ToModel_FromModel_RoundTripsModifierModeSelector()
        {
            var provider = TestServiceProviderFactory.Create();
            var texCoord = provider.GetRequiredService<TexCoordModifier>();

            texCoord.ModifierModeSelector.Mode.Value = ModifierMode.ToSpread;
            texCoord.ModifierModeSelector.Count.Value = 5;

            var model = texCoord.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<TexCoordModifier>();
            reloaded.FromModel(model);

            Assert.Equal(ModifierMode.ToSpread, reloaded.ModifierModeSelector.Mode.Value);
            Assert.Equal(5, reloaded.ModifierModeSelector.Count.Value);
        }
    }
}
