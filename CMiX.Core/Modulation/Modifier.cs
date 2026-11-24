// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Modifiers;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Modulation
{
    public abstract partial class Modifier : ObservableObject, IPrefab, IModifier, IModulatable, IHasCompositionID, IDisposable
    {
        protected Modifier(PrefabService prefabService, ModulatorHost modulators, ControlRepository controlRepository, IEnumerable<IModulatorBindable> nestedBindables = null)
        {
            PrefabService = prefabService;
            Modulators = modulators;
            ControlRepository = controlRepository;
            Modulators.Add(nestedBindables ?? Enumerable.Empty<IModulatorBindable>());
        }

        public void Dispose() => Modulators.Dispose();

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
        public ModulatorHost Modulators { get; }
        public PrefabManager ModulatorManager => Modulators.Manager;
        public ControlRepository ControlRepository { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public List<ModulatableValue<float>> Bindables { get; set; } = new();

        protected void PopulateBaseModel(IModifierModel model)
        {
            model.ID = ID;
            model.PrefabService = (PrefabServiceModel)PrefabService.ToModel();
            model.IsExpanded = IsExpanded;
            model.ModulatorManager = (PrefabManagerModel)ModulatorManager.ToModel();
        }

        protected void LoadBaseModel(IModifierModel model)
        {
            ID = model.ID;
            PrefabService.FromModel(model.PrefabService);
            IsExpanded = model.IsExpanded;

            // The subclass makes its own bindables after the base constructor runs.
            Modulators.Add(Bindables);
            Modulators.Load(model.ModulatorManager);
        }

        public abstract IControlModel ToModel();
        public abstract void FromModel(IControlModel model);
    }
}
