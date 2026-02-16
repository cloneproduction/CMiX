// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Materials
{
    public partial class Material : ObservableObject, IPrefab, IModifiable
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

        public IControlModel ToModel() => this.ToModel();
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
    }
}
