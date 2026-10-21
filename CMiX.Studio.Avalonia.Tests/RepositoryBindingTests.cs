using System.Text.RegularExpressions;
using CMiX.Core.Prefabs;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // A binding to a name that does not exist fails without a message and the list stays empty.
    public class RepositoryBindingTests
    {
        private static readonly Regex Comment = new("<!--.*?-->", RegexOptions.Singleline);
        private static readonly Regex Path = new(@"ControlRepository\.([A-Za-z]+)");

        [Fact]
        public void EveryBindingToTheRepository_NamesAMemberThatExists()
        {
            var members = typeof(ControlRepository).GetMembers().Select(member => member.Name).ToHashSet();
            var violations = new List<string>();
            var pathCount = 0;

            foreach (var file in SourceTree.AxamlFiles())
            {
                var text = Comment.Replace(File.ReadAllText(file), "");

                foreach (Match match in Path.Matches(text))
                {
                    pathCount++;
                    if (!members.Contains(match.Groups[1].Value))
                        violations.Add($"{SourceTree.Relative(file)}: ControlRepository.{match.Groups[1].Value} does not exist");
                }
            }

            Assert.True(pathCount >= 10, $"The scan found only {pathCount} paths. The scan is broken.");
            Assert.True(violations.Count == 0, string.Join(Environment.NewLine, violations));
        }
    }
}
