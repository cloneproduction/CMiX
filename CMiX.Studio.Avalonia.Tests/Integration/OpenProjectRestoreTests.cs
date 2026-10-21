using Avalonia.Headless.XUnit;
using CMiX.Core.Compositing;
using CMiX.Core.Persistence;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests.Integration
{
    // What an open restores from the file besides the compositions.
    public class OpenProjectRestoreTests
    {
        private static string NewPath() => Path.Combine(Path.GetTempPath(), $"cmix-openrestore-{Guid.NewGuid():N}.cmix");

        [AvaloniaFact]
        public async Task OpenProject_SavedWithNoSelection_LeavesTheLastCompositionSelected()
        {
            var path = NewPath();
            try
            {
                var (_, _, saving) = TestServiceProviderFactory.ShowMainWindow();
                for (var i = 0; i < 3; i++)
                    saving.Project.CompositionManager.AddItem(typeof(Composition));
                saving.Project.CompositionManager.ManagerData.ClearSelection();
                ProjectSerializer.Save(ProjectModelBuilder.Build(saving.Project), path);
                Assert.Equal(-1, ProjectSerializer.Load(path).CompositionManager.ManagerData.SelectedIndex);

                var (_, _, opening) = TestServiceProviderFactory.ShowMainWindow();
                await opening.MainMenu.OpenProjectFromPath(path);
                TestServiceProviderFactory.Pump();

                var compositions = opening.Project.CompositionManager;
                Assert.Equal(3, compositions.ManagerData.Items.Count);
                Assert.Same(compositions.ManagerData.Items[2], compositions.SelectedItem);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [AvaloniaFact]
        public async Task OpenProject_RestoresTheMasterBeatOfTheFile()
        {
            var path = NewPath();
            try
            {
                var (_, _, saving) = TestServiceProviderFactory.ShowMainWindow();
                saving.Project.MasterBeat.Period.Value = 437.5f;
                ProjectSerializer.Save(ProjectModelBuilder.Build(saving.Project), path);

                var (_, _, opening) = TestServiceProviderFactory.ShowMainWindow();
                Assert.NotEqual(437.5f, opening.Project.MasterBeat.Period.Value);

                await opening.MainMenu.OpenProjectFromPath(path);
                TestServiceProviderFactory.Pump();

                Assert.Equal(437.5f, opening.Project.MasterBeat.Period.Value);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }
    }
}
