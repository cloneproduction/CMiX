using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class DitherTests
    {
        [Fact]
        public void ModulatorManager_CanAddRandomModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var dither = provider.GetRequiredService<Dither>();

            dither.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(dither.ModulatorManager.ManagerData.Items);
            Assert.IsType<RandomModulator>(dither.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void ReorderingModulators_DoesNotUnassignTheMovedOnesBindings()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var dither = (Dither)factory.Create(typeof(Dither));

            dither.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod1 = (BeatRandomModulator)dither.ModulatorManager.ManagerData.Items[0];
            dither.ModulatorManager.AddItem(typeof(BeatRandomModulator));

            dither.Threshold.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod1, mod1.Outputs[0]));

            dither.ModulatorManager.ManagerData.Items.Move(0, 1);

            Assert.Equal(mod1.ID, dither.Threshold.ModulatorID);
            Assert.Same(mod1, dither.Threshold.BoundModulator);
        }

        [Fact]
        public void DeletingAssignedModulator_UnassignsItFromEveryBindableUsingIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var dither = provider.GetRequiredService<Dither>();
            dither.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)dither.ModulatorManager.ManagerData.Items[0];

            dither.Threshold.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            dither.ModulatorManager.DeleteItem(randomModulator);

            Assert.Null(dither.Threshold.ModulatorID);
            Assert.Null(dither.Threshold.BoundModulator);
        }

        [Fact]
        public void Dither_ToModel_FromModel_RoundTripsBindableValuesAndLiveBoundModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var dither = provider.GetRequiredService<Dither>();

            dither.Threshold.Value = 0.4f;
            dither.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)dither.ModulatorManager.ManagerData.Items[0];
            dither.Threshold.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            var model = dither.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<Dither>();
            reloaded.FromModel(model);

            Assert.Equal(0.4f, reloaded.Threshold.Value);
            var reloadedRandomModulator = Assert.IsType<RandomModulator>(reloaded.ModulatorManager.ManagerData.Items[0]);
            Assert.Same(reloadedRandomModulator, reloaded.Threshold.BoundModulator);
        }
    }
}
