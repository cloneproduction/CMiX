// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Modifiers.Transform;
using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.Texturing.Sampling;
using CMiX.Core.Presentations.Texturing.Sources;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Texturing;
using CommunityToolkit.Mvvm.ComponentModel;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.BaseControl;

namespace CMiX.Core.Presentations.Texturing
{
    public class Texture : ObservableObject, ITexture, IPrefab
    {
        public Texture(TextureModel textureModel, CompositionService compositionService)
        {
            this.ID = textureModel.ID;
            Name = new StringValue(textureModel.Name);
            IsSelected = new BooleanValue(textureModel.IsSelected);
            IsRenaming = new BooleanValue(textureModel.IsRenaming);

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

        public BooleanValue IsSelected { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public StringValue Name { get; set; }
        public IntegerValue SelectedAssetType { get; set; }
        public TypeWriter TypeWriter { get; set; }
        public VideoIn VideoIn { get; set; }
        public VideoPlayer VideoPlayer { get; set; }
        public ProceduralSelector ProceduralSelector { get; set; }
    }
}
