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
        public void Grid_HasSixBindablesAcrossTwoGroups()
        {
            var provider = TestServiceProviderFactory.Create();
            var grid = provider.GetRequiredService<GridModifier>();

            Assert.Equal(6, grid.Bindables.Count);
            Assert.Same(grid.Bindables[0], grid.Width.X);
            Assert.Same(grid.Bindables[1], grid.Width.Y);
            Assert.Same(grid.Bindables[2], grid.Width.Z);
            Assert.Same(grid.Bindables[3], grid.Phase.X);
            Assert.Same(grid.Bindables[4], grid.Phase.Y);
            Assert.Same(grid.Bindables[5], grid.Phase.Z);
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
            grid.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)grid.ModulatorManager.ManagerData.Items[0];

            grid.Width.X.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            grid.Phase.Z.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            Assert.Equal(randomModulator.ID, grid.Width.X.ModulatorID);
            Assert.Null(grid.Width.Y.ModulatorID);
            Assert.Null(grid.Phase.X.ModulatorID);
            Assert.Equal(randomModulator.ID, grid.Phase.Z.ModulatorID);
        }

        [Fact]
        public void Grid_ToModel_FromModel_RoundTripsBindableValuesAndBinding()
        {
            var provider = TestServiceProviderFactory.Create();
            var grid = provider.GetRequiredService<GridModifier>();

            grid.Width.X.Value = 5f;
            var modulatorId = Guid.NewGuid();
            grid.Phase.Y.ModulatorID = modulatorId;

            var model = grid.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<GridModifier>();
            reloaded.FromModel(model);

            Assert.Equal(5f, reloaded.Width.X.Value);
            Assert.Equal(modulatorId, reloaded.Phase.Y.ModulatorID);
        }

        [Fact]
        public void Grid_ToModel_FromModel_RoundTripsCount()
        {
            var provider = TestServiceProviderFactory.Create();
            var grid = provider.GetRequiredService<GridModifier>();

            grid.Count.X.Value = 3;
            grid.Count.Y.Value = 4;
            grid.Count.Z.Value = 5;

            var model = grid.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<GridModifier>();
            reloaded.FromModel(model);

            Assert.Equal(3, reloaded.Count.X.Value);
            Assert.Equal(4, reloaded.Count.Y.Value);
            Assert.Equal(5, reloaded.Count.Z.Value);
        }

        [Fact]
        public void Grid_ToModel_FromModel_ResolvesEachCountAxisLiveBoundModulatorIndependently()
        {
            var provider = TestServiceProviderFactory.Create();
            var grid = provider.GetRequiredService<GridModifier>();
            grid.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)grid.ModulatorManager.ManagerData.Items[0];

            grid.Count.X.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            var model = grid.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<GridModifier>();
            reloaded.FromModel(model);

            var reloadedRandomModulator = Assert.IsType<RandomModulator>(reloaded.ModulatorManager.ManagerData.Items[0]);
            Assert.Same(reloadedRandomModulator, reloaded.Count.X.BoundModulator);
            Assert.Null(reloaded.Count.Y.BoundModulator);
            Assert.Null(reloaded.Count.Z.BoundModulator);
        }

        [Fact]
        public void Grid_ToModel_FromModel_ResolvesACountAxisAfterAnUnboundOne()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var grid = (GridModifier)factory.Create(typeof(GridModifier));
            grid.ModulatorManager.AddItem(typeof(LFOModulator));
            var lfo = (LFOModulator)grid.ModulatorManager.ManagerData.Items[0];
            var cycles = lfo.Outputs.First(o => o.Name == "Cycles");

            grid.Count.Z.SetModulatorCommand.Execute(new ModulatorOutputSelection(lfo, cycles));

            var model = grid.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var factory2 = provider2.GetRequiredService<ControlFactory>();
            var reloaded = (GridModifier)factory2.Create(typeof(GridModifier));
            reloaded.FromModel(model);

            Assert.Null(reloaded.Count.X.BoundModulator);
            Assert.NotNull(reloaded.Count.Z.BoundModulator);
            Assert.Equal(lfo.ID, reloaded.Count.Z.ModulatorID);
            Assert.Equal("Cycles", reloaded.Count.Z.BoundOutputName);
        }

        [Fact]
        public void DeletingAssignedModulator_UnassignsItFromEveryCountAxisUsingIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var grid = provider.GetRequiredService<GridModifier>();
            grid.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)grid.ModulatorManager.ManagerData.Items[0];

            grid.Count.X.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            grid.Count.Z.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            grid.ModulatorManager.DeleteItem(randomModulator);

            Assert.Null(grid.Count.X.ModulatorID);
            Assert.Null(grid.Count.X.BoundModulator);
            Assert.Null(grid.Count.Z.ModulatorID);
            Assert.Null(grid.Count.Z.BoundModulator);
            Assert.Null(grid.Count.Y.ModulatorID);
        }

        [Fact]
        public void CountXYZ_ConstructAtDefaultOfOne_AndResetReturnsToItAfterAChange()
        {
            var provider = TestServiceProviderFactory.Create();
            var grid = (GridModifier)provider.GetRequiredService<ControlFactory>().Create(typeof(GridModifier));

            Assert.Equal(1, grid.Count.X.Value);
            Assert.Equal(1, grid.Count.Y.Value);
            Assert.Equal(1, grid.Count.Z.Value);

            grid.Count.X.Value = 7;
            grid.Count.X.ResetCommand.Execute(null);

            Assert.Equal(1, grid.Count.X.Value);
        }
    }
}
