// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Sources
{
    // Every texture source shares this exact shape (ID, PrefabService, TextureModifierManager,
    // isExpanded, and the same Dispose/load-manager plumbing) with no exceptions across the
    // whole family, so it lives here once instead of being copy-pasted into each source. Each
    // source still owns its own Model type, its own Resolution/AssetSelector (from
    // ITextureSource/IAssetTextureSource, not universal enough to belong here), and its own
    // extra fields - PopulateBaseModel/LoadBaseModel only take care of the shared three.
    public abstract partial class TextureSourceBase : ObservableObject, ITextureModifiable, IDisposable
    {
        protected TextureSourceBase(PrefabService prefabService, PrefabManager textureModifierManager)
        {
            PrefabService = prefabService;
            TextureModifierManager = textureModifierManager;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public PrefabManager TextureModifierManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        // The filter modifiers are reachable through this manager alone, so a texture torn down
        // without disposing it leaves their repository and their deleter registrations behind.
        public void Dispose() => DisposeAll(TextureModifierManager);

        protected void PopulateBaseModel(ITextureSourceModel model)
        {
            model.ID = ID;
            model.PrefabService = (PrefabServiceModel)PrefabService.ToModel();
            model.TextureModifierManager = (PrefabManagerModel)TextureModifierManager.ToModel();
        }

        protected void LoadBaseModel(ITextureSourceModel model)
        {
            ID = model.ID;
            PrefabService.FromModel(model.PrefabService);
            LoadManager(TextureModifierManager, model.TextureModifierManager);
        }

        public abstract IControlModel ToModel();
        public abstract void FromModel(IControlModel model);
    }
}
