// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Prefab;
using CMiX.Core.Presentation.ViewModels.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Texture : ObservableObject, ITexture, IPrefab
    {
        public Texture(TextureModel textureModel, CompositionService compositionService)
        {
            this.ID = textureModel.ID;
            Name = this.GetType().Name;

            IsEnabled = new BooleanValue(textureModel.IsEnabled);
            TextureModifierManager = new ModifierManager(textureModel.TextureModifierManager, new TextureFilterFactory(compositionService), compositionService);
            TextureTransformModifierManager = new ModifierManager(textureModel.TextureTransformModifierManager, new ModifierFactory(compositionService), compositionService);
            SamplerState = new SamplerState(textureModel.SamplerState, compositionService);

            ProceduralSelector = new ProceduralSelector(textureModel.ProceduralSelector, compositionService);

            VideoIn = new VideoIn(textureModel.VideoIn);
            VideoPlayer = new VideoPlayer(textureModel.VideoPlayer);
            SelectedAssetType = new IntegerValue(textureModel.SelectedAssetType);
            TypeWriter = new TypeWriter(textureModel.TypeWriter, compositionService);
            TransformTexture = new TransformTexture(textureModel.TransformTexture, compositionService);
        }


        public Guid ID { get; set; }
        public ModifierManager TextureModifierManager { get; set; }
        public ModifierManager TextureTransformModifierManager { get; set; }

        public BooleanValue IsEnabled { get; set; }
        public SamplerState SamplerState { get; set; }
        public TransformTexture TransformTexture { get; set; }


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


        public IntegerValue SelectedAssetType { get; set; }
        public TypeWriter TypeWriter { get; set; }
        public VideoIn VideoIn { get; set; }
        public VideoPlayer VideoPlayer { get; set; }
        public ProceduralSelector ProceduralSelector { get; set; }
    }
}
