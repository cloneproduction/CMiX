using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class BlurTests
    {
        [Fact]
        public void ModulatorManager_CanAddRandomModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var blur = provider.GetRequiredService<Blur>();

            blur.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(blur.ModulatorManager.ManagerData.Items);
            Assert.IsType<RandomModulator>(blur.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void ReorderingModulators_DoesNotUnassignTheMovedOnesBindings()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var blur = (Blur)factory.Create(typeof(Blur));

            blur.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod1 = (BeatRandomModulator)blur.ModulatorManager.ManagerData.Items[0];
            blur.ModulatorManager.AddItem(typeof(BeatRandomModulator));

            blur.Strength.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod1, mod1.Outputs[0]));

            blur.ModulatorManager.ManagerData.Items.Move(0, 1);

            Assert.Equal(mod1.ID, blur.Strength.ModulatorID);
            Assert.Same(mod1, blur.Strength.BoundModulator);
        }

        [Fact]
        public void DeletingAssignedModulator_UnassignsItFromEveryBindableUsingIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var blur = provider.GetRequiredService<Blur>();
            blur.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)blur.ModulatorManager.ManagerData.Items[0];

            blur.Strength.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            blur.ModulatorManager.DeleteItem(randomModulator);

            Assert.Null(blur.Strength.ModulatorID);
            Assert.Null(blur.Strength.BoundModulator);
        }

        [Fact]
        public void Blur_ToModel_FromModel_RoundTripsBindableValuesAndLiveBoundModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var blur = provider.GetRequiredService<Blur>();

            blur.Strength.Value = 0.8f;
            blur.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)blur.ModulatorManager.ManagerData.Items[0];
            blur.Strength.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            var model = blur.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<Blur>();
            reloaded.FromModel(model);

            Assert.Equal(0.8f, reloaded.Strength.Value);
            var reloadedRandomModulator = Assert.IsType<RandomModulator>(reloaded.ModulatorManager.ManagerData.Items[0]);
            Assert.Same(reloadedRandomModulator, reloaded.Strength.BoundModulator);
        }
    }
}
