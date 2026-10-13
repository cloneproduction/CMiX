using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CMiX.Studio.Avalonia.Tests
{
    // The AXAML is compiled into the assemblies, so the scanning tests read the files of the source tree.
    internal static class SourceTree
    {
        private static readonly string[] Projects = { "CMiX.Studio.Avalonia", "CMiX.UI" };

        public static string Root()
        {
            for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
            {
                if (File.Exists(Path.Combine(dir.FullName, "CMiX.Studio.Avalonia", "CMiX.Studio.Avalonia.csproj")))
                    return dir.FullName;
            }

            throw new DirectoryNotFoundException("The folder CMiX.Studio.Avalonia was not found above " + AppContext.BaseDirectory);
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
