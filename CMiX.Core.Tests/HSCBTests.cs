using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class HSCBTests
    {
        [Fact]
        public void ModulatorManager_CanAddRandomModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var hscb = provider.GetRequiredService<HSCB>();

            hscb.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(hscb.ModulatorManager.ManagerData.Items);
            Assert.IsType<RandomModulator>(hscb.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void ReorderingModulators_DoesNotUnassignTheMovedOnesBindings()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var hscb = (HSCB)factory.Create(typeof(HSCB));

            hscb.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod1 = (BeatRandomModulator)hscb.ModulatorManager.ManagerData.Items[0];
            hscb.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod2 = (BeatRandomModulator)hscb.ModulatorManager.ManagerData.Items[1];

            hscb.Hue.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod1, mod1.Outputs[0]));
            hscb.Saturation.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod2, mod2.Outputs[0]));

            hscb.ModulatorManager.ManagerData.Items.Move(0, 1);

            Assert.Equal(mod1.ID, hscb.Hue.ModulatorID);
            Assert.Same(mod1, hscb.Hue.BoundModulator);
            Assert.Equal(mod2.ID, hscb.Saturation.ModulatorID);
            Assert.Same(mod2, hscb.Saturation.BoundModulator);
        }

        [Fact]
        public void DeletingAssignedModulator_UnassignsItFromEveryBindableUsingIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var hscb = provider.GetRequiredService<HSCB>();
            hscb.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)hscb.ModulatorManager.ManagerData.Items[0];

            hscb.Hue.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            hscb.Saturation.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            hscb.Contrast.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            hscb.Brightness.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            hscb.ModulatorManager.DeleteItem(randomModulator);

            Assert.Null(hscb.Hue.ModulatorID);
            Assert.Null(hscb.Hue.BoundModulator);
            Assert.Null(hscb.Saturation.ModulatorID);
            Assert.Null(hscb.Saturation.BoundModulator);
            Assert.Null(hscb.Contrast.ModulatorID);
            Assert.Null(hscb.Contrast.BoundModulator);
            Assert.Null(hscb.Brightness.ModulatorID);
            Assert.Null(hscb.Brightness.BoundModulator);
        }

        [Fact]
        public void HSCB_ToModel_FromModel_RoundTripsBindableValuesAndLiveBoundModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var hscb = provider.GetRequiredService<HSCB>();

            hscb.Hue.Value = 0.3f;
            hscb.Saturation.Value = 0.6f;
            hscb.Contrast.Value = 0.4f;
            hscb.Brightness.Value = 0.5f;
            hscb.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)hscb.ModulatorManager.ManagerData.Items[0];
            hscb.Brightness.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            var model = hscb.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<HSCB>();
            reloaded.FromModel(model);

            Assert.Equal(0.3f, reloaded.Hue.Value);
            Assert.Equal(0.6f, reloaded.Saturation.Value);
            Assert.Equal(0.4f, reloaded.Contrast.Value);
            Assert.Equal(0.5f, reloaded.Brightness.Value);
            var reloadedRandomModulator = Assert.IsType<RandomModulator>(reloaded.ModulatorManager.ManagerData.Items[0]);
            Assert.Same(reloadedRandomModulator, reloaded.Brightness.BoundModulator);
        }
    }
}
