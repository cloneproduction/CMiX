// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation.Modifiers
{
    // [ModifierPanel] removed - superseded by Modulation.XYZModifier. No longer addable via the
    // picker; kept so already-saved Project data referencing RandomXYZ still loads.
    public partial class RandomXYZ : BeatModifiableModifierBase, ISpreadableModifier
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
            : base(prefabService, beatModifierManager)
        {
            ModifierModeSelector = modifierModeSelector;
            Gaussian = gaussian;
            RandomizeLocation = randomizeLocation;
            Location = location;
            RandomizeScale = randomizeScale;
            Scale = scale;
            RandomizeRotation = randomizeRotation;
            Rotation = rotation;
        }

        public ModifierModeSelector ModifierModeSelector { get; set; }
        public GenericValue<bool> Gaussian { get; set; }
        public GenericValue<bool> RandomizeLocation { get; set; }
        public Vector3 Location { get; set; }
        public GenericValue<bool> RandomizeScale { get; set; }
        public Vector3 Scale { get; set; }
        public GenericValue<bool> RandomizeRotation { get; set; }
        public Vector3 Rotation { get; set; }
        public string DisplayName => "Random XYZ";

        [ObservableProperty]
        private bool randomizeLocationIsExpanded;

        [ObservableProperty]
        private bool randomizeScaleIsExpanded;

        [ObservableProperty]
        private bool randomizeRotationIsExpanded;

        public override IControlModel ToModel()
        {
            var model = new RandomXYZModel
            {
                Gaussian = (GenericValueModel<bool>)Gaussian.ToModel(),
                RandomizeLocation = (GenericValueModel<bool>)RandomizeLocation.ToModel(),
                Location = (Vector3Model)Location.ToModel(),
                RandomizeScale = (GenericValueModel<bool>)RandomizeScale.ToModel(),
                Scale = (Vector3Model)Scale.ToModel(),
                RandomizeRotation = (GenericValueModel<bool>)RandomizeRotation.ToModel(),
                Rotation = (Vector3Model)Rotation.ToModel(),
                ModifierModeSelector = (ModifierModeSelectorModel)ModifierModeSelector.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (RandomXYZModel)model;
            LoadBaseModel(m);
            Gaussian.FromModel(m.Gaussian);
            RandomizeLocation.FromModel(m.RandomizeLocation);
            Location.FromModel(m.Location);
            RandomizeScale.FromModel(m.RandomizeScale);
            Scale.FromModel(m.Scale);
            RandomizeRotation.FromModel(m.RandomizeRotation);
            Rotation.FromModel(m.Rotation);
            ModifierModeSelector.FromModel(m.ModifierModeSelector);
        }
    }
}
