// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Prefabs;
using CMiX.Core.Texturing;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Materials
{
    public partial class Material : ObservableObject, IPrefab, IDisposable, IHasCompositionID
    {
        public Material(PrefabService prefabService,
                        MaterialSettings materialSettings, 
                        DiffuseTexture diffuseTexture, 
                        MaskTexture maskTexture)
        {
            PrefabService = prefabService;
            DiffuseTexture = diffuseTexture;
            MaskTexture = maskTexture;
            MaterialSettings = materialSettings;
        }

        public Guid ID { get; set; } 
        public DiffuseTexture DiffuseTexture { get; set; }
        public MaskTexture MaskTexture { get; set; }
        public MaterialSettings MaterialSettings { get; set; }
        public PrefabService PrefabService { get; set; }

        private Guid _compositionID;
        public Guid CompositionID
        {
            get => _compositionID;
            set
            {
                _compositionID = value;
                DiffuseTexture.CompositionID = value;
                MaskTexture.CompositionID = value;
            }
        }

        [ObservableProperty]
        private bool isExpanded = false;

        public IControlModel ToModel() => new MaterialModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            DiffuseTexture = (DiffuseTextureModel)DiffuseTexture.ToModel(),
            MaskTexture = (MaskTextureModel)MaskTexture.ToModel(),
            MaterialSettings = (MaterialSettingsModel)MaterialSettings.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (MaterialModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            DiffuseTexture.FromModel(m.DiffuseTexture);
            MaskTexture.FromModel(m.MaskTexture);
            MaterialSettings.FromModel(m.MaterialSettings);
        }

        // The two texture slots own a texture manager each, and nothing else reaches their
        // teardown, the same way the mesh owns two of its own under an entity.
        public void Dispose() => DisposeAll(DiffuseTexture.TextureManager, MaskTexture.TextureManager);
    }
}
