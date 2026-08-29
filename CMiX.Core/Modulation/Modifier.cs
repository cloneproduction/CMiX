// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Modulation
{
    // Property-first container - e.g. "Scale" owning Channels X/Y/Z. Implements the old
    // CMiX.Core.Modifiers.IModifier (a pure marker, adds no members) so every concrete Modifier
    // is discoverable the same way old modifiers already are: VL uses IModifier as its own
    // category filter to find "all modifiers" on the engine side, and ControlFactory.NameControl
    // special-cases it for naming (skips the .001/.002 de-duplication old modifiers don't use
    // either). This is independent of CMiX.Studio.Avalonia's own "Add Modifier" picker, which
    // discovers candidates purely by the [ModifierPanel(typeof(Owner))] attribute on each
    // concrete class - see ScaleModifier.cs. ModulatorManager is this Modifier's own private
    // stack (a PrefabManager of IModulator items) - not shared with any other Modifier, matching
    // how BeatModifiableModifierBase's BeatModifierManager already works for the old system.
    public abstract partial class Modifier : ObservableObject, IPrefab, IModifier, IDisposable
    {
        protected Modifier(PrefabService prefabService, PrefabManager modulatorManager)
        {
            PrefabService = prefabService;
            ModulatorManager = modulatorManager;
            ModulatorManager.ManagerData.Items.CollectionChanged += OnModulatorManagerItemsChanged;
        }

        // Deleting or resetting a modulator in this Modifier's own stack unassigns it from any
        // channel that was pointing at it, rather than leaving a dangling ModulatorID/BoundModulator
        // behind. CollectionManager.ResetItem replaces an item via an indexer-set, which raises
        // Replace rather than Remove - both actions carry the old item(s) in OldItems, so both are
        // handled the same way.
        private void OnModulatorManagerItemsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems == null) return;

            foreach (IControl removed in e.OldItems)
                foreach (var channel in Channels.Where(c => c.Binding.ModulatorID == removed.ID))
                    channel.Binding.SetModulatorCommand.Execute(null);
        }

        public void Dispose()
        {
            ModulatorManager.ManagerData.Items.CollectionChanged -= OnModulatorManagerItemsChanged;
            ModulatorManager.Dispose();
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public PrefabManager ModulatorManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public List<Modulatable> Channels { get; set; } = new();

        protected void PopulateBaseModel(IModifierModel model)
        {
            model.ID = ID;
            model.PrefabService = (PrefabServiceModel)PrefabService.ToModel();
            model.IsExpanded = IsExpanded;
            model.Channels = Channels.Select(c => (ModulatableModel)c.ToModel()).ToList();
            model.ModulatorManager = (PrefabManagerModel)ModulatorManager.ToModel();
        }

        protected void LoadBaseModel(IModifierModel model)
        {
            ID = model.ID;
            PrefabService.FromModel(model.PrefabService);
            IsExpanded = model.IsExpanded;

            // ModulatorManager loads first so each Modulatable's BoundModulator can be re-resolved by
            // ID right after - ChannelBinding.FromModel only restores ModulatorID, since it has no
            // access to the manager's items itself.
            LoadManager(ModulatorManager, model.ModulatorManager);

            for (int i = 0; i < Channels.Count && i < model.Channels.Count; i++)
                Channels[i].FromModel(model.Channels[i]);

            foreach (var channel in Channels)
            {
                if (channel.Binding.ModulatorID is not { } modulatorId) continue;
                channel.Binding.BoundModulator = ModulatorManager.ManagerData.Items
                    .OfType<IModulator>()
                    .FirstOrDefault(m => m.ID == modulatorId);
            }
        }

        public abstract IControlModel ToModel();
        public abstract void FromModel(IControlModel model);
    }
}
