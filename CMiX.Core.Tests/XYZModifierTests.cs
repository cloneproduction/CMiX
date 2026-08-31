using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class XYZModifierTests
    {
        [Fact]
        public void XYZModifier_HasNineChannelsAcrossThreeGroups()
        {
            var provider = TestServiceProviderFactory.Create();
            var xyz = provider.GetRequiredService<XYZModifier>();

            Assert.Equal(9, xyz.Channels.Count);
            Assert.Same(xyz.Channels[0], xyz.Location.X);
            Assert.Same(xyz.Channels[1], xyz.Location.Y);
            Assert.Same(xyz.Channels[2], xyz.Location.Z);
            Assert.Same(xyz.Channels[3], xyz.Scale.X);
            Assert.Same(xyz.Channels[4], xyz.Scale.Y);
            Assert.Same(xyz.Channels[5], xyz.Scale.Z);
            Assert.Same(xyz.Channels[6], xyz.Rotation.X);
            Assert.Same(xyz.Channels[7], xyz.Rotation.Y);
            Assert.Same(xyz.Channels[8], xyz.Rotation.Z);
        }

        [Fact]
        public void XYZModifier_IsDiscoverableOnEntity()
        {
            var attributes = typeof(XYZModifier).GetCustomAttributes(typeof(ModifierPanelAttribute), false);
            var owners = System.Array.ConvertAll(attributes, a => ((ModifierPanelAttribute)a).PanelOwner);

            Assert.Contains(typeof(Entity), owners);
        }

        [Fact]
        public void XYZModifier_IsAlsoAnIModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var xyz = provider.GetRequiredService<XYZModifier>();

            Assert.IsAssignableFrom<IModifier>(xyz);
        }

        [Fact]
        public void LocationScaleAndRotationGroups_ShareOneModulatorManagerIndependently()
        {
            var provider = TestServiceProviderFactory.Create();
            var xyz = provider.GetRequiredService<XYZModifier>();
            xyz.ModulatorManager.AddItem(typeof(BeatModifier));
            var beatModifier = (BeatModifier)xyz.ModulatorManager.ManagerData.Items[0];

            xyz.Location.X.Binding.SetModulatorCommand.Execute(beatModifier);
            xyz.Rotation.Z.Binding.SetModulatorCommand.Execute(beatModifier);

            Assert.Equal(beatModifier.ID, xyz.Location.X.Binding.ModulatorID);
            Assert.Null(xyz.Scale.Y.Binding.ModulatorID);
            Assert.Equal(beatModifier.ID, xyz.Rotation.Z.Binding.ModulatorID);
        }

        [Fact]
        public void XYZModifier_ToModel_FromModel_RoundTripsChannelValuesAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var xyz = provider.GetRequiredService<XYZModifier>();

            xyz.Scale.Y.Value.Value = 3f;
            var modulatorId = Guid.NewGuid();
            xyz.Rotation.Z.Binding.ModulatorID = modulatorId;

            var model = xyz.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<XYZModifier>();
            reloaded.FromModel(model);

            Assert.Equal(3f, reloaded.Scale.Y.Value.Value);
            Assert.Equal(modulatorId, reloaded.Rotation.Z.Binding.ModulatorID);
        }

        [Fact]
        public void XYZModifier_ToModel_FromModel_RoundTripsModifierModeSelectorGaussianAndRandomizeFlags()
        {
            var provider = TestServiceProviderFactory.Create();
            var xyz = provider.GetRequiredService<XYZModifier>();

            xyz.ModifierModeSelector.Mode.Value = ModifierMode.ToSpread;
            xyz.ModifierModeSelector.Count.Value = 5;
            xyz.Gaussian.Value = true;
            xyz.RandomizeLocation.Value = false;
            xyz.RandomizeScale.Value = true;
            xyz.RandomizeRotation.Value = false;

            var model = xyz.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<XYZModifier>();
            reloaded.FromModel(model);

            Assert.Equal(ModifierMode.ToSpread, reloaded.ModifierModeSelector.Mode.Value);
            Assert.Equal(5, reloaded.ModifierModeSelector.Count.Value);
            Assert.True(reloaded.Gaussian.Value);
            Assert.False(reloaded.RandomizeLocation.Value);
            Assert.True(reloaded.RandomizeScale.Value);
            Assert.False(reloaded.RandomizeRotation.Value);
        }
    }
}
