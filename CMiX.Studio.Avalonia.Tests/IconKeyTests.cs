using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia.Headless.XUnit;
using CMiX.Studio.Avalonia.Themes;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // An AppIcon with an unknown key throws when the key is set. This test finds a wrong key
    // in the AXAML before a screen shows it. The App registers the Studio keys in Initialize.
    public class IconKeyTests
    {
        private static readonly Regex IconKey = new(@"IconKey=""([A-Za-z0-9_]+)""", RegexOptions.Compiled);

        private static string StudioFolder()
        {
            for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
            {
                var studio = Path.Combine(dir.FullName, "CMiX.Studio.Avalonia");
                if (File.Exists(Path.Combine(studio, "CMiX.Studio.Avalonia.csproj")))
                    return studio;
            }

            throw new DirectoryNotFoundException("The folder CMiX.Studio.Avalonia was not found above " + AppContext.BaseDirectory);
        }

        [AvaloniaFact]
        public void EveryIconKeyInTheAxaml_IsRegistered()
        {
            var studio = StudioFolder();
            var found = 0;
            var unknown = new List<string>();

            foreach (var file in Directory.EnumerateFiles(studio, "*.axaml", SearchOption.AllDirectories)
                         .Where(f => !f.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar)
                                  && !f.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar)))
            {
                foreach (Match match in IconKey.Matches(File.ReadAllText(file)))
                {
                    found++;
                    var key = match.Groups[1].Value;
                    if (!Icons.Kinds.ContainsKey(key))
                        unknown.Add($"{Path.GetRelativePath(studio, file)}: {key}");
                }
            }

            Assert.True(found >= 10, $"The scan found only {found} icon keys. The scan is broken.");
            Assert.True(unknown.Count == 0, string.Join(Environment.NewLine, unknown));
        }

        [Fact]
        public void Register_AcceptsTheSameKindAgain_AndRejectsAnotherKind()
        {
            var kind = Icons.Kinds["Add"];

            Icons.Register("Add", kind);

            Assert.Throws<InvalidOperationException>(() => Icons.Register("Add", Icons.Kinds["Sub"]));
        }
    }
}
