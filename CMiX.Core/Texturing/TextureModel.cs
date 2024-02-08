// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;

namespace CMiX.Core.Texturing
{
    public class TextureModel : IPrefabModel
    {
        public TextureModel()
        {
            PrefabService = new PrefabServiceModel();
            ID = PrefabService.ID;

            Visibility = new GenericValueModel<bool>(true);
            TextureSourceSelector = new TextureSourceSelectorModel();
            VideoPlayer = new VideoPlayerModel();
            ModifierManager = new PrefabManagerModel();
            VideoIn = new VideoInModel();
            IsEnabled = new GenericValueModel<bool>();
            SelectedAssetType = new GenericValueModel<int>(0);
            SamplerState = new SamplerStateModel();
            TypeWriter = new TypeWriterModel();
            TransformTexture = new TransformTextureModel();
            Name = new GenericValueModel<string>("Texture");
            IsSelected = new GenericValueModel<bool>(false);
            IsRenaming = new GenericValueModel<bool>(false);
            Gradient = new GradientModel();
            BubbleNoise = new BubbleNoiseModel();
            Image = new ImageModel();
        }

        public Guid ID { get; set; }

        public PrefabServiceModel PrefabService { get; set; }
        public PrefabManagerModel ModifierManager { get; set; }
        public GenericValueModel<bool> IsEnabled { get; set; }

        public GradientModel Gradient { get; set; }
        public ImageModel Image { get; set; }
        public BubbleNoiseModel BubbleNoise { get; set; }
        public VideoPlayerModel VideoPlayer { get; set; }
        public VideoInModel VideoIn { get; set; }
        public GenericValueModel<int> SelectedAssetType { get; set; }
        public TypeWriterModel TypeWriter { get; set; }
        public SamplerStateModel SamplerState { get; set; }
        public TextureSourceSelectorModel TextureSourceSelector { get; set; }
        public TransformTextureModel TransformTexture { get; set; }
        public GenericValueModel<string> Name { get; set; }
        public GenericValueModel<bool> IsSelected { get; set; }
        public GenericValueModel<bool> IsRenaming { get; set; }
        public GenericValueModel<bool> Visibility { get; set; }
    }
}
