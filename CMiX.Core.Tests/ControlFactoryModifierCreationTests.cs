using System.Linq;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ControlFactoryModifierCreationTests
    {
        [Fact]
        public void FreshLFOModifier_BindablesMatchOldLFODefaults()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var lfo = (LFOModifier)factory.Create(typeof(LFOModifier));

            Assert.Equal(0.0f, lfo.From.Value.Value);
            Assert.Equal(1.0f, lfo.To.Value.Value);
        }

        [Fact]
        public void FreshCircularSpread_BindablesMatchOldCircularSpreadDefaults()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var circularSpread = (CircularSpreadModifier)factory.Create(typeof(CircularSpreadModifier));

            Assert.Equal(1.0f, circularSpread.X.Value.Value);
            Assert.Equal(1.0f, circularSpread.Y.Value.Value);
            Assert.Equal(1.0f, circularSpread.Factor.Value.Value);
        }

        [Fact]
        public void FreshVisibilityModifier_BindableMatchesOldRandomVisibilityDefault()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var visibility = (VisibilityModifier)factory.Create(typeof(VisibilityModifier));

            Assert.Equal(0.5f, visibility.Value.Value.Value);
        }

        [Fact]
        public void FreshRenderRandomEntityModifier_BindableMatchesOldDefault()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var render = (RenderRandomEntityModifier)factory.Create(typeof(RenderRandomEntityModifier));

            Assert.Equal(1.0f, render.Control.Value.Value);
        }

        [Fact]
        public void FreshRenderSequenceEntityModifier_BindableMatchesOldDefault()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var render = (RenderSequenceEntityModifier)factory.Create(typeof(RenderSequenceEntityModifier));

            Assert.Equal(1.0f, render.Control.Value.Value);
        }

        [Fact]
        public void FreshCameraLFOModifier_BindablesMatchOldCameraLFODefaults()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var cameraLFO = (CameraLFOModifier)factory.Create(typeof(CameraLFOModifier));

            Assert.Equal(0.0f, cameraLFO.From.Value.Value);
            Assert.Equal(1.0f, cameraLFO.To.Value.Value);
        }

        [Fact]
        public void FreshGrid_NonBindableFieldsMatchOldGridDefaults()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var grid = (GridModifier)factory.Create(typeof(GridModifier));

            Assert.Equal(1, grid.ModifierModeSelector.CountX.Value.Value);
            Assert.Equal(1, grid.ModifierModeSelector.CountY.Value.Value);
            Assert.Equal(1, grid.ModifierModeSelector.CountZ.Value.Value);
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
        public void EveryAddableModifier_CanBeCreatedViaControlFactory()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var addableModifierTypes = typeof(ModifierPanelAttribute).Assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(t => t.GetCustomAttributes(typeof(ModifierPanelAttribute), false).Length > 0);

            foreach (var type in addableModifierTypes)
            {
                var control = factory.Create(type);
                Assert.NotNull(control);
            }
        }

        [Fact]
        public void Modifier_DisposeUnsubscribesAndDisposesModulatorManager()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var scale = (ScaleModifier)factory.Create(typeof(ScaleModifier));
            Assert.IsAssignableFrom<IDisposable>(scale);

            scale.Dispose();
            scale.ModulatorManager.AddItem(typeof(BeatModulator));
            Assert.Single(scale.ModulatorManager.ManagerData.Items);
        }

        [Fact]
        public void Modifier_FromModel_ReResolvesBoundModulatorFromModulatorID()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var scale = (ScaleModifier)factory.Create(typeof(ScaleModifier));
            scale.ModulatorManager.AddItem(typeof(BeatModulator));
            var beatModulator = (BeatModulator)scale.ModulatorManager.ManagerData.Items[0];
            scale.X.SetModulatorCommand.Execute(new ModulatorOutputSelection(beatModulator, beatModulator.Outputs[0]));

            var model = scale.ToModel();

            var reloaded = (ScaleModifier)factory.Create(model);

            Assert.Equal(beatModulator.ID, reloaded.X.ModulatorID.Value);
            Assert.NotNull(reloaded.X.BoundModulator);
            Assert.Equal(beatModulator.ID, reloaded.X.BoundModulator.ID);
        }
    }
}
