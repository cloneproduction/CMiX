using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class PixelateTests
    {
        [Fact]
        public void ModulatorManager_CanAddRandomModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var pixelate = provider.GetRequiredService<Pixelate>();

            pixelate.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(pixelate.ModulatorManager.ManagerData.Items);
            Assert.IsType<RandomModulator>(pixelate.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void ReorderingModulators_DoesNotUnassignTheMovedOnesBindings()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var pixelate = (Pixelate)factory.Create(typeof(Pixelate));

            pixelate.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod1 = (BeatRandomModulator)pixelate.ModulatorManager.ManagerData.Items[0];
            pixelate.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod2 = (BeatRandomModulator)pixelate.ModulatorManager.ManagerData.Items[1];

            pixelate.X.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod1, mod1.Outputs[0]));
            pixelate.Y.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod2, mod2.Outputs[0]));

            pixelate.ModulatorManager.ManagerData.Items.Move(0, 1);

            Assert.Equal(mod1.ID, pixelate.X.ModulatorID);
            Assert.Same(mod1, pixelate.X.BoundModulator);
            Assert.Equal(mod2.ID, pixelate.Y.ModulatorID);
            Assert.Same(mod2, pixelate.Y.BoundModulator);
        }

        [Fact]
        public void DeletingAssignedModulator_UnassignsItFromEveryBindableUsingIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var pixelate = provider.GetRequiredService<Pixelate>();
            pixelate.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)pixelate.ModulatorManager.ManagerData.Items[0];

            pixelate.X.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            pixelate.Y.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            pixelate.ModulatorManager.DeleteItem(randomModulator);

            Assert.Null(pixelate.X.ModulatorID);
            Assert.Null(pixelate.X.BoundModulator);
            Assert.Null(pixelate.Y.ModulatorID);
            Assert.Null(pixelate.Y.BoundModulator);
        }

        [Fact]
        public void Pixelate_ToModel_FromModel_RoundTripsBindableValuesAndLiveBoundModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var pixelate = provider.GetRequiredService<Pixelate>();

            pixelate.X.Value = 4f;
            pixelate.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)pixelate.ModulatorManager.ManagerData.Items[0];
            pixelate.X.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            var model = pixelate.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<Pixelate>();
            reloaded.FromModel(model);

            Assert.Equal(4f, reloaded.X.Value);
            var reloadedRandomModulator = Assert.IsType<RandomModulator>(reloaded.ModulatorManager.ManagerData.Items[0]);
            Assert.Same(reloadedRandomModulator, reloaded.X.BoundModulator);
        }

        [Fact]
        public void XAndY_DefaultToPointFive()
        {
            var provider = TestServiceProviderFactory.Create();
            var pixelate = provider.GetRequiredService<Pixelate>();

            Assert.Equal(0.5f, pixelate.X.Value);
            Assert.Equal(0.5f, pixelate.Y.Value);
        }
    }
}
