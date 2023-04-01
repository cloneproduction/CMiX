// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.Prefabs;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.Modifiers;
using CMiX.Core.Presentations.Texturing.Sampling;
using CMiX.Core.Presentations.Texturing.Sources;
using CMiX.Core.Texturing.Filters;

namespace CMiX.Core.Presentations.Texturing
{
    public class TextureModel : IPrefabModel
    {
        public TextureModel()
        {
            ID = Guid.NewGuid();

            ProceduralSelector = new ProceduralSelectorModel();
            VideoPlayer = new VideoPlayerModel();

            TextureModifierManager = new ModifierManagerModel();
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

        public ModifierManagerModel TextureModifierManager { get; set; }
        public ModifierManagerModel TextureTransformModifierManager { get; set; }

        public BooleanValueModel IsEnabled { get; internal set; }
        public VideoPlayerModel VideoPlayer { get; set; }
        public VideoInModel VideoIn { get; internal set; }
        public IntegerValueModel SelectedAssetType { get; internal set; }
        public TypeWriterModel TypeWriter { get; internal set; }
        public SamplerStateModel SamplerState { get; internal set; }
        public ProceduralSelectorModel ProceduralSelector { get; internal set; }
        public TransformTextureModel TransformTexture { get; internal set; }
        public StringValueModel Name { get; internal set; }
        public BooleanValueModel IsSelected { get; internal set; }
        public BooleanValueModel IsRenaming { get; internal set; }
    }
}
