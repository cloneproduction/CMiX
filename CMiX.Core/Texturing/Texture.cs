// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefab;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sources;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing
{
    public partial class Texture : ObservableObject, IPrefab, IModifiable
    {
        public Texture()
        {
            Name = new StringValue();
            IsSelected = new BooleanValue();
            IsRenaming = new BooleanValue();

            TextureSourceSelector = new TextureSourceSelector();
            VideoIn = new VideoIn();
            VideoPlayer = new VideoPlayer();
            SelectedAssetType = new IntegerValue();
            TypeWriter = new TypeWriter();
            ModifierManager = new ModifierManager(new TextureFilterFactory());
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public StringValue Name { get; set; }
        public BooleanValue IsSelected { get; set; }
        public BooleanValue IsRenaming { get; set; }

        public TextureSourceSelector TextureSourceSelector { get; set; }
        public IntegerValue SelectedAssetType { get; set; }
        public TypeWriter TypeWriter { get; set; }
        public VideoIn VideoIn { get; set; }
        public VideoPlayer VideoPlayer { get; set; }
        public ModifierManager ModifierManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = false;
    }
}
