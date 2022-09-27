// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Models.Assets;
using CMiX.Core.Presentation.ViewModels.Assets;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Network;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Texture : ObservableObject, ITexture, IPrefab
    {
        public Texture(TextureModel textureModel, CompositionService compositionService)
        {
            this.ID = textureModel.ID;
            Name = this.GetType().Name;

            IsEnabled = new ToggleButton(textureModel.IsEnabled);
            TextureModifierManager = new ModifierManager(textureModel.ModifierManagerModel, new TextureFilterFactory(compositionService));
            TextureTransformModifierManager = new ModifierManager(textureModel.TransformModifierManager, new TransformModifierFactory(compositionService));
            SamplerState = new SamplerState(textureModel.SamplerState, compositionService);

            ImageSelector = new ImageSelector(new AssetImage(), textureModel.TextureSelectorModel);
            VideoSelector = new VideoSelector(new AssetVideo(), textureModel.VideoSelectorModel);

            VideoIn = new VideoIn(textureModel.VideoIn);
            VideoPlayer = new VideoPlayer(textureModel.VideoPlayerModel);
            SelectedAssetType = new ComboBox<int>(textureModel.SelectedAssetType);
            TypeWriter = new TypeWriter(textureModel.TypeWriter, compositionService);
        }


        public Guid ID { get; set; }
        public ModifierManager TextureModifierManager { get; set; }
        public ModifierManager TextureTransformModifierManager { get; set; }

        public ToggleButton IsEnabled { get; set; }
        public SamplerState SamplerState { get; set; }


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private bool _isRenaming;
        public bool IsRenaming
        {
            get => _isRenaming;
            set => SetProperty(ref _isRenaming, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public ComboBox<int> SelectedAssetType { get; set; }


        public TypeWriter TypeWriter { get; set; }
        public VideoIn VideoIn { get; set; }
        public VideoPlayer VideoPlayer { get; set; }

        public ImageSelector ImageSelector { get; set; }
        public VideoSelector VideoSelector { get; set; }


        public IModel GetModel()
        {
            TextureModel model = new TextureModel();

            model.ID = this.ID;

            model.IsEnabled = (ToggleButtonModel)this.IsEnabled.GetModel();
            model.ModifierManagerModel = (ModifierManagerModel)this.TextureModifierManager.GetModel();
            model.TransformModifierManager = (ModifierManagerModel)this.TextureTransformModifierManager.GetModel();
            model.TextureSelectorModel = (ImageSelectorModel)this.ImageSelector.GetModel();
            model.VideoSelectorModel = (VideoSelectorModel)this.VideoSelector.GetModel();
            model.VideoPlayerModel = (VideoPlayerModel)this.VideoPlayer.GetModel();
            model.SelectedAssetType = (ComboBoxModel<int>)this.SelectedAssetType.GetModel();
            model.TypeWriter = (TypeWriterModel)this.TypeWriter.GetModel();
            model.SamplerState = (SamplerStateModel)this.SamplerState.GetModel();

            return model;
        }

        public void SetViewModel(IModel model)
        {
            TextureModel textureModel = model as TextureModel;

            this.ID = textureModel.ID;

            this.IsEnabled.SetViewModel(textureModel.IsEnabled);
            this.TextureModifierManager.SetViewModel(textureModel.ModifierManagerModel);
            this.TextureTransformModifierManager.SetViewModel(textureModel.TransformModifierManager);
            this.ImageSelector.SetViewModel(textureModel.TextureSelectorModel);
            this.VideoSelector.SetViewModel(textureModel.VideoSelectorModel);
            this.VideoPlayer.SetViewModel(textureModel.VideoPlayerModel);
            this.SelectedAssetType.SetViewModel(textureModel.SelectedAssetType);
            this.TypeWriter.SetViewModel(textureModel.TypeWriter);
            this.SamplerState.SetViewModel(textureModel.SamplerState);
        }
    }
}
