using CMiX.Core.Animations;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Exercises the real "Add Modifier" path (ControlFactory.Create(Type), the same call
    // AutoSelectionPanel's Add button makes) rather than resolving modifiers directly via DI.
    // Every other *ModifierTests.cs file in this project uses direct DI resolution, which bypasses
    // ControlFactory.Create's FromModel(freshModel) step entirely - that step is where a fresh
    // model's empty Channels list previously left every channel at its DI default of 0, silently
    // dropping every old non-zero default. These tests would have caught that regression.
    public class ControlFactoryModifierCreationTests
    {
        [Fact]
        public void FreshLFOModifier_ChannelsMatchOldLFODefaults()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var lfo = (LFOModifier)factory.Create(typeof(LFOModifier));

            Assert.Equal(0.0f, lfo.From.Value.Value);
            Assert.Equal(1.0f, lfo.To.Value.Value);
        }

        [Fact]
        public void FreshCircularSpread_ChannelsMatchOldCircularSpreadDefaults()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var circularSpread = (CircularSpreadModifier)factory.Create(typeof(CircularSpreadModifier));

            Assert.Equal(1.0f, circularSpread.X.Value.Value);
            Assert.Equal(1.0f, circularSpread.Y.Value.Value);
            Assert.Equal(1.0f, circularSpread.Factor.Value.Value);
        }

        [Fact]
        public void FreshVisibilityModifier_ChannelMatchesOldRandomVisibilityDefault()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var visibility = (VisibilityModifier)factory.Create(typeof(VisibilityModifier));

            Assert.Equal(0.5f, visibility.Value.Value.Value);
        }

        [Fact]
        public void FreshRenderRandomEntityModifier_ChannelMatchesOldDefault()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var render = (RenderRandomEntityModifier)factory.Create(typeof(RenderRandomEntityModifier));

            Assert.Equal(1.0f, render.Control.Value.Value);
        }

        [Fact]
        public void FreshRenderSequenceEntityModifier_ChannelMatchesOldDefault()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var render = (RenderSequenceEntityModifier)factory.Create(typeof(RenderSequenceEntityModifier));

            Assert.Equal(1.0f, render.Control.Value.Value);
        }

        [Fact]
        public void FreshCameraLFOModifier_ChannelsMatchOldCameraLFODefaults()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var cameraLFO = (CameraLFOModifier)factory.Create(typeof(CameraLFOModifier));

            Assert.Equal(0.0f, cameraLFO.From.Value.Value);
            Assert.Equal(1.0f, cameraLFO.To.Value.Value);
        }

        [Fact]
        public void FreshGrid_NonChannelFieldsMatchOldGridDefaults()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var grid = (GridModifier)factory.Create(typeof(GridModifier));

            Assert.Equal(1, grid.Count.X.Value);
            Assert.Equal(1, grid.Count.Y.Value);
            Assert.Equal(1, grid.Count.Z.Value);
            Assert.Equal(CMiX.Core.Modifiers.ModifierMode.ToSpread, grid.ModifierModeSelector.Mode.Value);
        }

        [Fact]
        public void FreshHSVModifier_ColorModeMatchesOldRandomHSVDefault()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var hsv = (HSVModifier)factory.Create(typeof(HSVModifier));

            Assert.Equal(ColorMode.HSV, hsv.ColorMode.Value);
        }

        [Fact]
        public void Modifier_DisposeUnsubscribesAndDisposesModulatorManager()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var scale = (ScaleModifier)factory.Create(typeof(ScaleModifier));
            Assert.IsAssignableFrom<IDisposable>(scale);

            scale.Dispose();
            // After Dispose, adding a modulator must not run the (now unsubscribed) cleanup
            // handler against a channel referencing an unrelated, already-removed modulator.
            scale.ModulatorManager.AddItem(typeof(BeatModifier));
            Assert.Single(scale.ModulatorManager.ManagerData.Items);
        }

        [Fact]
        public void Modifier_FromModel_ReResolvesBoundModulatorFromModulatorID()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var scale = (ScaleModifier)factory.Create(typeof(ScaleModifier));
            scale.ModulatorManager.AddItem(typeof(BeatModifier));
            var beatModifier = (BeatModifier)scale.ModulatorManager.ManagerData.Items[0];
            scale.X.Binding.SetModulatorCommand.Execute(beatModifier);

            var model = scale.ToModel();

            var reloaded = (ScaleModifier)factory.Create(model);

            Assert.Equal(beatModifier.ID, reloaded.X.Binding.ModulatorID);
            Assert.NotNull(reloaded.X.Binding.BoundModulator);
            Assert.Equal(beatModifier.ID, reloaded.X.Binding.BoundModulator.ID);
        }
    }
}
