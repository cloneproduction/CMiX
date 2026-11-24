using CMiX.Core.Prefabs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Every value of a control with a model must survive a save and a load. A value that is
    // missing from ToModel or FromModel comes back with its default and fails here.
    public class FilterAndModifierRoundTripTests
    {
        public static IEnumerable<object[]> Types() =>
            typeof(ControlFactory).Assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition)
                .Where(t => typeof(IControl).IsAssignableFrom(t))
                .Where(t => t.Assembly.GetType(t.FullName + "Model") != null)
                .OrderBy(t => t.Name, StringComparer.Ordinal)
                .Select(t => new object[] { t });

        // Values that ToModel does not write today. The owner decides if they must.
        private static readonly HashSet<string> UnsavedPaths = new(StringComparer.Ordinal)
        {
            "BeatStepModulator.Center",
            "BeatStepModulator.Width",
        };

        private static ControlFactory CreateFactory() =>
            TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();

        private static object Changed(WalkedValue value, int n) => value.Value switch
        {
            float f => f + 0.37f + n * 0.001f,
            int i => i + 3,
            bool b => !b,
            string s => s + "x",
            Guid => new Guid(n + 1, 0, 0, new byte[8]),
            Enum e => NextValue(e),
            null when value.ValueType == typeof(string) => "x",
            null when value.ValueType == typeof(Guid?) => new Guid(n + 1, 0, 0, new byte[8]),
            _ => value.Value
        };

        private static object NextValue(Enum value)
        {
            var values = Enum.GetValues(value.GetType());
            return values.GetValue((Array.IndexOf(values, value) + 1) % values.Length);
        }

        [Fact]
        public void Types_OfTheCoreAssembly_HoldEveryControlWithAModel()
        {
            Assert.True(Types().Count() > 80, $"Only {Types().Count()} control types were found.");
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void EveryValue_SurvivesSaveAndLoad(Type type)
        {
            var control = CreateFactory().Create(type);
            var before = ValueWalker.Collect(control);
            var n = 0;
            foreach (var value in before.Values)
                value.Value = Changed(value, n++);

            var loaded = CreateFactory().Create(control.ToModel());
            var after = ValueWalker.Collect(loaded);

            foreach (var (path, value) in before.Where(entry => !UnsavedPaths.Contains(entry.Key)))
            {
                Assert.True(after.ContainsKey(path), $"{path} is missing after the load.");
                Assert.True(Equals(value.Value, after[path].Value),
                    $"{path} was {value.Value} before the save and {after[path].Value} after the load.");
            }
        }
    }
}
