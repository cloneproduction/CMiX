// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sampling;

namespace CMiX.Core.Texturing
{
    public class TextureModel : IPrefabModel
    {
        public TextureModel()
        {
            ID = Guid.NewGuid();
            TextureSourceSelector = new TextureSourceSelectorModel();
            VideoPlayer = new VideoPlayerModel();
            ModifierManager = new ModifierManagerModel();
            TextureTransformModifierManager = new ModifierManagerModel();
            VideoIn = new VideoInModel();
            IsEnabled = new BooleanValueModel();
            SelectedAssetType = new IntegerValueModel(0);
            SamplerState = new SamplerStateModel();
            TypeWriter = new TypeWriterModel();
            TransformTexture = new TransformTextureModel();
            Name = new StringValueModel("Texture");
            IsSelected = new BooleanValueModel(false);
            IsRenaming = new BooleanValueModel(false);
        }

        public Guid ID { get; set; }
        public ModifierManagerModel ModifierManager { get; set; }
        public ModifierManagerModel TextureTransformModifierManager { get; set; }
        public BooleanValueModel IsEnabled { get; set; }
        public VideoPlayerModel VideoPlayer { get; set; }
        public VideoInModel VideoIn { get; set; }
        public IntegerValueModel SelectedAssetType { get; set; }
        public TypeWriterModel TypeWriter { get; set; }
        public SamplerStateModel SamplerState { get; set; }
        public TextureSourceSelectorModel TextureSourceSelector { get; set; }
        public TransformTextureModel TransformTexture { get; set; }
        public StringValueModel Name { get; set; }
        public BooleanValueModel IsSelected { get; set; }
        public BooleanValueModel IsRenaming { get; set; }
    }
}
