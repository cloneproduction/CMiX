using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // Heights come from the tokens of Geometry.axaml, so one change of the scale changes the whole UI.
    // A number written as a Height, a MinHeight or a MaxHeight in the AXAML fails this test, unless it is
    // in the list below with its reason. The size of a Window is not a height of the UI and is not checked.
    public class HeightLiteralTests
    {
        private static readonly Regex Comment = new("<!--.*?-->", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex Attribute = new(@"\b(Height|MinHeight|MaxHeight)\s*=\s*""(-?\d+(?:\.\d+)?)""", RegexOptions.Compiled);
        private static readonly Regex Setter = new(@"Property\s*=\s*""(Height|MinHeight|MaxHeight)""\s+Value\s*=\s*""(-?\d+(?:\.\d+)?)""", RegexOptions.Compiled);

        // A file (the end of its path), the text, and why it stays a number. Take an entry out when the number
        // becomes a token. The test fails for an entry that no longer matches anything.
        private static readonly (string File, string Text, string Reason)[] Allowed =
        {
            ("CMiX.UI/Themes/Styles/Sliders/ColorPickerWheel.axaml", "Height=\"12\"", "the cursor of the color wheel is drawn, it is not a control"),
            ("CMiX.UI/Themes/Styles/Sliders/ColorPickerWheel.axaml", "Height=\"9\"", "a thumb of the color wheel is drawn, it is not a control"),
            ("CMiX.UI/Themes/Styles/Sliders/ColorPickerWheel.axaml", "Height=\"10\"", "a thumb of the color wheel is drawn, it is not a control"),
            ("CMiX.Studio.Avalonia/Views/Messenger/SyncPanel.axaml", "MinHeight=\"50\"", "room for the list of peers, to put on the scale"),
            ("CMiX.Studio.Avalonia/Views/Messenger/SyncSettings.axaml", "MinHeight=\"40\"", "room for an error message, to put on the scale"),
        };

        private static IEnumerable<(string File, string Text)> Literals()
        {
            foreach (var file in SourceTree.AxamlFiles())
            {
                var text = Comment.Replace(File.ReadAllText(file), match => new string(' ', match.Length));
                var relative = SourceTree.Relative(file).Replace('\\', '/');

                foreach (var match in Attribute.Matches(text).Concat(Setter.Matches(text)))
                {
                    var tagStart = text.LastIndexOf('<', match.Index);
                    var tag = Regex.Match(text.Substring(tagStart + 1), @"^[\w:.]+").Value;

                    if (tag == "Window")
                        continue;

                    yield return (relative, match.Value);
                }
            }
        }

        private static bool IsAllowed((string File, string Text) literal) =>
            Allowed.Any(a => literal.File.EndsWith(a.File, StringComparison.Ordinal) && literal.Text == a.Text);

        [Fact]
        public void NoHeightIsANumber_ExceptTheListedOnes()
        {
            var all = Literals().ToList();
            var wrong = all.Where(l => !IsAllowed(l)).Select(l => $"{l.Text} in {l.File}").ToList();

            Assert.True(all.Count >= Allowed.Length, "The scan found fewer numbers than the list allows. The scan is broken.");
            Assert.True(wrong.Count == 0,
                "Use a token of Geometry.axaml, or add the number to the list with its reason:" + Environment.NewLine + string.Join(Environment.NewLine, wrong));
        }

        [Fact]
        public void EveryListedNumber_IsStillInTheFiles()
        {
            var all = Literals().ToList();
            var stale = Allowed
                .Where(a => !all.Any(l => l.File.EndsWith(a.File, StringComparison.Ordinal) && l.Text == a.Text))
                .Select(a => $"{a.Text} in {a.File}")
                .ToList();

            Assert.True(stale.Count == 0, "Take these out of the list, they are gone from the files:" + Environment.NewLine + string.Join(Environment.NewLine, stale));
        }
    }
}
