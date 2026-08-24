// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Animations
{
    // Every IBeatModifiable modifier shares this exact shape (ID, PrefabService,
    // BeatModifierManager, isExpanded, and the same Dispose/load-manager plumbing) with no
    // exceptions across the whole family, so it lives here once instead of being copy-pasted
    // into each modifier. Each modifier still owns its own Model type and its own extra fields -
    // PopulateBaseModel/LoadBaseModel only take care of the shared three.
    //
    // IBeatModifiable isn't consumed anywhere in this C# codebase - Dispose/LoadBaseModel below
    // reach BeatModifierManager directly, not through the interface. Keep it anyway: the vvvv/VL
    // Engine side (outside this repo) relies on it to find each modifier's BeatModifierManager.
    // Don't remove it just because static analysis here can't find a consumer.
    public abstract partial class BeatModifiableModifierBase : ObservableObject, IPrefab, IBeatModifiable, IDisposable
    {
        protected BeatModifiableModifierBase(PrefabService prefabService, PrefabManager beatModifierManager)
        {
            PrefabService = prefabService;
            BeatModifierManager = beatModifierManager;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public PrefabManager BeatModifierManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public void Dispose()
        {
            BeatModifierManager.Dispose();
        }

        protected void PopulateBaseModel(IBeatModifiableModifierModel model)
        {
            model.ID = ID;
            model.PrefabService = (PrefabServiceModel)PrefabService.ToModel();
            model.BeatModifierManager = (PrefabManagerModel)BeatModifierManager.ToModel();
        }

        protected void LoadBaseModel(IBeatModifiableModifierModel model)
        {
            ID = model.ID;
            PrefabService.FromModel(model.PrefabService);
            LoadManager(BeatModifierManager, model.BeatModifierManager);
        }

        public abstract IControlModel ToModel();
        public abstract void FromModel(IControlModel model);
    }
}
