using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Every value of a filter or a modifier must survive a save and a load. A value that is
    // missing from ToModel or FromModel comes back with its default and fails here.
    public class FilterAndModifierRoundTripTests
    {
        public static IEnumerable<object[]> Types() =>
            typeof(ControlFactory).Assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => typeof(TextureFilterBase).IsAssignableFrom(t) || typeof(Modifier).IsAssignableFrom(t))
                .Where(t => t.Assembly.GetType(t.FullName + "Model") != null)
                .OrderBy(t => t.Name)
                .Select(t => new object[] { t });

        // Values that are not saved today: Blur.Visible is never read or written anywhere.
        private static readonly HashSet<string> NotSaved = new() { "Blur.Visible" };

        private static ControlFactory CreateFactory() =>
            TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();

        private static object Changed(object value, int n) => value switch
        {
            float f => f + 0.37f + n * 0.001f,
            int i => i + 3,
            bool b => !b,
            string s => s + "x",
            _ => value
        };

        [Theory]
        [MemberData(nameof(Types))]
        public void EveryValue_SurvivesSaveAndLoad(Type type)
        {
            var control = CreateFactory().Create(type);
            var before = ValueWalker.Collect(control);
            var n = 0;
            foreach (var value in before.Values)
                value.Value = Changed(value.Value, n++);

            var loaded = CreateFactory().Create(control.ToModel());
            var after = ValueWalker.Collect(loaded);

            foreach (var (path, value) in before)
            {
                if (NotSaved.Contains(path)) continue;
                Assert.True(after.ContainsKey(path), $"{path} is missing after the load.");
                Assert.True(Equals(value.Value, after[path].Value),
                    $"{path} was {value.Value} before the save and {after[path].Value} after the load.");
            }
        }
    }
}
