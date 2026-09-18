// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Compositing
{
    public partial class Entity : ObservableObject, IPrefab, IModifiable, IDisposable, IHasCompositionID
    {
        public Entity(PrefabService prefabService,
                      Mesh mesh,
                      TransformSRT transformSRT,
                      PrefabSelector materialSelector,
                      PrefabManager modifierManager,
                      Color color)
        {
            ID = prefabService.ID;
            PrefabService = prefabService;
            Mesh = mesh;
            TransformSRT = transformSRT;
            MaterialSelector = materialSelector;
            ModifierManager = modifierManager;
            Color = color;

            // MaterialSelector has no added-item event to watch, only a change notification, and
            // that notification does not fire when a project load sets the material directly. So
            // the initial material is stamped by the CompositionID setter below instead, and this
            // handler only has to catch a material picked or swapped in afterward, live.
            MaterialSelector.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(PrefabSelector.SelectedItem) &&
                    MaterialSelector.SelectedItem is IHasCompositionID material)
                    material.CompositionID = _compositionID;
            };
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public PrefabManager ModifierManager { get; set; }
        public PrefabSelector MaterialSelector { get; set; }
        public TransformSRT TransformSRT { get; set; }
        public Mesh Mesh { get; set; }
        public Color Color { get; set; }

        private Guid _compositionID;
        public Guid CompositionID
        {
            get => _compositionID;
            set
            {
                _compositionID = value;
                new CompositionIDAssigner(ModifierManager, value);
                if (MaterialSelector.SelectedItem is IHasCompositionID material)
                    material.CompositionID = value;
            }
        }

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
            MaterialSelector = (PrefabSelectorModel)MaterialSelector.ToModel()
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
            MaterialSelector.FromModel(m.MaterialSelector);
        }

        // The mesh owns two texture managers of its own, and nothing else reaches its teardown.
        // MaterialSelector is disposed rather than only cleared, so the material the entity held
        // is torn down with its own texture slots instead of outliving the entity.
        public void Dispose() => DisposeAll(ModifierManager, Mesh, MaterialSelector);
    }
}
