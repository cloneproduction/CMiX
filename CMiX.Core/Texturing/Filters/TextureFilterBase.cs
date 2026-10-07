// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.Specialized;
using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Filters
{
    public abstract partial class TextureFilterBase : ObservableObject, IPrefab, ITextureFilter, IHasCompositionID, IDisposable
    {
        protected TextureFilterBase(PrefabService prefabService, Blend blend, PrefabManager modulatorManager)
        {
            PrefabService = prefabService;
            Blend = blend;
            ModulatorManager = modulatorManager;
            ModulatorManager.ManagerData.Items.CollectionChanged += OnModulatorManagerItemsChanged;
        }

        private void OnModulatorManagerItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Remove) return;
            if (e.OldItems == null) return;

            foreach (IControl removed in e.OldItems)
                foreach (var bindable in Bindables.Cast<IModulatorBindable>().Where(b => b.ModulatorID == removed.ID))
                    bindable.SetModulatorCommand.Execute(null);
        }

        public virtual void Dispose()
        {
            ModulatorManager.ManagerData.Items.CollectionChanged -= OnModulatorManagerItemsChanged;
            ModulatorManager.Dispose();
        }

        public Guid ID { get; set; } = Guid.NewGuid();

        private Guid _compositionID;
        public Guid CompositionID
        {
            get => _compositionID;
            set
            {
                _compositionID = value;
                ModulatorManager.CompositionID = value;
            }
        }

        public PrefabService PrefabService { get; set; }
        public Blend Blend { get; set; }
        public PrefabManager ModulatorManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public List<ModulatableValue<float>> Bindables { get; set; } = new();

        protected void PopulateBaseModel(ITextureFilterModel model)
        {
            model.ID = ID;
            model.PrefabService = (PrefabServiceModel)PrefabService.ToModel();
            model.Blend = (BlendModel)Blend.ToModel();
            model.Bindables = Bindables.Select(c => (ModulatableValueModel<float>)c.ToModel()).ToList();
            model.ModulatorManager = (PrefabManagerModel)ModulatorManager.ToModel();
        }

        protected void LoadBaseModel(ITextureFilterModel model)
        {
            ID = model.ID;
            PrefabService.FromModel(model.PrefabService);
            Blend.FromModel(model.Blend);

            LoadManager(ModulatorManager, model.ModulatorManager);

            for (int i = 0; i < Bindables.Count && i < model.Bindables.Count; i++)
                Bindables[i].FromModel(model.Bindables[i]);

            foreach (var bindable in Bindables.Cast<IModulatorBindable>())
                bindable.ModulatorLookup = id => ModulatorManager.ManagerData.Items
                    .OfType<IModulator>()
                    .FirstOrDefault(m => m.ID == id);
        }

        public abstract IControlModel ToModel();
        public abstract void FromModel(IControlModel model);
    }
}
