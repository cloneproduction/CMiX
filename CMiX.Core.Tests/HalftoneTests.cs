using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class HalftoneTests
    {
        [Fact]
        public void ModulatorManager_CanAddRandomModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var halftone = provider.GetRequiredService<Halftone>();

            halftone.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(halftone.ModulatorManager.ManagerData.Items);
            Assert.IsType<RandomModulator>(halftone.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void ReorderingModulators_DoesNotUnassignTheMovedOnesBindings()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var halftone = (Halftone)factory.Create(typeof(Halftone));

            halftone.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod1 = (BeatRandomModulator)halftone.ModulatorManager.ManagerData.Items[0];
            halftone.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod2 = (BeatRandomModulator)halftone.ModulatorManager.ManagerData.Items[1];

            halftone.NumberOfTiles.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod1, mod1.Outputs[0]));
            halftone.DotSize.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod2, mod2.Outputs[0]));

            halftone.ModulatorManager.ManagerData.Items.Move(0, 1);

            Assert.Equal(mod1.ID, halftone.NumberOfTiles.ModulatorID);
            Assert.Same(mod1, halftone.NumberOfTiles.BoundModulator);
            Assert.Equal(mod2.ID, halftone.DotSize.ModulatorID);
            Assert.Same(mod2, halftone.DotSize.BoundModulator);
        }

        [Fact]
        public void DeletingAssignedModulator_UnassignsItFromEveryBindableUsingIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var halftone = provider.GetRequiredService<Halftone>();
            halftone.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)halftone.ModulatorManager.ManagerData.Items[0];

            halftone.NumberOfTiles.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            halftone.DotSize.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            halftone.Softness.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            halftone.Brightness.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            halftone.ModulatorManager.DeleteItem(randomModulator);

            Assert.Null(halftone.NumberOfTiles.ModulatorID);
            Assert.Null(halftone.NumberOfTiles.BoundModulator);
            Assert.Null(halftone.DotSize.ModulatorID);
            Assert.Null(halftone.DotSize.BoundModulator);
            Assert.Null(halftone.Softness.ModulatorID);
            Assert.Null(halftone.Softness.BoundModulator);
            Assert.Null(halftone.Brightness.ModulatorID);
            Assert.Null(halftone.Brightness.BoundModulator);
        }

        [Fact]
        public void Halftone_ToModel_FromModel_RoundTripsBindableValuesAndLiveBoundModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var halftone = provider.GetRequiredService<Halftone>();

            halftone.NumberOfTiles.Value = 32f;
            halftone.DotSize.Value = 0.2f;
            halftone.Softness.Value = 2f;
            halftone.Brightness.Value = 0.7f;
            halftone.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)halftone.ModulatorManager.ManagerData.Items[0];
            halftone.Brightness.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            var model = halftone.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<Halftone>();
            reloaded.FromModel(model);

            Assert.Equal(32f, reloaded.NumberOfTiles.Value);
            Assert.Equal(0.2f, reloaded.DotSize.Value);
            Assert.Equal(2f, reloaded.Softness.Value);
            Assert.Equal(0.7f, reloaded.Brightness.Value);
            var reloadedRandomModulator = Assert.IsType<RandomModulator>(reloaded.ModulatorManager.ManagerData.Items[0]);
            Assert.Same(reloadedRandomModulator, reloaded.Brightness.BoundModulator);
        }
    }
}
