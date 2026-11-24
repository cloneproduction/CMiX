// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Compositing;
using CMiX.Core.Layering.Modifiers;
using CMiX.Core.Materials;
using CMiX.Core.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Rendering;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Text;
using CMiX.Core.Text.Modifiers;
using CMiX.Core.Texturing;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;
using VL.Lib.Mathematics;
using Xunit;

namespace CMiX.Core.Tests
{
    // A project file stores an enum as its integer, so a member must keep its position.
    public class EnumFileValuesTests
    {
        public static TheoryData<Type, string[]> SavedEnums => new()
        {
            { typeof(PipelineType), ["Constant", "Metallic", "Specular"] },
            { typeof(CullModeType), ["None", "Back", "Front"] },
            { typeof(TransparencyType), ["Blend", "CutOff"] },
            { typeof(MeshType), ["Plane", "Box", "Sphere", "Torus", "Cone", "Cylinder", "FromFile", "Text"] },
            { typeof(Assets.Mesh.HorizontalAlignment), ["Leading", "Trailing", "Center", "Justified"] },
            { typeof(Assets.Mesh.ParagraphAlignment), ["Leading", "Trailing", "Center", "Justified"] },
            { typeof(ResamplingMethod), ["Repeat", "Point", "Linear"] },
            { typeof(BlendMode), ["Normal", "Average", "GeometricMean", "Darken", "Multiply", "ColorBurn", "LinearBurn", "DarkerColor", "Lighten", "Screen", "Add", "LighterColor", "Glow", "Overlay", "Softlight", "ColorDodge", "Reflect", "HardMix", "Freeze", "Pinlight", "Hardlight", "VividLight", "LinearLight", "Difference", "Exclusion", "Subtract", "Divide", "Negation", "AdditiveSubstractive", "Heat", "Phoenix", "GrainExtract", "GrainMerge", "Hue", "Saturation", "Color", "Lightness", "Value", "Red", "Green", "Blue"] },
            { typeof(MaskChannel), ["Luma", "Value", "Saturation", "Alpha", "Hue", "Red", "Green", "Blue"] },
            { typeof(MaskMode), ["OneBelow", "AllBelow"] },
            { typeof(TextStyle), ["Normal", "Bold", "Italic", "BoldItalic"] },
            { typeof(TextHorizontalAlignment), ["Left", "Center", "Right"] },
            { typeof(TextAnchor), ["TopLeft", "TopCenter", "TopRight", "MiddleLeft", "Center", "MiddleRight", "BottomLeft", "BottomCenter", "BottomRight"] },
            { typeof(ModifierMode), ["PerInstance", "ToSpread"] },
            { typeof(ColorMode), ["RGB", "HSV", "HSL"] },
            { typeof(EntityType), ["Entity", "Camera", "Light"] },
            { typeof(WaveType), ["Cosine", "Sine", "Sawtooth", "Triangle", "Rectangle"] },
            { typeof(LightType), ["SpotLight", "PointLight", "AmbientLight", "DirectionalLight"] },
            { typeof(TexcoordSemantic), ["Texcoord0", "Texcoord1", "Texcoord2", "Texcoord3", "Texcoord4", "Texcoord5", "Texcoord6", "Texcoord7", "Texcoord8", "Texcoord9"] },
            { typeof(SplitType), ["Character", "Words", "Separator"] },
            { typeof(HalftoneMode), ["Pixels", "Halftone", "SoftHalftone", "BWHalftone", "LineHalftone"] },
            { typeof(InvertChannel), ["RGB", "LinearRGB", "Red", "Green", "Blue", "Value", "Luma", "Hue"] },
            { typeof(KuwaharaType), ["Standard", "Polynomal", "Anisotropic", "Papari"] },
            { typeof(PulseChannel), ["Red", "Green", "Blue", "Hue", "Saturation", "Value", "Lightness", "Alpha"] },
            { typeof(AlphaChannel), ["Red", "Green", "Blue", "Alpha", "Value", "Lightness", "Saturation", "Hue", "Cyan", "Magenta", "Yellow", "Black", "AverageRGBA"] },
            { typeof(TextureAddressMode), ["Clamp", "Mirror", "Wrap", "Border", "MirrorOnce"] },
            { typeof(GradientMeshType), ["Linear", "Spectral"] },
            { typeof(TweenerTransition), ["Linear", "Sine", "Quad", "Cubic", "Quart", "Quint", "Expo", "Circular", "Elastic", "Back", "Bounce"] },
            { typeof(TweenerMode), ["In", "Out", "InOut", "OutIn"] },
        };

        [Theory]
        [MemberData(nameof(SavedEnums))]
        public void GetValues_OfASavedEnum_KeepsEveryMemberAtItsSavedInteger(Type enumType, string[] names)
        {
            var values = Enum.GetValues(enumType);

            Assert.Equal(names.Length, values.Length);
            for (var index = 0; index < names.Length; index++)
            {
                var value = values.GetValue(index);
                Assert.Equal(index, Convert.ToInt32(value));
                Assert.Equal(names[index], Enum.GetName(enumType, value));
            }
        }
    }
}
