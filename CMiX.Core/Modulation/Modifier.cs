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
    public abstract partial class Modifier : ObservableObject, IPrefab, IModifier, IHasCompositionID, IDisposable
    {
        protected Modifier(PrefabService prefabService, PrefabManager modulatorManager, ControlRepository controlRepository, IEnumerable<IModulatorBindable> nestedBindables = null)
        {
            PrefabService = prefabService;
            ModulatorManager = modulatorManager;
            ControlRepository = controlRepository;
            _nestedBindables = nestedBindables ?? Enumerable.Empty<IModulatorBindable>();
            ModulatorManager.ManagerData.Items.CollectionChanged += OnModulatorManagerItemsChanged;
        }

        private readonly IEnumerable<IModulatorBindable> _nestedBindables;

        private void OnModulatorManagerItemsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems == null) return;

            var bindables = Bindables.Cast<IModulatorBindable>().Concat(_nestedBindables);
            foreach (IControl removed in e.OldItems)
                foreach (var bindable in bindables.Where(b => b.ModulatorID == removed.ID))
                    bindable.SetModulatorCommand.Execute(null);
        }

        protected void ResolveNestedBindables()
        {
            foreach (var bindable in _nestedBindables)
            {
                if (bindable.ModulatorID is not { } modulatorId) return;

                var modulator = ModulatorManager.ManagerData.Items.OfType<IModulator>().FirstOrDefault(m => m.ID == modulatorId);
                var output = modulator?.Outputs.FirstOrDefault(o => o.Name == bindable.BoundOutputName);
                if (output != null)
                    bindable.SetModulatorCommand.Execute(new ModulatorOutputSelection(modulator, output));
            }

        }

        public void Dispose()
        {
            ModulatorManager.ManagerData.Items.CollectionChanged -= OnModulatorManagerItemsChanged;
            ModulatorManager.Dispose();
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Guid CompositionID { get; set; }
        public PrefabService PrefabService { get; set; }
        public PrefabManager ModulatorManager { get; set; }
        public ControlRepository ControlRepository { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public List<ModulatableValue<float>> Bindables { get; set; } = new();

        protected void PopulateBaseModel(IModifierModel model)
        {
            model.ID = ID;
            model.PrefabService = (PrefabServiceModel)PrefabService.ToModel();
            model.IsExpanded = IsExpanded;
            model.Bindables = Bindables.Select(c => (ModulatableValueModel<float>)c.ToModel()).ToList();
            model.ModulatorManager = (PrefabManagerModel)ModulatorManager.ToModel();
        }

        protected void LoadBaseModel(IModifierModel model)
        {
            ID = model.ID;
            PrefabService.FromModel(model.PrefabService);
            IsExpanded = model.IsExpanded;

            LoadManager(ModulatorManager, model.ModulatorManager);

            for (int i = 0; i < Bindables.Count && i < model.Bindables.Count; i++)
                Bindables[i].FromModel(model.Bindables[i]);

            foreach (var bindable in Bindables.Cast<IModulatorBindable>().Concat(_nestedBindables))
                bindable.ModulatorResolver = id => ModulatorManager.ManagerData.Items
                    .OfType<IModulator>()
                    .FirstOrDefault(m => m.ID == id);
        }

        public abstract IControlModel ToModel();
        public abstract void FromModel(IControlModel model);
    }
}
