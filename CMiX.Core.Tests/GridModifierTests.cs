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
            grid.ModulatorManager.AddItem(typeof(BeatModulator));
            var beatModulator = (BeatModulator)grid.ModulatorManager.ManagerData.Items[0];

            grid.Width.X.SetModulatorCommand.Execute(new ModulatorOutputSelection(beatModulator, beatModulator.Outputs[0]));
            grid.Phase.Z.SetModulatorCommand.Execute(new ModulatorOutputSelection(beatModulator, beatModulator.Outputs[0]));

            Assert.Equal(beatModulator.ID, grid.Width.X.ModulatorID.Value);
            Assert.Null(grid.Width.Y.ModulatorID.Value);
            Assert.Null(grid.Phase.X.ModulatorID.Value);
            Assert.Equal(beatModulator.ID, grid.Phase.Z.ModulatorID.Value);
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
            grid.ModifierModeSelector.CountX.Value.Value = 3;
            grid.ModifierModeSelector.CountY.Value.Value = 4;
            grid.ModifierModeSelector.CountZ.Value.Value = 5;

            var model = grid.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<GridModifier>();
            reloaded.FromModel(model);

            Assert.Equal(ModifierMode.ToSpread, reloaded.ModifierModeSelector.Mode.Value);
            Assert.Equal(3, reloaded.ModifierModeSelector.CountX.Value.Value);
            Assert.Equal(4, reloaded.ModifierModeSelector.CountY.Value.Value);
            Assert.Equal(5, reloaded.ModifierModeSelector.CountZ.Value.Value);
        }

        [Fact]
        public void Grid_ToModel_FromModel_ResolvesEachCountAxisLiveBoundModulatorIndependently()
        {
            var provider = TestServiceProviderFactory.Create();
            var grid = provider.GetRequiredService<GridModifier>();
            grid.ModulatorManager.AddItem(typeof(BeatModulator));
            var beatModulator = (BeatModulator)grid.ModulatorManager.ManagerData.Items[0];

            // Only CountX is bound - CountY/CountZ must stay unbound through the round trip,
            // proving each axis resolves independently rather than all three collapsing together.
            grid.ModifierModeSelector.CountX.SetModulatorCommand.Execute(new ModulatorOutputSelection(beatModulator, beatModulator.Outputs[0]));

            var model = grid.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<GridModifier>();
            reloaded.FromModel(model);

            var reloadedBeatModulator = Assert.IsType<BeatModulator>(reloaded.ModulatorManager.ManagerData.Items[0]);
            Assert.Same(reloadedBeatModulator, reloaded.ModifierModeSelector.CountX.BoundModulator);
            Assert.Null(reloaded.ModifierModeSelector.CountY.BoundModulator);
            Assert.Null(reloaded.ModifierModeSelector.CountZ.BoundModulator);
        }

        [Fact]
        public void DeletingAssignedModulator_UnassignsItFromEveryCountAxisUsingIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var grid = provider.GetRequiredService<GridModifier>();
            grid.ModulatorManager.AddItem(typeof(BeatModulator));
            var beatModulator = (BeatModulator)grid.ModulatorManager.ManagerData.Items[0];

            grid.ModifierModeSelector.CountX.SetModulatorCommand.Execute(new ModulatorOutputSelection(beatModulator, beatModulator.Outputs[0]));
            grid.ModifierModeSelector.CountZ.SetModulatorCommand.Execute(new ModulatorOutputSelection(beatModulator, beatModulator.Outputs[0]));

            grid.ModulatorManager.DeleteItem(beatModulator);

            Assert.Null(grid.ModifierModeSelector.CountX.ModulatorID.Value);
            Assert.Null(grid.ModifierModeSelector.CountX.BoundModulator);
            Assert.Null(grid.ModifierModeSelector.CountZ.ModulatorID.Value);
            Assert.Null(grid.ModifierModeSelector.CountZ.BoundModulator);
            // CountY was never assigned - confirms the cleanup only touches axes that referenced
            // the deleted modulator.
            Assert.Null(grid.ModifierModeSelector.CountY.ModulatorID.Value);
        }
    }
}
