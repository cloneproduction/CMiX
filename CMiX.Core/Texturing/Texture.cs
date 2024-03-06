// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Sources;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing
{
    public partial class Texture : ObservableObject, IControl, IPrefab//, IModifiable
    {
        public Texture(PrefabService prefabService,
                       PrefabManager modifierManager,
                       TextureSourceSelector textureSourceSelector,
                       Gradient gradient,
                       BubbleNoise bubbleNoise,
                       Image image,
                       VideoIn videoIn,
                       VideoPlayer videoPlayer,
                       GenericValue<int> selectedAssetType,
                       TypeWriter typeWriter
                      )
        {
            ID = prefabService.ID;
            PrefabService = prefabService;

            TextureSourceSelector = textureSourceSelector;
            Gradient = gradient;
            BubbleNoise = bubbleNoise;
            Image = image;
            VideoIn = videoIn;
            VideoPlayer = videoPlayer;
            SelectedAssetType = selectedAssetType;
            TypeWriter = typeWriter;
            ModifierManager = modifierManager;
        }

        public Guid ID { get; set; }

        public PrefabService PrefabService { get; set; }

        public BubbleNoise BubbleNoise { get; set; }
        public Image Image { get; set; }
        public TextureSourceSelector TextureSourceSelector { get; set; }
        public GenericValue<int> SelectedAssetType { get; set; }
        public TypeWriter TypeWriter { get; set; }
        public Gradient Gradient { get; set; }

        public VideoIn VideoIn { get; set; }
        public VideoPlayer VideoPlayer { get; set; }
        public PrefabManager ModifierManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = false;
    }
}
