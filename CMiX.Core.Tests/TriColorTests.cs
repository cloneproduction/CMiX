using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class TriColorTests
    {
        [Fact]
        public void ModulatorManager_CanAddRandomModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var triColor = provider.GetRequiredService<TriColor>();

            triColor.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(triColor.ModulatorManager.ManagerData.Items);
            Assert.IsType<RandomModulator>(triColor.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void ReorderingModulators_DoesNotUnassignTheMovedOnesBindings()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var triColor = (TriColor)factory.Create(typeof(TriColor));

            triColor.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod1 = (BeatRandomModulator)triColor.ModulatorManager.ManagerData.Items[0];
            triColor.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod2 = (BeatRandomModulator)triColor.ModulatorManager.ManagerData.Items[1];

            triColor.Smooth.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod1, mod1.Outputs[0]));
            triColor.Center.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod2, mod2.Outputs[0]));

            triColor.ModulatorManager.ManagerData.Items.Move(0, 1);

            Assert.Equal(mod1.ID, triColor.Smooth.ModulatorID);
            Assert.Same(mod1, triColor.Smooth.BoundModulator);
            Assert.Equal(mod2.ID, triColor.Center.ModulatorID);
            Assert.Same(mod2, triColor.Center.BoundModulator);
        }

        [Fact]
        public void DeletingAssignedModulator_UnassignsItFromEveryBindableUsingIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var triColor = provider.GetRequiredService<TriColor>();
            triColor.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)triColor.ModulatorManager.ManagerData.Items[0];

            triColor.Smooth.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            triColor.Center.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            triColor.ModulatorManager.DeleteItem(randomModulator);

            Assert.Null(triColor.Smooth.ModulatorID);
            Assert.Null(triColor.Smooth.BoundModulator);
            Assert.Null(triColor.Center.ModulatorID);
            Assert.Null(triColor.Center.BoundModulator);
        }

        [Fact]
        public void TriColor_ToModel_FromModel_RoundTripsBindableValuesAndLiveBoundModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var triColor = provider.GetRequiredService<TriColor>();

            triColor.Smooth.Value = 0.4f;
            triColor.Center.Value = 0.3f;
            triColor.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)triColor.ModulatorManager.ManagerData.Items[0];
            triColor.Center.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            var model = triColor.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<TriColor>();
            reloaded.FromModel(model);

            Assert.Equal(0.4f, reloaded.Smooth.Value);
            Assert.Equal(0.3f, reloaded.Center.Value);
            var reloadedRandomModulator = Assert.IsType<RandomModulator>(reloaded.ModulatorManager.ManagerData.Items[0]);
            Assert.Same(reloadedRandomModulator, reloaded.Center.BoundModulator);
        }
    }
}
