using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Layering.Modifiers;
using CMiX.Core.Materials.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Rendering.Cameras.Modifiers;
using CMiX.Core.Text.Modifiers;
using CMiX.Core.Transformation;
using CMiX.Core.Transformation.Modifiers;
using Xunit;

namespace CMiX.Core.Tests
{
    // Every class here has a Modulation replacement and is no longer addable via the "Add
    // Modifier" picker - kept only so already-saved Project data referencing it still loads. This
    // guards against the exact mistake already made once this session (a class renamed to *Legacy
    // that kept its old [ModifierPanel] attributes because the content edit landed in a separate
    // step from the git mv).
    public class LegacyModifiersNotDiscoverableTests
    {
        [Theory]
        [InlineData(typeof(RandomPosition))]
        [InlineData(typeof(RandomRotation))]
        [InlineData(typeof(RandomScale))]
        [InlineData(typeof(RandomVisibility))]
        [InlineData(typeof(RandomTexCoord))]
        [InlineData(typeof(RandomXYZ))]
        [InlineData(typeof(Flip))]
        [InlineData(typeof(LFO))]
        [InlineData(typeof(RandomHSV))]
        [InlineData(typeof(SelectRandomTexture))]
        [InlineData(typeof(RenderRandomEntity))]
        [InlineData(typeof(RenderSequenceEntity))]
        [InlineData(typeof(CameraLFO))]
        [InlineData(typeof(CameraRandom))]
        [InlineData(typeof(CharWriter))]
        [InlineData(typeof(Scale))]
        [InlineData(typeof(Rotation))]
        [InlineData(typeof(Translate))]
        public void SupersededModifier_HasNoModifierPanelAttribute(Type modifierType)
        {
            var attributes = modifierType.GetCustomAttributes(typeof(ModifierPanelAttribute), false);

            Assert.Empty(attributes);
        }
    }
}
