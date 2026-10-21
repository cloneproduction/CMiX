// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Sources
{
    public abstract partial class TextureSourceBase : ObservableObject, ITextureModifiable, IHasCompositionID, IDisposable
    {
        protected TextureSourceBase(PrefabService prefabService,
                                    PrefabManager textureModifierManager,
                                    PrefabManager modulatorManager,
                                    GenericValue<bool> useCompositionResolution)
        {
            PrefabService = prefabService;
            TextureModifierManager = textureModifierManager;
            Modulators = new ModulatorHost(modulatorManager);
            UseCompositionResolution = useCompositionResolution;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public PrefabManager TextureModifierManager { get; set; }
        public ModulatorHost Modulators { get; }
        public PrefabManager ModulatorManager => Modulators.Manager;
        public GenericValue<bool> UseCompositionResolution { get; set; }

        private Guid _compositionID;
        public Guid CompositionID
        {
            get => _compositionID;
            set
            {
                _compositionID = value;
                TextureModifierManager.CompositionID = value;
                Modulators.Manager.CompositionID = value;
            }
        }

        [ObservableProperty]
        private bool isExpanded = false;

        public void Dispose() => DisposeAll(TextureModifierManager, Modulators);

        protected void PopulateBaseModel(ITextureSourceModel model)
        {
            model.ID = ID;
            model.PrefabService = (PrefabServiceModel)PrefabService.ToModel();
            model.TextureModifierManager = (PrefabManagerModel)TextureModifierManager.ToModel();
            model.ModulatorManager = (PrefabManagerModel)ModulatorManager.ToModel();
            model.UseCompositionResolution = (GenericValueModel<bool>)UseCompositionResolution.ToModel();
        }

        protected void LoadBaseModel(ITextureSourceModel model)
        {
            ID = model.ID;
            PrefabService.FromModel(model.PrefabService);
            LoadManager(TextureModifierManager, model.TextureModifierManager);
            Modulators.Load(model.ModulatorManager);
            UseCompositionResolution.FromModel(model.UseCompositionResolution);
        }

        public abstract IControlModel ToModel();
        public abstract void FromModel(IControlModel model);
    }
}
