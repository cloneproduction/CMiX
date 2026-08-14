// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Linq;
using Avalonia.Media;
using CoreColorMode = CMiX.Core.ColorMode;
using CoreMeshType = CMiX.Core.MeshType;
using MeshHorizontalAlignment = CMiX.Core.Assets.Mesh.HorizontalAlignment;
using MeshParagraphAlignment = CMiX.Core.Assets.Mesh.ParagraphAlignment;
using SourceFontStyle = CMiX.Core.Texturing.Sources.FontStyle;
using TextAnchor = CMiX.Core.Text.Anchor;
using TextHorizontalAlignmentEnum = CMiX.Core.Text.HorizontalAlignment;

namespace CMiX.Studio.Avalonia.Resources
{
    // Static replacement for the WPF ObjectDataProvider dictionary in Resources\DataProviders.xaml.
    // Consumed from XAML via {x:Static Resources:EnumProviders.X}.
    public static class EnumProviders
    {
        public static CMiX.Core.Texturing.Filters.KuwaharaType[] KuwaharaType { get; } =
            Enum.GetValues<CMiX.Core.Texturing.Filters.KuwaharaType>();

        public static CMiX.Core.Materials.Modifiers.TextureFrom[] TextureFrom { get; } =
            Enum.GetValues<CMiX.Core.Materials.Modifiers.TextureFrom>();

        public static CMiX.Core.Compositing.ResamplingMethod[] ResamplingMethod { get; } =
            Enum.GetValues<CMiX.Core.Compositing.ResamplingMethod>();

        public static TextAnchor[] Anchor { get; } =
            Enum.GetValues<TextAnchor>();

        public static TextHorizontalAlignmentEnum[] TextHorizontalAlignment { get; } =
            Enum.GetValues<TextHorizontalAlignmentEnum>();

        public static CMiX.Core.Text.Modifiers.SplitType[] SplitType { get; } =
            Enum.GetValues<CMiX.Core.Text.Modifiers.SplitType>();

        public static MeshHorizontalAlignment[] HorizontalAlignment { get; } =
            Enum.GetValues<MeshHorizontalAlignment>();

        public static MeshParagraphAlignment[] ParagraphAlignment { get; } =
            Enum.GetValues<MeshParagraphAlignment>();

        public static CMiX.Core.Layering.Modifiers.EntityType[] EntityType { get; } =
            Enum.GetValues<CMiX.Core.Layering.Modifiers.EntityType>();

        public static CMiX.Core.Rendering.Lights.LightType[] LightType { get; } =
            Enum.GetValues<CMiX.Core.Rendering.Lights.LightType>();

        public static CMiX.Core.Rendering.Cameras.CameraAxis[] CameraAxis { get; } =
            Enum.GetValues<CMiX.Core.Rendering.Cameras.CameraAxis>();

        public static CoreMeshType[] MeshType { get; } =
            Enum.GetValues<CoreMeshType>();

        public static VL.Lib.Mathematics.TweenerTransition[] TweenerTransition { get; } =
            Enum.GetValues<VL.Lib.Mathematics.TweenerTransition>();

        public static VL.Lib.Mathematics.TweenerMode[] TweenerMode { get; } =
            Enum.GetValues<VL.Lib.Mathematics.TweenerMode>();

        public static CMiX.Core.Transformation.TransformType[] TransformType { get; } =
            Enum.GetValues<CMiX.Core.Transformation.TransformType>();

        public static CMiX.Core.Modifiers.ModifierMode[] ModifierMode { get; } =
            Enum.GetValues<CMiX.Core.Modifiers.ModifierMode>();

        public static CoreColorMode[] ColorMode { get; } =
            Enum.GetValues<CoreColorMode>();

        public static CMiX.Core.Texturing.TextureAddressMode[] TextureAddressMode { get; } =
            Enum.GetValues<CMiX.Core.Texturing.TextureAddressMode>();

        public static CMiX.Core.Texturing.Filters.InvertChannel[] InvertChannel { get; } =
            Enum.GetValues<CMiX.Core.Texturing.Filters.InvertChannel>();

        public static SourceFontStyle[] FontStyle { get; } =
            Enum.GetValues<SourceFontStyle>();

        public static CMiX.Core.Materials.PipelineType[] PipelineType { get; } =
            Enum.GetValues<CMiX.Core.Materials.PipelineType>();

        public static CMiX.Core.Materials.TransparencyType[] TransparencyType { get; } =
            Enum.GetValues<CMiX.Core.Materials.TransparencyType>();

        public static CMiX.Core.Materials.CullModeType[] CullModeType { get; } =
            Enum.GetValues<CMiX.Core.Materials.CullModeType>();

        public static CMiX.Core.Texturing.BlendModeEnum[] BlendMode { get; } =
            Enum.GetValues<CMiX.Core.Texturing.BlendModeEnum>();

        public static CMiX.Core.Texturing.MaskMode[] MaskMode { get; } =
            Enum.GetValues<CMiX.Core.Texturing.MaskMode>();

        public static CMiX.Core.Texturing.MaskChannel[] MaskChannel { get; } =
            Enum.GetValues<CMiX.Core.Texturing.MaskChannel>();

        public static CMiX.Core.Texturing.Filters.AlphaChannel[] AlphaChannel { get; } =
            Enum.GetValues<CMiX.Core.Texturing.Filters.AlphaChannel>();

        public static CMiX.Core.Texturing.Filters.HalftoneMode[] HalftoneMode { get; } =
            Enum.GetValues<CMiX.Core.Texturing.Filters.HalftoneMode>();

        // Replacement for the WPF SortedFontsCollection CollectionViewSource
        // over Fonts.SystemFontFamilies with a SortDescription on Source.
        public static FontFamily[] SortedFontsCollection { get; } =
            FontManager.Current.SystemFonts.OrderBy(f => f.Name).ToArray();
    }
}
