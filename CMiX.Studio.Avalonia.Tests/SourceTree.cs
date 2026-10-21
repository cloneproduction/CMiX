using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CMiX.Studio.Avalonia.Tests
{
    // The AXAML is compiled into the assemblies, so the scanning tests read the files of the source tree.
    internal static class SourceTree
    {
        private static readonly string[] Projects = { "CMiX.Studio.Avalonia", "CMiX.UI" };

        // The compiler writes the path of this file into the test assembly. The tests then find the source files
        // from any output folder, not only from a folder under the project.
        private static string ThisFile([CallerFilePath] string path = "") => path;

        public static string Root()
        {
            // This file is in CMiX.Studio.Avalonia.Tests, so the folder above it holds all the projects.
            var root = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(ThisFile())!, ".."));
            if (!File.Exists(Path.Combine(root, "CMiX.Studio.Avalonia", "CMiX.Studio.Avalonia.csproj")))
                throw new DirectoryNotFoundException("The folder CMiX.Studio.Avalonia was not found above " + ThisFile());

            return root;
        }

        // Every AXAML file of the Studio and of CMiX.UI, without the build folders.
        public static IEnumerable<string> AxamlFiles()
        {
            var root = Root();
            return Projects
                .SelectMany(project => Directory.EnumerateFiles(Path.Combine(root, project), "*.axaml", SearchOption.AllDirectories))
                .Where(f => !f.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar)
                         && !f.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar));
        }

        public static string Relative(string file) => Path.GetRelativePath(Root(), file);
    }
}
