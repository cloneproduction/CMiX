using Avalonia.Headless.XUnit;
using CMiX.Core.Texturing.Sources;
using CMiX.Studio.Avalonia.Session;
using CMiX.Studio.Avalonia.Tests.Support;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The six repository lists are not part of a project. ProjectSession empties them on a peer snapshot,
    // on File > New and on File > Open, and the repository lets the items go.
    public class ProjectSessionTests : IDisposable
    {
        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        [AvaloniaFact]
        public void ClearAll_AFullProject_EmptiesTheListsAndTheRepository()
        {
            var (provider, _, viewModel) = windows.ShowMainWindow();

            viewModel.TextureManager.AddItem(typeof(CheckerBoard));
            TestServiceProviderFactory.Pump();

            Assert.Single(viewModel.TextureManager.ManagerData.Items);

            provider.GetRequiredService<ProjectSession>().ClearAll();
            TestServiceProviderFactory.Pump();

            Assert.Empty(viewModel.TextureManager.ManagerData.Items);
            Assert.Empty(viewModel.ControlRepository.Textures);
        }
    }
}
