using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class KuwaharaTests
    {
        [Fact]
        public void ModulatorManager_CanAddRandomModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var kuwahara = provider.GetRequiredService<Kuwahara>();

            kuwahara.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(kuwahara.ModulatorManager.ManagerData.Items);
            Assert.IsType<RandomModulator>(kuwahara.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void ReorderingModulators_DoesNotUnassignTheMovedOnesBindings()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var kuwahara = (Kuwahara)factory.Create(typeof(Kuwahara));

            kuwahara.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod1 = (BeatRandomModulator)kuwahara.ModulatorManager.ManagerData.Items[0];
            kuwahara.ModulatorManager.AddItem(typeof(BeatRandomModulator));

            kuwahara.Radius.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod1, mod1.Outputs[0]));

            kuwahara.ModulatorManager.ManagerData.Items.Move(0, 1);

            Assert.Equal(mod1.ID, kuwahara.Radius.ModulatorID);
            Assert.Same(mod1, kuwahara.Radius.BoundModulator);
        }

        [Fact]
        public void DeletingAssignedModulator_UnassignsItFromEveryBindableUsingIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var kuwahara = provider.GetRequiredService<Kuwahara>();
            kuwahara.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)kuwahara.ModulatorManager.ManagerData.Items[0];

            kuwahara.Radius.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            kuwahara.ModulatorManager.DeleteItem(randomModulator);

            Assert.Null(kuwahara.Radius.ModulatorID);
            Assert.Null(kuwahara.Radius.BoundModulator);
        }

        [Fact]
        public void Kuwahara_ToModel_FromModel_RoundTripsBindableValuesAndLiveBoundModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var kuwahara = provider.GetRequiredService<Kuwahara>();

            kuwahara.Radius.Value = 4f;
            kuwahara.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)kuwahara.ModulatorManager.ManagerData.Items[0];
            kuwahara.Radius.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            var model = kuwahara.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<Kuwahara>();
            reloaded.FromModel(model);

            Assert.Equal(4f, reloaded.Radius.Value);
            var reloadedRandomModulator = Assert.IsType<RandomModulator>(reloaded.ModulatorManager.ManagerData.Items[0]);
            Assert.Same(reloadedRandomModulator, reloaded.Radius.BoundModulator);
        }
    }
}
