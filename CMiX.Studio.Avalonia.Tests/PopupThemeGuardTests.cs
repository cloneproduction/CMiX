using System.Text.RegularExpressions;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // Every menu and flyout in the AXAML must use a shared popup theme. A new popup must not get its own look.
    public class PopupThemeGuardTests
    {
        private static readonly Regex Comment = new("<!--.*?-->", RegexOptions.Singleline);

        // Opening tags only. The lookahead skips property elements such as ContextMenu.Items.
        private static readonly Regex Tag = new(@"<(ContextMenu|Flyout|MenuFlyout)(?![.\w])((?:""[^""]*""|[^>""])*)>");

        private static readonly Regex FlyoutPresenterTheme = new(@"FlyoutPresenterTheme\s*=\s*""[^""]*(PopupFlyoutStyle|MenuFlyoutPresenterDefault)[^""]*""");

        // The AXAML is compiled into the assembly, so the test reads the files of the source tree.
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

        [Fact]
        public void EveryContextMenuAndFlyoutInTheAxaml_UsesASharedPopupTheme()
        {
            var studio = StudioFolder();
            var violations = new List<string>();
            var tagCount = 0;

            foreach (var file in Directory.EnumerateFiles(studio, "*.axaml", SearchOption.AllDirectories)
                         .Where(f => !f.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar)
                                  && !f.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar)))
            {
                var text = Comment.Replace(File.ReadAllText(file), "");

                foreach (Match match in Tag.Matches(text))
                {
                    tagCount++;
                    var name = match.Groups[1].Value;
                    var attributes = match.Groups[2].Value;

                    var shared = name == "ContextMenu"
                        ? attributes.Contains("ContextMenuDefault")
                        : FlyoutPresenterTheme.IsMatch(attributes);
                    if (!shared)
                        violations.Add($"{Path.GetRelativePath(studio, file)}: {name} has no shared popup theme");
                }
            }

            Assert.True(tagCount >= 8, $"The scan found only {tagCount} tags. The scan is broken.");
            Assert.True(violations.Count == 0, string.Join(Environment.NewLine, violations));
        }
    }
}
