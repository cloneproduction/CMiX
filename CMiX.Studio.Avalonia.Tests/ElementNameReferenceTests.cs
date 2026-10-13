using System.Text.RegularExpressions;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // A binding to an element name that does not exist fails without an error when the view runs.
    // Not every view is loaded by a test, so this test checks the AXAML text.
    public class ElementNameReferenceTests
    {
        private static readonly Regex Comment = new("<!--.*?-->", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex Name = new(@"\bName=""([^""]+)""", RegexOptions.Compiled);
        private static readonly Regex Reference = new(
            @"ElementName\s*=\s*([A-Za-z_]\w*)|\{Binding\s+#([A-Za-z_]\w*)|\{x:Reference\s+([A-Za-z_]\w*)",
            RegexOptions.Compiled);

        [Fact]
        public void EveryElementNameInTheAxaml_PointsToANameOfTheSameFile()
        {
            var checkedCount = 0;
            var missing = new List<string>();

            foreach (var file in SourceTree.AxamlFiles())
            {
                var text = Comment.Replace(File.ReadAllText(file), "");
                var names = Name.Matches(text).Select(m => m.Groups[1].Value).ToHashSet();

                foreach (Match match in Reference.Matches(text))
                {
                    var name = match.Groups.Cast<Group>().Skip(1).First(g => g.Success).Value;
                    checkedCount++;
                    if (!names.Contains(name))
                        missing.Add($"{name} (in {SourceTree.Relative(file)})");
                }
            }

            Assert.True(checkedCount > 100, $"The scan found only {checkedCount} references. The scan is broken.");
            Assert.True(missing.Count == 0, string.Join(Environment.NewLine, missing));
        }
    }
}
