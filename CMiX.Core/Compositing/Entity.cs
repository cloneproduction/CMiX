// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Colors;
using CMiX.Core.Materials;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Compositing
{
    public partial class Entity : ObservableObject, IControl, IPrefab, IModifiable
    {
        public Entity(PrefabService prefabService, 
                      Mesh mesh, 
                      Material material,
                      TransformSRT transformSRT,
                      PrefabManager materialManager,
                      PrefabManager modifierManager,
                      PrefabManager colorPaletteManager)
        {
            ID = prefabService.ID;
            PrefabService = prefabService;
            Mesh = mesh;
            TransformSRT = transformSRT;
            Material = material;
            MaterialManager = materialManager;
            ModifierManager = modifierManager;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public PrefabManager ModifierManager { get; set; }
        public PrefabManager MaterialManager { get; set; }
        public TransformSRT TransformSRT { get; set; }
        public Mesh Mesh { get; set; }
        public Material Material { get; set; }


        [ObservableProperty]
        private bool transformSRTIsExpanded = true;

        [ObservableProperty]
        private bool modifierManagerIsExpanded = true;

        [ObservableProperty]
        private bool materialManagerIsExpanded = true;

        [ObservableProperty]
        private bool colorPaletteManagerIsExpanded = true;

        [ObservableProperty]
        private bool meshIsExpanded = true;
    }
}
