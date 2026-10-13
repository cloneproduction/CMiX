using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia.Headless.XUnit;
using Xunit;
using CMiX.UI.Themes;

namespace CMiX.Studio.Avalonia.Tests
{
    // An AppIcon with an unknown key throws when the key is set. This test finds a wrong key
    // in the AXAML before a screen shows it. The App registers the Studio keys in Initialize.
    public class IconKeyTests
    {
        private static readonly Regex IconKey = new(@"IconKey=""([A-Za-z0-9_]+)""", RegexOptions.Compiled);

        [AvaloniaFact]
        public void EveryIconKeyInTheAxaml_IsRegistered()
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

        [Fact]
        public void Register_AcceptsTheSameKindAgain_AndRejectsAnotherKind()
        {
            var kind = Icons.Kinds["Add"];

            Icons.Register("Add", kind);

            Assert.Throws<InvalidOperationException>(() => Icons.Register("Add", Icons.Kinds["Sub"]));
        }
    }
}
