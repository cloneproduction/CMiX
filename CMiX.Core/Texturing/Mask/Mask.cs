// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sampling;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Transformation.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing
{
    public partial class Mask : ObservableObject, ITexture
    {
        public Mask(MaskModel maskModel)
        {
            ID = maskModel.ID;
            IsEnabled = new BooleanValue(maskModel.IsEnabled);
            ModifierManager = new ModifierManager(maskModel.ModifierManager, new TextureFilterFactory());
            TextureTransformModifierManager = new ModifierManager(maskModel.TextureTransformModifierManager, new ModifierFactory());
            SamplerState = new SamplerState(maskModel.SamplerState);
            Invert = new BooleanValue(maskModel.Invert);
            VideoIn = new VideoIn(maskModel.VideoIn);
            VideoPlayer = new VideoPlayer(maskModel.VideoPlayer);
            SelectedAssetType = new IntegerValue(maskModel.SelectedAssetType);
            TypeWriter = new TypeWriter(maskModel.TypeWriter);
            TextureSourceSelector = new TextureSourceSelector(maskModel.TextureSourceSelector);
            TransformTexture = new TransformTexture(maskModel.TransformTexture);

            isExpanded = false;
        }

        public Guid ID { get; set; }
        public ModifierManager ModifierManager { get; set; }
        public ModifierManager TextureTransformModifierManager { get; set; }
        public BooleanValue IsEnabled { get; set; }
        public BooleanValue Invert { get; set; }
        public SamplerState SamplerState { get; set; }
        public TransformTexture TransformTexture { get; set; }
        public IntegerValue SelectedAssetType { get; set; }
        public TypeWriter TypeWriter { get; set; }
        public VideoIn VideoIn { get; set; }
        public VideoPlayer VideoPlayer { get; set; }
        public TextureSourceSelector TextureSourceSelector { get; set; }


        [ObservableProperty]
        private bool isExpanded;
    }
}
