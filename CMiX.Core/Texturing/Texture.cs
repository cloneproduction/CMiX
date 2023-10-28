// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Collections;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing.Sources;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing
{
    public partial class Texture : ObservableObject, IPrefab, IModifiable
    {
        public Texture(PrefabService prefabService, ModifierManager modifierManager)
        {
            ID = prefabService.ID;
            Name = prefabService.Name;
            IsSelected = prefabService.IsSelected;
            IsRenaming = prefabService.IsRenaming;
            Visibility = prefabService.Visibility;

            TextureSourceSelector = new TextureSourceSelector();
            Gradient = new Gradient();
            BubbleNoise = new BubbleNoise();
            Image = new Image();
            VideoIn = new VideoIn();
            VideoPlayer = new VideoPlayer();
            SelectedAssetType = new IntegerValue();
            TypeWriter = new TypeWriter();

            ModifierManager = modifierManager;
        }

        public Guid ID { get; set; }
        public StringValue Name { get; set; }
        public BooleanValue IsSelected { get; set; }
        public BooleanValue IsRenaming { get; set; }
        public BooleanValue Visibility { get; set; }

        public BubbleNoise BubbleNoise { get; set; }
        public Image Image { get; set; }
        public TextureSourceSelector TextureSourceSelector { get; set; }
        public IntegerValue SelectedAssetType { get; set; }
        public TypeWriter TypeWriter { get; set; }
        public Gradient Gradient { get; set; }

        public VideoIn VideoIn { get; set; }
        public VideoPlayer VideoPlayer { get; set; }
        public ICollectionManager ModifierManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = false;
    }
}
