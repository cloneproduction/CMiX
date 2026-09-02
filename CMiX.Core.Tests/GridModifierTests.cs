using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Lights;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class GridModifierTests
    {
        [Fact]
        public void Grid_HasSixChannelsAcrossTwoGroups()
        {
            var provider = TestServiceProviderFactory.Create();
            var grid = provider.GetRequiredService<GridModifier>();

            Assert.Equal(6, grid.Channels.Count);
            Assert.Same(grid.Channels[0], grid.Width.X);
            Assert.Same(grid.Channels[1], grid.Width.Y);
            Assert.Same(grid.Channels[2], grid.Width.Z);
            Assert.Same(grid.Channels[3], grid.Phase.X);
            Assert.Same(grid.Channels[4], grid.Phase.Y);
            Assert.Same(grid.Channels[5], grid.Phase.Z);
        }

        [Fact]
        public void Grid_IsDiscoverableOnBothEntityAndLightEntity()
        {
            var attributes = typeof(GridModifier).GetCustomAttributes(typeof(ModifierPanelAttribute), false);
            var owners = System.Array.ConvertAll(attributes, a => ((ModifierPanelAttribute)a).PanelOwner);

            Assert.Contains(typeof(Entity), owners);
            Assert.Contains(typeof(LightEntity), owners);
        }

        [Fact]
        public void Grid_IsAlsoAnIModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var grid = provider.GetRequiredService<GridModifier>();

            Assert.IsAssignableFrom<IModifier>(grid);
        }

        [Fact]
        public void WidthAndPhaseGroups_ShareOneModulatorManagerIndependently()
        {
            var provider = TestServiceProviderFactory.Create();
            var grid = provider.GetRequiredService<GridModifier>();
            grid.ModulatorManager.AddItem(typeof(BeatModifier));
            var beatModifier = (BeatModifier)grid.ModulatorManager.ManagerData.Items[0];

            grid.Width.X.SetModulatorCommand.Execute(new ModulatorOutputSelection(beatModifier, "Value"));
            grid.Phase.Z.SetModulatorCommand.Execute(new ModulatorOutputSelection(beatModifier, "Value"));

            Assert.Equal(beatModifier.ID, grid.Width.X.ModulatorID.Value);
            Assert.Null(grid.Width.Y.ModulatorID.Value);
            Assert.Null(grid.Phase.X.ModulatorID.Value);
            Assert.Equal(beatModifier.ID, grid.Phase.Z.ModulatorID.Value);
        }

        [Fact]
        public void Grid_ToModel_FromModel_RoundTripsChannelValuesAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var grid = provider.GetRequiredService<GridModifier>();

            grid.Width.X.Value.Value = 5f;
            var modulatorId = Guid.NewGuid();
            grid.Phase.Y.ModulatorID.Value = modulatorId;

            var model = grid.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<GridModifier>();
            reloaded.FromModel(model);

            Assert.Equal(5f, reloaded.Width.X.Value.Value);
            Assert.Equal(modulatorId, reloaded.Phase.Y.ModulatorID.Value);
        }

        [Fact]
        public void Grid_ToModel_FromModel_RoundTripsModifierModeSelectorAndCount()
        {
            var provider = TestServiceProviderFactory.Create();
            var grid = provider.GetRequiredService<GridModifier>();

            grid.ModifierModeSelector.Mode.Value = ModifierMode.ToSpread;
            grid.ModifierModeSelector.Count.X.Value = 3;
            grid.ModifierModeSelector.Count.Y.Value = 4;
            grid.ModifierModeSelector.Count.Z.Value = 5;

            var model = grid.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<GridModifier>();
            reloaded.FromModel(model);

            Assert.Equal(ModifierMode.ToSpread, reloaded.ModifierModeSelector.Mode.Value);
            Assert.Equal(3, reloaded.ModifierModeSelector.Count.X.Value);
            Assert.Equal(4, reloaded.ModifierModeSelector.Count.Y.Value);
            Assert.Equal(5, reloaded.ModifierModeSelector.Count.Z.Value);
        }
    }
}
