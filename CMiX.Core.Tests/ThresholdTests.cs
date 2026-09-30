using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ThresholdTests
    {
        [Fact]
        public void ModulatorManager_CanAddRandomModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var threshold = provider.GetRequiredService<Threshold>();

            threshold.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(threshold.ModulatorManager.ManagerData.Items);
            Assert.IsType<RandomModulator>(threshold.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void ReorderingModulators_DoesNotUnassignTheMovedOnesBindings()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var threshold = (Threshold)factory.Create(typeof(Threshold));

            threshold.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod1 = (BeatRandomModulator)threshold.ModulatorManager.ManagerData.Items[0];
            threshold.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod2 = (BeatRandomModulator)threshold.ModulatorManager.ManagerData.Items[1];

            threshold.Smooth.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod1, mod1.Outputs[0]));
            threshold.ThresholdValue.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod2, mod2.Outputs[0]));

            threshold.ModulatorManager.ManagerData.Items.Move(0, 1);

            Assert.Equal(mod1.ID, threshold.Smooth.ModulatorID);
            Assert.Same(mod1, threshold.Smooth.BoundModulator);
            Assert.Equal(mod2.ID, threshold.ThresholdValue.ModulatorID);
            Assert.Same(mod2, threshold.ThresholdValue.BoundModulator);
        }

        [Fact]
        public void DeletingAssignedModulator_UnassignsItFromEveryBindableUsingIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var threshold = provider.GetRequiredService<Threshold>();
            threshold.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)threshold.ModulatorManager.ManagerData.Items[0];

            threshold.Smooth.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            threshold.ThresholdValue.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            threshold.ModulatorManager.DeleteItem(randomModulator);

            Assert.Null(threshold.Smooth.ModulatorID);
            Assert.Null(threshold.Smooth.BoundModulator);
            Assert.Null(threshold.ThresholdValue.ModulatorID);
            Assert.Null(threshold.ThresholdValue.BoundModulator);
        }

        [Fact]
        public void Threshold_ToModel_FromModel_RoundTripsBindableValuesAndLiveBoundModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var threshold = provider.GetRequiredService<Threshold>();

            threshold.Smooth.Value = 0.3f;
            threshold.ThresholdValue.Value = 0.7f;
            threshold.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)threshold.ModulatorManager.ManagerData.Items[0];
            threshold.ThresholdValue.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            var model = threshold.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<Threshold>();
            reloaded.FromModel(model);

            Assert.Equal(0.3f, reloaded.Smooth.Value);
            Assert.Equal(0.7f, reloaded.ThresholdValue.Value);
            var reloadedRandomModulator = Assert.IsType<RandomModulator>(reloaded.ModulatorManager.ManagerData.Items[0]);
            Assert.Same(reloadedRandomModulator, reloaded.ThresholdValue.BoundModulator);
        }
    }
}
