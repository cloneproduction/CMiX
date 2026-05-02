// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Sources
{
    public partial class VideoIn : ObservableRecipient, ITextureSource
    {
        public VideoIn(PrefabService prefabService, 
                       PrefabManager filterManager,
                       GenericValue<int> sizeX, 
                       GenericValue<int> sizeZ)
        {
            FilterManager = filterManager;
            PrefabService = prefabService;
            SizeX = sizeX;
            SizeY = sizeZ;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<int> SizeX { get; set; }
        public GenericValue<int> SizeY { get; set; }
        public PrefabService PrefabService { get; set; }
        public Integer2 Resolution { get; set; }
        public PrefabManager FilterManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new VideoInModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            FilterManager = (PrefabManagerModel)FilterManager.ToModel(),
            SizeX = (GenericValueModel<int>)SizeX.ToModel(),
            SizeY = (GenericValueModel<int>)SizeY.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (VideoInModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            SizeX.FromModel(m.SizeX);
            SizeY.FromModel(m.SizeY);

            LoadManager(FilterManager, m.FilterManager);
        }
    }
}
