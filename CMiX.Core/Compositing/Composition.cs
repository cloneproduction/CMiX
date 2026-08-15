// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering;
using CMiX.Core.Texturing;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Compositing
{
    public partial class Composition : ObservableObject, IPrefab, ITextureModifiable, IModifiable, IDisposable
    {
        public Composition(PrefabService prefabService, 
                           PrefabManager prefabManager,
                           PrefabManager textureModifierManager, 
                           PrefabManager modifierManager,
                           OutputSettings outputSettings)
        {
            ID = prefabService.ID;
            PrefabService = prefabService;
            OutputSettings = outputSettings;
            LayerManager = prefabManager;
            ModifierManager = modifierManager;
            TextureModifierManager = textureModifierManager;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public PrefabManager LayerManager { get; set; }
        public PrefabManager TextureModifierManager { get; set; }
        public OutputSettings OutputSettings { get; set; }
        public PrefabManager ModifierManager { get; set; }


        [ObservableProperty]
        private bool textureModifierIsExpanded = false;

        [ObservableProperty]
        private bool outputSettingsIsExpanded = false;

        public IControlModel ToModel() => new CompositionModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            OutputSettings = (OutputSettingsModel)OutputSettings.ToModel(),
            TextureModifierManager = (PrefabManagerModel)TextureModifierManager.ToModel(),
            LayerManager = (PrefabManagerModel)LayerManager.ToModel(),
            ModifierManager = (PrefabManagerModel)ModifierManager.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (CompositionModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            OutputSettings.FromModel(m.OutputSettings);

            LoadManager(TextureModifierManager, m.TextureModifierManager);
            LoadManager(LayerManager, m.LayerManager);
            LoadManager(ModifierManager, m.ModifierManager);
        }

        public void Dispose()
        {
            LayerManager.Dispose();
            TextureModifierManager.Dispose();
            ModifierManager.Dispose();
        }
    }
}
