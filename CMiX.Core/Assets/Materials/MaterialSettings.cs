// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Materials;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Assets.Materials
{
    public class MaterialSettings : ObservableObject
    {
        public MaterialSettings(
            GenericValue<PipelineType> pipeline,
            GenericValue<CullModeType> cullMode,
            GenericValue<TransparencyType> transparency,
            FloatValue metalness,
            FloatValue specularity,
            FloatValue glossiness,
            FloatValue alpha,
            BooleanValue isShadowCaster,
            ColorValue baseColor)
        {
            Pipeline = pipeline; // new GenericValue<PipelineType>(PipelineType.Constant);
            CullMode = cullMode; // new GenericValue<CullModeType>(CullModeType.Back);
            Transparency = transparency; // new GenericValue<TransparencyType>(TransparencyType.CutOff);

            Metalness = metalness; // new FloatValue(0.5f);
            Specularity = specularity; // new FloatValue(0.5f);
            Glossiness = glossiness; // new FloatValue(0.5f);
            Alpha = alpha; // new FloatValue(1.0f);
            IsShadowCaster = isShadowCaster; // new BooleanValue(false);

            BaseColor = baseColor; // new ColorValue(Color.FromArgb(255, 255, 255, 255));
        }

        public ColorValue BaseColor { get; set; }
        public GenericValue<PipelineType> Pipeline { get; set; }
        public GenericValue<TransparencyType> Transparency { get; set; }
        public GenericValue<CullModeType> CullMode { get; set; }
        public FloatValue Metalness { get; set; }
        public FloatValue Specularity { get; set; }
        public FloatValue Glossiness { get; set; }
        public FloatValue Alpha { get; set; }
        public BooleanValue IsShadowCaster { get; set; }
    }
}
