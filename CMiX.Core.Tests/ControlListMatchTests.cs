using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Compositing;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Texturing.Sources;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ControlListMatchTests
    {
        private static readonly Type[] ListTypes =
        {
            typeof(Composition), typeof(Layer), typeof(ITextureSource), typeof(Entity),
            typeof(Camera), typeof(LightEntity), typeof(TextEntity), typeof(ColorPaletteModifier)
        };

        [Fact]
        public void NoControlClass_BelongsToMoreThanOneTypedList()
        {
            var controlClasses = typeof(IControl).Assembly.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && typeof(IControl).IsAssignableFrom(type))
                .ToList();

            var offenders = controlClasses
                .Select(type => (Type: type, Lists: ListTypes.Where(list => list.IsAssignableFrom(type)).Select(list => list.Name).ToList()))
                .Where(entry => entry.Lists.Count > 1)
                .Select(entry => $"{entry.Type.Name} matches {string.Join(", ", entry.Lists)}")
                .ToList();

            Assert.True(controlClasses.Count > 50, $"The scan found only {controlClasses.Count} control classes. The scan is broken.");
            Assert.True(offenders.Count == 0, string.Join(Environment.NewLine, offenders));
        }
    }
}
