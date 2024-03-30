// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;

namespace CMiX.Core.Texturing
{
    public class MaskModel : IControlModel
    {
        public MaskModel()
        {
            VideoPlayer = new VideoPlayerModel();
            ModifierManager = new PrefabManagerModel();
            TextureTransformModifierManager = new PrefabManagerModel();
            SamplerState = new SamplerStateModel();
            VideoIn = new VideoInModel();
            Invert = new GenericValueModel<bool>();
            IsEnabled = new GenericValueModel<bool>();
            SelectedAssetType = new GenericValueModel<int>(0);
            TypeWriter = new TypeWriterModel();
            TransformTexture = new TransformTextureModel();

            MaskChannel = new GenericValueModel<MaskChannel>();
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<MaskChannel> MaskChannel { get; set; }
        public PrefabManagerModel ModifierManager { get; set; }
        public PrefabManagerModel TextureTransformModifierManager { get; set; }
        public GenericValueModel<bool> IsEnabled { get; set; }
        public VideoPlayerModel VideoPlayer { get; set; }
        public VideoInModel VideoIn { get; internal set; }
        public GenericValueModel<int> SelectedAssetType { get; internal set; }
        public TypeWriterModel TypeWriter { get; internal set; }
        public SamplerStateModel SamplerState { get; internal set; }
        public GenericValueModel<bool> Invert { get; internal set; }
        public TransformTextureModel TransformTexture { get; set; }
    }
}
