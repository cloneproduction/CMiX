using System.Linq;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ControlFactoryModifierCreationTests
    {
        [Fact]
        public void FreshCircularSpread_BindablesMatchOldCircularSpreadDefaults()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var circularSpread = (CircularSpreadModifier)factory.Create(typeof(CircularSpreadModifier));

            Assert.Equal(1.0f, circularSpread.X.Value);
            Assert.Equal(1.0f, circularSpread.Y.Value);
            Assert.Equal(1.0f, circularSpread.Factor.Value);
        }

        [Fact]
        public void FreshVisibilityModifier_BindableMatchesOldRandomVisibilityDefault()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var visibility = (VisibilityModifier)factory.Create(typeof(VisibilityModifier));

            Assert.Equal(0.5f, visibility.Value.Value);
        }

        [Fact]
        public void FreshRenderRandomEntityModifier_BindableMatchesOldDefault()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var render = (RenderRandomEntityModifier)factory.Create(typeof(RenderRandomEntityModifier));

            Assert.Equal(1.0f, render.Control.Value);
        }

        [Fact]
        public void FreshRenderSequenceEntityModifier_BindableMatchesOldDefault()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var render = (RenderSequenceEntityModifier)factory.Create(typeof(RenderSequenceEntityModifier));

            Assert.Equal(1.0f, render.Control.Value);
        }

        [Fact]
        public void FreshGrid_NonBindableFieldsMatchOldGridDefaults()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var grid = (GridModifier)factory.Create(typeof(GridModifier));

            Assert.Equal(1, grid.Count.X.Value);
            Assert.Equal(1, grid.Count.Y.Value);
            Assert.Equal(1, grid.Count.Z.Value);
        }

        [Fact]
        public void EveryFreshModifier_WithASingleModifierModeSelector_DefaultsCountToOne()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var addableModifierTypes = typeof(ModifierPanelAttribute).Assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(t => t.GetCustomAttributes(typeof(ModifierPanelAttribute), false).Length > 0);

            foreach (var type in addableModifierTypes)
            {
                var property = type.GetProperty("ModifierModeSelector");
                if (property == null || property.PropertyType != typeof(CMiX.Core.Modifiers.ModifierModeSelector))
                    continue;

                var control = factory.Create(type);
                var selector = (CMiX.Core.Modifiers.ModifierModeSelector)property.GetValue(control);

                Assert.True(1 == selector.Count.Value, $"{type.Name}'s ModifierModeSelector.Count should default to 1.");
            }
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
            scale.ModulatorManager.AddItem(typeof(RandomModulator));
            Assert.Single(scale.ModulatorManager.ManagerData.Items);
        }

        [Fact]
        public void Modifier_FromModel_ReResolvesBoundModulatorFromModulatorID()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();

            var scale = (ScaleModifier)factory.Create(typeof(ScaleModifier));
            scale.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)scale.ModulatorManager.ManagerData.Items[0];
            scale.X.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            var model = scale.ToModel();

            var reloaded = (ScaleModifier)factory.Create(model);

            Assert.Equal(randomModulator.ID, reloaded.X.ModulatorID);
            Assert.NotNull(reloaded.X.BoundModulator);
            Assert.Equal(randomModulator.ID, reloaded.X.BoundModulator.ID);
        }

        [Fact]
        public void Undo_AfterAssigningAModulator_RevertsIdNameAndBoundModulatorInOneStep()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var undoManager = provider.GetRequiredService<UndoManager>();

            var scale = (ScaleModifier)factory.Create(typeof(ScaleModifier));
            scale.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)scale.ModulatorManager.ManagerData.Items[0];

            scale.X.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            Assert.Equal(randomModulator.ID, scale.X.ModulatorID);
            Assert.Equal("Value", scale.X.BoundOutputName);
            Assert.NotNull(scale.X.BoundModulator);

            undoManager.Undo();

            Assert.Null(scale.X.ModulatorID);
            Assert.Null(scale.X.BoundOutputName);
            Assert.Null(scale.X.BoundModulator);
        }
    }
}
