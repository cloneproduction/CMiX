using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia.Headless.XUnit;
using Xunit;
using CMiX.Studio.Avalonia.ViewModels.Tree;
using CMiX.UI.Themes;

namespace CMiX.Studio.Avalonia.Tests
{
    // An AppIcon with an unknown key throws when the key is set. This test finds a wrong key
    // in the AXAML before a screen shows it. The App registers the Studio keys in Initialize.
    public class IconKeyTests
    {
        private static readonly Regex IconKey = new(@"IconKey=""([A-Za-z0-9_]+)""", RegexOptions.Compiled);

        // Every icon has the one size IconSizeXS, and a slot that holds an icon (the icon of a menu row) is not smaller than the icon.
        [Fact]
        public void Check_EveryIconSizeAndSlotInTheAxaml_IsIconSizeXS()
        {
            var other = new List<string>();
            var size = new Regex(@"<(?:ui:)?AppIcon\b[^>]*?\bSize=""([^""]+)""", RegexOptions.Singleline);
            foreach (var file in SourceTree.AxamlFiles())
            {
                var text = File.ReadAllText(file);
                foreach (Match match in size.Matches(text))
                    if (match.Groups[1].Value != "{DynamicResource IconSizeXS}")
                        other.Add($"{SourceTree.Relative(file)}: {match.Groups[1].Value}");

                foreach (Match match in Regex.Matches(text, @"Name=""PART_IconPresenter""[^>]*?(?:Width|Height)=""([^""]+)""", RegexOptions.Singleline))
                    if (match.Groups[1].Value != "{DynamicResource IconSizeXS}")
                        other.Add($"{SourceTree.Relative(file)}: slot {match.Groups[1].Value}");
            }

            Assert.True(other.Count == 0, "Icons with another size:\n" + string.Join("\n", other));
        }

        [Fact]
        public void Duplicate_TheKey_IsTheDuplicateIconAndNotTheCopyIcon()
        {
            Assert.Equal(Material.Icons.MaterialIconKind.ContentDuplicate, Icons.Kinds["Duplicate"]);
            Assert.NotEqual(Icons.Kinds["Copy"], Icons.Kinds["Duplicate"]);
        }

        [AvaloniaFact]
        public void Scan_EveryIconKeyInTheAxaml_IsRegistered()
        {
            var found = 0;
            var unknown = new List<string>();

            foreach (var file in SourceTree.AxamlFiles())
            {
                foreach (Match match in IconKey.Matches(File.ReadAllText(file)))
                {
                    found++;
                    var key = match.Groups[1].Value;
                    if (!Icons.Kinds.ContainsKey(key))
                        unknown.Add($"{SourceTree.Relative(file)}: {key}");
                }
            }

            Assert.True(found >= 10, $"The scan found only {found} icon keys. The scan is broken.");
            Assert.True(unknown.Count == 0, string.Join(Environment.NewLine, unknown));
        }

        [AvaloniaFact]
        public void OfCategory_EachCategory_IsAKeyOfTheIcons()
        {
            var unknown = Enum.GetValues<TreeCategory>()
                .Select(TreeIcons.OfCategory)
                .Where(key => !Icons.Kinds.ContainsKey(key))
                .ToList();

            Assert.True(unknown.Count == 0, "Unknown icon keys: " + string.Join(", ", unknown));
        }

        [Fact]
        public void Register_AcceptsTheSameKindAgain_AndRejectsAnotherKind()
        {
            var kind = Icons.Kinds["Add"];

            Icons.Register("Add", kind);

            Assert.Throws<InvalidOperationException>(() => Icons.Register("Add", Icons.Kinds["Sub"]));
        }
    }
}
