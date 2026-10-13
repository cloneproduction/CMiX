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

        private static readonly Regex FlyoutPresenterTheme = new(@"FlyoutPresenterTheme\s*=\s*""[^""]*(FlyoutPresenterPanel|MenuFlyoutPresenterDefault)[^""]*""");

        [Fact]
        public void EveryContextMenuAndFlyoutInTheAxaml_UsesASharedPopupTheme()
        {
            var violations = new List<string>();
            var tagCount = 0;

            foreach (var file in SourceTree.AxamlFiles())
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
                        violations.Add($"{SourceTree.Relative(file)}: {name} has no shared popup theme");
                }
            }

            Assert.True(tagCount >= 8, $"The scan found only {tagCount} tags. The scan is broken.");
            Assert.True(violations.Count == 0, string.Join(Environment.NewLine, violations));
        }
    }
}
