// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Transformation.Modifiers
{
    public partial class RandomXYZ : ObservableObject, IBeatModifiable, ISpreadableModifier
    {
        public RandomXYZ(PrefabService prefabService,
                         PrefabManager beatModifierManager,
                         ModifierModeSelector modifierModeSelector,
                         GenericValue<bool> gaussian,
                         GenericValue<bool> randomizeLocation,
                         Vector3 location,
                         GenericValue<bool> randomizeScale,
                         Vector3 scale,
                         GenericValue<bool> randomizeRotation,
                         Vector3 rotation)
        {
            PrefabService = prefabService;
            BeatModifierManager = beatModifierManager;
            ModifierModeSelector = modifierModeSelector;
            Gaussian = gaussian;
            RandomizeLocation = randomizeLocation;
            Location = location;
            RandomizeScale = randomizeScale;
            Scale = scale;
            RandomizeRotation = randomizeRotation;
            Rotation = rotation;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabService PrefabService { get; set; }
        public PrefabManager BeatModifierManager{ get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public GenericValue<bool> Gaussian { get; set; }
        public GenericValue<bool> RandomizeLocation { get; set; }
        public Vector3 Location { get; set; }
        public GenericValue<bool> RandomizeScale { get; set; }
        public Vector3 Scale { get; set; }
        public GenericValue<bool> RandomizeRotation { get; set; }
        public Vector3 Rotation { get; set; }


        [ObservableProperty]
        private bool isExpanded = true;

        [ObservableProperty]
        private bool randomizeLocationIsExpanded;

        [ObservableProperty]
        private bool randomizeScaleIsExpanded;

        [ObservableProperty]
        private bool randomizeRotationIsExpanded;

        public IControlModel ToModel() => new RandomXYZModel
        {
            ID = ID,
            PrefabService = (PrefabServiceModel)PrefabService.ToModel(),
            Gaussian = (GenericValueModel<bool>)Gaussian.ToModel(),
            RandomizeLocation = (GenericValueModel<bool>)RandomizeLocation.ToModel(),
            Location = (Vector3Model)Location.ToModel(),
            RandomizeScale = (GenericValueModel<bool>)RandomizeScale.ToModel(),
            Scale = (Vector3Model)Scale.ToModel(),
            RandomizeRotation = (GenericValueModel<bool>)RandomizeRotation.ToModel(),
            Rotation = (Vector3Model)Rotation.ToModel(),
            BeatModifierManager = (PrefabManagerModel)BeatModifierManager.ToModel(),
            ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (RandomXYZModel)model;
            ID = m.ID;
            PrefabService.FromModel(m.PrefabService);
            Gaussian.FromModel(m.Gaussian);
            RandomizeLocation.FromModel(m.RandomizeLocation);
            Location.FromModel(m.Location);
            RandomizeScale.FromModel(m.RandomizeScale);
            Scale.FromModel(m.Scale);
            RandomizeRotation.FromModel(m.RandomizeRotation);
            Rotation.FromModel(m.Rotation);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);

            LoadManager(BeatModifierManager, m.BeatModifierManager);
        }
    }
}
