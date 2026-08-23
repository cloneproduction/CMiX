// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using CMiX.Core.Compositing;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // File > Open replaces the session with the file it loaded. It used to only add the file's
    // composition to whatever was already open, so opening the same file twice left the first
    // load's compositions, entities, materials and textures in the repository next to the second
    // load's, and every reopen grew the session by another copy.
    public class OpenProjectTests
    {
        // Hands back the provider as well, so a test that needs a service the main view model does
        // not expose, the undo manager, can resolve the very instance this session was built with.
        private static (IServiceProvider provider, MainViewModel viewModel) ShowMainWindow()
        {
            var provider = TestServiceProviderFactory.Create();
            var window = TestServiceProviderFactory.CreateMainWindow(provider);
            window.Show();
            Dispatcher.UIThread.RunJobs();
            return (provider, provider.GetRequiredService<MainViewModel>());
        }

        private static void Pump()
        {
            Dispatcher.UIThread.RunJobs();
            Dispatcher.UIThread.RunJobs();
        }

        // The four counts the reported symptom is visible in, plus the layers and the total, which
        // catch anything the per type collections would miss.
        private static (int Compositions, int Layers, int Entities, int Materials, int Textures, int Controls)
            Counts(ControlRepository repository) =>
            (repository.Compositions.Count, repository.Layers.Count, repository.Entities.Count,
             repository.Materials.Count, repository.Textures.Count, repository.Controls.Count);

        [AvaloniaFact]
        public async Task OpenProject_OpenedTwice_ReplacesTheFirstLoadInsteadOfAccumulating()
        {
            var path = Path.Combine(Path.GetTempPath(), $"cmix-openproject-{Guid.NewGuid():N}.cmix");
            try
            {
                WriteProjectWithMaterialTextures(path);

                var (_, viewModel) = ShowMainWindow();
                var repository = viewModel.ControlRepository;

                await viewModel.MainMenu.OpenProjectFromPath(path);
                Pump();

                var afterFirstLoad = Counts(repository);
                Assert.Equal((1, 1, 1, 1, 2, 6), afterFirstLoad);
                // Held by reference rather than by count, so the second load has to be shown to
                // have released these very instances and not merely to hold the same number.
                var firstLoadControls = repository.Controls.ToList();

                await viewModel.MainMenu.OpenProjectFromPath(path);
                Pump();

                Assert.Equal(afterFirstLoad, Counts(repository));
                foreach (var control in firstLoadControls)
                    Assert.DoesNotContain(repository.Controls, item => ReferenceEquals(item, control));

                Assert.Equal(path, viewModel.MainMenu.FolderPath);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        // The session is only swept once the file has been parsed and has yielded a composition, so
        // a file that cannot be read leaves what is open untouched instead of emptying the session
        // and then reporting the failure over an empty window.
        [AvaloniaFact]
        public async Task OpenProject_OnAFileThatFailsToParse_LeavesTheOpenSessionIntact()
        {
            var path = Path.Combine(Path.GetTempPath(), $"cmix-openproject-{Guid.NewGuid():N}.cmix");
            var corruptPath = Path.Combine(Path.GetTempPath(), $"cmix-corrupt-{Guid.NewGuid():N}.cmix");
            try
            {
                WriteProjectWithMaterialTextures(path);
                File.WriteAllText(corruptPath, "{ this is not a project");

                var (_, viewModel) = ShowMainWindow();
                var repository = viewModel.ControlRepository;

                await viewModel.MainMenu.OpenProjectFromPath(path);
                Pump();

                var afterFirstLoad = Counts(repository);
                var loadedControls = repository.Controls.ToList();

                // The failure is reported through a message box, and the headless host has no
                // desktop lifetime for the dialog manager to fall back to for an owner window, so
                // the report itself throws here. It throws after the point the session would have
                // been swept, which is what the assertions below are about.
                await Assert.ThrowsAsync<ArgumentException>(
                    () => viewModel.MainMenu.OpenProjectFromPath(corruptPath));
                Pump();

                Assert.Equal(afterFirstLoad, Counts(repository));
                foreach (var control in loadedControls)
                    Assert.Contains(repository.Controls, item => ReferenceEquals(item, control));

                Assert.Equal(path, viewModel.MainMenu.FolderPath);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
                if (File.Exists(corruptPath)) File.Delete(corruptPath);
            }
        }

        // The undo stack of the replaced session refers to instances the open released, so it goes
        // down with the session; left standing, an undo of a delete recorded before the open would
        // reinsert a control of the old session into the new one. What is measured is that an open
        // leaves the same number of steps behind whether or not the session it replaced had a
        // history of its own, which since the load stopped recording is none either way.
        [AvaloniaFact]
        public async Task OpenProject_DropsTheUndoHistoryOfTheSessionItReplaced()
        {
            var path = Path.Combine(Path.GetTempPath(), $"cmix-openproject-{Guid.NewGuid():N}.cmix");
            try
            {
                WriteProjectWithMaterialTextures(path);

                var fromAnUntouchedSession = await CountUndoStepsLeftByAnOpen(path, withEditsBeforeTheOpen: false);
                var fromAnEditedSession = await CountUndoStepsLeftByAnOpen(path, withEditsBeforeTheOpen: true);

                Assert.Equal(fromAnUntouchedSession, fromAnEditedSession);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        // Opens the file in a fresh session, optionally after an add and a delete that leave two
        // entries on the stack, and reports how many undo steps the open left behind.
        private static async Task<int> CountUndoStepsLeftByAnOpen(string path, bool withEditsBeforeTheOpen)
        {
            var (provider, viewModel) = ShowMainWindow();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var compositionManager = viewModel.Project.CompositionManager;
            Composition? discarded = null;

            if (withEditsBeforeTheOpen)
            {
                compositionManager.AddItem(typeof(Composition));
                discarded = (Composition)compositionManager.SelectedItem;
                compositionManager.DeleteItem(discarded);
                Pump();
                Assert.True(undoManager.CanUndo);
            }

            await viewModel.MainMenu.OpenProjectFromPath(path);
            Pump();

            if (discarded != null)
                Assert.DoesNotContain(viewModel.ControlRepository.Compositions,
                    item => ReferenceEquals(item, discarded));

            // Bounded by more than the stack's own cap, so a command that fails to pop cannot spin.
            var steps = 0;
            for (; steps < 256 && undoManager.CanUndo; steps++)
                viewModel.MainMenu.UndoCommand.Execute(null);
            Pump();

            Assert.False(undoManager.CanUndo);
            return steps;
        }

        // The load walks the same managers a user action walks, so building the graph used to leave
        // the whole construction on the stack the reset had just emptied, some three dozen add and
        // select entries deep. A Ctrl+Z on a freshly opened file stepped backwards into the load and
        // started taking the file apart instead of doing nothing.
        [AvaloniaFact]
        public async Task OpenProject_LeavesNothingOnTheUndoStack()
        {
            var path = Path.Combine(Path.GetTempPath(), $"cmix-openproject-{Guid.NewGuid():N}.cmix");
            try
            {
                WriteProjectWithMaterialTextures(path);

                var (provider, viewModel) = ShowMainWindow();
                var undoManager = provider.GetRequiredService<UndoManager>();

                await viewModel.MainMenu.OpenProjectFromPath(path);
                Pump();

                Assert.False(undoManager.CanUndo);
                Assert.False(undoManager.CanRedo);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        // Recording is only off for the duration of the load, so the first thing the user does after
        // an open records the one step it should and undoes it without touching what was loaded.
        [AvaloniaFact]
        public async Task OpenProject_ThenAUserAction_RecordsOneStepAndUndoesItCleanly()
        {
            var path = Path.Combine(Path.GetTempPath(), $"cmix-openproject-{Guid.NewGuid():N}.cmix");
            try
            {
                WriteProjectWithMaterialTextures(path);

                var (provider, viewModel) = ShowMainWindow();
                var undoManager = provider.GetRequiredService<UndoManager>();
                var repository = viewModel.ControlRepository;

                await viewModel.MainMenu.OpenProjectFromPath(path);
                Pump();

                var loaded = repository.Controls.ToList();

                viewModel.Project.CompositionManager.AddItem(typeof(Composition));
                var added = (Composition)viewModel.Project.CompositionManager.SelectedItem;
                Pump();

                Assert.True(undoManager.CanUndo);

                var steps = 0;
                for (; steps < 256 && undoManager.CanUndo; steps++)
                    viewModel.MainMenu.UndoCommand.Execute(null);
                Pump();

                Assert.Equal(1, steps);
                Assert.DoesNotContain(repository.Compositions, item => ReferenceEquals(item, added));
                foreach (var control in loaded)
                    Assert.Contains(repository.Controls, item => ReferenceEquals(item, control));
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        // File > New still has to empty a session that came from an open rather than from the
        // managers, which is the case the reset the open path now shares with it has to keep working.
        [AvaloniaFact]
        public async Task NewProject_AfterAnOpen_StillClearsEverything()
        {
            var path = Path.Combine(Path.GetTempPath(), $"cmix-openproject-{Guid.NewGuid():N}.cmix");
            try
            {
                WriteProjectWithMaterialTextures(path);

                var (_, viewModel) = ShowMainWindow();

                await viewModel.MainMenu.OpenProjectFromPath(path);
                Pump();
                await viewModel.MainMenu.OpenProjectFromPath(path);
                Pump();

                viewModel.MainMenu.NewProjectCommand.Execute(null);
                Pump();

                Assert.Equal((0, 0, 0, 0, 0, 0), Counts(viewModel.ControlRepository));
                Assert.Null(viewModel.MainMenu.FolderPath);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        // Same fixture NewProjectTests authors, built through the managers the app uses so it round
        // trips the real model shape: a composition holding a layer, holding an entity, whose
        // material selector holds a material with a texture in each of its two slots.
        private static void WriteProjectWithMaterialTextures(string path)
        {
            var (_, viewModel) = ShowMainWindow();

            viewModel.Project.CompositionManager.AddItem(typeof(Composition));
            var composition = (Composition)viewModel.Project.CompositionManager.SelectedItem;
            composition.LayerManager.AddItem(typeof(Layer));
            var layer = (Layer)composition.LayerManager.SelectedItem;
            layer.ModelEntityManager.AddItem(typeof(Entity));
            var entity = (Entity)layer.ModelEntityManager.SelectedItem;
            entity.MaterialSelector.AddItemCommand.Execute(typeof(CMiX.Core.Materials.Material));
            var material = (CMiX.Core.Materials.Material)entity.MaterialSelector.SelectedItem;
            material.DiffuseTexture.TextureManager.AddItem(typeof(CheckerBoard));
            material.MaskTexture.TextureManager.AddItem(typeof(BubbleNoise));
            Pump();

            ProjectSerializer.Save(ProjectModelBuilder.Build(composition, viewModel.Project.MasterBeat, viewModel.Project.OutputMappingManager), path);
        }
    }
}
