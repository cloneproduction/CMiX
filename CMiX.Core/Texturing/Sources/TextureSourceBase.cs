// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
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
                                    GenericValue<bool> useCompositionResolution)
        {
            PrefabService = prefabService;
            TextureModifierManager = textureModifierManager;
            UseCompositionResolution = useCompositionResolution;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public PrefabManager TextureModifierManager { get; set; }
        public GenericValue<bool> UseCompositionResolution { get; set; }

        private Guid _compositionID;
        public Guid CompositionID
        {
            get => _compositionID;
            set
            {
                _compositionID = value;
                new CompositionIDAssigner(TextureModifierManager, value);
            }
        }

        [ObservableProperty]
        private bool isExpanded = true;

        public void Dispose() => DisposeAll(TextureModifierManager);

        protected void PopulateBaseModel(ITextureSourceModel model)
        {
            model.ID = ID;
            model.PrefabService = (PrefabServiceModel)PrefabService.ToModel();
            model.TextureModifierManager = (PrefabManagerModel)TextureModifierManager.ToModel();
            model.UseCompositionResolution = (GenericValueModel<bool>)UseCompositionResolution.ToModel();
        }

        protected void LoadBaseModel(ITextureSourceModel model)
        {
            ID = model.ID;
            PrefabService.FromModel(model.PrefabService);
            LoadManager(TextureModifierManager, model.TextureModifierManager);
            UseCompositionResolution.FromModel(model.UseCompositionResolution);
        }

        public abstract IControlModel ToModel();
        public abstract void FromModel(IControlModel model);
    }
}
