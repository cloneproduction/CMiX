using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Lights;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class GridTests
    {
        [Fact]
        public void Grid_HasSixChannelsAcrossTwoGroups()
        {
            var provider = TestServiceProviderFactory.Create();
            var grid = provider.GetRequiredService<Grid>();

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
            var attributes = typeof(Grid).GetCustomAttributes(typeof(ModifierPanelAttribute), false);
            var owners = System.Array.ConvertAll(attributes, a => ((ModifierPanelAttribute)a).PanelOwner);

            Assert.Contains(typeof(Entity), owners);
            Assert.Contains(typeof(LightEntity), owners);
        }

        [Fact]
        public void Grid_IsAlsoAnIModifier()
        {
            var provider = TestServiceProviderFactory.Create();
            var grid = provider.GetRequiredService<Grid>();

            Assert.IsAssignableFrom<IModifier>(grid);
        }

        [Fact]
        public void WidthAndPhaseGroups_ShareOneModulatorManagerIndependently()
        {
            var provider = TestServiceProviderFactory.Create();
            var grid = provider.GetRequiredService<Grid>();
            grid.ModulatorManager.AddItem(typeof(BeatModifier));
            var beatModifier = (BeatModifier)grid.ModulatorManager.ManagerData.Items[0];

            grid.Width.X.Binding.SetModulatorCommand.Execute(beatModifier);
            grid.Phase.Z.Binding.SetModulatorCommand.Execute(beatModifier);

            Assert.Equal(beatModifier.ID, grid.Width.X.Binding.ModulatorID);
            Assert.Null(grid.Width.Y.Binding.ModulatorID);
            Assert.Null(grid.Phase.X.Binding.ModulatorID);
            Assert.Equal(beatModifier.ID, grid.Phase.Z.Binding.ModulatorID);
        }

        [Fact]
        public void Grid_ToModel_FromModel_RoundTripsChannelValuesAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var grid = provider.GetRequiredService<Grid>();

            grid.Width.X.Value.Value = 5f;
            var modulatorId = Guid.NewGuid();
            grid.Phase.Y.Binding.ModulatorID = modulatorId;

            var model = grid.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<Grid>();
            reloaded.FromModel(model);

            Assert.Equal(5f, reloaded.Width.X.Value.Value);
            Assert.Equal(modulatorId, reloaded.Phase.Y.Binding.ModulatorID);
        }

        [Fact]
        public void Grid_ToModel_FromModel_RoundTripsModifierModeSelectorAndCount()
        {
            var provider = TestServiceProviderFactory.Create();
            var grid = provider.GetRequiredService<Grid>();

            grid.ModifierModeSelector.Mode.Value = ModifierMode.ToSpread;
            grid.ModifierModeSelector.Count.Value = 5;
            grid.Count.X.Value = 3;
            grid.Count.Y.Value = 4;
            grid.Count.Z.Value = 5;

            var model = grid.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<Grid>();
            reloaded.FromModel(model);

            Assert.Equal(ModifierMode.ToSpread, reloaded.ModifierModeSelector.Mode.Value);
            Assert.Equal(5, reloaded.ModifierModeSelector.Count.Value);
            Assert.Equal(3, reloaded.Count.X.Value);
            Assert.Equal(4, reloaded.Count.Y.Value);
            Assert.Equal(5, reloaded.Count.Z.Value);
        }
    }
}
