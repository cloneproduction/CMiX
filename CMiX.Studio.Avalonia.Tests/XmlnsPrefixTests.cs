using System.Text.RegularExpressions;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // A prefix means one namespace in every AXAML file, and a namespace has one prefix.
    // Then a prefix in one file reads the same in all the others.
    public class XmlnsPrefixTests
    {
        private static readonly Regex Declaration = new(@"xmlns:(\w+)=""([^""]+)""", RegexOptions.Compiled);

        private static (Dictionary<string, HashSet<string>> byPrefix, Dictionary<string, HashSet<string>> byNamespace) Scan()
        {
            var byPrefix = new Dictionary<string, HashSet<string>>();
            var byNamespace = new Dictionary<string, HashSet<string>>();

            foreach (var file in SourceTree.AxamlFiles())
            {
                foreach (Match match in Declaration.Matches(File.ReadAllText(file)))
                {
                    var prefix = match.Groups[1].Value;
                    var ns = match.Groups[2].Value;
                    if (!byPrefix.TryGetValue(prefix, out var namespaces))
                        byPrefix[prefix] = namespaces = new HashSet<string>();
                    namespaces.Add(ns);

                    if (!byNamespace.TryGetValue(ns, out var prefixes))
                        byNamespace[ns] = prefixes = new HashSet<string>();
                    prefixes.Add(prefix);
                }
            }

            return (byPrefix, byNamespace);
        }

        [Fact]
        public void EveryPrefix_MeansOneNamespace()
        {
            var (byPrefix, _) = Scan();

            Assert.True(byPrefix.Count > 10, "The scan found almost no prefixes. The scan is broken.");
            var wrong = byPrefix.Where(p => p.Value.Count > 1)
                .Select(p => $"{p.Key} means {string.Join(" and ", p.Value)}");
            Assert.True(!wrong.Any(), string.Join(Environment.NewLine, wrong));
        }

        [Fact]
        public void EveryNamespace_HasOnePrefix()
        {
            var (_, byNamespace) = Scan();

            var wrong = byNamespace.Where(n => n.Value.Count > 1)
                .Select(n => $"{n.Key} has the prefixes {string.Join(", ", n.Value)}");
            Assert.True(!wrong.Any(), string.Join(Environment.NewLine, wrong));
        }
    }
}
