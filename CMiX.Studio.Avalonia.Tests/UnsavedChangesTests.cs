using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using CMiX.Core.Compositing;
using CMiX.Studio.Avalonia.Services;
using CMiX.Studio.Avalonia.Tests.Integration;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;

namespace CMiX.Studio.Avalonia.Tests
{
    // The project has unsaved changes when it differs from the last time it was saved, opened or started new.
    public class UnsavedChangesTests
    {
        [AvaloniaFact]
        public void ARunningSession_HasNoChanges_UntilSomethingIsEdited()
        {
            var (provider, _, _) = TestServiceProviderFactory.ShowMainWindow();

            Assert.False(provider.GetRequiredService<UnsavedChanges>().HasChanges);
        }

        [AvaloniaFact]
        public void AnEdit_IsAChange()
        {
            var (provider, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();

            viewModel.Project.CompositionManager.AddItem(typeof(Composition));

            Assert.True(provider.GetRequiredService<UnsavedChanges>().HasChanges);
        }

        [AvaloniaFact]
        public void AnEditThatIsUndone_IsNoChangeAnymore()
        {
            var (provider, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));

            provider.GetRequiredService<CMiX.Core.Undo.UndoManager>().Undo();

            Assert.False(provider.GetRequiredService<UnsavedChanges>().HasChanges);
        }

        [AvaloniaFact]
        public void MarkSaved_TakesTheCurrentStateAsTheSavedOne()
        {
            var (provider, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            var unsaved = provider.GetRequiredService<UnsavedChanges>();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));

            unsaved.MarkSaved();

            Assert.False(unsaved.HasChanges);
        }

        [AvaloniaFact]
        public async Task ARunningSession_StaysWithoutChanges_WhileTimePasses()
        {
            var (provider, _, _) = TestServiceProviderFactory.ShowMainWindow();
            var unsaved = provider.GetRequiredService<UnsavedChanges>();

            for (var i = 0; i < 10; i++)
            {
                await Task.Delay(50);
                Dispatcher.UIThread.RunJobs();
                Assert.False(unsaved.HasChanges);
            }
        }

        [AvaloniaFact]
        public async Task OpeningAFile_IsNoChange_AndSavingAfterAnEditClearsTheChange()
        {
            var path = Path.Combine(Path.GetTempPath(), $"cmix-unsaved-{Guid.NewGuid():N}.cmix");
            try
            {
                ProjectFixtures.WriteProjectWithRenamedOutputMappings(path);
                var (provider, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();
                var unsaved = provider.GetRequiredService<UnsavedChanges>();

                await viewModel.ProjectFile.OpenProjectFromPath(path);
                TestServiceProviderFactory.Pump();
                Assert.False(unsaved.HasChanges);

                viewModel.Project.OutputMappingManager.Items[0].Name.Value = "Changed";
                Assert.True(unsaved.HasChanges);

                await ((CommunityToolkit.Mvvm.Input.IAsyncRelayCommand)viewModel.ProjectFile.SaveProjectCommand).ExecuteAsync(null);
                Assert.False(unsaved.HasChanges);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [AvaloniaFact]
        public void ANewProject_IsTheSavedState()
        {
            var (provider, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            Assert.True(provider.GetRequiredService<UnsavedChanges>().HasChanges);
            provider.GetRequiredService<FakeUnsavedChangesPrompt>().Answer = UnsavedChangesChoice.DontSave;

            viewModel.ProjectFile.NewProjectCommand.Execute(null);
            TestServiceProviderFactory.Pump();

            Assert.False(provider.GetRequiredService<UnsavedChanges>().HasChanges);
        }

        [AvaloniaFact]
        public void NewProjectCommand_WithUnsavedChangesAndCancel_KeepsTheProject()
        {
            var (provider, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var prompt = provider.GetRequiredService<FakeUnsavedChangesPrompt>();

            viewModel.ProjectFile.NewProjectCommand.Execute(null);
            TestServiceProviderFactory.Pump();

            Assert.Equal(1, prompt.TimesAsked);
            Assert.Single(viewModel.Project.CompositionManager.ManagerData.Items);
            Assert.True(provider.GetRequiredService<UnsavedChanges>().HasChanges);
        }

        [AvaloniaFact]
        public void NewProjectCommand_WithoutChanges_DoesNotAsk()
        {
            var (provider, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();

            viewModel.ProjectFile.NewProjectCommand.Execute(null);
            TestServiceProviderFactory.Pump();

            Assert.Equal(0, provider.GetRequiredService<FakeUnsavedChangesPrompt>().TimesAsked);
        }

        [AvaloniaFact]
        public async Task OpenProjectCommand_WithUnsavedChangesAndCancel_DoesNotShowTheFileDialog()
        {
            var dialogs = new FakeFileDialogManager("never.cmix");
            var (provider, _, viewModel) = TestServiceProviderFactory.ShowMainWindow(services =>
                services.AddSingleton<IDialogService>(provider => new DialogService(dialogs, viewModelFactory: type => provider.GetService(type))));
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));

            await ((IAsyncRelayCommand)viewModel.ProjectFile.OpenProjectCommand).ExecuteAsync(null);

            Assert.Equal(1, provider.GetRequiredService<FakeUnsavedChangesPrompt>().TimesAsked);
            Assert.Equal(0, dialogs.TimesShown);
            Assert.Single(viewModel.Project.CompositionManager.ManagerData.Items);
        }
    }
}
