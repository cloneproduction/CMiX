// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Modifiers.Transform;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.Texturing.Sampling;
using CMiX.Core.Presentations.Texturing.Sources;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.Texturing
{
    public partial class Mask : ObservableObject, ITexture
    {
        public Mask(MaskModel maskModel, CompositionService compositionService)
        {
            ID = maskModel.ID;

            IsEnabled = new BooleanValue(maskModel.IsEnabled, compositionService);
            TextureModifierManager = new ModifierManager(maskModel.ModifierManagerModel, new TextureFilterFactory(compositionService), compositionService);
            TextureTransformModifierManager = new ModifierManager(maskModel.TextureTransformModifierManager, new ModifierFactory(compositionService), compositionService);
            SamplerState = new SamplerState(maskModel.SamplerState, compositionService);
            Invert = new BooleanValue(maskModel.Invert, compositionService);
            VideoIn = new VideoIn(maskModel.VideoIn, compositionService);
            VideoPlayer = new VideoPlayer(maskModel.VideoPlayerModel, compositionService);
            SelectedAssetType = new IntegerValue(maskModel.SelectedAssetType, compositionService);
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


        [ObservableProperty]
        private bool isExpanded;
    }
}
