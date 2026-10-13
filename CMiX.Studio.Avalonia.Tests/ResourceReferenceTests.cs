using System.Text.RegularExpressions;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // A StaticResource or DynamicResource that names a missing key fails when the view loads.
    // Not every view is loaded by a test, so this test checks the AXAML text.
    public class ResourceReferenceTests
    {
        // Known gaps. AssetManager.axaml uses a style that is only a comment in ListBox.axaml.
        private static readonly HashSet<string> KnownMissing = new() { "ListBoxItemAlternationStyle" };

        private static readonly Regex Comment = new("<!--.*?-->", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex Definition = new(@"x:Key=""([^""{]+)""", RegexOptions.Compiled);
        private static readonly Regex Reference = new(@"\{(?:Static|Dynamic)Resource\s+([A-Za-z0-9_.]+)\s*\}", RegexOptions.Compiled);

        [Fact]
        public void EveryResourceReferenceInTheAxaml_HasADefinition()
        {
            var defined = new HashSet<string>();
            var references = new Dictionary<string, List<string>>();

            foreach (var file in SourceTree.AxamlFiles())
            {
                var text = Comment.Replace(File.ReadAllText(file), "");
                foreach (Match match in Definition.Matches(text))
                    defined.Add(match.Groups[1].Value);

                foreach (Match match in Reference.Matches(text))
                {
                    var key = match.Groups[1].Value;
                    if (!references.TryGetValue(key, out var files))
                        references[key] = files = new List<string>();
                    files.Add(Path.GetFileName(file));
                }
            }

            var missing = references
                .Where(r => !defined.Contains(r.Key) && !KnownMissing.Contains(r.Key))
                .Select(r => $"{r.Key} (used in {string.Join(", ", r.Value.Distinct().Take(3))})")
                .ToList();

            Assert.True(references.Count > 50, $"The scan found only {references.Count} references. The scan is broken.");
            Assert.True(missing.Count == 0, string.Join(Environment.NewLine, missing));
        }
    }
}
