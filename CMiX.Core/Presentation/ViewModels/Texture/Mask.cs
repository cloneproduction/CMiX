// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Models.Assets;
using CMiX.Core.Presentation.ViewModels.Assets;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Mask : ObservableObject, ITexture
    {
        public Mask(MaskModel textureModel, CompositionService compositionService)
        {
            this.ID = textureModel.ID;

            IsEnabled = new ToggleButton(textureModel.IsEnabled);
            TextureModifierManager = new ModifierManager(textureModel.ModifierManagerModel, new TextureFilterFactory(compositionService));
            TextureTransformModifierManager = new ModifierManager(textureModel.TextureTransformModifierManager, new TransformModifierFactory(compositionService));
            SamplerState = new SamplerState(textureModel.SamplerState, compositionService);
            Invert = new ToggleButton(textureModel.Invert);

            ImageSelector = new ImageSelector(new AssetImage(), textureModel.TextureSelectorModel);
            VideoSelector = new VideoSelector(new AssetVideo(), textureModel.VideoSelectorModel);

            VideoIn = new VideoIn(textureModel.VideoIn);
            VideoPlayer = new VideoPlayer(textureModel.VideoPlayerModel);
            SelectedAssetType = new ComboBox<int>(textureModel.SelectedAssetType);
            TypeWriter = new TypeWriter(textureModel.TypeWriter, compositionService);
            ProceduralSelector = new ProceduralSelector(textureModel.ProceduralSelector, compositionService);
        }


        public Guid ID { get; set; }
        public ModifierManager TextureModifierManager { get; set; }
        public ModifierManager TextureTransformModifierManager { get; set; }

        public ToggleButton IsEnabled { get; set; }
        public ToggleButton Invert { get; set; }

        public SamplerState SamplerState { get; set; }


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }


        public ComboBox<int> SelectedAssetType { get; set; }


        public TypeWriter TypeWriter { get; set; }
        public VideoIn VideoIn { get; set; }
        public VideoPlayer VideoPlayer { get; set; }

        public ImageSelector ImageSelector { get; set; }
        public VideoSelector VideoSelector { get; set; }

        public ProceduralSelector ProceduralSelector { get; set; }

        public IModel GetModel()
        {
            MaskModel model = new MaskModel();

            model.ID = this.ID;

            model.IsEnabled = (ToggleButtonModel)this.IsEnabled.GetModel();
            model.Invert = (ToggleButtonModel)this.Invert.GetModel();

            model.ModifierManagerModel = (ModifierManagerModel)this.TextureModifierManager.GetModel();
            model.TextureTransformModifierManager = (ModifierManagerModel)this.TextureTransformModifierManager.GetModel();
            model.TextureSelectorModel = (ImageSelectorModel)this.ImageSelector.GetModel();
            model.VideoSelectorModel = (VideoSelectorModel)this.VideoSelector.GetModel();
            model.VideoPlayerModel = (VideoPlayerModel)this.VideoPlayer.GetModel();
            model.SelectedAssetType = (ComboBoxModel<int>)this.SelectedAssetType.GetModel();
            model.TypeWriter = (TypeWriterModel)this.TypeWriter.GetModel();
            model.SamplerState = (SamplerStateModel)this.SamplerState.GetModel();
            model.ProceduralSelector = (ProceduralSelectorModel)this.ProceduralSelector.GetModel();

            return model;
        }

        public void SetViewModel(IModel model)
        {
            MaskModel textureModel = model as MaskModel;

            this.ID = textureModel.ID;

            this.IsEnabled.SetViewModel(textureModel.IsEnabled);
            this.Invert.SetViewModel(textureModel.Invert);

            this.TextureModifierManager.SetViewModel(textureModel.ModifierManagerModel);
            this.TextureTransformModifierManager.SetViewModel(textureModel.TextureTransformModifierManager);
            this.ImageSelector.SetViewModel(textureModel.TextureSelectorModel);
            this.VideoSelector.SetViewModel(textureModel.VideoSelectorModel);
            this.VideoPlayer.SetViewModel(textureModel.VideoPlayerModel);
            this.SelectedAssetType.SetViewModel(textureModel.SelectedAssetType);
            this.TypeWriter.SetViewModel(textureModel.TypeWriter);
            this.SamplerState.SetViewModel(textureModel.SamplerState);
            this.ProceduralSelector.SetViewModel(textureModel.ProceduralSelector);
        }
    }
}
