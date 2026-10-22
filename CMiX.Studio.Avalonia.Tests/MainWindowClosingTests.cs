using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using CMiX.Core.Compositing;
using CMiX.Studio.Avalonia.Services;
using CMiX.Studio.Avalonia.Tests.Integration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Composition = CMiX.Core.Compositing.Composition;

namespace CMiX.Studio.Avalonia.Tests
{
    // The cross button, Alt+F4 and the close command all close the window. With unsaved changes the user is asked first.
    public class MainWindowClosingTests
    {
        private static async Task<bool> ClosesWithin(Views.MainWindow window, TimeSpan time)
        {
            var closed = false;
            window.Closed += (_, _) => closed = true;
            window.Close();

            var until = DateTime.UtcNow + time;
            while (!closed && DateTime.UtcNow < until)
            {
                Dispatcher.UIThread.RunJobs();
                await Task.Delay(10);
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
        public async Task WithoutChanges_TheWindowClosesAtOnce_WithoutAsking()
        {
            var (provider, window, _) = TestServiceProviderFactory.ShowMainWindow();

            var closed = await ClosesWithin(window, TimeSpan.FromSeconds(2));

            Assert.True(closed);
            Assert.Equal(0, provider.GetRequiredService<FakeUnsavedChangesPrompt>().TimesAsked);
        }

        [AvaloniaFact]
        public async Task WithChanges_Cancel_KeepsTheWindowOpen()
        {
            var (window, prompt, _) = ShowWithChanges(UnsavedChangesChoice.Cancel);

            var closed = await ClosesWithin(window, TimeSpan.FromMilliseconds(400));

            Assert.False(closed);
            Assert.True(window.IsVisible);
            Assert.Equal(1, prompt.TimesAsked);
        }

        [AvaloniaFact]
        public async Task WithChanges_DontSave_ClosesTheWindow()
        {
            var (window, prompt, _) = ShowWithChanges(UnsavedChangesChoice.DontSave);

            var closed = await ClosesWithin(window, TimeSpan.FromSeconds(2));

            Assert.True(closed);
            Assert.Equal(1, prompt.TimesAsked);
        }

        [AvaloniaFact]
        public async Task WithChanges_Save_WritesTheFile_ThenClosesTheWindow()
        {
            var path = Path.Combine(Path.GetTempPath(), $"cmix-closing-{Guid.NewGuid():N}.cmix");
            try
            {
                ProjectFixtures.WriteProjectWithRenamedOutputMappings(path);
                var (provider, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
                await viewModel.MainMenu.OpenProjectFromPath(path);
                TestServiceProviderFactory.Pump();
                provider.GetRequiredService<FakeUnsavedChangesPrompt>().Answer = UnsavedChangesChoice.Save;
                viewModel.Project.OutputMappingManager.Items[0].Name.Value = "Saved on close";

                var closed = await ClosesWithin(window, TimeSpan.FromSeconds(5));

                Assert.True(closed);
                Assert.Contains("Saved on close", File.ReadAllText(path));
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }
    }
}
