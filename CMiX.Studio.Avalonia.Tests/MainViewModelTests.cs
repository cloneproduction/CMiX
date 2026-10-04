using Avalonia.Headless.XUnit;
using CMiX.Core.Texturing.Sources;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // A snapshot from another peer replaces the project, but not the six repository managers.
    // MainViewModel empties them on the SnapshotApplied event of its peer, the way File > New and
    // File > Open empty them through MainMenu.
    public class MainViewModelTests
    {
        [AvaloniaFact]
        public void ClearRepositoryManagers_EmptiesTheManagersAndTheRepository()
        {
            var (_, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();

            viewModel.TextureManager.AddItem(typeof(CheckerBoard));
            TestServiceProviderFactory.Pump();

            Assert.Single(viewModel.TextureManager.ManagerData.Items);

            viewModel.ClearRepositoryManagers();
            TestServiceProviderFactory.Pump();

            Assert.Empty(viewModel.TextureManager.ManagerData.Items);
            Assert.Empty(viewModel.ControlRepository.Textures);
        }
    }
}
