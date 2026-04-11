// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Layering.Modifiers
{
    public partial class RenderRandomEntity : ObservableObject, IBeatModifiable, IModifier
    {
        public RenderRandomEntity(PrefabManager beatModifierManager,
                                  PrefabService prefabService,
                                  GenericValue<EntityType> entityType,
                                  GenericValue<float> control)
        {
            BeatModifierManager = beatModifierManager;
            PrefabService = prefabService;
            EntityType = entityType;
            Control = control;
        }

        public Guid ID { get; set; }
        public PrefabService PrefabService { get; set; }
        public PrefabManager BeatModifierManager { get; set; }
        public GenericValue<EntityType> EntityType { get; set; }
        public GenericValue<float> Control { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new RenderRandomEntityModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            BeatModifierManager = (PrefabManagerModel)BeatModifierManager.ToModel(),
            EntityType = (GenericValueModel<EntityType>)EntityType.ToModel(),
            Control = (GenericValueModel<float>)Control.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (RenderRandomEntityModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            EntityType.FromModel(m.EntityType);
            Control.FromModel(m.Control);

            LoadManager(BeatModifierManager, m.BeatModifierManager);
        }
    }
}
