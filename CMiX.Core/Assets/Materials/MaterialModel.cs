// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefab;
using CMiX.Core.Texturing;

namespace CMiX.Core.Materials
{
    public class MaterialModel : IControlModel
    {
        public MaterialModel()
        {
            Name = new StringValueModel("Material");
            IsSelected = new BooleanValueModel(false);
            IsRenaming = new BooleanValueModel(false);

            BaseColor = new ColorSelectorModel("#ffffffff");
            DiffuseTexture = new DiffuseTextureModel();
            MaskTexture = new MaskTextureModel();

            Pipeline = new GenericValueModel<PipelineType>(PipelineType.Constant);
            CullMode = new GenericValueModel<CullModeType>(CullModeType.Back);
            Transparency = new GenericValueModel<TransparencyType>(TransparencyType.CutOff); //if blend is use by default depthbuffer doesn't work in stride ? bug ?

            Metalness = new FloatValueModel(0.0f);
            Glossiness = new FloatValueModel(0.5f);
            Specularity = new FloatValueModel(0.5f);

            Alpha = new FloatValueModel(1.0f);
            IsShadowCaster = new BooleanValueModel(true);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public DiffuseTextureModel DiffuseTexture { get; set; }
        public MaskTextureModel MaskTexture { get; set; }

        public GenericValueModel<PipelineType> Pipeline { get; set; }
        public GenericValueModel<CullModeType> CullMode { get; set; }
        public GenericValueModel<TransparencyType> Transparency { get; set; }

        public ColorSelectorModel BaseColor { get; internal set; }

        public FloatValueModel Metalness { get; set; }
        public FloatValueModel Specularity { get; set; }
        public FloatValueModel Glossiness { get; set; }
        public FloatValueModel Alpha { get; set; }
        public BooleanValueModel IsShadowCaster { get; set; }
        public GenericValueModel<MaskChannel> MaskChannel { get; set; }
        public StringValueModel Name { get; set; }
        public BooleanValueModel IsSelected { get; set; }
        public BooleanValueModel IsRenaming { get; set; }
    }
}
