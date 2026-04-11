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

        public Guid ID { get; set; } = Guid.NewGuid();
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

        public IControlModel ToModel() => new MaterialSettingsModel
        {
            ID = ID,
            Pipeline = (GenericValueModel<PipelineType>)Pipeline.ToModel(),
            CullMode = (GenericValueModel<CullModeType>)CullMode.ToModel(),
            Transparency = (GenericValueModel<TransparencyType>)Transparency.ToModel(),
            BaseColor = (GenericValueModel<string>)BaseColor.ToModel(),
            Metalness = (GenericValueModel<float>)Metalness.ToModel(),
            Specularity = (GenericValueModel<float>)Specularity.ToModel(),
            Glossiness = (GenericValueModel<float>)Glossiness.ToModel(),
            Alpha = (GenericValueModel<float>)Alpha.ToModel(),
            IsShadowCaster = (GenericValueModel<bool>)IsShadowCaster.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (MaterialSettingsModel)model;
            ID = m.ID;
            Pipeline.FromModel(m.Pipeline);
            CullMode.FromModel(m.CullMode);
            Transparency.FromModel(m.Transparency);
            BaseColor.FromModel(m.BaseColor);
            Metalness.FromModel(m.Metalness);
            Specularity.FromModel(m.Specularity);
            Glossiness.FromModel(m.Glossiness);
            Alpha.FromModel(m.Alpha);
            IsShadowCaster.FromModel(m.IsShadowCaster);
        }
    }
}
