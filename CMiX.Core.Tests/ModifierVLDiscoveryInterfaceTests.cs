using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Transformation;
using CMiX.Core.Transformation.Modifiers;
using Xunit;

namespace CMiX.Core.Tests
{
    public class ModifierVLDiscoveryInterfaceTests
    {
        [Theory]
        [InlineData(typeof(ScaleModifier))]
        [InlineData(typeof(TranslateModifier))]
        [InlineData(typeof(RotationModifier))]
        [InlineData(typeof(HSVModifier))]
        [InlineData(typeof(TexCoordModifier))]
        [InlineData(typeof(TransformSRTModifier))]
        [InlineData(typeof(CircularSpreadModifier))]
        [InlineData(typeof(LinearModifier))]
        public void PortedSpreadableModifier_StillImplementsISpreadableModifier(Type modifierType)
        {
            Assert.True(typeof(ISpreadableModifier).IsAssignableFrom(modifierType),
                $"{modifierType.Name} must implement ISpreadableModifier to stay eligible for VL-side Spread grouping, matching its old counterpart.");
        }

        [Theory]
        [InlineData(typeof(GridModifier))]
        public void PortedSpreadableModifier_StillImplementsISpreadableModifier3(Type modifierType)
        {
            Assert.True(typeof(ISpreadableModifier3).IsAssignableFrom(modifierType),
                $"{modifierType.Name} must implement ISpreadableModifier3 to stay eligible for VL-side Spread grouping, matching its old counterpart.");
        }
    }
}
