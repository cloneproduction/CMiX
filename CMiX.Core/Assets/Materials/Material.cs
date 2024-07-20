// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Materials
{
    public partial class Material : ObservableObject, IPrefab
    {
        public Material(PrefabService prefabService,
                        PrefabManager explodeTriangleTextureManager,
                        GenericValue<float> explodeStrength,
                        MaterialSettings materialSettings, 
                        DiffuseTexture diffuseTexture, 
                        MaskTexture maskTexture)
        {
            ID = Guid.NewGuid();
            PrefabService = prefabService;
            DiffuseTexture = diffuseTexture;
            MaskTexture = maskTexture;
            MaterialSettings = materialSettings;
            ExplodeTriangleTextureManager = explodeTriangleTextureManager;
            ExplodeStrength = explodeStrength;
        }

        public Guid ID { get; set; } 
        public DiffuseTexture DiffuseTexture { get; set; }
        public MaskTexture MaskTexture { get; set; }
        public MaterialSettings MaterialSettings { get; set; }
        public PrefabService PrefabService { get; set; }

        public PrefabManager ExplodeTriangleTextureManager { get; set; }
        public GenericValue<float> ExplodeStrength { get; set; }


        [ObservableProperty]
        private bool isExpanded = false;
    }
}
