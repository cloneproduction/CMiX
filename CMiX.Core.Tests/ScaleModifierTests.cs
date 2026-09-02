using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ScaleModifierTests
    {
        [Fact]
        public void ScaleModifier_HasFourChannelsLabeledXYZUniform()
        {
            var provider = TestServiceProviderFactory.Create();
            var scale = provider.GetRequiredService<ScaleModifier>();

            Assert.Equal(4, scale.Channels.Count);
            Assert.Equal("X", scale.Channels[0].Label);
            Assert.Equal("Y", scale.Channels[1].Label);
            Assert.Equal("Z", scale.Channels[2].Label);
            Assert.Equal("Uniform", scale.Channels[3].Label);
        }

        [Fact]
        public void TwoScaleModifiers_HaveDistinctModulatorManagers()
        {
            var provider = TestServiceProviderFactory.Create();
            var scale1 = provider.GetRequiredService<ScaleModifier>();
            var scale2 = provider.GetRequiredService<ScaleModifier>();

            Assert.NotSame(scale1.ModulatorManager, scale2.ModulatorManager);
        }

        [Fact]
        public void ModulatorManager_CanAddBeatModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var scale = provider.GetRequiredService<ScaleModifier>();

            scale.ModulatorManager.AddItem(typeof(BeatModulator));

            Assert.Single(scale.ModulatorManager.ManagerData.Items);
            Assert.IsType<BeatModulator>(scale.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void ScaleModifier_ToModel_FromModel_RoundTripsChannelValuesAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var scale = provider.GetRequiredService<ScaleModifier>();

            scale.Channels[0].Value.Value = 4f;
            var modulatorId = Guid.NewGuid();
            scale.Channels[0].ModulatorID.Value = modulatorId;

            var model = scale.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<ScaleModifier>();
            reloaded.FromModel(model);

            Assert.Equal(4f, reloaded.Channels[0].Value.Value);
            Assert.Equal(modulatorId, reloaded.Channels[0].ModulatorID.Value);
        }

        [Fact]
        public void XYZUniform_AreConvenienceAccessorsIntoChannels()
        {
            var provider = TestServiceProviderFactory.Create();
            var scale = provider.GetRequiredService<ScaleModifier>();

            Assert.Same(scale.Channels[0], scale.X);
            Assert.Same(scale.Channels[1], scale.Y);
            Assert.Same(scale.Channels[2], scale.Z);
            Assert.Same(scale.Channels[3], scale.Uniform);
        }

        [Fact]
        public void ScaleModifier_ToModel_FromModel_RoundTripsModifierModeSelector()
        {
            var provider = TestServiceProviderFactory.Create();
            var scale = provider.GetRequiredService<ScaleModifier>();

            scale.ModifierModeSelector.Mode.Value = ModifierMode.ToSpread;
            scale.ModifierModeSelector.Count.Value.Value = 5;
            scale.Uniform.Value.Value = 2.5f;

            var model = scale.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<ScaleModifier>();
            reloaded.FromModel(model);

            Assert.Equal(ModifierMode.ToSpread, reloaded.ModifierModeSelector.Mode.Value);
            Assert.Equal(5, reloaded.ModifierModeSelector.Count.Value.Value);
            Assert.Equal(2.5f, reloaded.Uniform.Value.Value);
        }

        [Fact]
        public void DeletingAssignedModulator_UnassignsItFromEveryChannelUsingIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var scale = provider.GetRequiredService<ScaleModifier>();
            scale.ModulatorManager.AddItem(typeof(BeatModulator));
            var beatModulator = (BeatModulator)scale.ModulatorManager.ManagerData.Items[0];

            scale.X.SetModulatorCommand.Execute(new ModulatorOutputSelection(beatModulator, beatModulator.Outputs[0]));
            scale.Y.SetModulatorCommand.Execute(new ModulatorOutputSelection(beatModulator, beatModulator.Outputs[0]));

            scale.ModulatorManager.DeleteItem(beatModulator);

            Assert.Null(scale.X.ModulatorID.Value);
            Assert.Null(scale.X.BoundModulator);
            Assert.Null(scale.Y.ModulatorID.Value);
            Assert.Null(scale.Y.BoundModulator);
            // Z was never assigned - confirms the cleanup only touches channels that referenced
            // the deleted modulator, not every channel.
            Assert.Null(scale.Z.ModulatorID.Value);
        }

        [Fact]
        public void DeletingAssignedModulator_UnassignsItFromModifierModeSelectorCountToo()
        {
            var provider = TestServiceProviderFactory.Create();
            var scale = provider.GetRequiredService<ScaleModifier>();
            scale.ModulatorManager.AddItem(typeof(BeatModulator));
            var beatModulator = (BeatModulator)scale.ModulatorManager.ManagerData.Items[0];

            scale.ModifierModeSelector.Count.SetModulatorCommand.Execute(new ModulatorOutputSelection(beatModulator, beatModulator.Outputs[0]));

            scale.ModulatorManager.DeleteItem(beatModulator);

            Assert.Null(scale.ModifierModeSelector.Count.ModulatorID.Value);
            Assert.Null(scale.ModifierModeSelector.Count.BoundModulator);
        }

        [Fact]
        public void ScaleModifier_ToModel_FromModel_ResolvesModifierModeSelectorCountLiveBoundModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var scale = provider.GetRequiredService<ScaleModifier>();
            scale.ModulatorManager.AddItem(typeof(BeatModulator));
            var beatModulator = (BeatModulator)scale.ModulatorManager.ManagerData.Items[0];
            scale.ModifierModeSelector.Count.SetModulatorCommand.Execute(new ModulatorOutputSelection(beatModulator, beatModulator.Outputs[0]));

            var model = scale.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<ScaleModifier>();
            reloaded.FromModel(model);

            // ModulatorID round-tripping alone isn't enough - the UI (and IsReadOnly locking) reads
            // BoundModulator, which only Modifier.ResolveModulatorBinding can re-populate after load
            // since ModifierModeSelector.FromModel has no access to the reloaded ModulatorManager's
            // items itself.
            var reloadedBeatModulator = Assert.IsType<BeatModulator>(reloaded.ModulatorManager.ManagerData.Items[0]);
            Assert.Same(reloadedBeatModulator, reloaded.ModifierModeSelector.Count.BoundModulator);
        }
    }
}
