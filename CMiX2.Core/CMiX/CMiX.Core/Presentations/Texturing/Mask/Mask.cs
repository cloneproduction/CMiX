// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Service;
using CMiX.Core.Texturing.Sources.TypeWriter;
using CMiX.Core.Texturing.Sources.VideoIn;
using CMiX.Core.Texturing.Sources.VideoPlayer;
using CMiX.Core.Presentations.Texturing.Sampling;
using CommunityToolkit.Mvvm.ComponentModel;
using CMiX.Core.Presentations.Modifiers.Transform;

namespace CMiX.Core.Presentations.Texturing.Mask
{
    public class Mask : ObservableObject, ITexture
    {
        public Mask(MaskModel maskModel, CompositionService compositionService)
        {
            ID = maskModel.ID;

            IsEnabled = new BooleanValue(maskModel.IsEnabled);
            TextureModifierManager = new ModifierManager(maskModel.ModifierManagerModel, new TextureFilterFactory(compositionService), compositionService);
            TextureTransformModifierManager = new ModifierManager(maskModel.TextureTransformModifierManager, new ModifierFactory(compositionService), compositionService);
            SamplerState = new SamplerState(maskModel.SamplerState, compositionService);
            Invert = new BooleanValue(maskModel.Invert);

            VideoIn = new VideoIn(maskModel.VideoIn);
            VideoPlayer = new VideoPlayer(maskModel.VideoPlayerModel);
            SelectedAssetType = new IntegerValue(maskModel.SelectedAssetType);
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
        public IntegerValue SelectedAssetType { get; set; }
        public TypeWriter TypeWriter { get; set; }
        public VideoIn VideoIn { get; set; }
        public VideoPlayer VideoPlayer { get; set; }
        public ProceduralSelector ProceduralSelector { get; set; }


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }
    }
}
