// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections.Specialized;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Filters
{
    public abstract partial class TextureFilterBase : ObservableObject, IPrefab, ITextureFilter, IModulatable, IHasCompositionID, IDisposable
    {
        protected TextureFilterBase(PrefabService prefabService, Blend blend, ModulatorHost modulators)
        {
            PrefabService = prefabService;
            Blend = blend;
            Modulators = modulators;
        }

        public virtual void Dispose() => Modulators.Dispose();

        public Guid ID { get; set; } = Guid.NewGuid();

        private Guid _compositionID;
        public virtual Guid CompositionID
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
        public ModulatorHost Modulators { get; }
        public PrefabManager ModulatorManager => Modulators.Manager;

        [ObservableProperty]
        private bool isExpanded = true;

        public List<ModulatableValue<float>> Bindables { get; set; } = new();

        protected void PopulateBaseModel(ITextureFilterModel model)
        {
            model.ID = ID;
            model.PrefabService = (PrefabServiceModel)PrefabService.ToModel();
            model.Blend = (BlendModel)Blend.ToModel();
            model.ModulatorManager = (PrefabManagerModel)ModulatorManager.ToModel();
        }

        protected void LoadBaseModel(ITextureFilterModel model)
        {
            ID = model.ID;
            PrefabService.FromModel(model.PrefabService);
            Blend.FromModel(model.Blend);

            // The subclass makes its own bindables after the base constructor runs.
            Modulators.Add(Bindables);
            Modulators.Load(model.ModulatorManager);
        }

        public abstract IControlModel ToModel();
        public abstract void FromModel(IControlModel model);
    }
}
