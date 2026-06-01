// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Transformation.Modifiers
{
    [ModifierPanel(typeof(Entity))]
    public partial class RandomScale : ObservableObject, IBeatModifiable, ISpreadableModifier, IDisposable
    {
        public RandomScale(PrefabManager beatModifierManager,
                           PrefabService prefabService, 
                           ModifierModeSelector modifierModeSelector, 
                           Vector3 scale, 
                           GenericValue<float> uniformXYZ)
        {
            BeatModifierManager = beatModifierManager;
            PrefabService = prefabService;
            ModifierModeSelector = modifierModeSelector;
            Scale = scale;
            UniformXYZ = uniformXYZ;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Vector3 Scale { get; set; }
        public GenericValue<float> UniformXYZ { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public PrefabService PrefabService { get; set; }
        public PrefabManager BeatModifierManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new RandomScaleModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            BeatModifierManager = (PrefabManagerModel)BeatModifierManager.ToModel(),
            Scale = (Vector3Model)Scale.ToModel(),
            UniformXYZ = (GenericValueModel<float>)UniformXYZ.ToModel(),
            ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (RandomScaleModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Scale.FromModel(m.Scale);
            UniformXYZ.FromModel(m.UniformXYZ);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);

            LoadManager(BeatModifierManager, m.BeatModifierManager);
        }
        public void Dispose()
        {
            BeatModifierManager.ClearAll();
        }
    }
}
