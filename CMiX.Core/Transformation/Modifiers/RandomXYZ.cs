// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation.Modifiers
{
    public partial class RandomXYZ : ObservableObject, IBeatModifiable, IModifier, ISpreadable
    {
        public RandomXYZ(
            BooleanValue visible,
            BeatModifier beatModifier,
            Easing easing,
            ModifierModeSelector modifierModeSelector,
            BooleanValue randomizeLocation,
            Vector3 location,
            BooleanValue randomizeScale,
            Vector3 scale,
            BooleanValue randomizeRotation,
            Vector3 rotation)
        {
            Visible = visible;
            BeatModifier = beatModifier;
            Easing = easing;
            ModifierModeSelector = modifierModeSelector;
            
           
            RandomizeLocation = randomizeLocation;
            Location = location; // new Vector3(0.0f, 0.0f, 0.0f);
            RandomizeScale = randomizeScale; // new BooleanValue(true);
            Scale = scale; // new Vector3(0.0f, 0.0f, 0.0f);
            RandomizeRotation = randomizeLocation;// new BooleanValue(true);
            Rotation = rotation;// new Vector3(0.0f, 0.0f, 0.0f);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public BooleanValue Visible { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public Easing Easing { get; set; }
        public BooleanValue RandomizeLocation { get; set; }
        public Vector3 Location { get; set; }
        public BooleanValue RandomizeScale { get; set; }
        public Vector3 Scale { get; set; }
        public BooleanValue RandomizeRotation { get; set; }
        public Vector3 Rotation { get; set; }


        [ObservableProperty]
        private bool isExpanded = true;

        [ObservableProperty]
        private bool randomizeLocationIsExpanded;

        [ObservableProperty]
        private bool randomizeScaleIsExpanded;

        [ObservableProperty]
        private bool randomizeRotationIsExpanded;
    }
}
