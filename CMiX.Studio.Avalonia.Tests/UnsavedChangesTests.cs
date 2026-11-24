using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using CMiX.Core.Compositing;
using CMiX.Studio.Avalonia.Notices;
using CMiX.Studio.Avalonia.Session;
using CMiX.Studio.Avalonia.Tests.Integration;
using CMiX.Studio.Avalonia.Tests.Support;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;

namespace CMiX.Studio.Avalonia.Tests
{
    // The project has unsaved changes when it differs from the last time it was saved, opened or started new.
    public class UnsavedChangesTests : IDisposable
    {
        private readonly TestWindows windows = new();

        public void Dispose() => windows.Dispose();

        [AvaloniaFact]
        public void ARunningSession_HasNoChanges_UntilSomethingIsEdited()
        {
            var (provider, _, _) = windows.ShowMainWindow();

            Assert.False(provider.GetRequiredService<UnsavedChanges>().HasChanges);
        }

        [AvaloniaFact]
        public void HasChanges_AnEdit_IsTrue()
        {
            var (provider, _, viewModel) = windows.ShowMainWindow();

            viewModel.Project.CompositionManager.AddItem(typeof(Composition));

            Assert.True(provider.GetRequiredService<UnsavedChanges>().HasChanges);
        }

        [AvaloniaFact]
        public void HasChanges_AnEditThatIsUndone_IsFalse()
        {
            var (provider, _, viewModel) = windows.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));

            provider.GetRequiredService<CMiX.Core.Undo.UndoManager>().Undo();

            Assert.False(provider.GetRequiredService<UnsavedChanges>().HasChanges);
        }

        [AvaloniaFact]
        public void MarkSaved_AfterAnEdit_TakesTheCurrentStateAsTheSavedOne()
        {
            var (provider, _, viewModel) = windows.ShowMainWindow();
            var unsaved = provider.GetRequiredService<UnsavedChanges>();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));

            unsaved.MarkSaved();

            Assert.False(unsaved.HasChanges);
        }

        [AvaloniaFact]
        public void HasChanges_ARunningSessionOverTenFrames_StaysFalse()
        {
            var (provider, _, _) = windows.ShowMainWindow();
            var unsaved = provider.GetRequiredService<UnsavedChanges>();

            for (var i = 0; i < 10; i++)
            {
                AvaloniaHeadlessPlatform.ForceRenderTimerTick();
                Dispatcher.UIThread.RunJobs();
                Assert.False(unsaved.HasChanges);
            }
        }

        [AvaloniaFact]
        public async Task OpeningAFile_IsNoChange_AndSavingAfterAnEditClearsTheChange()
        {
            using var temp = new TempFolder("unsaved");
            var path = temp.PathOf("project.cmix");
            ProjectFixtures.WriteProjectWithRenamedOutputMappings(path);
            var (provider, _, viewModel) = windows.ShowMainWindow();
            var unsaved = provider.GetRequiredService<UnsavedChanges>();

            await viewModel.ProjectFile.OpenProjectFromPath(path);
            TestServiceProviderFactory.Pump();
            Assert.False(unsaved.HasChanges);

            viewModel.Project.OutputMappingManager.Items[0].Name.Value = "Changed";
            Assert.True(unsaved.HasChanges);

            await ((CommunityToolkit.Mvvm.Input.IAsyncRelayCommand)viewModel.ProjectFile.SaveProjectCommand).ExecuteAsync(null);
            Assert.False(unsaved.HasChanges);
        }

        [AvaloniaFact]
        public void NewProject_WithChanges_IsTheSavedState()
        {
            var (provider, _, viewModel) = windows.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            Assert.True(provider.GetRequiredService<UnsavedChanges>().HasChanges);
            provider.GetRequiredService<FakeUnsavedChangesPrompt>().Answer = UnsavedChangesChoice.DontSave;

            viewModel.ProjectFile.NewProjectCommand.Execute(null);
            TestServiceProviderFactory.Pump();

            Assert.False(provider.GetRequiredService<UnsavedChanges>().HasChanges);
        }
    }
}
