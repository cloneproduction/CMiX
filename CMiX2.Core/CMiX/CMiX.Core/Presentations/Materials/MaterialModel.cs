// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentations.Texturing.Mask;
using CMiX.Core.Texturing;

namespace CMiX.Core.Presentations.Materials
{
    public class MaterialModel : IPrefabModel
    {
        public MaterialModel()
        {
            ID = Guid.NewGuid();

            ColorModel = new ColorSelectorModel();
            Texture = new TextureModel();
            Mask = new MaskModel();
            MaskChannelSelector = new GenericValueModel<MaskChannel>(MaskChannel.Luma);

            ModifierManager = new ModifierManagerModel();

            Pipeline = new GenericValueModel<PipelineType>(PipelineType.Constant);
            CullMode = new GenericValueModel<CullModeType>(CullModeType.Back);
            Transparency = new GenericValueModel<TransparencyType>(TransparencyType.CutOff); //if blend is use by default depthbuffer doesn't work in stride ? bug ?

            Metalness = new FloatValueModel(0.0f);
            Glossiness = new FloatValueModel(0.5f);
            Specularity = new FloatValueModel(0.5f);

            Alpha = new FloatValueModel(1.0f);
            IsShadowCaster = new BooleanValueModel(true);
        }

        public Guid ID { get; set; }

        public ColorSelectorModel ColorModel { get; set; }

        public TextureModel Texture { get; set; }
        public MaskModel Mask { get; set; }

        public ModifierManagerModel ModifierManager { get; set; }

        public GenericValueModel<PipelineType> Pipeline { get; internal set; }
        public GenericValueModel<CullModeType> CullMode { get; internal set; }
        public GenericValueModel<TransparencyType> Transparency { get; internal set; }
        public FloatValueModel Metalness { get; internal set; }
        public FloatValueModel Specularity { get; internal set; }
        public FloatValueModel Glossiness { get; internal set; }
        public FloatValueModel Alpha { get; internal set; }
        public BooleanValueModel IsShadowCaster { get; internal set; }
        public GenericValueModel<MaskChannel> MaskChannelSelector { get; internal set; }
    }
}
