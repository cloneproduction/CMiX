// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Sources
{
    public class CheckerBoard : ObservableObject, ITextureSource, IPrefab
    {
        public CheckerBoard(PrefabService prefabService,
                            PrefabManager filterManager,
                            Integer2 resolution, 
                            Vector2 cellCount,
                            GenericValue<string> colorA,
                            GenericValue<string> colorB,
                            Transform2D transform2D)
        {
            ID = Guid.NewGuid();
            PrefabService = prefabService;
            Resolution = resolution;
            ColorA = colorA;
            ColorB = colorB;
            CellCount = cellCount;
            FilterManager = filterManager;
            Transform2D = transform2D;
        }

        public Guid ID { get; set; }
        public Integer2 Resolution { get; set; }
        public PrefabService PrefabService { get; set; }
        public Transform2D Transform2D { get; set; }
        public Vector2 CellCount { get; set; }
        public GenericValue<string> ColorA { get; set; }
        public GenericValue<string> ColorB { get; set; }
        public PrefabManager FilterManager { get; set; }
    }
}
