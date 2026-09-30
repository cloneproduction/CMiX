using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class EdgeTests
    {
        [Fact]
        public void ModulatorManager_CanAddRandomModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var edge = provider.GetRequiredService<Edge>();

            edge.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(edge.ModulatorManager.ManagerData.Items);
            Assert.IsType<RandomModulator>(edge.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void ReorderingModulators_DoesNotUnassignTheMovedOnesBindings()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var edge = (Edge)factory.Create(typeof(Edge));

            edge.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod1 = (BeatRandomModulator)edge.ModulatorManager.ManagerData.Items[0];
            edge.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod2 = (BeatRandomModulator)edge.ModulatorManager.ManagerData.Items[1];

            edge.Radius.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod1, mod1.Outputs[0]));
            edge.Brightness.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod2, mod2.Outputs[0]));

            edge.ModulatorManager.ManagerData.Items.Move(0, 1);

            Assert.Equal(mod1.ID, edge.Radius.ModulatorID);
            Assert.Same(mod1, edge.Radius.BoundModulator);
            Assert.Equal(mod2.ID, edge.Brightness.ModulatorID);
            Assert.Same(mod2, edge.Brightness.BoundModulator);
        }

        [Fact]
        public void DeletingAssignedModulator_UnassignsItFromEveryBindableUsingIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var edge = provider.GetRequiredService<Edge>();
            edge.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)edge.ModulatorManager.ManagerData.Items[0];

            edge.Radius.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            edge.Brightness.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            edge.ModulatorManager.DeleteItem(randomModulator);

            Assert.Null(edge.Radius.ModulatorID);
            Assert.Null(edge.Radius.BoundModulator);
            Assert.Null(edge.Brightness.ModulatorID);
            Assert.Null(edge.Brightness.BoundModulator);
        }

        [Fact]
        public void Edge_ToModel_FromModel_RoundTripsBindableValuesAndLiveBoundModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var edge = provider.GetRequiredService<Edge>();

            edge.Radius.Value = 4f;
            edge.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)edge.ModulatorManager.ManagerData.Items[0];
            edge.Radius.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            var model = edge.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<Edge>();
            reloaded.FromModel(model);

            Assert.Equal(4f, reloaded.Radius.Value);
            var reloadedRandomModulator = Assert.IsType<RandomModulator>(reloaded.ModulatorManager.ManagerData.Items[0]);
            Assert.Same(reloadedRandomModulator, reloaded.Radius.BoundModulator);
        }
    }
}
