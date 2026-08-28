using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Transformation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class LinearXYZTests
    {
        [Fact]
        public void LinearXYZ_HasTwoChannelsLabeledWidthAndPhase()
        {
            var provider = TestServiceProviderFactory.Create();
            var linearXYZ = provider.GetRequiredService<LinearXYZ>();

            Assert.Equal(2, linearXYZ.Channels.Count);
            Assert.Equal("Width", linearXYZ.Channels[0].Label);
            Assert.Equal("Phase", linearXYZ.Channels[1].Label);
            Assert.Same(linearXYZ.Channels[0], linearXYZ.Width);
            Assert.Same(linearXYZ.Channels[1], linearXYZ.Phase);
        }

        [Fact]
        public void LinearXYZ_IsDiscoverableOnEntity()
        {
            var attributes = typeof(LinearXYZ).GetCustomAttributes(typeof(ModifierPanelAttribute), false);
            var owners = System.Array.ConvertAll(attributes, a => ((ModifierPanelAttribute)a).PanelOwner);

            Assert.Contains(typeof(Entity), owners);
        }

        [Fact]
        public void LinearXYZ_IsAlsoAnIModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var linearXYZ = provider.GetRequiredService<LinearXYZ>();

            Assert.IsAssignableFrom<IModifier>(linearXYZ);
        }

        [Fact]
        public void WidthAndPhase_CanIndependentlyShareOneModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var linearXYZ = provider.GetRequiredService<LinearXYZ>();
            linearXYZ.ModulatorManager.AddItem(typeof(BeatModifier));
            var beatModifier = (BeatModifier)linearXYZ.ModulatorManager.ManagerData.Items[0];

            linearXYZ.Width.Binding.SetModulatorCommand.Execute(beatModifier);
            linearXYZ.Phase.Binding.SetModulatorCommand.Execute(beatModifier);

            Assert.Equal(beatModifier.ID, linearXYZ.Width.Binding.ModulatorID);
            Assert.Equal(beatModifier.ID, linearXYZ.Phase.Binding.ModulatorID);
        }

        [Fact]
        public void LinearXYZ_ToModel_FromModel_RoundTripsChannelValuesAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var linearXYZ = provider.GetRequiredService<LinearXYZ>();

            linearXYZ.Width.Value.Value = 3f;
            var modulatorId = Guid.NewGuid();
            linearXYZ.Phase.Binding.ModulatorID = modulatorId;

            var model = linearXYZ.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<LinearXYZ>();
            reloaded.FromModel(model);

            Assert.Equal(3f, reloaded.Width.Value.Value);
            Assert.Equal(modulatorId, reloaded.Phase.Binding.ModulatorID);
        }

        [Fact]
        public void LinearXYZ_ToModel_FromModel_RoundTripsModifierModeSelectorTransformTypeAndDirection()
        {
            var provider = TestServiceProviderFactory.Create();
            var linearXYZ = provider.GetRequiredService<LinearXYZ>();

            linearXYZ.ModifierModeSelector.Mode.Value = ModifierMode.ToSpread;
            linearXYZ.ModifierModeSelector.Count.Value = 5;
            linearXYZ.TransformTypeSelector.Value = TransformType.Rotation;
            linearXYZ.DirectionXYZ.DirectionX.Value = false;
            linearXYZ.DirectionXYZ.DirectionY.Value = true;
            linearXYZ.DirectionXYZ.DirectionZ.Value = true;

            var model = linearXYZ.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<LinearXYZ>();
            reloaded.FromModel(model);

            Assert.Equal(ModifierMode.ToSpread, reloaded.ModifierModeSelector.Mode.Value);
            Assert.Equal(5, reloaded.ModifierModeSelector.Count.Value);
            Assert.Equal(TransformType.Rotation, reloaded.TransformTypeSelector.Value);
            Assert.False(reloaded.DirectionXYZ.DirectionX.Value);
            Assert.True(reloaded.DirectionXYZ.DirectionY.Value);
            Assert.True(reloaded.DirectionXYZ.DirectionZ.Value);
        }
    }
}
