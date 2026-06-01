// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Rendering.Lights;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Transformation.Modifiers
{
    [ModifierPanel(typeof(LightEntity))]
    [ModifierPanel(typeof(Entity))]
    public partial class RandomRotation : ObservableObject, IBeatModifiable, ISpreadableModifier, IDisposable
    {
        public RandomRotation(PrefabManager beatModifierManager,
                              PrefabService prefabService, 
                              ModifierModeSelector modifierModeSelector, 
                              Vector3 rotation)
        {
            BeatModifierManager = beatModifierManager;
            PrefabService = prefabService;
            ModifierModeSelector = modifierModeSelector;
            Rotation = rotation;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public Vector3 Rotation { get; set; }
        public PrefabManager BeatModifierManager { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;

        public IControlModel ToModel() => new RandomRotationModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Rotation = (Vector3Model)Rotation.ToModel(),
            ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel(),
            BeatModifierManager = (PrefabManagerModel)BeatModifierManager.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (RandomRotationModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Rotation.FromModel(m.Rotation);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);

            LoadManager(BeatModifierManager, m.BeatModifierManager);
        }
        public void Dispose()
        {
            BeatModifierManager.ClearAll();
        }
    }
}
