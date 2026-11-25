using System.Text.RegularExpressions;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The DataContext of a value editor is the value it edits, so a drag belongs to that value.
    public class EditorDataContextTests
    {
        private static readonly Regex Comment = new("<!--.*?-->", RegexOptions.Singleline);
        private static readonly Regex Editor = new(@"<ui:(?:DragValue|DragValueInteger|SliderCMiX)\b[^>]*>", RegexOptions.Singleline);
        private static readonly Regex ValueBinding = new(@"\sValue=""\{Binding ([^,}\s]+)");

        [Fact]
        public void ValueEditors_InEveryView_BindValueWithoutADot()
        {
            var violations = new List<string>();
            var editorCount = 0;

            var views = Path.Combine("CMiX.Studio.Avalonia", "Views") + Path.DirectorySeparatorChar;

            foreach (var file in SourceTree.AxamlFiles().Where(f => f.Contains(views)))
            {
                var text = Comment.Replace(File.ReadAllText(file), "");

                foreach (Match tag in Editor.Matches(text))
                {
                    var binding = ValueBinding.Match(tag.Value);
                    if (!binding.Success) continue;

                    editorCount++;
                    var path = binding.Groups[1].Value;

                    // The BPM of the master beat is a number that the MasterBeat computes from its period.
                    if (file.EndsWith("PanelMasterBeat.axaml") && path.EndsWith(".BPM")) continue;

                    if (path.Contains('.') || path.StartsWith("$"))
                        violations.Add($"{SourceTree.Relative(file)}: Value binds {path}");
                }
            }

            Assert.True(editorCount >= 20, $"The scan found only {editorCount} editors. The scan is broken.");
            Assert.True(violations.Count == 0, string.Join(Environment.NewLine, violations));
        }
    }
}
