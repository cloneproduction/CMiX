// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Materials;

namespace CMiX.Core.Materials
{
    public class MaterialSettingsModel : IControlModel
    {
        public MaterialSettingsModel()
        {
            ID = Guid.NewGuid();
            BaseColor = new GenericValueModel<string>("#FFFF00FF");
            Alpha = new GenericValueModel<float>(1.0f);

            Pipeline = new GenericValueModel<PipelineType>(PipelineType.Metallic);
            CullMode = new GenericValueModel<CullModeType>(CullModeType.Back);
            Transparency = new GenericValueModel<TransparencyType>(TransparencyType.Blend);

            Metalness = new GenericValueModel<float>(0.0f);
            Glossiness = new GenericValueModel<float>(0.5f);
            Specularity = new GenericValueModel<float>(0.5f);
    
            IsShadowCaster = new GenericValueModel<bool>(true);
        }

        public Guid ID { get; set; }
        public GenericValueModel<PipelineType> Pipeline { get; set; }
        public GenericValueModel<CullModeType> CullMode { get; set; }
        public GenericValueModel<TransparencyType> Transparency { get; set; }
        public GenericValueModel<string> BaseColor { get; set; }
        public GenericValueModel<float> Metalness { get; set; }
        public GenericValueModel<float> Specularity { get; set; }
        public GenericValueModel<float> Glossiness { get; set; }
        public GenericValueModel<float> Alpha { get; set; }
        public GenericValueModel<bool> IsShadowCaster { get; set; }
    }
}
