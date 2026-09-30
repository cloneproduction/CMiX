using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class EchoTests
    {
        [Fact]
        public void ModulatorManager_CanAddRandomModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var echo = provider.GetRequiredService<Echo>();

            echo.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(echo.ModulatorManager.ManagerData.Items);
            Assert.IsType<RandomModulator>(echo.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void ReorderingModulators_DoesNotUnassignTheMovedOnesBindings()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var echo = (Echo)factory.Create(typeof(Echo));

            echo.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod1 = (BeatRandomModulator)echo.ModulatorManager.ManagerData.Items[0];
            echo.ModulatorManager.AddItem(typeof(BeatRandomModulator));

            echo.Factor.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod1, mod1.Outputs[0]));

            echo.ModulatorManager.ManagerData.Items.Move(0, 1);

            Assert.Equal(mod1.ID, echo.Factor.ModulatorID);
            Assert.Same(mod1, echo.Factor.BoundModulator);
        }

        [Fact]
        public void DeletingAssignedModulator_UnassignsItFromEveryBindableUsingIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var echo = provider.GetRequiredService<Echo>();
            echo.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)echo.ModulatorManager.ManagerData.Items[0];

            echo.Factor.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            echo.ModulatorManager.DeleteItem(randomModulator);

            Assert.Null(echo.Factor.ModulatorID);
            Assert.Null(echo.Factor.BoundModulator);
        }

        [Fact]
        public void Echo_ToModel_FromModel_RoundTripsBindableValuesAndLiveBoundModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var echo = provider.GetRequiredService<Echo>();

            echo.Factor.Value = 0.6f;
            echo.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)echo.ModulatorManager.ManagerData.Items[0];
            echo.Factor.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            var model = echo.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<Echo>();
            reloaded.FromModel(model);

            Assert.Equal(0.6f, reloaded.Factor.Value);
            var reloadedRandomModulator = Assert.IsType<RandomModulator>(reloaded.ModulatorManager.ManagerData.Items[0]);
            Assert.Same(reloadedRandomModulator, reloaded.Factor.BoundModulator);
        }
    }
}
