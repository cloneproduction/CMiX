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
        public Mask(MaskModel maskModel, CompositionService compositionService)
        {
            this.ID = maskModel.ID;

            IsEnabled = new BooleanValue(maskModel.IsEnabled);
            TextureModifierManager = new ModifierManager(maskModel.ModifierManagerModel, new TextureFilterFactory(compositionService));
            TextureTransformModifierManager = new ModifierManager(maskModel.TextureTransformModifierManager, new ModifierFactory(compositionService));
            SamplerState = new SamplerState(maskModel.SamplerState, compositionService);
            Invert = new BooleanValue(maskModel.Invert);

            //ImageSelector = new ImageSelector(new AssetImage(), maskModel.TextureSelectorModel);
            //VideoSelector = new VideoSelector(new AssetVideo(), maskModel.VideoSelectorModel);

            VideoIn = new VideoIn(maskModel.VideoIn);
            VideoPlayer = new VideoPlayer(maskModel.VideoPlayerModel);
            SelectedAssetType = new GenericValue<int>(maskModel.SelectedAssetType);
            TypeWriter = new TypeWriter(maskModel.TypeWriter, compositionService);
            ProceduralSelector = new ProceduralSelector(maskModel.ProceduralSelector, compositionService);
            TransformTexture = new TransformTexture(maskModel.TransformTexture, compositionService);
        }


        public Guid ID { get; set; }
        public ModifierManager TextureModifierManager { get; set; }
        public ModifierManager TextureTransformModifierManager { get; set; }
        public BooleanValue IsEnabled { get; set; }
        public BooleanValue Invert { get; set; }
        public SamplerState SamplerState { get; set; }
        public TransformTexture TransformTexture { get; set; }
        public GenericValue<int> SelectedAssetType { get; set; }
        public TypeWriter TypeWriter { get; set; }
        public VideoIn VideoIn { get; set; }
        public VideoPlayer VideoPlayer { get; set; }
        //public ImageSelector ImageSelector { get; set; }
        //public VideoSelector VideoSelector { get; set; }
        public ProceduralSelector ProceduralSelector { get; set; }


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }


        public IModel GetModel()
        {
            MaskModel model = new MaskModel();

            model.ID = this.ID;

            model.IsEnabled = (BooleanValueModel)this.IsEnabled.GetModel();
            model.Invert = (BooleanValueModel)this.Invert.GetModel();

            model.ModifierManagerModel = (ModifierManagerModel)this.TextureModifierManager.GetModel();
            model.TextureTransformModifierManager = (ModifierManagerModel)this.TextureTransformModifierManager.GetModel();
            //model.TextureSelectorModel = (ImageSelectorModel)this.ImageSelector.GetModel();
            //model.VideoSelectorModel = (VideoSelectorModel)this.VideoSelector.GetModel();
            model.VideoPlayerModel = (VideoPlayerModel)this.VideoPlayer.GetModel();
            model.SelectedAssetType = (GenericValueModel<int>)this.SelectedAssetType.GetModel();
            model.TypeWriter = (TypeWriterModel)this.TypeWriter.GetModel();
            model.SamplerState = (SamplerStateModel)this.SamplerState.GetModel();
            model.ProceduralSelector = (ProceduralSelectorModel)this.ProceduralSelector.GetModel();
            model.TransformTexture = (TransformTextureModel)this.TransformTexture.GetModel();

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
            //this.ImageSelector.SetViewModel(textureModel.TextureSelectorModel);
            //this.VideoSelector.SetViewModel(textureModel.VideoSelectorModel);
            this.VideoPlayer.SetViewModel(textureModel.VideoPlayerModel);
            this.SelectedAssetType.SetViewModel(textureModel.SelectedAssetType);
            this.TypeWriter.SetViewModel(textureModel.TypeWriter);
            this.SamplerState.SetViewModel(textureModel.SamplerState);
            this.ProceduralSelector.SetViewModel(textureModel.ProceduralSelector);
            this.TransformTexture.SetViewModel(textureModel.TransformTexture);
        }
    }
}
