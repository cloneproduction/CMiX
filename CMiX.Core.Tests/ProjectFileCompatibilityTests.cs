using CMiX.Core.Compositing;
using CMiX.Core.Persistence;
using CMiX.Core.Prefabs;
using CMiX.Core.Undo;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Opens the saved projects of real users. Set CMIX_SAMPLE_PROJECTS to a folder with .cmix files.
    // The test is skipped when the variable is not set, so no user file is stored in the repository.
    public class ProjectFileCompatibilityTests : IDisposable
    {
        private const string FolderVariable = "CMIX_SAMPLE_PROJECTS";

        private readonly TempDirectoryFixture _tempDir = new();

        public void Dispose() => _tempDir.Dispose();

        private static string[] SampleFiles()
        {
            var folder = Environment.GetEnvironmentVariable(FolderVariable);
            return string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder)
                ? Array.Empty<string>()
                : Directory.GetFiles(folder, "*.cmix");
        }

        private static int Open(ProjectModel model) => OpenProject(model).CompositionManager.ManagerData.Items.Count;

        private static Project OpenProject(ProjectModel model)
        {
            var provider = TestServiceProviderFactory.Create();
            var project = provider.GetRequiredService<Project>();
            var repository = provider.GetRequiredService<ControlRepository>();
            var undoManager = provider.GetRequiredService<UndoManager>();

            var compositions = model.CompositionManager.ManagerData.Items
                .Cast<CompositionModel>()
                .Select(composition => ControlCloner.CloneWithNewGuids(composition))
                .ToList();

            undoManager.SuppressUndo();
            try
            {
                project.Open(model, compositions);
            }
            finally
            {
                undoManager.ResumeUndo();
            }

            Assert.Equal(compositions.Count, project.CompositionManager.ManagerData.Items.Count);
            foreach (var composition in project.CompositionManager.ManagerData.Items)
                Assert.Same(composition, repository.GetControl(composition.ID));

            return project;
        }

        [SkippableFact]
        public void EverySampleProject_HashesTheSameTwiceAfterOpening()
        {
            var files = SampleFiles();
            Skip.If(files.Length == 0, $"{FolderVariable} is not set or the folder has no .cmix file.");

            var unstable = files
                .Where(file =>
                {
                    var project = OpenProject(ProjectSerializer.Load(file));
                    return ProjectStateHash.Compute(project) != ProjectStateHash.Compute(project);
                })
                .Select(Path.GetFileName);

            Assert.Empty(unstable);
        }

        [SkippableFact]
        public void EverySampleProject_OpensAndSavesAgain()
        {
            var files = SampleFiles();
            Skip.If(files.Length == 0, $"{FolderVariable} is not set or the folder has no .cmix file.");

            var failures = new List<string>();
            foreach (var file in files)
            {
                try
                {
                    var model = ProjectSerializer.Load(file);
                    var count = Open(model);

                    var copy = Path.Combine(_tempDir.Path, Path.GetFileName(file));
                    ProjectSerializer.Save(model, copy);
                    Assert.Equal(count, Open(ProjectSerializer.Load(copy)));
                }
                catch (Exception exception)
                {
                    failures.Add($"{Path.GetFileName(file)}: {exception.GetType().Name}: {exception.Message}");
                }
            }

            Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
        }
    }
}
