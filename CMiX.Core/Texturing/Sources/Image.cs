// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Sources
{
    public partial class Image : ObservableObject, ITextureSource
    {
        public Image(PrefabService prefabService, 
                     PrefabManager filterManager,
                     Integer2 resolution, 
                     AssetSelector assetSelector)
        {
            PrefabService = prefabService;
            FilterManager = filterManager;
            Resolution = resolution;
            AssetSelector = assetSelector;
        }

        public IControlModel ToModel() => this.ToModel();
        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public Integer2 Resolution { get; set; }
        public PrefabManager FilterManager { get; set; }
        public AssetSelector AssetSelector { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
