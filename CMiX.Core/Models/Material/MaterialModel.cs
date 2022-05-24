// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Modifiers;

namespace CMiX.Core.Models
{
    public class MaterialModel : IPrefabModel
    {
        public MaterialModel()
        {
            ID = Guid.NewGuid();
            BeatModifierModel = new BeatModifierModel();
            ColorSelectorModel = new ColorSelectorModel();
            Texture = new TextureModel();
            Mask = new MaskModel();
            MaskChannelSelector = new ComboBoxModel<MaskChannel>(MaskChannel.Luma);

            ModifierManager = new ModifierManagerModel();

            Pipeline = new ComboBoxModel<PipelineType>(PipelineType.Constant);
            CullMode = new ComboBoxModel<CullModeType>(CullModeType.Back);
            Transparency = new ComboBoxModel<TransparencyType>(TransparencyType.Blend);

            Metalness = new SliderModel(0.0f);
            Glossiness = new SliderModel(0.5f);
            Specularity = new SliderModel(0.5f);

            Alpha = new SliderModel(1.0f);
            IsShadowCaster = new ToggleButtonModel(true);
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }

        public ColorSelectorModel ColorSelectorModel { get; set; }

        public BeatModifierModel BeatModifierModel { get; set; }

        public TextureModel Texture { get; set; }
        public MaskModel Mask { get; set; }

        public ModifierManagerModel ModifierManager { get; set; }

        public ComboBoxModel<PipelineType> Pipeline { get; internal set; }
        public ComboBoxModel<CullModeType> CullMode { get; internal set; }
        public ComboBoxModel<TransparencyType> Transparency { get; internal set; }
        public SliderModel Metalness { get; internal set; }
        public SliderModel Specularity { get; internal set; }
        public SliderModel Glossiness { get; internal set; }
        public SliderModel Alpha { get; internal set; }
        public ToggleButtonModel IsShadowCaster { get; internal set; }
        public ComboBoxModel<MaskChannel> MaskChannelSelector { get; internal set; }
    }
}
