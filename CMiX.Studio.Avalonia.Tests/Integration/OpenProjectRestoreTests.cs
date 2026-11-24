using Avalonia.Headless.XUnit;
using CMiX.Core.Compositing;
using CMiX.Core.Persistence;
using CMiX.Studio.Avalonia.Tests.Support;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests.Integration
{
    // What an open restores from the file besides the compositions.
    public class OpenProjectRestoreTests : IDisposable
    {
        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        [AvaloniaFact]
        public async Task OpenProject_SavedWithNoSelection_LeavesNothingSelected()
        {
            using var temp = new TempFolder("openrestore");
            var path = temp.PathOf("project.cmix");
            var (_, _, saving) = windows.ShowMainWindow();
            for (var i = 0; i < 3; i++)
                saving.Project.CompositionManager.AddItem(typeof(Composition));
            saving.Project.CompositionManager.ManagerData.ClearSelection();
            ProjectSerializer.Save(ProjectModelBuilder.Build(saving.Project), path);
            Assert.Equal(-1, ProjectSerializer.Load(path).CompositionManager.ManagerData.SelectedIndex);

            var (_, _, opening) = windows.ShowMainWindow();
            await opening.ProjectFile.OpenProjectFromPath(path);
            TestServiceProviderFactory.Pump();

            var compositions = opening.Project.CompositionManager;
            Assert.Equal(3, compositions.ManagerData.Items.Count);
            Assert.Null(compositions.SelectedItem);
        }

        [AvaloniaFact]
        public async Task OpenProject_AFileWithAMasterBeat_RestoresTheMasterBeat()
        {
            using var temp = new TempFolder("openrestore");
            var path = temp.PathOf("project.cmix");
            var (_, _, saving) = windows.ShowMainWindow();
            saving.Project.MasterBeat.Period.Value = 437.5f;
            ProjectSerializer.Save(ProjectModelBuilder.Build(saving.Project), path);

            var (_, _, opening) = windows.ShowMainWindow();
            Assert.NotEqual(437.5f, opening.Project.MasterBeat.Period.Value);

            await opening.ProjectFile.OpenProjectFromPath(path);
            TestServiceProviderFactory.Pump();

            Assert.Equal(437.5f, opening.Project.MasterBeat.Period.Value);
        }
    }
}
