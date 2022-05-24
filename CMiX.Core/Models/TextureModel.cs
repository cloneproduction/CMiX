// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models.Assets;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Modifiers;

namespace CMiX.Core.Models
{
    public class TextureModel : IPrefabModel
    {
        public TextureModel()
        {
            this.ID = Guid.NewGuid();

            TextureSelectorModel = new ImageSelectorModel();
            VideoSelectorModel = new VideoSelectorModel();

            VideoPlayerModel = new VideoPlayerModel();

            ModifierManagerModel = new ModifierManagerModel();
            TransformModifierManager = new ModifierManagerModel();

            VideoIn = new VideoInModel();

            IsEnabled = new ToggleButtonModel();

            SelectedAssetType = new ComboBoxModel<int>(0);
            SamplerState = new SamplerStateModel();
            TypeWriter = new TypeWriterModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }

        public ModifierManagerModel ModifierManagerModel { get; set; }
        public ModifierManagerModel TransformModifierManager { get; set; }

        public ImageSelectorModel TextureSelectorModel { get; set; }

        public ToggleButtonModel IsEnabled { get; internal set; }
        public VideoSelectorModel VideoSelectorModel { get; internal set; }
        public VideoPlayerModel VideoPlayerModel { get; set; }
        public VideoInModel VideoIn { get; internal set; }
        public ComboBoxModel<int> SelectedAssetType { get; internal set; }
        public TypeWriterModel TypeWriter { get; internal set; }
        public SamplerStateModel SamplerState { get; internal set; }
    }
}
