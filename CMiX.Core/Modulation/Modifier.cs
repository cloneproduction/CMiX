// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.Modulation.Modulators;
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
        // handled the same way. Walks Channels plus AdditionalModulatorBindables so anything nested
        // one level deeper (e.g. a ModifierModeSelector's Count) gets the same cleanup.
        private void OnModulatorManagerItemsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems == null) return;

            var bindables = Channels.Cast<IModulatorBindable>().Concat(AdditionalModulatorBindables);
            foreach (IControl removed in e.OldItems)
                foreach (var bindable in bindables.Where(b => b.ModulatorID == removed.ID))
                    bindable.SetModulatorCommand.Execute(null);
        }

        // Empty by default - overridden by the handful of Modifiers whose ModifierModeSelector(3)
        // has a bindable Count/CountX/CountY/CountZ, which live one level inside that selector
        // rather than directly in Channels, so OnModulatorManagerItemsChanged above wouldn't reach
        // them otherwise.
        protected virtual IEnumerable<IModulatorBindable> AdditionalModulatorBindables => Enumerable.Empty<IModulatorBindable>();

        // Re-resolves one bindable's live BoundModulator reference by ID against this Modifier's own
        // ModulatorManager - the same lookup Channels get automatically below in LoadBaseModel, but
        // callable explicitly for anything nested deeper. Needed because a nested control (e.g.
        // ModifierModeSelector) only gets its own ModulatorID populated once the owning Modifier's
        // FromModel loads it, which happens after LoadBaseModel returns - so the concrete Modifier
        // must call this again once that nested FromModel has run.
        protected void ResolveModulatorBinding(IModulatorBindable bindable)
        {
            if (bindable.ModulatorID is not { } modulatorId) return;

            var modulator = ModulatorManager.ManagerData.Items.OfType<IModulator>().FirstOrDefault(m => m.ID == modulatorId);
            var output = modulator?.Outputs.FirstOrDefault(o => o.Name == bindable.BoundOutputName);
            if (output != null)
                bindable.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, output));
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

        public List<ModulatableFloat> Channels { get; set; } = new();

        protected void PopulateBaseModel(IModifierModel model)
        {
            model.ID = ID;
            model.PrefabService = (PrefabServiceModel)PrefabService.ToModel();
            model.IsExpanded = IsExpanded;
            model.Channels = Channels.Select(c => (ModulatableFloatModel)c.ToModel()).ToList();
            model.ModulatorManager = (PrefabManagerModel)ModulatorManager.ToModel();
        }

        protected void LoadBaseModel(IModifierModel model)
        {
            ID = model.ID;
            PrefabService.FromModel(model.PrefabService);
            IsExpanded = model.IsExpanded;

            // ModulatorManager loads first so each ModulatableFloat's BoundModulator can be re-resolved by
            // ID right after - ModulatableFloat.FromModel only restores ModulatorID, since it has no
            // access to the manager's items itself.
            LoadManager(ModulatorManager, model.ModulatorManager);

            for (int i = 0; i < Channels.Count && i < model.Channels.Count; i++)
                Channels[i].FromModel(model.Channels[i]);

            foreach (var channel in Channels)
            {
                if (channel.ModulatorID.Value is not { } modulatorId) continue;
                channel.BoundModulator = ModulatorManager.ManagerData.Items
                    .OfType<IModulator>()
                    .FirstOrDefault(m => m.ID == modulatorId);
            }
        }

        public abstract IControlModel ToModel();
        public abstract void FromModel(IControlModel model);
    }
}
