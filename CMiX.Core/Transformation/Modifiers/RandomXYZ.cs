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
        public RandomXYZ()
        {
            isExpanded = true;
        }

        public Guid ID { get; set; }
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
        private bool isExpanded;

        [ObservableProperty]
        private bool randomizeLocationIsExpanded;

        [ObservableProperty]
        private bool randomizeScaleIsExpanded;

        [ObservableProperty]
        private bool randomizeRotationIsExpanded;
    }
}
