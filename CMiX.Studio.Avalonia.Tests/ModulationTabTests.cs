using Avalonia.Headless.XUnit;
using CMiX.Core.Modulation;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    public class ModulationTabTests
    {
        [AvaloniaFact]
        public void ModulatorTestManager_AddItem_CreatesScaleModifierWithFourChannels()
        {
            var (_, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();

            viewModel.ModulatorTestManager.AddItem(typeof(ScaleModifier));
            TestServiceProviderFactory.Pump();

            Assert.Single(viewModel.ModulatorTestManager.ManagerData.Items);
            var created = Assert.IsType<ScaleModifier>(viewModel.ModulatorTestManager.ManagerData.Items[0]);
            Assert.Equal(4, created.Channels.Count);
        }
    }
}
