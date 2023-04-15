// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sampling;
using CMiX.Core.Texturing.Sources;
using CMiX.Core.Transformation.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing
{
    public partial class Texture : ObservableObject, ITexture, IPrefab
    {
        public Texture(TextureModel textureModel)
        {
            this.ID = textureModel.ID;
            Name = new StringValue(textureModel.Name);
            IsSelected = new BooleanValue(textureModel.IsSelected);
            IsRenaming = new BooleanValue(textureModel.IsRenaming);
            IsEnabled = new BooleanValue(textureModel.IsEnabled);
            TextureModifierManager = new ModifierManager(textureModel.TextureModifierManager, new TextureFilterFactory());
            TextureTransformModifierManager = new ModifierManager(textureModel.TextureTransformModifierManager, new ModifierFactory());
            ProceduralSelector = new ProceduralSelector(textureModel.ProceduralSelector);
            VideoIn = new VideoIn(textureModel.VideoIn);
            VideoPlayer = new VideoPlayer(textureModel.VideoPlayer);
            SelectedAssetType = new IntegerValue(textureModel.SelectedAssetType);
            TypeWriter = new TypeWriter(textureModel.TypeWriter);
            TransformTexture = new TransformTexture(textureModel.TransformTexture);
            isExpanded = false;
        }

        public Guid ID { get; set; }
        public ModifierManager TextureModifierManager { get; set; }
        public ModifierManager TextureTransformModifierManager { get; set; }
        public BooleanValue IsEnabled { get; set; }
        public TransformTexture TransformTexture { get; set; }
        public BooleanValue IsSelected { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public StringValue Name { get; set; }
        public IntegerValue SelectedAssetType { get; set; }
        public TypeWriter TypeWriter { get; set; }
        public VideoIn VideoIn { get; set; }
        public VideoPlayer VideoPlayer { get; set; }
        public ProceduralSelector ProceduralSelector { get; set; }
        public SamplerState SamplerState { get; set; }

        [ObservableProperty]
        private bool isExpanded;
    }
}
