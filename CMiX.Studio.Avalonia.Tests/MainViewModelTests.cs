using Avalonia.Headless.XUnit;
using CMiX.Core.Texturing.Sources;
using CMiX.Studio.Avalonia.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // A snapshot from another peer replaces the project, but not the six repository lists.
    // ProjectSession empties them on the SnapshotApplied event of its peer, the way File > New and
    // File > Open empty them through ProjectSession.ClearAll.
    public class MainViewModelTests
    {
        [AvaloniaFact]
        public void ClearLists_EmptiesTheListsAndTheRepository()
        {
            var (provider, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();

            viewModel.TextureManager.AddItem(typeof(CheckerBoard));
            TestServiceProviderFactory.Pump();

            Assert.Single(viewModel.TextureManager.ManagerData.Items);

            provider.GetRequiredService<ProjectSession>().ClearLists();
            TestServiceProviderFactory.Pump();

            Assert.Empty(viewModel.TextureManager.ManagerData.Items);
            Assert.Empty(viewModel.ControlRepository.Textures);
        }
    }
}
