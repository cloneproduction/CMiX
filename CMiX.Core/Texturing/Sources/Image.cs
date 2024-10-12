// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.ViewModels;
using CMiX.Core.ViewModels.Assets;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Sources
{
    public partial class Image : ObservableObject, ITextureSource, IPrefab
    {
        public Image(PrefabService prefabService, 
                     PrefabManager filterManager,
                     Integer2 resolution, 
                     GenericValue<IAsset> asset)
        {
            ID = Guid.NewGuid();
            PrefabService = prefabService;
            FilterManager = filterManager;
            Resolution = resolution;
            Asset = asset;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public Integer2 Resolution { get; set; }
        public GenericValue<IAsset> Asset { get; set; }
        public PrefabManager FilterManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
