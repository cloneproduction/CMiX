using System.Text.RegularExpressions;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // A number editor in a view must bind its Value. Without it the box does not follow the model.
    public class NumberEditorBindingTests
    {
        private static readonly Regex Editor = new(@"<ui:(DragValue|IntegerValue)(?![.\w])(?<attributes>(?:""[^""]*""|[^>""])*)>", RegexOptions.Compiled);
        private static readonly Regex Comment = new("<!--.*?-->", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex ValueBinding = new(@"\bValue\s*=\s*""\{", RegexOptions.Compiled);

        [Fact]
        public void EveryDragValueAndIntegerValueInTheAxaml_BindsItsValue()
        {
            var found = 0;
            var violations = new List<string>();

            foreach (var file in SourceTree.AxamlFiles())
            {
                var text = Comment.Replace(File.ReadAllText(file), "");
                foreach (Match match in Editor.Matches(text))
                {
                    found++;
                    if (!ValueBinding.IsMatch(match.Groups["attributes"].Value))
                        violations.Add($"{SourceTree.Relative(file)}: {match.Groups[1].Value} has no Value binding");
                }
            }

            Assert.True(found >= 20, $"The scan found only {found} editors. The scan is broken.");
            Assert.True(violations.Count == 0, string.Join(Environment.NewLine, violations));
        }
    }
}
