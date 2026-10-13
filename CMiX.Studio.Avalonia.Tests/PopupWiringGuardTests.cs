using System.Text.RegularExpressions;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // Every Popup in the AXAML is opened by a toggle button through TogglePopup.Toggle. A new popup must not wire itself by hand.
    public class PopupWiringGuardTests
    {
        private static readonly Regex Comment = new("<!--.*?-->", RegexOptions.Singleline);

        // Opening tags of Popup only. The lookahead skips property elements such as Popup.Child and names such as PopupButton.
        private static readonly Regex Tag = new(@"<Popup(?![.\w])((?:""[^""]*""|[^>""])*)>");

        [Fact]
        public void EveryPopupInTheAxaml_IsOpenedWithTogglePopup()
        {
            var violations = new List<string>();
            var tagCount = 0;

            foreach (var file in SourceTree.AxamlFiles())
            {
                var text = Comment.Replace(File.ReadAllText(file), "");

                foreach (Match match in Tag.Matches(text))
                {
                    tagCount++;
                    if (!match.Groups[1].Value.Contains("TogglePopup.Toggle"))
                        violations.Add($"{SourceTree.Relative(file)}: a Popup does not use TogglePopup.Toggle");
                }
            }

            // PopupButton, PanelItemList, PrefabSlotManager and ColorSelector have one popup each.
            Assert.True(tagCount >= 4, $"The scan found only {tagCount} tags. The scan is broken.");
            Assert.True(violations.Count == 0, string.Join(Environment.NewLine, violations));
        }
    }
}
