using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ShadowTests
    {
        [Fact]
        public void ModulatorManager_CanAddRandomModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var shadow = provider.GetRequiredService<Shadow>();

            shadow.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(shadow.ModulatorManager.ManagerData.Items);
            Assert.IsType<RandomModulator>(shadow.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void ReorderingModulators_DoesNotUnassignTheMovedOnesBindings()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var shadow = (Shadow)factory.Create(typeof(Shadow));

            shadow.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod1 = (BeatRandomModulator)shadow.ModulatorManager.ManagerData.Items[0];
            shadow.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod2 = (BeatRandomModulator)shadow.ModulatorManager.ManagerData.Items[1];

            shadow.Height.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod1, mod1.Outputs[0]));
            shadow.LightDirection.Z.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod2, mod2.Outputs[0]));

            shadow.ModulatorManager.ManagerData.Items.Move(0, 1);

            Assert.Equal(mod1.ID, shadow.Height.ModulatorID);
            Assert.Same(mod1, shadow.Height.BoundModulator);
            Assert.Equal(mod2.ID, shadow.LightDirection.Z.ModulatorID);
            Assert.Same(mod2, shadow.LightDirection.Z.BoundModulator);
        }

        [Fact]
        public void DeletingAssignedModulator_UnassignsItFromANestedVectorBindable()
        {
            var provider = TestServiceProviderFactory.Create();
            var shadow = provider.GetRequiredService<Shadow>();
            shadow.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)shadow.ModulatorManager.ManagerData.Items[0];

            shadow.LightDirection.Y.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            shadow.ModulatorManager.DeleteItem(randomModulator);

            Assert.Null(shadow.LightDirection.Y.ModulatorID);
            Assert.Null(shadow.LightDirection.Y.BoundModulator);
        }

        [Fact]
        public void Shadow_ToModel_FromModel_RoundTripsNestedVectorValueAndLiveBoundModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var shadow = provider.GetRequiredService<Shadow>();

            shadow.LightDirection.X.Value = 4f;
            shadow.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)shadow.ModulatorManager.ManagerData.Items[0];
            shadow.LightDirection.X.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            var model = shadow.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<Shadow>();
            reloaded.FromModel(model);

            Assert.Equal(4f, reloaded.LightDirection.X.Value);
            var reloadedRandomModulator = Assert.IsType<RandomModulator>(reloaded.ModulatorManager.ManagerData.Items[0]);
            Assert.Same(reloadedRandomModulator, reloaded.LightDirection.X.BoundModulator);
        }
    }
}
