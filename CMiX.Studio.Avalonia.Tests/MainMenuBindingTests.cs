// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
using CMiX.Core.Networking;
using CMiX.Core.Texturing.Sources;
using CMiX.Studio.Avalonia.Tests.Integration;
using CMiX.Studio.Avalonia.ViewModels;
using CMiX.UI.Controls.String;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // A row of the main menu holds the command of its source. The title bar shows the open project.
    public class MainMenuBindingTests
    {
        private static MenuItem OpenRow(Window window, string top, string row)
        {
            var item = window.GetVisualDescendants().OfType<Menu>().First().Items.OfType<MenuItem>().Single(menuItem => (string?)menuItem.Header == top);
            item.Open();
            TestServiceProviderFactory.Pump();
            return item.Items.OfType<MenuItem>().Single(menuItem => (string?)menuItem.Header == row);
        }

        private static ICommand ExpectedCommand(MainViewModel viewModel, string top, string row) => (top, row) switch
        {
            ("File", "New") => viewModel.ProjectFile.NewProjectCommand,
            ("File", "Open") => viewModel.ProjectFile.OpenProjectCommand,
            ("File", "Save") => viewModel.ProjectFile.SaveProjectCommand,
            ("File", "Save As") => viewModel.ProjectFile.SaveAsProjectCommand,
            ("File", "Quit") => viewModel.MainWindowController.CloseWindowCommand,
            ("Edit", "Undo") => viewModel.UndoCommand,
            ("Edit", "Redo") => viewModel.RedoCommand,
            ("Compo", "New Composition") => viewModel.MainMenu.AddCompositionCommand,
            ("Compo", "Delete") => viewModel.MainMenu.RemoveCompositionCommand,
            ("Compo", "Duplicate") => viewModel.MainMenu.DuplicateCompositionCommand,
            ("Layer", "New Layer") => viewModel.MainMenu.AddLayerCommand,
            ("Layer", "Delete") => viewModel.MainMenu.RemoveLayerCommand,
            ("Layer", "Duplicate") => viewModel.MainMenu.DuplicateLayerCommand,
            _ => throw new ArgumentOutOfRangeException(nameof(row), $"{top} > {row}")
        };

        private static TextBlockTrimmed TitleBarText(Window window) =>
            window.FindControl<Border>("titleBar")!.GetVisualDescendants().OfType<TextBlockTrimmed>().Single();

        private static string TempProjectPath() => Path.Combine(Path.GetTempPath(), $"cmix-mainmenu-{Guid.NewGuid():N}.cmix");

        [AvaloniaTheory]
        [InlineData("File", "New")]
        [InlineData("File", "Open")]
        [InlineData("File", "Save")]
        [InlineData("File", "Save As")]
        [InlineData("File", "Quit")]
        [InlineData("Edit", "Undo")]
        [InlineData("Edit", "Redo")]
        [InlineData("Compo", "New Composition")]
        [InlineData("Compo", "Delete")]
        [InlineData("Compo", "Duplicate")]
        [InlineData("Layer", "New Layer")]
        [InlineData("Layer", "Delete")]
        [InlineData("Layer", "Duplicate")]
        public void Open_AMainMenuRow_HoldsTheCommandOfItsSource(string top, string row)
        {
            var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();

            var rowItem = OpenRow(window, top, row);

            Assert.Same(ExpectedCommand(viewModel, top, row), rowItem.Command);
        }

        [AvaloniaFact]
        public async Task OpenProjectFromPath_AFile_ShowsItsNameInTheTitleBarAndItsPathInTheToolTip()
        {
            var path = TempProjectPath();
            try
            {
                ProjectFixtures.WriteProjectWithMaterialTextures(path);
                var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow();
                Assert.Equal("CMiX - (Unsaved)", TitleBarText(window).Text);

                await viewModel.ProjectFile.OpenProjectFromPath(path);
                TestServiceProviderFactory.Pump();

                Assert.Equal($"CMiX - {Path.GetFileName(path)}", TitleBarText(window).Text);
                Assert.Equal(path, TitleBarText(window).ToolTipText);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [AvaloniaFact]
        public async Task SaveAsProjectCommand_ANewPath_ShowsItsNameInTheTitleBarAndItsPathInTheToolTip()
        {
            var path = TempProjectPath();
            var newPath = TempProjectPath();
            try
            {
                ProjectFixtures.WriteProjectWithMaterialTextures(path);
                var (_, window, viewModel) = TestServiceProviderFactory.ShowMainWindow(services =>
                    services.AddSingleton<IDialogService>(provider => new DialogService(
                        new FakeSaveFileDialogManager(newPath),
                        viewModelFactory: type => provider.GetService(type))));
                await viewModel.ProjectFile.OpenProjectFromPath(path);
                TestServiceProviderFactory.Pump();

                await ((IAsyncRelayCommand)viewModel.ProjectFile.SaveAsProjectCommand).ExecuteAsync(null);
                TestServiceProviderFactory.Pump();

                Assert.True(File.Exists(newPath));
                Assert.Equal($"CMiX - {Path.GetFileName(newPath)}", TitleBarText(window).Text);
                Assert.Equal(newPath, TitleBarText(window).ToolTipText);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
                if (File.Exists(newPath)) File.Delete(newPath);
            }
        }

        [AvaloniaFact]
        public async Task OpenProjectFromPath_AnApplyFailureAfterAGoodParse_LeavesThePathNullAndShowsTheNotice()
        {
            var path = TempProjectPath();
            try
            {
                ProjectFixtures.WriteProjectWithMaterialTextures(path);
                var (provider, _, viewModel) = TestServiceProviderFactory.ShowMainWindow(services =>
                    services.AddSingleton<ISyncTarget, ThrowingSyncTarget>());
                viewModel.ProjectFile.ProjectPath = "earlier.cmix";

                await viewModel.ProjectFile.OpenProjectFromPath(path);

                var (text, title) = Assert.Single(provider.GetRequiredService<FakeMessageNotice>().Messages);
                Assert.Equal("Open Project", title);
                Assert.Equal($"{Path.GetFileName(path)}: The apply failed.", text);
                Assert.Null(viewModel.ProjectFile.ProjectPath);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        [AvaloniaFact]
        public async Task OpenProjectFromPath_AfterATextureOfTheTexturesTab_RemovesTheTexture()
        {
            var path = TempProjectPath();
            try
            {
                ProjectFixtures.WriteProjectWithMaterialTextures(path);
                var (_, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();
                viewModel.TextureManager.AddItem(typeof(CheckerBoard));
                var texture = Assert.Single(viewModel.TextureManager.ManagerData.Items);
                TestServiceProviderFactory.Pump();
                Assert.Contains(viewModel.ControlRepository.Textures, item => ReferenceEquals(item, texture));

                await viewModel.ProjectFile.OpenProjectFromPath(path);
                TestServiceProviderFactory.Pump();

                Assert.Empty(viewModel.TextureManager.ManagerData.Items);
                Assert.DoesNotContain(viewModel.ControlRepository.Textures, item => ReferenceEquals(item, texture));
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }
    }
}
