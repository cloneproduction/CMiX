// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Assets;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sampling;
using CMiX.Core.Texturing.Sources;

namespace CMiX.Core.Texturing
{
    public class MaskModel : IControlModel
    {
        public MaskModel()
        {
            ID = Guid.NewGuid();
            VideoPlayer = new VideoPlayerModel();
            ModifierManager = new ModifierManagerModel();
            TextureTransformModifierManager = new ModifierManagerModel();
            SamplerState = new SamplerStateModel();
            VideoIn = new VideoInModel();
            Invert = new BooleanValueModel();
            IsEnabled = new BooleanValueModel();
            SelectedAssetType = new IntegerValueModel(0);
            TypeWriter = new TypeWriterModel();
            TextureSourceSelector = new TextureSourceSelectorModel();
            TransformTexture = new TransformTextureModel();
        }

        public Guid ID { get; set; }
        public ModifierManagerModel ModifierManager { get; set; }
        public ModifierManagerModel TextureTransformModifierManager { get; set; }
        public BooleanValueModel IsEnabled { get; set; }
        public VideoPlayerModel VideoPlayer { get; set; }
        public VideoInModel VideoIn { get; internal set; }
        public IntegerValueModel SelectedAssetType { get; internal set; }
        public TypeWriterModel TypeWriter { get; internal set; }
        public SamplerStateModel SamplerState { get; internal set; }
        public BooleanValueModel Invert { get; internal set; }
        public TextureSourceSelectorModel TextureSourceSelector { get; set; }
        public TransformTextureModel TransformTexture { get; set; }
    }
}
