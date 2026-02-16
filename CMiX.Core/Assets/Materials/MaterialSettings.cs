// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Materials
{
    public partial class MaterialSettings : ObservableObject, IControl
    {
        public MaterialSettings(GenericValue<PipelineType> pipeline,
                                GenericValue<CullModeType> cullMode,
                                GenericValue<TransparencyType> transparency,
                                GenericValue<float> metalness,
                                GenericValue<float> specularity,
                                GenericValue<float> glossiness,
                                GenericValue<float> alpha,
                                GenericValue<bool> isShadowCaster,
                                GenericValue<string> baseColor)
        {
            Pipeline = pipeline;
            CullMode = cullMode;
            Transparency = transparency;
            Metalness = metalness;
            Specularity = specularity;
            Glossiness = glossiness;
            Alpha = alpha;
            IsShadowCaster = isShadowCaster;
            BaseColor = baseColor;
        }

        public IControlModel ToModel() => this.ToModel();
        public Guid ID { get; set; }
        public GenericValue<string> BaseColor { get; set; }
        public GenericValue<PipelineType> Pipeline { get; set; }
        public GenericValue<TransparencyType> Transparency { get; set; }
        public GenericValue<CullModeType> CullMode { get; set; }
        public GenericValue<float> Metalness { get; set; }
        public GenericValue<float> Specularity { get; set; }
        public GenericValue<float> Glossiness { get; set; }
        public GenericValue<float> Alpha { get; set; }
        public GenericValue<bool> IsShadowCaster { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
