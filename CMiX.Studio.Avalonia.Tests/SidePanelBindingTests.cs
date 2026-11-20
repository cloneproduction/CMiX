using System.Text.RegularExpressions;
using CMiX.Studio.Avalonia.ViewModels;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // A binding to a name that does not exist fails without a message and the panel stays empty.
    public class SidePanelBindingTests
    {
        private static readonly Regex Comment = new("<!--.*?-->", RegexOptions.Singleline);
        private static readonly Regex Path = new(@"Context\.([A-Za-z]+)");

        [Fact]
        public void Bind_EveryContextPathOfTheSidePanels_NamesAMemberOfMainViewModel()
        {
            var members = typeof(MainViewModel).GetProperties().Select(property => property.Name).ToHashSet();
            var file = SourceTree.AxamlFiles().Single(f => System.IO.Path.GetFileName(f) == "PanelSide.axaml");
            var text = Comment.Replace(File.ReadAllText(file), "");
            var violations = new List<string>();

            var matches = Path.Matches(text);
            foreach (Match match in matches)
            {
                if (!members.Contains(match.Groups[1].Value))
                    violations.Add($"Context.{match.Groups[1].Value} is not a public property of MainViewModel");
            }

            Assert.True(matches.Count >= 2, $"The scan found only {matches.Count} paths. The scan is broken.");
            Assert.True(violations.Count == 0, string.Join(Environment.NewLine, violations));
        }
    }
}
