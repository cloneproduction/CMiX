using CMiX.Core.Modulation.Modifiers;
using CMiX.Core.Rendering.Cameras.Modifiers;
using CMiX.Core.Transformation.Modifiers;
using Xunit;

namespace CMiX.Core.Tests
{
    // ISpreadableModifier/ICameraModifier have no C# consumers - they exist purely as VL/vvvv-side
    // reflection markers, so nothing here would fail to compile if a class silently stopped
    // implementing one. These tests exist to catch exactly that: a future edit to any of these
    // classes that drops the interface (as happened once already during this migration) fails
    // loudly here instead of silently, only discoverable by opening the live VL patch.
    public class ModifierVLDiscoveryInterfaceTests
    {
        [Theory]
        [InlineData(typeof(ScaleModifier))]
        [InlineData(typeof(TranslateModifier))]
        [InlineData(typeof(RotationModifier))]
        [InlineData(typeof(HSVModifier))]
        [InlineData(typeof(LFOModifier))]
        [InlineData(typeof(TexCoordModifier))]
        [InlineData(typeof(XYZModifier))]
        [InlineData(typeof(GridModifier))]
        [InlineData(typeof(CircularSpreadModifier))]
        [InlineData(typeof(LinearXYZModifier))]
        public void PortedSpreadableModifier_StillImplementsISpreadableModifier(Type modifierType)
        {
            Assert.True(typeof(ISpreadableModifier).IsAssignableFrom(modifierType),
                $"{modifierType.Name} must implement ISpreadableModifier to stay eligible for VL-side Spread grouping, matching its old counterpart.");
        }

        [Theory]
        [InlineData(typeof(CameraLFOModifier))]
        [InlineData(typeof(CameraRandomModifier))]
        public void PortedCameraModifier_StillImplementsICameraModifier(Type modifierType)
        {
            Assert.True(typeof(ICameraModifier).IsAssignableFrom(modifierType),
                $"{modifierType.Name} must implement ICameraModifier to stay eligible for VL-side camera discovery, matching its old counterpart.");
        }
    }
}
