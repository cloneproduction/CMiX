using System.Text.Json;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Lights;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class VisibilityModifierTests
    {
        private static VisibilityModifier Create() =>
            (VisibilityModifier)TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>().Create(typeof(VisibilityModifier));

        [Fact]
        public void Fresh_IsVisibleByDefault()
        {
            Assert.True(Create().Visible.Value);
        }

        [Fact]
        public void VisibilityModifier_IsDiscoverableOnBothEntityAndLightEntity()
        {
            var attributes = typeof(VisibilityModifier).GetCustomAttributes(typeof(ModifierPanelAttribute), false);
            var owners = System.Array.ConvertAll(attributes, a => ((ModifierPanelAttribute)a).PanelOwner);

            Assert.Contains(typeof(Entity), owners);
            Assert.Contains(typeof(LightEntity), owners);
        }

        [Fact]
        public void VisibilityModifier_IsAlsoAnIModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var visibility = provider.GetRequiredService<VisibilityModifier>();

            Assert.IsAssignableFrom<IModifier>(visibility);
        }

        [Fact]
        public void ModulatorManager_CanAddRandomModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var visibility = provider.GetRequiredService<VisibilityModifier>();

            visibility.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(visibility.ModulatorManager.ManagerData.Items);
            Assert.IsType<RandomModulator>(visibility.ModulatorManager.ManagerData.Items[0]);
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

        // Before the toggle, the modifier saved a float named Value. A project file with that key must open.
        [Fact]
        public void AFileSavedWithTheOldFloatValue_Opens_AndTheModifierIsVisible()
        {
            const string json = "{\"$type\":\"CMiX.Core.Modulation.Modifiers.VisibilityModifierModel\",\"Value\":{\"Value\":{\"Value\":0.25}}}";
            var model = JsonSerializer.Deserialize<IControlModel>(json, ProjectSerializer.Options)!;

            var modifier = Create();
            modifier.FromModel(model);

            Assert.True(modifier.Visible.Value);
            Assert.Null(modifier.Visible.ModulatorID);
        }
    }
}
