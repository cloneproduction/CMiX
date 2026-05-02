// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;
using CMiX.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Sources
{
    public partial class CheckerBoard : ObservableObject, ITextureSource
    {
        public CheckerBoard(PrefabService prefabService,
                            PrefabManager filterManager,
                            Integer2 resolution, 
                            Vector2 cellCount,
                            GenericValue<string> colorA,
                            GenericValue<string> colorB,
                            Transform2D transform2D)
        {
            PrefabService = prefabService;
            Resolution = resolution;
            ColorA = colorA;
            ColorB = colorB;
            CellCount = cellCount;
            FilterManager = filterManager;
            Transform2D = transform2D;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Integer2 Resolution { get; set; }
        public PrefabService PrefabService { get; set; }
        public Transform2D Transform2D { get; set; }
        public Vector2 CellCount { get; set; }
        public GenericValue<string> ColorA { get; set; }
        public GenericValue<string> ColorB { get; set; }
        public PrefabManager FilterManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new CheckerBoardModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            FilterManager = (PrefabManagerModel)FilterManager.ToModel(),
            Resolution = (Integer2Model)Resolution.ToModel(),
            Transform2D = (Transform2DModel)Transform2D.ToModel(),
            CellCount = (Vector2Model)CellCount.ToModel(),
            ColorA = (GenericValueModel<string>)ColorA.ToModel(),
            ColorB = (GenericValueModel<string>)ColorB.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (CheckerBoardModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Resolution.FromModel(m.Resolution);
            Transform2D.FromModel(m.Transform2D);
            CellCount.FromModel(m.CellCount);
            ColorA.FromModel(m.ColorA);
            ColorB.FromModel(m.ColorB);

            LoadManager(FilterManager, m.FilterManager);
        }
    }
}
