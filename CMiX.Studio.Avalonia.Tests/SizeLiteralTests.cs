using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // Sizes come from the tokens of Geometry.axaml, so one change of the scale changes the whole UI.
    // A number written as a Height, a MinHeight, a MaxHeight, a Width, a MinWidth or a MaxWidth in the AXAML fails this test,
    // unless it is in the list below with its reason. In the files of SpaceFiles, a number written as a Margin or a Padding
    // fails too. A zero is not a size and is not checked. The size of a Window is not a size of the UI and is not checked.
    public class SizeLiteralTests
    {
        private const string Number = @"-?\d+(?:\.\d+)?";
        private const string Numbers = Number + @"(?:\s*,\s*" + Number + ")*";

        private static readonly Regex Comment = new("<!--.*?-->", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex SizeAttribute = new(@"\b(Height|MinHeight|MaxHeight|Width|MinWidth|MaxWidth)\s*=\s*""(" + Number + @")""", RegexOptions.Compiled);
        private static readonly Regex SizeSetter = new(@"Property\s*=\s*""(Height|MinHeight|MaxHeight|Width|MinWidth|MaxWidth)""\s+Value\s*=\s*""(" + Number + @")""", RegexOptions.Compiled);
        private static readonly Regex SpaceAttribute = new(@"\b(Margin|Padding)\s*=\s*""(" + Numbers + @")""", RegexOptions.Compiled);
        private static readonly Regex SpaceSetter = new(@"Property\s*=\s*""(Margin|Padding)""\s+Value\s*=\s*""(" + Numbers + @")""", RegexOptions.Compiled);

        // The files of the tree and of the scroll bar. Their margins and paddings are tokens too.
        // The other files keep their layout numbers: a scan of all of them would list hundreds of values.
        private static readonly string[] SpaceFiles =
        {
            "CMiX.UI/Themes/Styles/ScrollViewer.axaml",
            "CMiX.UI/Themes/Styles/GridSplitter.axaml",
        };

        // A file (the end of its path), the text, and why it stays a number. Take an entry out when the number
        // becomes a token. The test fails for an entry that no longer matches anything.
        private static readonly (string File, string Text, string Reason)[] Allowed =
        {
            ("CMiX.UI/Themes/Styles/Sliders/ColorPickerWheel.axaml", "Height=\"12\"", "the cursor of the color wheel is drawn, it is not a control"),
            ("CMiX.UI/Themes/Styles/Sliders/ColorPickerWheel.axaml", "Height=\"9\"", "a thumb of the color wheel is drawn, it is not a control"),
            ("CMiX.UI/Themes/Styles/Sliders/ColorPickerWheel.axaml", "Height=\"10\"", "a thumb of the color wheel is drawn, it is not a control"),
            ("CMiX.Studio.Avalonia/Views/Messenger/PanelSync.axaml", "MinHeight=\"50\"", "room for the list of peers, to put on the scale"),
            ("CMiX.Studio.Avalonia/Views/Messenger/SettingsSync.axaml", "MinHeight=\"40\"", "room for an error message, to put on the scale"),
            ("CMiX.UI/Themes/Styles/Sliders/ColorPickerWheel.axaml", "Width=\"12\"", "the cursor of the color wheel is drawn, it is not a control"),
            ("CMiX.UI/Controls/ColorPicker/ColorPickerFull.axaml", "Width=\"16\"", "the slider at the side of the color wheel, to put on the scale"),
            ("CMiX.UI/Themes/Styles/ComboBox.axaml", "Width=\"22\"", "the toggle of the combo box, to put on the scale"),
            ("CMiX.UI/Themes/Styles/ComboBox.axaml", "Width=\"266\"", "the drop down of the combo box, to put on the scale"),
            ("CMiX.UI/Themes/Styles/Menu/PopupFlyout.axaml", "Property=\"Width\" Value=\"300\"", "the width of a popup flyout, to put on the scale"),
            ("CMiX.Studio.Avalonia/Views/Dialogs/DialogMessage.axaml", "MaxWidth=\"420\"", "the line length of a message, to put on the scale"),
            ("CMiX.Studio.Avalonia/Views/MainWindow.axaml", "MinWidth=\"240\"", "the least width of the tree and of the tabs, to put on the scale"),
        };

        private static bool IsZero(Match match) =>
            match.Groups[2].Value.Split(',').All(part => double.Parse(part.Trim(), CultureInfo.InvariantCulture) == 0);

        private static IEnumerable<(string File, string Text)> Literals()
        {
            foreach (var file in SourceTree.AxamlFiles())
            {
                var text = Comment.Replace(File.ReadAllText(file), match => new string(' ', match.Length));
                var relative = SourceTree.Relative(file).Replace('\\', '/');

                var matches = SizeAttribute.Matches(text).Concat(SizeSetter.Matches(text));
                if (SpaceFiles.Contains(relative))
                    matches = matches.Concat(SpaceAttribute.Matches(text)).Concat(SpaceSetter.Matches(text));

                foreach (var match in matches)
                {
                    if (IsZero(match))
                        continue;

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
        public void Scan_NumbersNotInTheList_AreNotFound()
        {
            var all = Literals().ToList();
            var wrong = all.Where(l => !IsAllowed(l)).Select(l => $"{l.Text} in {l.File}").ToList();

            Assert.True(all.Count >= Allowed.Length, "The scan found fewer numbers than the list allows. The scan is broken.");
            Assert.True(wrong.Count == 0,
                "Use a token of Geometry.axaml, or add the number to the list with its reason:" + Environment.NewLine + string.Join(Environment.NewLine, wrong));
        }

        [Fact]
        public void Allowed_EveryEntry_IsStillInTheFiles()
        {
            var all = Literals().ToList();
            var stale = Allowed
                .Where(a => !all.Any(l => l.File.EndsWith(a.File, StringComparison.Ordinal) && l.Text == a.Text))
                .Select(a => $"{a.Text} in {a.File}")
                .ToList();

            Assert.True(stale.Count == 0, "Take these out of the list, they are gone from the files:" + Environment.NewLine + string.Join(Environment.NewLine, stale));
        }

        [Fact]
        public void SpaceFiles_EveryFile_Exists()
        {
            var root = SourceTree.Root();

            Assert.All(SpaceFiles, file => Assert.True(File.Exists(Path.Combine(root, file)), file));
        }
    }
}
