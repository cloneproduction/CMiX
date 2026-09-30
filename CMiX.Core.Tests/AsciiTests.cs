using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class AsciiTests
    {
        [Fact]
        public void ModulatorManager_CanAddRandomModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var ascii = provider.GetRequiredService<Ascii>();

            ascii.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(ascii.ModulatorManager.ManagerData.Items);
            Assert.IsType<RandomModulator>(ascii.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void ReorderingModulators_DoesNotUnassignTheMovedOnesBindings()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var ascii = (Ascii)factory.Create(typeof(Ascii));

            ascii.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod1 = (BeatRandomModulator)ascii.ModulatorManager.ManagerData.Items[0];
            ascii.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod2 = (BeatRandomModulator)ascii.ModulatorManager.ManagerData.Items[1];

            ascii.GridSize.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod1, mod1.Outputs[0]));
            ascii.X.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod2, mod2.Outputs[0]));

            ascii.ModulatorManager.ManagerData.Items.Move(0, 1);

            Assert.Equal(mod1.ID, ascii.GridSize.ModulatorID);
            Assert.Same(mod1, ascii.GridSize.BoundModulator);
            Assert.Equal(mod2.ID, ascii.X.ModulatorID);
            Assert.Same(mod2, ascii.X.BoundModulator);
        }

        [Fact]
        public void DeletingAssignedModulator_UnassignsItFromEveryBindableUsingIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var ascii = provider.GetRequiredService<Ascii>();
            ascii.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)ascii.ModulatorManager.ManagerData.Items[0];

            ascii.GridSize.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            ascii.X.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            ascii.Y.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            ascii.ModulatorManager.DeleteItem(randomModulator);

            Assert.Null(ascii.GridSize.ModulatorID);
            Assert.Null(ascii.GridSize.BoundModulator);
            Assert.Null(ascii.X.ModulatorID);
            Assert.Null(ascii.X.BoundModulator);
            Assert.Null(ascii.Y.ModulatorID);
            Assert.Null(ascii.Y.BoundModulator);
        }

        [Fact]
        public void Ascii_ToModel_FromModel_RoundTripsBindableValuesAndLiveBoundModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var ascii = provider.GetRequiredService<Ascii>();

            ascii.GridSize.Value = 0.4f;
            ascii.X.Value = 12f;
            ascii.Y.Value = 20f;
            ascii.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)ascii.ModulatorManager.ManagerData.Items[0];
            ascii.Y.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            var model = ascii.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<Ascii>();
            reloaded.FromModel(model);

            Assert.Equal(0.4f, reloaded.GridSize.Value);
            Assert.Equal(12f, reloaded.X.Value);
            Assert.Equal(20f, reloaded.Y.Value);
            var reloadedRandomModulator = Assert.IsType<RandomModulator>(reloaded.ModulatorManager.ManagerData.Items[0]);
            Assert.Same(reloadedRandomModulator, reloaded.Y.BoundModulator);
        }
    }
}
