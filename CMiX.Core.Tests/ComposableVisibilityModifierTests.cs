using System.Linq;
using System.Reflection;
using CMiX.Core.Compositing;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ComposableVisibilityModifierTests
    {
        private static ComposableVisibilityModifier Create() =>
            (ComposableVisibilityModifier)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(ComposableVisibilityModifier));

        [Fact]
        public void Fresh_IsVisibleByDefault()
        {
            Assert.True(Create().Visible.Value);
        }

        [Theory]
        [InlineData(typeof(Layer))]
        [InlineData(typeof(Composition))]
        public void IsOfferedForOwner(Type owner)
        {
            Assert.Contains(typeof(ComposableVisibilityModifier).GetCustomAttributes<ModifierPanelAttribute>(), a => a.PanelOwner == owner);
        }

        [Fact]
        public void KeepsItsValueAndBindingThroughTheModel()
        {
            var modifier = Create();
            modifier.ModulatorManager.AddItem(typeof(LFOModulator));
            var lfo = (LFOModulator)modifier.ModulatorManager.ManagerData.Items[0];
            var gate = lfo.Outputs.Single(o => o is ModulatorOutput<bool>);
            modifier.Visible.Value = false;
            modifier.Visible.SetModulatorCommand.Execute(new ModulatorOutputSelection(lfo, gate));

            var copy = Create();
            copy.FromModel(modifier.ToModel());

            Assert.False(copy.Visible.Value);
            Assert.Equal(lfo.ID, copy.Visible.ModulatorID);
            Assert.Equal("Gate", copy.Visible.BoundOutputName);
            Assert.NotNull(copy.Visible.BoundModulator);
        }

        [Fact]
        public void RemovingTheModulator_UnbindsTheValue()
        {
            var modifier = Create();
            modifier.ModulatorManager.AddItem(typeof(LFOModulator));
            var lfo = (LFOModulator)modifier.ModulatorManager.ManagerData.Items[0];
            var gate = lfo.Outputs.Single(o => o is ModulatorOutput<bool>);
            modifier.Visible.SetModulatorCommand.Execute(new ModulatorOutputSelection(lfo, gate));

            modifier.ModulatorManager.DeleteItem(lfo);

            Assert.Null(modifier.Visible.ModulatorID);
        }

        [Fact]
        public void IsNamedVisibilityInTheListsAndOnTheNewItem()
        {
            Assert.Equal("Visibility", ControlFactory.StringHelper.LabelOf(typeof(ComposableVisibilityModifier)));
            Assert.Equal("Visibility Modifier", Create().PrefabService.Name.Value);
        }

        [Fact]
        public void CompositionAndLayer_AcceptTheModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var compositionManager = provider.GetRequiredService<PrefabManager>();
            compositionManager.AddItem(typeof(Composition));
            var composition = (Composition)compositionManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;

            composition.ModifierManager.AddItem(typeof(ComposableVisibilityModifier));
            layer.ModifierManager.AddItem(typeof(ComposableVisibilityModifier));

            Assert.Single(composition.ModifierManager.ManagerData.Items.OfType<ComposableVisibilityModifier>());
            Assert.Single(layer.ModifierManager.ManagerData.Items.OfType<ComposableVisibilityModifier>());
        }
    }
}
