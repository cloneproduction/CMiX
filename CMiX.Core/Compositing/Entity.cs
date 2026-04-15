// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;
using CMiX.Core.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Compositing
{
    public partial class Entity : ObservableObject, IPrefab, IModifiable
    {
        public Entity(PrefabService prefabService, 
                      Mesh mesh,
                      TransformSRT transformSRT,
                      PrefabManager materialManager,
                      PrefabManager modifierManager,
                      Color color)
        {
            ID = prefabService.ID;
            PrefabService = prefabService;
            Mesh = mesh;
            TransformSRT = transformSRT;
            MaterialManager = materialManager;
            ModifierManager = modifierManager;
            Color = color;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public PrefabManager ModifierManager { get; set; }
        public PrefabManager MaterialManager { get; set; }
        public TransformSRT TransformSRT { get; set; }
        public Mesh Mesh { get; set; }
        public Color Color { get; set; }

        [ObservableProperty]
        private bool transformSRTIsExpanded = false;

        [ObservableProperty]
        private bool modifierManagerIsExpanded = false;

        [ObservableProperty]
        private bool materialManagerIsExpanded = false;

        [ObservableProperty]
        private bool colorPaletteManagerIsExpanded = false;

        [ObservableProperty]
        private bool meshIsExpanded = false;

        public IControlModel ToModel() => new EntityModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Mesh = (MeshModel)Mesh.ToModel(),
            TransformSRT = (TransformSRTModel)TransformSRT.ToModel(),
            Color = (ColorModel)Color.ToModel(),
            ModifierManager = (PrefabManagerModel)ModifierManager.ToModel(),
            MaterialManager = (PrefabManagerModel)MaterialManager.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (EntityModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Mesh.FromModel(m.Mesh);
            TransformSRT.FromModel(m.TransformSRT);
            Color.FromModel(m.Color);

            LoadManager(ModifierManager, m.ModifierManager);
            LoadManager(MaterialManager, m.MaterialManager);
        }
    }
}
