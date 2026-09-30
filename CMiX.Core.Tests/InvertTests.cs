using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class InvertTests
    {
        [Fact]
        public void ModulatorManager_CanAddRandomModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var invert = provider.GetRequiredService<Invert>();

            invert.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(invert.ModulatorManager.ManagerData.Items);
            Assert.IsType<RandomModulator>(invert.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void ReorderingModulators_DoesNotUnassignTheMovedOnesBindings()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var invert = (Invert)factory.Create(typeof(Invert));

            invert.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod1 = (BeatRandomModulator)invert.ModulatorManager.ManagerData.Items[0];
            invert.ModulatorManager.AddItem(typeof(BeatRandomModulator));

            invert.Factor.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod1, mod1.Outputs[0]));

            invert.ModulatorManager.ManagerData.Items.Move(0, 1);

            Assert.Equal(mod1.ID, invert.Factor.ModulatorID);
            Assert.Same(mod1, invert.Factor.BoundModulator);
        }

        [Fact]
        public void DeletingAssignedModulator_UnassignsItFromEveryBindableUsingIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var invert = provider.GetRequiredService<Invert>();
            invert.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)invert.ModulatorManager.ManagerData.Items[0];

            invert.Factor.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            invert.ModulatorManager.DeleteItem(randomModulator);

            Assert.Null(invert.Factor.ModulatorID);
            Assert.Null(invert.Factor.BoundModulator);
        }

        [Fact]
        public void Invert_ToModel_FromModel_RoundTripsBindableValuesAndLiveBoundModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var invert = provider.GetRequiredService<Invert>();

            invert.Factor.Value = 0.6f;
            invert.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)invert.ModulatorManager.ManagerData.Items[0];
            invert.Factor.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            var model = invert.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<Invert>();
            reloaded.FromModel(model);

            Assert.Equal(0.6f, reloaded.Factor.Value);
            var reloadedRandomModulator = Assert.IsType<RandomModulator>(reloaded.ModulatorManager.ManagerData.Items[0]);
            Assert.Same(reloadedRandomModulator, reloaded.Factor.BoundModulator);
        }
    }
}
