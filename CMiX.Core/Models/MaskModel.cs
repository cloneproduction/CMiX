// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models.Assets;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Modifiers;

namespace CMiX.Core.Models
{
    public class MaskModel : IModel
    {
        public MaskModel()
        {
            this.ID = Guid.NewGuid();

            TextureSelectorModel = new ImageSelectorModel();
            VideoSelectorModel = new VideoSelectorModel();

            VideoPlayerModel = new VideoPlayerModel();

            ModifierManagerModel = new ModifierManagerModel();
            TextureTransformModifierManager = new ModifierManagerModel();
            SamplerState = new SamplerStateModel();
            VideoIn = new VideoInModel();
            Invert = new BooleanValueModel();

            IsEnabled = new BooleanValueModel();

            SelectedAssetType = new GenericValueModel<int>(0);

            TypeWriter = new TypeWriterModel();
            ProceduralSelector = new ProceduralSelectorModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }

        public ModifierManagerModel ModifierManagerModel { get; set; }
        public ModifierManagerModel TextureTransformModifierManager { get; set; }

        public ImageSelectorModel TextureSelectorModel { get; set; }

        public BooleanValueModel IsEnabled { get; internal set; }
        public VideoSelectorModel VideoSelectorModel { get; internal set; }
        public VideoPlayerModel VideoPlayerModel { get; set; }
        public VideoInModel VideoIn { get; internal set; }
        public GenericValueModel<int> SelectedAssetType { get; internal set; }
        public TypeWriterModel TypeWriter { get; internal set; }
        public SamplerStateModel SamplerState { get; internal set; }
        public BooleanValueModel Invert { get; internal set; }
        public ProceduralSelectorModel ProceduralSelector { get; set; }
    }
}
