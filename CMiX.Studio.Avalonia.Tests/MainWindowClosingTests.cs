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
    // The cross button, Alt+F4 and the close command all close the window. With unsaved changes the user is asked first.
    // The New and the Open commands ask in the same way.
    public class MainWindowClosingTests
    {
        private static Task<bool> ClosesWithin(Views.MainWindow window, TimeSpan time) => ClosesWithin(window, window.Close, time);

        private static async Task<bool> ClosesWithin(Views.MainWindow window, Action close, TimeSpan time)
        {
            var closed = false;
            window.Closed += (_, _) => closed = true;
            close();

            var until = DateTime.UtcNow + time;
            while (!closed && DateTime.UtcNow < until)
            {
                Dispatcher.UIThread.RunJobs();
                await Task.Yield();
            }

            return closed;
        }

        private static (Views.MainWindow Window, FakeUnsavedChangesPrompt Prompt, ViewModels.MainViewModel ViewModel) ShowWithChanges(UnsavedChangesChoice answer)
        {
            var (provider, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            var prompt = provider.GetRequiredService<FakeUnsavedChangesPrompt>();
            prompt.Answer = answer;
            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            return (window, prompt, viewModel);
        }

        [AvaloniaFact]
        public async Task Close_WithoutChanges_ClosesAtOnceWithoutAsking()
        {
            var (provider, window, _) = TestServiceProviderFactory.ShowMainWindow();

            var closed = await ClosesWithin(window, TimeSpan.FromSeconds(2));

            Assert.True(closed);
            Assert.Equal(0, provider.GetRequiredService<FakeUnsavedChangesPrompt>().TimesAsked);
        }

        [AvaloniaFact]
        public async Task Close_WithChangesAndCancel_KeepsTheWindowOpen()
        {
            var (window, prompt, _) = ShowWithChanges(UnsavedChangesChoice.Cancel);

            var closed = await ClosesWithin(window, TimeSpan.FromMilliseconds(400));

            Assert.False(closed);
            Assert.True(window.IsVisible);
            Assert.Equal(1, prompt.TimesAsked);
        }

        [AvaloniaFact]
        public async Task Close_WithChangesAndDontSave_ClosesTheWindow()
        {
            var (window, prompt, _) = ShowWithChanges(UnsavedChangesChoice.DontSave);

            var closed = await ClosesWithin(window, TimeSpan.FromSeconds(2));

            Assert.True(closed);
            Assert.Equal(1, prompt.TimesAsked);
        }

        [AvaloniaFact]
        public async Task Close_WithChangesAndSave_WritesTheFileThenClosesTheWindow()
        {
            using var temp = new TempFolder("closing");
            var path = temp.PathOf("project.cmix");
            ProjectFixtures.WriteProjectWithRenamedOutputMappings(path);
            var (provider, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            await viewModel.ProjectFile.OpenProjectFromPath(path);
            TestServiceProviderFactory.Pump();
            provider.GetRequiredService<FakeUnsavedChangesPrompt>().Answer = UnsavedChangesChoice.Save;
            viewModel.Project.OutputMappingManager.Items[0].Name.Value = "Saved on close";

            var closed = await ClosesWithin(window, TimeSpan.FromSeconds(5));

            Assert.True(closed);
            Assert.Contains("Saved on close", File.ReadAllText(path));
        }

        [AvaloniaFact]
        public async Task QuitCommand_WithChanges_AsksThenClosesTheWindow()
        {
            var (window, prompt, viewModel) = ShowWithChanges(UnsavedChangesChoice.DontSave);

            var closed = await ClosesWithin(window, () => viewModel.MainMenu.QuitCommand.Execute(null), TimeSpan.FromSeconds(2));

            Assert.True(closed);
            Assert.Equal(1, prompt.TimesAsked);
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
