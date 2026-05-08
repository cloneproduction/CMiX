// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Materials
{
    public record MaterialSettingsModel : IControlModel
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<PipelineType> Pipeline { get; set; } = new(PipelineType.Constant);
        public GenericValueModel<CullModeType> CullMode { get; set; } = new(CullModeType.Back);
        public GenericValueModel<TransparencyType> Transparency { get; set; } = new(TransparencyType.CutOff);
        public GenericValueModel<string> BaseColor { get; set; } = new("#FFFFFFFF");
        public GenericValueModel<float> Metalness { get; set; } = new(0.0f);
        public GenericValueModel<float> Roughness { get; set; } = new(0.0f);
        public GenericValueModel<float> Specularity { get; set; } = new(0.5f);
        public GenericValueModel<float> Glossiness { get; set; } = new(0.5f);
        public GenericValueModel<float> Alpha { get; set; } = new(1.0f);
        public GenericValueModel<bool> IsShadowCaster { get; set; } = new(false);
    }
}
