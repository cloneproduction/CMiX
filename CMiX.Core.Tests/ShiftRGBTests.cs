using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ShiftRGBTests
    {
        [Fact]
        public void ModulatorManager_CanAddRandomModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var shiftRGB = provider.GetRequiredService<ShiftRGB>();

            shiftRGB.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(shiftRGB.ModulatorManager.ManagerData.Items);
            Assert.IsType<RandomModulator>(shiftRGB.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void ReorderingModulators_DoesNotUnassignTheMovedOnesBindings()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var shiftRGB = (ShiftRGB)factory.Create(typeof(ShiftRGB));

            shiftRGB.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod1 = (BeatRandomModulator)shiftRGB.ModulatorManager.ManagerData.Items[0];
            shiftRGB.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod2 = (BeatRandomModulator)shiftRGB.ModulatorManager.ManagerData.Items[1];

            shiftRGB.Direction.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod1, mod1.Outputs[0]));
            shiftRGB.Shift.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod2, mod2.Outputs[0]));

            shiftRGB.ModulatorManager.ManagerData.Items.Move(0, 1);

            Assert.Equal(mod1.ID, shiftRGB.Direction.ModulatorID);
            Assert.Same(mod1, shiftRGB.Direction.BoundModulator);
            Assert.Equal(mod2.ID, shiftRGB.Shift.ModulatorID);
            Assert.Same(mod2, shiftRGB.Shift.BoundModulator);
        }

        [Fact]
        public void DeletingAssignedModulator_UnassignsItFromEveryBindableUsingIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var shiftRGB = provider.GetRequiredService<ShiftRGB>();
            shiftRGB.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)shiftRGB.ModulatorManager.ManagerData.Items[0];

            shiftRGB.Direction.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            shiftRGB.Shift.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            shiftRGB.Hue.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            shiftRGB.ModulatorManager.DeleteItem(randomModulator);

            Assert.Null(shiftRGB.Direction.ModulatorID);
            Assert.Null(shiftRGB.Direction.BoundModulator);
            Assert.Null(shiftRGB.Shift.ModulatorID);
            Assert.Null(shiftRGB.Shift.BoundModulator);
            Assert.Null(shiftRGB.Hue.ModulatorID);
            Assert.Null(shiftRGB.Hue.BoundModulator);
        }

        [Fact]
        public void ShiftRGB_ToModel_FromModel_RoundTripsBindableValuesAndLiveBoundModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var shiftRGB = provider.GetRequiredService<ShiftRGB>();

            shiftRGB.Direction.Value = 0.4f;
            shiftRGB.Shift.Value = 0.3f;
            shiftRGB.Hue.Value = 0.7f;
            shiftRGB.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)shiftRGB.ModulatorManager.ManagerData.Items[0];
            shiftRGB.Hue.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            var model = shiftRGB.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<ShiftRGB>();
            reloaded.FromModel(model);

            Assert.Equal(0.4f, reloaded.Direction.Value);
            Assert.Equal(0.3f, reloaded.Shift.Value);
            Assert.Equal(0.7f, reloaded.Hue.Value);
            var reloadedRandomModulator = Assert.IsType<RandomModulator>(reloaded.ModulatorManager.ManagerData.Items[0]);
            Assert.Same(reloadedRandomModulator, reloaded.Hue.BoundModulator);
        }
    }
}
