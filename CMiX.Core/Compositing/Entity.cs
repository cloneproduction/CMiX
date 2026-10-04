// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Materials;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing;
using CMiX.Core.Transformation;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Compositing
{
    public partial class Entity : ObservableObject, IPrefab, ITexturable, IModifiable, IDisposable, IHasCompositionID
    {
        public Entity(PrefabService prefabService,
                      Mesh mesh,
                      TransformSRT transformSRT,
                      PrefabManager modifierManager,
                      Material material,
                      Texture texture,
                      Color color)
        {
            ID = prefabService.ID;
            PrefabService = prefabService;
            Mesh = mesh;
            TransformSRT = transformSRT;
            
            ModifierManager = modifierManager;
            Color = color;
            Texture = texture;
            Material = material;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public PrefabManager ModifierManager { get; set; }
        public TransformSRT TransformSRT { get; set; }
        public Mesh Mesh { get; set; }
        public Color Color { get; set; }
        public Texture Texture { get; set; }
        public Material Material { get; set; }


        private Guid _compositionID;
        public Guid CompositionID
        {
            get => _compositionID;
            set
            {
                _compositionID = value;
                new CompositionIDAssigner(ModifierManager, value);
                Texture.CompositionID = value;
            }
        }

        [ObservableProperty]
        private bool transformSRTIsExpanded = false;

        [ObservableProperty]
        private bool modifierManagerIsExpanded = false;

        [ObservableProperty]
        private bool materialIsExpanded = false;

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
            Texture = (TextureModel)Texture.ToModel(),
            Material = (MaterialModel)Material.ToModel(),
        };  

        public void FromModel(IControlModel model)
        {
            var m = (EntityModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Mesh.FromModel(m.Mesh);
            TransformSRT.FromModel(m.TransformSRT);
            Color.FromModel(m.Color);
            Texture.FromModel(m.Texture);
            Material.FromModel(m.Material);

            LoadManager(ModifierManager, m.ModifierManager);
        }

        public void Dispose() => DisposeAll(ModifierManager, Mesh, Material,
                                            Texture.DiffuseTexture.TextureManager, Texture.MaskTexture.TextureManager);
    }
}
