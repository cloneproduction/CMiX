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
        public Mask()
        {
            IsEnabled = new BooleanValue();
            ModifierManager = new ModifierManager(new TextureFilterFactory());
            TextureTransformModifierManager = new ModifierManager(new ModifierFactory());
            SamplerState = new SamplerState();
            Invert = new BooleanValue();
            VideoIn = new VideoIn();
            VideoPlayer = new VideoPlayer();
            SelectedAssetType = new IntegerValue();
            TypeWriter = new TypeWriter();
            TextureSourceSelector = new TextureSourceSelector();
            TransformTexture = new TransformTexture();
        }

        public Guid ID { get; set; } = Guid.NewGuid();
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
        private bool isExpanded = false;
    }
}
