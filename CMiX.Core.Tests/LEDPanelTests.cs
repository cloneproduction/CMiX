using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    public class LEDPanelTests
    {
        [Fact]
        public void ModulatorManager_CanAddRandomModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var ledPanel = provider.GetRequiredService<LEDPanel>();

            ledPanel.ModulatorManager.AddItem(typeof(RandomModulator));

            Assert.Single(ledPanel.ModulatorManager.ManagerData.Items);
            Assert.IsType<RandomModulator>(ledPanel.ModulatorManager.ManagerData.Items[0]);
        }

        [Fact]
        public void ReorderingModulators_DoesNotUnassignTheMovedOnesBindings()
        {
            var provider = TestServiceProviderFactory.Create();
            var factory = provider.GetRequiredService<ControlFactory>();
            var ledPanel = (LEDPanel)factory.Create(typeof(LEDPanel));

            ledPanel.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod1 = (BeatRandomModulator)ledPanel.ModulatorManager.ManagerData.Items[0];
            ledPanel.ModulatorManager.AddItem(typeof(BeatRandomModulator));
            var mod2 = (BeatRandomModulator)ledPanel.ModulatorManager.ManagerData.Items[1];

            ledPanel.PixelSize.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod1, mod1.Outputs[0]));
            ledPanel.MaskStagger.SetModulatorCommand.Execute(new ModulatorOutputSelection(mod2, mod2.Outputs[0]));

            ledPanel.ModulatorManager.ManagerData.Items.Move(0, 1);

            Assert.Equal(mod1.ID, ledPanel.PixelSize.ModulatorID);
            Assert.Same(mod1, ledPanel.PixelSize.BoundModulator);
            Assert.Equal(mod2.ID, ledPanel.MaskStagger.ModulatorID);
            Assert.Same(mod2, ledPanel.MaskStagger.BoundModulator);
        }

        [Fact]
        public void DeletingAssignedModulator_UnassignsItFromEveryBindableUsingIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var ledPanel = provider.GetRequiredService<LEDPanel>();
            ledPanel.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)ledPanel.ModulatorManager.ManagerData.Items[0];

            ledPanel.PixelSize.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            ledPanel.MaskStagger.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            ledPanel.MaskBorder.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));
            ledPanel.MaskIntensity.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            ledPanel.ModulatorManager.DeleteItem(randomModulator);

            Assert.Null(ledPanel.PixelSize.ModulatorID);
            Assert.Null(ledPanel.PixelSize.BoundModulator);
            Assert.Null(ledPanel.MaskStagger.ModulatorID);
            Assert.Null(ledPanel.MaskStagger.BoundModulator);
            Assert.Null(ledPanel.MaskBorder.ModulatorID);
            Assert.Null(ledPanel.MaskBorder.BoundModulator);
            Assert.Null(ledPanel.MaskIntensity.ModulatorID);
            Assert.Null(ledPanel.MaskIntensity.BoundModulator);
        }

        [Fact]
        public void LEDPanel_ToModel_FromModel_RoundTripsBindableValuesAndLiveBoundModulator()
        {
            var provider = TestServiceProviderFactory.Create();
            var ledPanel = provider.GetRequiredService<LEDPanel>();

            ledPanel.PixelSize.Value = 20f;
            ledPanel.MaskStagger.Value = 0.3f;
            ledPanel.MaskBorder.Value = 0.4f;
            ledPanel.MaskIntensity.Value = 0.6f;
            ledPanel.ModulatorManager.AddItem(typeof(RandomModulator));
            var randomModulator = (RandomModulator)ledPanel.ModulatorManager.ManagerData.Items[0];
            ledPanel.MaskIntensity.SetModulatorCommand.Execute(new ModulatorOutputSelection(randomModulator, randomModulator.Outputs[0]));

            var model = ledPanel.ToModel();

            var provider2 = TestServiceProviderFactory.Create();
            var reloaded = provider2.GetRequiredService<LEDPanel>();
            reloaded.FromModel(model);

            Assert.Equal(20f, reloaded.PixelSize.Value);
            Assert.Equal(0.3f, reloaded.MaskStagger.Value);
            Assert.Equal(0.4f, reloaded.MaskBorder.Value);
            Assert.Equal(0.6f, reloaded.MaskIntensity.Value);
            var reloadedRandomModulator = Assert.IsType<RandomModulator>(reloaded.ModulatorManager.ManagerData.Items[0]);
            Assert.Same(reloadedRandomModulator, reloaded.MaskIntensity.BoundModulator);
        }
    }
}
