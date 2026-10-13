using System.Text.RegularExpressions;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The panels of the library folder Collection get their data only through their own properties.
    // A binding that reads a name from the DataContext would tie the panel to one model, and no
    // compiler would see it. So every binding in these files must point to the control itself,
    // to its template parent, or to a named element.
    public class CollectionPanelContractTests
    {
        private static readonly Regex Comment = new("<!--.*?-->", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex Binding = new(@"\{Binding\b([^}]*)\}", RegexOptions.Compiled);
        private static readonly Regex KnownSource = new(@"ElementName\s*=|RelativeSource|\$parent|\$self|#\w|Source\s*=", RegexOptions.Compiled);

        private static IEnumerable<string> PanelFiles() =>
            SourceTree.AxamlFiles().Where(f =>
            {
                var path = f.Replace('\\', '/');
                return path.Contains("/CMiX.UI/Controls/Collection/")
                    || path.EndsWith("/CMiX.UI/Themes/Styles/PanelItem.axaml")
                    || path.EndsWith("/CMiX.UI/Themes/Styles/PanelItemList.axaml");
            });

        [Fact]
        public void NoBindingOfACollectionPanel_ReadsTheDataContext()
        {
            var files = PanelFiles().ToList();
            var checkedCount = 0;
            var wrong = new List<string>();

            foreach (var file in files)
            {
                var text = Comment.Replace(File.ReadAllText(file), "");
                foreach (Match match in Binding.Matches(text))
                {
                    checkedCount++;
                    if (!KnownSource.IsMatch(match.Groups[1].Value))
                        wrong.Add($"{match.Value.Trim()} in {SourceTree.Relative(file)}");
                }
            }

            Assert.True(files.Count >= 4, $"The scan found only {files.Count} files. The scan is broken.");
            Assert.True(checkedCount > 10, $"The scan found only {checkedCount} bindings. The scan is broken.");
            Assert.True(wrong.Count == 0, string.Join(Environment.NewLine, wrong));
        }
    }
}
