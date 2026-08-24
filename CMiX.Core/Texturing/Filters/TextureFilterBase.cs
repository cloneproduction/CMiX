// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public abstract partial class TextureFilterBase : ObservableObject, IPrefab, ITextureFilter
    {
        protected TextureFilterBase(PrefabService prefabService, GenericValue<float> control, Blend blend)
        {
            PrefabService = prefabService;
            Control = control;
            Blend = blend;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public GenericValue<float> Control { get; set; }
        public Blend Blend { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        protected void PopulateBaseModel(ITextureFilterModel model)
        {
            model.ID = ID;
            model.PrefabService = (PrefabServiceModel)PrefabService.ToModel();
            model.Control = (GenericValueModel<float>)Control.ToModel();
            model.Blend = (BlendModel)Blend.ToModel();
        }

        protected void LoadBaseModel(ITextureFilterModel model)
        {
            ID = model.ID;
            PrefabService.FromModel(model.PrefabService);
            Control.FromModel(model.Control);
            Blend.FromModel(model.Blend);
        }

        public abstract IControlModel ToModel();
        public abstract void FromModel(IControlModel model);
    }
}
