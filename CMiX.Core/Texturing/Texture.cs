// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefab;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sampling;
using CMiX.Core.Texturing.Sources;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing
{
    public partial class Texture : ObservableObject, ITexture, IPrefab, IModifiable
    {
        public Texture()
        {
            Name = new StringValue();
            IsSelected = new BooleanValue();
            IsRenaming = new BooleanValue();
            IsEnabled = new BooleanValue();
            ModifierManager = new ModifierManager(new TextureFilterFactory());
            TextureSourceSelector = new TextureSourceSelector();
            VideoIn = new VideoIn();
            VideoPlayer = new VideoPlayer();
            SelectedAssetType = new IntegerValue();
            TypeWriter = new TypeWriter();
            TransformTexture = new TransformTexture();
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public ModifierManager ModifierManager { get; set; }
        public BooleanValue IsEnabled { get; set; }
        public TransformTexture TransformTexture { get; set; }
        public BooleanValue IsSelected { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public StringValue Name { get; set; }
        public IntegerValue SelectedAssetType { get; set; }
        public TypeWriter TypeWriter { get; set; }
        public VideoIn VideoIn { get; set; }
        public VideoPlayer VideoPlayer { get; set; }
        public TextureSourceSelector TextureSourceSelector { get; set; }
        public SamplerState SamplerState { get; set; }

        [ObservableProperty]
        private bool isExpanded = false;
    }
}
