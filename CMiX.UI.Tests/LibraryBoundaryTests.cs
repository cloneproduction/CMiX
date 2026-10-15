using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using CMiX.UI;
using CMiX.UI.Themes;
using Xunit;

namespace CMiX.UI.Tests
{
    // CMiX.UI is the generic part of the UI. It must not know the model or the Studio.
    public class LibraryBoundaryTests
    {
        private static readonly string[] Forbidden = { "CMiX.Core", "CMiX.Studio" };

        // The compiler writes the path of this file into the test assembly, so the test finds the source
        // files from any output folder.
        private static string ThisFile([CallerFilePath] string path = "") => path;

        private static string LibraryFolder()
        {
            // This file is in CMiX.UI.Tests, so the folder above it holds all the projects.
            var folder = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(ThisFile())!, "..", "CMiX.UI"));
            if (!File.Exists(Path.Combine(folder, "CMiX.UI.csproj")))
                throw new DirectoryNotFoundException("The folder CMiX.UI was not found next to " + ThisFile());

            return folder;
        }

        [Fact]
        public void TheAssembly_ReferencesNoCoreOrStudioAssembly()
        {
            var references = typeof(CMiXTheme).Assembly.GetReferencedAssemblies().Select(a => a.Name!).ToList();

            Assert.True(references.Count > 5, "The reference list is empty. The check is broken.");
            Assert.DoesNotContain(references, name => Forbidden.Any(f => name!.StartsWith(f)));
        }

        [Fact]
        public void TheProject_HasNoProjectReference()
        {
            var project = File.ReadAllText(Path.Combine(LibraryFolder(), "CMiX.UI.csproj"));

            Assert.DoesNotContain("ProjectReference", project);
        }

        [Fact]
        public void TheSourceFiles_NameNoCoreOrStudioNamespace()
        {
            var folder = LibraryFolder();
            var files = Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories)
                .Where(f => (f.EndsWith(".cs") || f.EndsWith(".axaml"))
                         && !f.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar)
                         && !f.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar))
                .ToList();
            var hit = new Regex(@"(clr-namespace:|using:|avares://|using\s+)(CMiX\.Core|CMiX\.Studio)");

            var violations = files.Where(f => hit.IsMatch(File.ReadAllText(f)))
                .Select(f => Path.GetRelativePath(folder, f)).ToList();

            Assert.True(files.Count > 50, $"The scan found only {files.Count} files. The scan is broken.");
            Assert.True(violations.Count == 0, string.Join(Environment.NewLine, violations));
        }
    }
}
