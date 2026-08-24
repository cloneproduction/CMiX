// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Materials
{
    public partial class Material : ObservableObject, IPrefab, IModifiable, IDisposable
    {
        public Material(PrefabService prefabService,
                        MaterialSettings materialSettings, 
                        DiffuseTexture diffuseTexture, 
                        MaskTexture maskTexture,
                        PrefabManager modifierManager)
        {
            PrefabService = prefabService;
            DiffuseTexture = diffuseTexture;
            MaskTexture = maskTexture;
            MaterialSettings = materialSettings;
            ModifierManager = modifierManager;
        }

        public Guid ID { get; set; } 
        public DiffuseTexture DiffuseTexture { get; set; }
        public MaskTexture MaskTexture { get; set; }
        public MaterialSettings MaterialSettings { get; set; }
        public PrefabService PrefabService { get; set; }
        public PrefabManager ModifierManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = false;

        [ObservableProperty]
        private bool modifierManagerIsExpanded = false;

        public IControlModel ToModel() => new MaterialModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            DiffuseTexture = (DiffuseTextureModel)DiffuseTexture.ToModel(),
            MaskTexture = (MaskTextureModel)MaskTexture.ToModel(),
            MaterialSettings = (MaterialSettingsModel)MaterialSettings.ToModel(),
            ModifierManager = (PrefabManagerModel)ModifierManager.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (MaterialModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            DiffuseTexture.FromModel(m.DiffuseTexture);
            MaskTexture.FromModel(m.MaskTexture);
            MaterialSettings.FromModel(m.MaterialSettings);

            LoadManager(ModifierManager, m.ModifierManager);
        }

        // The two texture slots own a texture manager each, and nothing else reaches their
        // teardown, the same way the mesh owns two of its own under an entity.
        public void Dispose() => DisposeAll(ModifierManager, DiffuseTexture.TextureManager, MaskTexture.TextureManager);
    }
}
