using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // Every value of a control with a model must survive a save and a load. A value that is
    // missing from ToModel or FromModel comes back with its default and fails here.
    public class ControlRoundTripTests
    {
        public static IEnumerable<object[]> Types() =>
            typeof(ControlFactory).Assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition)
                .Where(t => typeof(IControl).IsAssignableFrom(t))
                .Where(t => t.Assembly.GetType(t.FullName + "Model") != null)
                .OrderBy(t => t.Name, StringComparer.Ordinal)
                .Select(t => new object[] { t });

        // Values that ToModel does not write today. The owner decides if they must.
        internal static readonly HashSet<string> UnsavedPaths = new(StringComparer.Ordinal)
        {
            "BeatStepModulator.Center",
            "BeatStepModulator.Width",
        };

        // OutputMappingManager gives a mapping its fixed slot ID. PrefabManager keeps the ID of its ManagerData.
        private static readonly HashSet<Type> IdNotLoaded = new() { typeof(OutputMapping), typeof(PrefabManager) };

        private static ControlFactory CreateFactory() =>
            TestServiceProviderFactory.Create().GetRequiredService<ControlFactory>();

        [Fact]
        public void Types_OfTheCoreAssembly_HoldEveryControlWithAModel()
        {
            Assert.True(Types().Count() > 80, $"Only {Types().Count()} control types were found.");
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void FromModel_OfToModel_KeepsEveryValue(Type type)
        {
            var control = CreateFactory().Create(type);
            var before = ValueWalker.Collect(control);
            var n = 0;
            foreach (var value in before.Values)
                value.Change(n++);

            var loaded = CreateFactory().Create(control.ToModel());
            var after = ValueWalker.Collect(loaded);

            if (!IdNotLoaded.Contains(type))
                Assert.Equal(control.ID, loaded.ID);
            foreach (var (path, value) in before.Where(entry => !UnsavedPaths.Contains(entry.Key)))
            {
                Assert.True(after.ContainsKey(path), $"{path} is missing after the load.");
                Assert.True(Equals(value.Value, after[path].Value),
                    $"{path} was {value.Value} before the save and {after[path].Value} after the load.");
                Assert.True(value.ID == after[path].ID, $"{path} has another ID after the load.");
            }
        }
    }
}
