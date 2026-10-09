// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Headless.XUnit;
using CMiX.Core.Assets;
using CMiX.Core.Compositing;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using CMiX.Core.Undo;
using CMiX.Studio.Avalonia.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests.Integration
{
    // File > Open replaces the session with the file it loaded. It used to only add the file's
    // composition to whatever was already open, so opening the same file twice left the first
    // load's compositions, entities, materials and textures in the repository next to the second
    // load's, and every reopen grew the session by another copy.
    public class OpenProjectTests
    {
        // The counts the reported symptom is visible in, plus the layers and the total, which
        // catch anything the per type collections would miss.
        private static (int Compositions, int Layers, int Entities, int Textures, int Controls)
            Counts(ControlRepository repository) =>
            (repository.Compositions.Count, repository.Layers.Count, repository.Entities.Count,
             repository.Textures.Count, repository.Controls.Count);

        [AvaloniaFact]
        public async Task OpenProject_OpenedTwice_ReplacesTheFirstLoadInsteadOfAccumulating()
        {
            var path = Path.Combine(Path.GetTempPath(), $"cmix-openproject-{Guid.NewGuid():N}.cmix");
            try
            {
                ProjectFixtures.WriteProjectWithMaterialTextures(path);

                var (_, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();
                var repository = viewModel.ControlRepository;

                await viewModel.MainMenu.OpenProjectFromPath(path);
                TestServiceProviderFactory.Pump();

                var afterFirstLoad = Counts(repository);
                Assert.Equal((1, 1, 1, 2, 5), afterFirstLoad);
                // Held by reference rather than by count, so the second load has to be shown to
                // have released these very instances and not merely to hold the same number.
                var firstLoadControls = repository.Controls.ToList();

                await viewModel.MainMenu.OpenProjectFromPath(path);
                TestServiceProviderFactory.Pump();

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
                ProjectFixtures.WriteProjectWithMaterialTextures(path);
                File.WriteAllText(corruptPath, "{ this is not a project");

                var (_, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();
                var repository = viewModel.ControlRepository;

                await viewModel.MainMenu.OpenProjectFromPath(path);
                TestServiceProviderFactory.Pump();

                var afterFirstLoad = Counts(repository);
                var loadedControls = repository.Controls.ToList();

                // The failure is reported through a message box, and the headless host has no
                // desktop lifetime for the dialog manager to fall back to for an owner window, so
                // the report itself throws here. It throws after the point the session would have
                // been swept, which is what the assertions below are about.
                await Assert.ThrowsAsync<ArgumentException>(
                    () => viewModel.MainMenu.OpenProjectFromPath(corruptPath));
                TestServiceProviderFactory.Pump();

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
                ProjectFixtures.WriteProjectWithMaterialTextures(path);

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
            var (provider, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            var undoManager = provider.GetRequiredService<UndoManager>();
            var compositionManager = viewModel.Project.CompositionManager;
            Composition? discarded = null;

            if (withEditsBeforeTheOpen)
            {
                compositionManager.AddItem(typeof(Composition));
                discarded = (Composition)compositionManager.SelectedItem;
                compositionManager.DeleteItem(discarded);
                TestServiceProviderFactory.Pump();
                Assert.True(undoManager.CanUndo);
            }

            await viewModel.MainMenu.OpenProjectFromPath(path);
            TestServiceProviderFactory.Pump();

            if (discarded != null)
                Assert.DoesNotContain(viewModel.ControlRepository.Compositions,
                    item => ReferenceEquals(item, discarded));

            // Bounded by more than the stack's own cap, so a command that fails to pop cannot spin.
            var steps = 0;
            for (; steps < 256 && undoManager.CanUndo; steps++)
                viewModel.MainMenu.UndoCommand.Execute(null);
            TestServiceProviderFactory.Pump();

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
                ProjectFixtures.WriteProjectWithMaterialTextures(path);

                var (provider, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();
                var undoManager = provider.GetRequiredService<UndoManager>();

                await viewModel.MainMenu.OpenProjectFromPath(path);
                TestServiceProviderFactory.Pump();

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
                ProjectFixtures.WriteProjectWithMaterialTextures(path);

                var (provider, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();
                var undoManager = provider.GetRequiredService<UndoManager>();
                var repository = viewModel.ControlRepository;

                await viewModel.MainMenu.OpenProjectFromPath(path);
                TestServiceProviderFactory.Pump();

                var loaded = repository.Controls.ToList();

                viewModel.Project.CompositionManager.AddItem(typeof(Composition));
                var added = (Composition)viewModel.Project.CompositionManager.SelectedItem;
                TestServiceProviderFactory.Pump();

                Assert.True(undoManager.CanUndo);

                var steps = 0;
                for (; steps < 256 && undoManager.CanUndo; steps++)
                    viewModel.MainMenu.UndoCommand.Execute(null);
                TestServiceProviderFactory.Pump();

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
                ProjectFixtures.WriteProjectWithMaterialTextures(path);

                var (_, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();

                await viewModel.MainMenu.OpenProjectFromPath(path);
                TestServiceProviderFactory.Pump();
                await viewModel.MainMenu.OpenProjectFromPath(path);
                TestServiceProviderFactory.Pump();

                viewModel.MainMenu.NewProjectCommand.Execute(null);
                TestServiceProviderFactory.Pump();

                Assert.Equal((0, 0, 0, 0, 0), Counts(viewModel.ControlRepository));
                Assert.Null(viewModel.MainMenu.FolderPath);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        // An open loads every composition of the file in file order. Each one keeps its own layer
        // and gets new ids of its own. The composition that was selected at save time is selected
        // again.
        [AvaloniaFact]
        public async Task OpenProject_WithThreeCompositions_OpensEachOneAndRestoresTheSavedSelection()
        {
            var path = Path.Combine(Path.GetTempPath(), $"cmix-openproject-{Guid.NewGuid():N}.cmix");
            try
            {
                ProjectFixtures.WriteProjectWithThreeCompositions(path);

                var saved = ProjectSerializer.Load(path).CompositionManager.ManagerData;
                Assert.Equal(1, saved.SelectedIndex);
                var savedIDs = saved.Items.Select(item => item.ID).ToList();
                Assert.Equal(3, savedIDs.Count);

                var (_, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();
                var compositionManager = viewModel.Project.CompositionManager;

                await viewModel.MainMenu.OpenProjectFromPath(path);
                TestServiceProviderFactory.Pump();

                var compositions = compositionManager.ManagerData.Items.Cast<Composition>().ToList();
                Assert.Equal(3, compositions.Count);

                var suffixes = new[] { "A", "B", "C" };
                for (var i = 0; i < suffixes.Length; i++)
                {
                    Assert.Equal("Composition" + suffixes[i], compositions[i].PrefabService.Name.Value);
                    var layer = (Layer)Assert.Single(compositions[i].LayerManager.ManagerData.Items);
                    Assert.Equal("Layer" + suffixes[i], layer.PrefabService.Name.Value);
                }

                // Each composition goes through its own clone, so no two share an id and none
                // keeps the id it has in the file.
                var loadedIDs = compositions.Select(composition => composition.ID).ToList();
                Assert.DoesNotContain(Guid.Empty, loadedIDs);
                Assert.Equal(3, loadedIDs.Distinct().Count());
                Assert.Empty(loadedIDs.Intersect(savedIDs));

                Assert.Same(compositions[1], compositionManager.SelectedItem);
                Assert.Equal(1, compositionManager.ManagerData.SelectedIndex);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        // The project name and the project model are not part of a composition. An open restores
        // them from the file too.
        [AvaloniaFact]
        public async Task OpenProject_RestoresTheProjectNameAndTheProjectModel()
        {
            const string projectName = "Saved Project";
            var path = Path.Combine(Path.GetTempPath(), $"cmix-openproject-{Guid.NewGuid():N}.cmix");
            var modelPath = Path.Combine(Path.GetTempPath(), $"cmix-model-{Guid.NewGuid():N}.obj");
            try
            {
                // The model selector makes an asset only for a file that exists.
                File.WriteAllText(modelPath, string.Empty);
                ProjectFixtures.WriteProjectWithNameAndModel(path, projectName, modelPath);

                var (_, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();
                var project = viewModel.Project;
                Assert.NotEqual(projectName, project.PrefabService.Name.Value);
                Assert.NotEqual(modelPath, project.Model.FilePath.Value);
                Assert.Null(project.Model.Asset);

                await viewModel.MainMenu.OpenProjectFromPath(path);
                TestServiceProviderFactory.Pump();

                Assert.Equal(projectName, project.PrefabService.Name.Value);
                Assert.Equal(modelPath, project.Model.FilePath.Value);
                var geometry = Assert.IsType<Geometry>(project.Model.Asset);
                Assert.Equal(modelPath, geometry.FilePath);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
                if (File.Exists(modelPath)) File.Delete(modelPath);
            }
        }

        // SelectedOutputMappingID is a pointer into the project's fixed output slots, not an
        // identity, but every composition goes through a clone that gives every id a fresh value.
        // A composition's saved, valid selection must survive that clone, not fall back to the
        // default slot the way a genuinely stale or corrupt reference correctly still does.
        [AvaloniaFact]
        public async Task OpenProject_WithDifferentOutputMappingsPerComposition_KeepsEachOnesSelection()
        {
            var path = Path.Combine(Path.GetTempPath(), $"cmix-openproject-{Guid.NewGuid():N}.cmix");
            try
            {
                ProjectFixtures.WriteProjectWithDifferentOutputMappings(path);

                var (_, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();
                var slots = viewModel.Project.OutputMappingManager.Items;

                await viewModel.MainMenu.OpenProjectFromPath(path);
                TestServiceProviderFactory.Pump();

                var compositions = viewModel.Project.CompositionManager.ManagerData.Items
                    .Cast<Composition>().ToList();
                Assert.Equal(2, compositions.Count);
                Assert.Same(slots[1], compositions[0].SelectedOutputMapping);
                Assert.Same(slots[2], compositions[1].SelectedOutputMapping);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
            }
        }

        // The same clone runs for Duplicate Composition, entirely inside one live session, no save
        // or load involved.
        [AvaloniaFact]
        public void DuplicateComposition_WithANonDefaultOutputMapping_KeepsItOnTheDuplicate()
        {
            var (_, _, viewModel) = TestServiceProviderFactory.ShowMainWindow();
            var compositionManager = viewModel.Project.CompositionManager;
            var slots = viewModel.Project.OutputMappingManager.Items;

            compositionManager.AddItem(typeof(Composition));
            var original = (Composition)compositionManager.SelectedItem;
            original.SelectedOutputMapping = slots[1];
            TestServiceProviderFactory.Pump();

            viewModel.MainMenu.DuplicateCompositionCommand.Execute(null);
            TestServiceProviderFactory.Pump();

            var duplicate = (Composition)compositionManager.SelectedItem;
            Assert.NotSame(original, duplicate);
            Assert.Same(slots[1], duplicate.SelectedOutputMapping);
        }
    }
}
