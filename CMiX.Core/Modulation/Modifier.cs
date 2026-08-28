// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Modulation
{
    // Property-first container - e.g. "Scale" owning Channels X/Y/Z. Deliberately does not
    // implement the old CMiX.Core.Modifiers.IModifier: how/whether this attaches to a real
    // owner's existing picker is future migration work, not decided yet. ModulatorManager is this
    // Modifier's own private stack (a PrefabManager of IModulator items) - not shared with any
    // other Modifier, matching how BeatModifiableModifierBase's BeatModifierManager already works
    // for the old system.
    public abstract partial class Modifier : ObservableObject, IPrefab
    {
        protected Modifier(PrefabService prefabService, PrefabManager modulatorManager)
        {
            PrefabService = prefabService;
            ModulatorManager = modulatorManager;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public PrefabManager ModulatorManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public List<Channel> Channels { get; set; } = new();

        protected void PopulateBaseModel(IModifierModel model)
        {
            model.ID = ID;
            model.PrefabService = (PrefabServiceModel)PrefabService.ToModel();
            model.IsExpanded = IsExpanded;
            model.Channels = Channels.Select(c => (ChannelModel)c.ToModel()).ToList();
            model.ModulatorManager = (PrefabManagerModel)ModulatorManager.ToModel();
        }

        protected void LoadBaseModel(IModifierModel model)
        {
            ID = model.ID;
            PrefabService.FromModel(model.PrefabService);
            IsExpanded = model.IsExpanded;
            for (int i = 0; i < Channels.Count && i < model.Channels.Count; i++)
                Channels[i].FromModel(model.Channels[i]);
            LoadManager(ModulatorManager, model.ModulatorManager);
        }

        public abstract IControlModel ToModel();
        public abstract void FromModel(IControlModel model);
    }
}
